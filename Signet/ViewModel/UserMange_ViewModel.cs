using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Signet.Common;
using Signet.Model;
using Signet.SqlSugarModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Signet.ViewModel
{
    public class UserMange_ViewModel : ViewModelBase
    {

        private UserManage_Model _mUserManage_Model;

        public UserManage_Model mUserManage_Model
        {
            get { return _mUserManage_Model; }
            set { _mUserManage_Model = value; RaisePropertyChanged(() => mUserManage_Model); }
        }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region Message
        private readonly IDialogCoordinator _dialogCoordinator;
        // Simple method which can be used on a Button
        public async void ShowMessage_async(string title, string msg)
        {
            await _dialogCoordinator.ShowMessageAsync(this, title, msg);
        }

        public void ShowMessage(string title, string msg)
        {
            _dialogCoordinator.ShowModalMessageExternal(this, title, msg);
        }

        #endregion


        private string OptionType;
        private long UserID;
        public UserMange_ViewModel(string optiontype,long userid)
        {
            _dialogCoordinator = DialogCoordinator.Instance;

            OptionType = optiontype;
            UserID = userid;
            mUserManage_Model = new UserManage_Model()
            {
                Departs = new System.Collections.ObjectModel.ObservableCollection<Depart_Table>
                (SqlSugarHelper.mDB.Queryable<SqlSugarModel.Depart_Table>().ToList()),
                Roles = new System.Collections.ObjectModel.ObservableCollection<Role_Table>
                (SqlSugarHelper.mDB.Queryable<SqlSugarModel.Role_Table>().ToList()),
            };

            if (optiontype == "Add")
            {
                mUserManage_Model.Header = "用户信息新增";

                mUserManage_Model.UserName = string.Empty;
                mUserManage_Model.Birthday = new DateTime(2001, 1, 1);
                mUserManage_Model.PhoneNum = string.Empty;
                mUserManage_Model.Email = string.Empty;
            }
            else if (optiontype == "Edit")
            {
                mUserManage_Model.Header = "用户信息编辑";

                var userInfo =
                SqlSugarHelper.mDB.Queryable<SqlSugarModel.User_Table, SqlSugarModel.Depart_Table, SqlSugarModel.Role_Table>((ut, dt, rt) =>
                new JoinQueryInfos(
                    JoinType.Left, ut.UserDeparID == dt.DepartID,
                    JoinType.Left, ut.UserRoleID == rt.Role_ID
                    ))
                    .Where(ut=>ut.UserID == UserID)
                    .Select((ut, dt, rt) => new UserInfo_Model
                    {
                        UserID = ut.UserID,
                        UserCode = ut.UserCode,
                        UserName = ut.UserName,
                        PhoneNum = ut.PhoneNum,
                        Email = ut.Email,
                        Birthday = ut.Birthday,
                        DepartID = dt.DepartID,
                        DepartmentName = dt.Depart_Name,
                        RoleID = rt.Role_ID,
                        RoleCode = rt.Role_Code,
                        RoleName = rt.Role_Name,
                    }).ToList().FirstOrDefault();

                mUserManage_Model.UserName = userInfo.UserName;
                mUserManage_Model.Selected_Depar = mUserManage_Model.Departs.Where(a=>a.DepartID == userInfo.DepartID).First();
                mUserManage_Model.Selected_Role = mUserManage_Model.Roles.Where(a => a.Role_ID == userInfo.RoleID).First();
                mUserManage_Model.Birthday = userInfo.Birthday;
                mUserManage_Model.PhoneNum = userInfo.PhoneNum;
                mUserManage_Model.Email = userInfo.Email;
            }
        }

        #region 确认
        private RelayCommand _Confirm_Command;
        public RelayCommand Confirm_Command
        {
            get
            {
                if (_Confirm_Command == null)
                {
                    _Confirm_Command = new RelayCommand(_Confirm);
                }
                return _Confirm_Command;
            }
            set { _Confirm_Command = value; }
        }
        /// <summary>
        /// 确认
        /// </summary>
        private void _Confirm()
        {
            if (!mUserManage_Model.IsValidated)
            {
                ShowMessage("提示!", mUserManage_Model.dataErrors.First().Value);
                logger.Info(mUserManage_Model.dataErrors.First().Value);
                return;
            }

            if (OptionType == "Add")
            {
                SqlSugarModel.User_Table user_Table = new SqlSugarModel.User_Table()
                {
                    UserName = mUserManage_Model.UserName,
                    UserDeparID = mUserManage_Model.Selected_Depar.DepartID,
                    Birthday = mUserManage_Model.Birthday,
                    Password = PasswordStorage.HashPassword("123456"),
                    UserRoleID = mUserManage_Model.Selected_Role.Role_ID,
                    PhoneNum = mUserManage_Model.PhoneNum,
                    Email = mUserManage_Model.Email,
                };

                #region 使用事务获取用户编码
                string usercode = string.Empty;
                using (var db = SqlSugarHelper.mDB)
                {
                    SqlSugarHelper.mDB.BeginTran();

                    int userid = SqlSugarHelper.mDB.Insertable(user_Table).ExecuteReturnIdentity();
                    usercode = $"C{userid:D4}";
                    db.Updateable<User_Table>()
                        .SetColumns(u => u.UserCode, usercode)
                        .Where(u => u.UserID == userid)
                        .ExecuteCommand();

                    db.CommitTran();
                }
                _dialogCoordinator.ShowModalInputExternal(this, "用户新增成功！", "新用户的用户唯一用户编码为", new MetroDialogSettings
                {
                    DefaultText = usercode,
                });

                #endregion
            }
            else if (OptionType == "Edit")
            {
                SqlSugarModel.User_Table user_Table = new SqlSugarModel.User_Table()
                {
                    UserID = UserID,
                    UserName = mUserManage_Model.UserName,
                    UserDeparID = mUserManage_Model.Selected_Depar.DepartID,
                    Birthday = mUserManage_Model.Birthday,
                    UserRoleID = mUserManage_Model.Selected_Role.Role_ID,
                    PhoneNum = mUserManage_Model.PhoneNum,
                    Email = mUserManage_Model.Email,
                };

                SqlSugarHelper.mDB.Updateable(user_Table)
                    .IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand();
            }
            ToClose = true;
        }

        #endregion

        #region 取消
        private RelayCommand _Cancel_Command;
        public RelayCommand Cancel_Command
        {
            get
            {
                if (_Cancel_Command == null)
                {
                    _Cancel_Command = new RelayCommand(_Cancel);
                }
                return _Cancel_Command;
            }
            set { _Cancel_Command = value; }
        }
        /// <summary>
        /// 取消
        /// </summary>
        private void _Cancel()
        {
            ToClose = true;
        }

        #endregion

        #region 关闭窗口函数
        private bool toClose = false;
        /// <summary>
        /// 是否要关闭窗口
        /// </summary>
        public bool ToClose
        {
            get
            {
                return toClose;
            }
            set
            {
                toClose = value;
                if (toClose)
                {
                    this.RaisePropertyChanged("ToClose");
                }
            }
        }
        #endregion
    }
}
