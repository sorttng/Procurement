using SqlSugar;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Signet.SqlSugarModel;
using Signet.Model;
namespace Signet.Common
{
    public class ConfigChangedEventArgs : EventArgs
    {
        public string Config_Key { get; set; }
        public object NewValue { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime ChangeTime { get; set; }
    }

    // 修复的 ConfigService 类
    public class ConfigService
    {
        private readonly ISqlSugarClient _db;
        private static readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
        private static DateTime _lastCacheUpdate = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        // 配置变更事件
        public event EventHandler<ConfigChangedEventArgs> OnConfigChanged;

        public ConfigService(ISqlSugarClient sqlSugarClient)
        {
            _db = sqlSugarClient;
        }

        // 获取配置值（带缓存和类型安全）
        public T GetConfigValue<T>(string configKey, T defaultValue = default)
        {
            // 检查并刷新缓存
            CheckAndRefreshCache();

            // 从缓存读取
            if (_cache.TryGetValue(configKey, out var cachedValue))
            {
                if (cachedValue is T typedValue)
                {
                    return typedValue;
                }
            }

            // 缓存中没有，从数据库获取
            var value = GetConfigFromDatabase<T>(configKey, defaultValue);
            if (value != null)
            {
                _cache[configKey] = value;
            }
            return value;
        }

        // 从数据库读取配置
        private T GetConfigFromDatabase<T>(string configKey, T defaultValue)
        {
            try
            {
                var config = _db.Queryable<SystemConfig_Table>()
                    .Where(x => x.Config_Key == configKey && x.IsActive)
                    .First();

                if (config != null && !string.IsNullOrEmpty(config.Config_Value))
                {
                    return ConvertValue<T>(config.Config_Value, config.Config_Type);
                }

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        // 设置配置值 - 同步版本
        public bool SetConfigValue<T>(string configKey, T value, long modifiedBy)
        {
            try
            {
                var config = new SystemConfig_Table
                {
                    Config_Key = configKey,
                    Config_Value = value?.ToString(),
                    Config_Type = GetTypeString(typeof(T)),
                    LastModifiedBy = modifiedBy,
                    LastModifiedTime = DateTime.Now,
                    IsActive = true,
                };

                // 检查是否已存在
                var existing = _db.Queryable<SystemConfig_Table>()
                    .Where(x => x.Config_Key == configKey)
                    .First();

                int result;
                if (existing != null)
                {
                    result = _db.Updateable(config)
                        .WhereColumns(x => x.Config_Key)
                        .IgnoreColumns(ignoreAllNullColumns: true)
                        .ExecuteCommand();
                }
                else
                {
                    result = _db.Insertable(config).ExecuteCommand();
                }

                if (result > 0)
                {
                    // 更新缓存
                    _cache[configKey] = value;
                    _lastCacheUpdate = DateTime.UtcNow;
                    // 触发事件
                    OnConfigChanged?.Invoke(this, new ConfigChangedEventArgs
                    {
                        Config_Key = configKey,
                        NewValue = value,
                        ModifiedBy = modifiedBy,
                        ChangeTime = DateTime.UtcNow
                    });
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"设置配置失败: {ex.Message}");
                return false;
            }
        }

        // 设置配置值 - 异步版本（修复版本）
        public async Task<bool> SetConfigValueAsync<T>(string configKey, T value, long modifiedBy)
        {
            try
            {
                // 使用 SqlSugar 的事务
                var result = await _db.Ado.UseTranAsync(async () =>
                {
                    var config = new SystemConfig_Table
                    {
                        Config_Key = configKey,
                        Config_Value = value?.ToString(),
                        Config_Type = GetTypeString(typeof(T)),
                        LastModifiedBy = modifiedBy,
                        LastModifiedTime = DateTime.UtcNow,
                        IsActive = true
                    };

                    // 检查是否已存在
                    var existing = await _db.Queryable<SystemConfig_Table>()
                        .Where(x => x.Config_Key == configKey)
                        .FirstAsync();

                    int rowsAffected = 0;
                    if (existing != null)
                    {
                        rowsAffected = await _db.Updateable(config)
                            .WhereColumns(x => x.Config_Key)
                            .ExecuteCommandAsync();
                    }
                    else
                    {
                        rowsAffected = await _db.Insertable(config).ExecuteCommandAsync();
                    }

                    return rowsAffected > 0;
                });

                // 检查事务是否成功
                if (result.IsSuccess && result.Data)
                {
                    // 更新缓存
                    _cache[configKey] = value;
                    _lastCacheUpdate = DateTime.UtcNow;

                    // 触发事件
                    OnConfigChanged?.Invoke(this, new ConfigChangedEventArgs
                    {
                        Config_Key = configKey,
                        NewValue = value,
                        ModifiedBy = modifiedBy,
                        ChangeTime = DateTime.UtcNow
                    });
                }
                else
                {
                    // 记录错误
                    System.Diagnostics.Debug.WriteLine($"设置配置失败，事务失败: {result.ErrorMessage}");
                }

                return result.IsSuccess && result.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"设置配置失败: {ex.Message}");
                return false;
            }
        }

        // 检查并刷新缓存
        private void CheckAndRefreshCache()
        {
            if (DateTime.UtcNow - _lastCacheUpdate > CacheDuration)
            {
                RefreshCache();
            }
        }

        // 刷新缓存
        private void RefreshCache()
        {
            try
            {
                var configs = _db.Queryable<SystemConfig_Table>()
                    .Where(x => x.IsActive)
                    .ToList();

                lock (_cache)
                {
                    _cache.Clear();
                    foreach (var config in configs)
                    {
                        var value = ConvertValue<object>(config.Config_Value, config.Config_Type);
                        _cache[config.Config_Key] = value;
                    }
                    _lastCacheUpdate = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"刷新配置缓存失败: {ex.Message}");
            }
        }

        // 类型转换辅助方法
        private static T ConvertValue<T>(string value, string type)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            try
            {
                if (string.IsNullOrEmpty(type))
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }

                string typeUpper = type.ToUpper();

                if (typeUpper == "INT")
                {
                    if (int.TryParse(value, out int intVal))
                    {
                        return (T)(object)intVal;
                    }
                }
                else if (typeUpper == "BOOL")
                {
                    if (bool.TryParse(value, out bool boolVal))
                    {
                        return (T)(object)boolVal;
                    }
                }
                else if (typeUpper == "DOUBLE")
                {
                    if (double.TryParse(value, out double doubleVal))
                    {
                        return (T)(object)doubleVal;
                    }
                }
                else if (typeUpper == "DATETIME")
                {
                    if (DateTime.TryParse(value, out DateTime dateVal))
                    {
                        return (T)(object)dateVal;
                    }
                }
                else if (typeUpper == "STRING")
                {
                    return (T)(object)value;
                }

                // 尝试通用转换
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        private static string GetTypeString(Type type)
        {
            if (type == typeof(int))
            {
                return "INT";
            }
            if (type == typeof(bool))
            {
                return "BOOL";
            }
            if (type == typeof(double))
            {
                return "DOUBLE";
            }
            if (type == typeof(DateTime))
            {
                return "DATETIME";
            }
            return "STRING";
        }
    }
}
