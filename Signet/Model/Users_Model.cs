using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using Signet.SqlSugarModel;
namespace Signet.Model
{
    public class Users_Model : ObservableObject
    {
        /// <summary>
        /// 查询条件
        /// </summary>
        private string _QueryConditions;
        public string QueryConditions
        {
            get { return _QueryConditions; }
            set { _QueryConditions = value; RaisePropertyChanged(() => QueryConditions); }
        }

        /// <summary>
        /// 用户信息列表
        /// </summary>
        private ObservableCollection<UserInfo_Model> _UserInfoList;
        public ObservableCollection<UserInfo_Model> UserInfoList
        {
            get { return _UserInfoList; }
            set { _UserInfoList = value; RaisePropertyChanged(() => UserInfoList); }
        }
    }
}
