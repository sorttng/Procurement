using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signet.SqlSugarModel
{
    /// <summary>
    /// 分类表
    ///</summary>
    [SugarTable("Classification_Table")]
    public class Classification_Table
    {
        /// <summary>
        /// 备  注:分类主键
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Classification_ID", IsPrimaryKey = true, IsIdentity = true)]
        public long Classification_ID { get; set; }

        /// <summary>
        /// 备  注:分类名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Classification_Name")]
        public string Classification_Name { get; set; }

    }
}
