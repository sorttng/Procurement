using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Signet.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace Signet.Model
{
    public class UserManage_Model: ValidateModelBase
    {
        //标题
        private string _Header;

        public string Header
        {
            get { return _Header; }
            set { _Header = value; RaisePropertyChanged(() => Header); }
        }

        ////用户信息
        //private SqlSugarModel.User_Table _mUser_Table;
        //public SqlSugarModel.User_Table mUser_Table
        //{
        //    get { return _mUser_Table; }
        //    set { _mUser_Table = value; RaisePropertyChanged(() => mUser_Table); }
        //}

        //用户名
        private string _UserName;
        [Required(ErrorMessage = "用户名不能为空")]
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; RaisePropertyChanged(() => UserName); }
        }


        //部门列表
        private ObservableCollection<SqlSugarModel.Depart_Table> _Departs;
        public ObservableCollection<SqlSugarModel.Depart_Table> Departs
        {
            get { return _Departs; }
            set { _Departs = value; RaisePropertyChanged(() => Departs); }
        }

        //选中的部门
        private SqlSugarModel.Depart_Table _Selected_Depart;
        [Required(ErrorMessage = "请选择部门")]
        public SqlSugarModel.Depart_Table Selected_Depar
        {
            get { return _Selected_Depart; }
            set { _Selected_Depart = value; RaisePropertyChanged(() => Selected_Depar); }
        }

        //角色列表
        private ObservableCollection<SqlSugarModel.Role_Table> _Roles;
        public ObservableCollection<SqlSugarModel.Role_Table> Roles
        {
            get { return _Roles; }
            set { _Roles = value; RaisePropertyChanged(() => Roles); }
        }

        //选中的角色
        private SqlSugarModel.Role_Table _Selected_Role;
        [Required(ErrorMessage = "请选择角色")]
        public SqlSugarModel.Role_Table Selected_Role
        {
            get { return _Selected_Role; }
            set { _Selected_Role = value; RaisePropertyChanged(() => Selected_Role); }
        }

        //电话
        private string _PhoneNum;
        [Required(ErrorMessage = "电话号码不能为空")]
        [RegularExpression(@"^(1[3-9]\d{9}|(0\d{2,3}-)?[1-9]\d{6,7})$",
        ErrorMessage = "请输入有效的电话号码（手机号11位或固话7-8位）")]
        [Display(Name = "电话号码")]
        public string PhoneNum
        {
            get { return _PhoneNum; }
            set { _PhoneNum = value; RaisePropertyChanged(() => PhoneNum); }
        }

        //邮箱
        private string _Email;
        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        [StringLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
        [Display(Name = "电子邮箱")]
        public string Email
        {
            get { return _Email; }
            set { _Email = value; RaisePropertyChanged(() => Email); }
        }

        //出生日期
        private DateTime _Birthday;
        [Required(ErrorMessage = "出生日期不能为空")]
        public DateTime Birthday
        {
            get { return _Birthday; }
            set { _Birthday = value; RaisePropertyChanged(() => Birthday); }
        }
    }
}
