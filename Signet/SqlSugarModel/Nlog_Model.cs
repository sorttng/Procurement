using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace Signet.SqlSugarModel
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Nlog")]
    public class Nlog_Model
    {
        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "MachineName")]
        public string MachineName { get; set; }

        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Logged")]
        public DateTime Logged { get; set; }

        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Level")]
        public string Level { get; set; }

        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Message")]
        public string Message { get; set; }

        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Logger")]
        public string Logger { get; set; }

        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Properties")]
        public string Properties { get; set; }

        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Exception")]
        public string Exception { get; set; }

        /// <summary>
        /// 备  注:用户id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "UserID")]
        public long UserID { get; set; }

        /// <summary>
        /// 备  注:用户名
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "UserName")]
        public string UserName { get; set; }

        [SugarColumn(IsIgnore = true)]//需要加上
        public int RowIndex { get; set; } //行号 序号
    }
}
