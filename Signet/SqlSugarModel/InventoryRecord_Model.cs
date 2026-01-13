using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signet.SqlSugarModel
{
    /// <summary>
    /// 出入库记录表
    ///</summary>
    [SugarTable("InventoryRecord_Table")]
    public class InventoryRecord_Table
    {


        /// <summary>
        /// 备  注:主键
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Record_ID", IsPrimaryKey = true, IsIdentity = true)]
        public long Record_ID { get; set; }

        /// <summary>
        /// 备  注:物品id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "GoodsID")]
        public long GoodsID { get; set; }

        /// <summary>
        /// 备  注:操作者
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Operator")]
        public long? Operator { get; set; }

        /// <summary>
        /// 备  注:备注
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// 备  注:出入库时间
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "UserTime")]
        public DateTime UserTime { get; set; }

        /// <summary>
        /// 备  注:操作时间
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "OperateTime",IsOnlyIgnoreInsert =true)]
        public DateTime OperateTime { get; set; }

        /// <summary>
        /// 备  注:出入库类型
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "InventoryType")]
        public string InventoryType { get; set; }

        /// <summary>
        /// 备  注:出入库数量
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "InventoryNum")]
        public int InventoryNum { get; set; }
    }
}
