using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signet.SqlSugarModel
{
    /// <summary>
    /// 部门表
    ///</summary>
    [SugarTable("Depart_Table")]
    public class Depart_Table
    {

        /// <summary>
        /// 备  注:部门id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "DepartID", IsPrimaryKey = true, IsIdentity = true)]
        public int DepartID { get; set; }

        /// <summary>
        /// 备  注:部门名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Depart_Name")]
        public string Depart_Name { get; set; }
    }
}
