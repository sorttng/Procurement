using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signet.SqlSugarModel
{
    /// <summary>
    /// 系统配置表
    ///</summary>
    [SugarTable("SystemConfig_Table")]
    public class SystemConfig_Table
    {
        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Config_ID", IsPrimaryKey = true, IsIdentity = true)]
        public long Config_ID { get; set; }

        /// <summary>
        /// 备  注:配置键
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Config_Key")]
        public string Config_Key { get; set; }


        /// <summary>
        /// 备  注:配置名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Config_Name")]
        public string Config_Name { get; set; }

        /// <summary>
        /// 备  注:配置类型
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Config_Type")]
        public string Config_Type { get; set; }

        /// <summary>
        /// 备  注:配置值
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Config_Value")]
        public string Config_Value { get; set; }
        
        /// <summary>
        /// 备  注:描述
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// 备  注:最后修改人
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "LastModifiedBy")]
        public long LastModifiedBy { get; set; }

        /// <summary>
        /// 备  注:最后修改时间
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "LastModifiedTime", IsOnlyIgnoreInsert = true)]
        public DateTime LastModifiedTime { get; set; }

        /// <summary>
        /// 备  注:是否有效
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "IsActive", IsOnlyIgnoreInsert = true)]
        public bool IsActive { get; set; }
    }
}
