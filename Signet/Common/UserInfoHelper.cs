using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Signet.SqlSugarModel;
namespace Signet.Model
{
    public class UserInfo_Model
    {
        public long SerialNum { get; set; }

        public long UserID { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string PhoneNum { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public long DepartID { get; set; }
        public string DepartmentName { get; set; }
        public long RoleID { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }

        public List<string> Authorities { get; set; }
    }

    public class UserInfo
    {
        public static UserInfo_Model LogedUserInfo;
    }
}
