using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signet.SqlSugarModel
{
    /// <summary>
    /// 物品表
    ///</summary>
    [SugarTable("Goods_Table")]
    public class Goods_Table
    {
        /// <summary>
        /// 备  注:主键
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Goods_ID", IsPrimaryKey = true, IsIdentity = true)]
        public long Goods_ID { get; set; }

        /// <summary>
        /// 备  注:物品编码
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Goods_Code")]
        public string Goods_Code { get; set; }


        /// <summary>
        /// 备  注:物品名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Goods_Name")]
        public string Goods_Name { get; set; }

        /// <summary>
        /// 备  注:物品型号
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Goods_Model")]
        public string Goods_Model { get; set; }

        /// <summary>
        /// 备  注:物质类型
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Goods_Type")]
        public long Goods_Type { get; set; }

        /// <summary>
        /// 备  注:物质单位
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Goods_Unit")]
        public long Goods_Unit { get; set; }

        /// <summary>
        /// 备  注:分类
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Classification")]
        public long Classification { get; set; }

        /// <summary>
        /// 备  注:库存
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Inventory",IsOnlyIgnoreInsert = true)]
        public int Inventory { get; set; }
    }
}
