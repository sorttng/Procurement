using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signet.SqlSugarModel
{
    /// <summary>
    /// 单位表
    ///</summary>
    [SugarTable("Unit_Table")]
    public class Unit_Table
    {

        /// <summary>
        /// 备  注:单位主键
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Unit_ID", IsPrimaryKey = true, IsIdentity = true)]
        public long Unit_ID { get; set; }

        /// <summary>
        /// 备  注:单位名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Unit_Name")]
        public string Unit_Name { get; set; }


    }
}
