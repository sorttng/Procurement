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
    [SugarTable("Role_Auth_Relationship_Table")]
    public class Role_Auth_Relationship_Table
    {
        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Role_Auth_Relationship_ID", IsPrimaryKey = true, IsIdentity = true)]
        public long? Role_Auth_Relationship_ID { get; set; }

        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Role_ID")]
        public long? Role_ID { get; set; }

        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Authority_ID")]
        public long? Authority_ID { get; set; }


    }

}
