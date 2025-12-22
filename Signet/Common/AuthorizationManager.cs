using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Signet.ViewModel;
using Signet.SqlSugarModel;
namespace Signet.Common
{
    // 权限枚举
    //public enum EAuthorizationItem
    //{
    //    [Description("日志查询")]
    //    LogQuery,

    //    [Description("新增用户")]
    //    UserInsert,
    //}
    // 新的权限管理器
    public class AuthorizationManager : ObservableObject
    {
        // 单例实例
        private static readonly Lazy<AuthorizationManager> instance =
            new Lazy<AuthorizationManager>(() => new AuthorizationManager());
        public static AuthorizationManager Instance => instance.Value;

        private List<SqlSugarModel.Authorities_Table> AllPermissions = new List<Authorities_Table>();

        // 当前用户权限集合
        private HashSet<string> _userPermissions = new HashSet<string>();

        // 私有构造函数
        private AuthorizationManager()
        {
            // 这里可以初始化默认权限
            // 或者从配置文件/数据库加载
            LoadAllPermissions();
        }

        private void LoadAllPermissions()
        {
            AllPermissions = SqlSugarHelper.mDB.Queryable<SqlSugarModel.Authorities_Table>()
                .Where(a => a.IsEffect == true).ToList();
        }

        public void LoadUserPermission(long roleID)
        {
            List<Authorities_Table> roleAuthorities = AuthorityCfg_ViewModel.LoadRoleAuthoritys(roleID);
            var permissions = roleAuthorities.Select(a=>a.Authority_Name).ToList();
            // 设置权限
            SetUserPermissions(permissions);
        }


        // 通用权限检查方法
        public bool HasPermission(string permission)
        {
            return _userPermissions.Contains(permission);
        }

        // 索引器 - 核心功能
        public bool this[string permission]
        {
            get { return HasPermission(permission); }
        }

        // 设置用户权限
        public void SetUserPermissions(List<string> permissions)
        {
            _userPermissions = new HashSet<string>(permissions);

            // 通知索引器属性变更
            // 这会使得所有绑定到索引器的UI重新检查权限
            RaisePropertyChanged(Binding.IndexerName);
        }


        // 清除所有权限
        public void ClearPermissions()
        {
            _userPermissions.Clear();
            RaisePropertyChanged(Binding.IndexerName);
        }



    }
}
