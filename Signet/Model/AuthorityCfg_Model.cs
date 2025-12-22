using GalaSoft.MvvmLight;
using Signet.SqlSugarModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Signet.Model
{
    public class AuthoritiesWithCheck
    {
        public bool IsCheck { get; set; }
        public long Authorities_ID { get; set; }

        public string Description { get; set; }

        public string Authority_Name { get; set; }

        public bool? IsEffect { get; set; }

    }

    public class AuthorityCfg_Model : ObservableObject
    {
        //角色列表
        private ObservableCollection<SqlSugarModel.Role_Table> _Roles;
        public ObservableCollection<SqlSugarModel.Role_Table> Roles
        {
            get { return _Roles; }
            set { _Roles = value; RaisePropertyChanged(() => Roles); }
        }

        //选中的角色
        private SqlSugarModel.Role_Table _Selected_Role;
        public SqlSugarModel.Role_Table Selected_Role
        {
            get { return _Selected_Role; }
            set { _Selected_Role = value; RaisePropertyChanged(() => Selected_Role); }
        }

        //权限列表
        private ObservableCollection<SqlSugarModel.Authorities_Table> _Authorities;
        public ObservableCollection<SqlSugarModel.Authorities_Table> Authorities
        {
            get { return _Authorities; }
            set { _Authorities = value; RaisePropertyChanged(() => Authorities); }
        }

        //权限列表带勾选框
        private ObservableCollection<AuthoritiesWithCheck> _AuthoritiesWithCheck;
        public ObservableCollection<AuthoritiesWithCheck> AuthoritiesWithCheck
        {
            get { return _AuthoritiesWithCheck; }
            set { _AuthoritiesWithCheck = value; RaisePropertyChanged(() => AuthoritiesWithCheck); }
        }
    }
}
