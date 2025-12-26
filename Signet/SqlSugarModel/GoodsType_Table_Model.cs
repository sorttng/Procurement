using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signet.SqlSugarModel
{
    /// <summary>
    /// 物品类型表
    ///</summary>
    [SugarTable("GoodsType_Table")]
    public class GoodsType_Table
    {
        /// <summary>
        /// 备  注:物品类型主键
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "GoodsType_ID", IsPrimaryKey = true, IsIdentity = true)]
        public long GoodsType_ID { get; set; }

        /// <summary>
        /// 备  注:物资类型名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "GoodsType_Name")]
        public string GoodsType_Name { get; set; }


    }
}
