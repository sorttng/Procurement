using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace Signet.SqlSugarModel
{
    /// <summary>
    /// User_Table
    /// </summary>
    [SugarTable("User_Table")]
    public class User_Table
    {
        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "UserID", IsPrimaryKey = true, IsIdentity = true)]
        public long UserID { get; set; }

        /// <summary>
        /// 备  注:用户名
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 备  注:部门
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "UserDeparID")]
        public long UserDeparID { get; set; }

        /// <summary>
        /// 备  注:出生日期
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Birthday")]
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 备  注:密码
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// 备  注:角色
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "UserRoleID")]
        public long UserRoleID { get; set; }

        /// <summary>
        /// 备  注:用户登录编码
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 备  注:电话号码
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "PhoneNum")]
        public string PhoneNum { get; set; }

        /// <summary>
        /// 备  注:
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName = "Email")]
        public string Email { get; set; }
    }
}
