using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Signet.Common;
using Signet.Model;
using Signet.Properties;
using Signet.SqlSugarModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using static IronPython.Modules.PythonDateTime;
namespace Signet.ViewModel
{
    public class Users_ViewModel : ViewModelBase
    {
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


        #region 分页
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }

        private int _pageSize = 50;
        public int PageSize
        {
            get => _pageSize;
            set => Set(ref _pageSize, value);
        }

        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            set => Set(ref _totalItems, value);
        }

        #endregion


        private Users_Model _mUsers_Model;
        public Users_Model mUsers_Model
        {
            get { return _mUsers_Model; }
            set { _mUsers_Model = value; RaisePropertyChanged(() => mUsers_Model); }
        }

        public Users_ViewModel()
        {
            _dialogCoordinator = DialogCoordinator.Instance;

            mUsers_Model = new Users_Model()
            {
                UserInfoList = new System.Collections.ObjectModel.ObservableCollection<UserInfo_Model>(),
            };

            //DataQuery(1, 50);

            // 监听分页变化
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CurrentPage) ||
                    e.PropertyName == nameof(PageSize))
                {
                    DataQuery(CurrentPage, PageSize);

                    //Console.WriteLine($"Loading page {CurrentPage} with size {PageSize}");
                }
            };
        }


        #region 查询
        private RelayCommand _Search_Command;
        public RelayCommand Search_Command
        {
            get
            {
                if (_Search_Command == null)
                {
                    _Search_Command = new RelayCommand(_Search);
                }
                return _Search_Command;
            }
            set { _Search_Command = value; }
        }
        /// <summary>
        /// 查询
        /// </summary>
        private void _Search()
        {
            if (CurrentPage == 1)
            {
                DataQuery(CurrentPage, PageSize);
            }
            else
            {
                CurrentPage = 1;
            }
        }

        private void DataQuery(int curPage, int pageSize)
        {
            try
            {
                int totalCount = 0;
                int totalPage = 0;

                var userList = 
                SqlSugarHelper.mDB.Queryable<SqlSugarModel.User_Table,SqlSugarModel.Depart_Table,SqlSugarModel.Role_Table>((ut,dt,rt)=>
                new JoinQueryInfos(
                    JoinType.Left,ut.UserDeparID == dt.DepartID,
                    JoinType.Left,ut.UserRoleID == rt.Role_ID
                    ))
                .WhereIF(!String.IsNullOrWhiteSpace(mUsers_Model.QueryConditions),
                (ut, dt, rt) =>
                ut.UserName.Contains(mUsers_Model.QueryConditions)||
                ut.UserCode.Contains(mUsers_Model.QueryConditions) ||
                ut.PhoneNum.Contains(mUsers_Model.QueryConditions) ||
                ut.Email.Contains(mUsers_Model.QueryConditions) ||
                dt.Depart_Name.Contains(mUsers_Model.QueryConditions) ||
                rt.Role_Name.Contains(mUsers_Model.QueryConditions) )
                .OrderBy(ut=>ut.UserID,OrderByType.Asc)
                .Select((ut, dt, rt)=>new UserInfo_Model {
                    //SerialNum = 0,
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
                }).ToPageList(curPage, pageSize, ref totalCount, ref totalPage);
                TotalItems = totalCount;

                // 计算起始行号
                int startRowNum = (curPage - 1) * pageSize + 1;
                // 添加行号
                int currentRowNum = startRowNum;
                foreach (var user in userList)
                {
                    user.SerialNum = currentRowNum;
                    currentRowNum++;
                }
                mUsers_Model.UserInfoList = new System.Collections.ObjectModel.ObservableCollection<UserInfo_Model>(userList);

                logger.Info("日志查询成功！");
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }
        #endregion

        #region 删除
        private RelayCommand<long> _Delete_Command;
        public RelayCommand<long> Delete_Command
        {
            get
            {
                if (_Delete_Command == null)
                {
                    _Delete_Command = new RelayCommand<long>(_Delete);
                }
                return _Delete_Command;
            }
            set { _Delete_Command = value; }
        }
        private void _Delete(long userid)
        {
            var settings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "确认",
                NegativeButtonText = "取消",
            };

            MessageDialogResult result = _dialogCoordinator.ShowModalMessageExternal(this, "是否确认删除该用户？", "",
                                        MessageDialogStyle.AffirmativeAndNegative, settings);
            if (result == MessageDialogResult.Negative)
            {
                return;
            }

            SqlSugarHelper.mDB.Deleteable<SqlSugarModel.User_Table>()
                .Where(a=>a.UserID == userid).ExecuteCommand();

            logger.Info($"删除用户：id={userid}");
            DataQuery(1, 50);
        }
        #endregion

        #region 新增
        private RelayCommand _Insert_Command;
        public RelayCommand Insert_Command
        {
            get
            {
                if (_Insert_Command == null)
                {
                    _Insert_Command = new RelayCommand(_Insert);
                }
                return _Insert_Command;
            }
            set { _Insert_Command = value; }
        }
        private void _Insert()
        {
            WindowManager.ShowDialog("UserManage", new UserMange_ViewModel("Add", -1));

            
            DataQuery(1, 50);
        }
        #endregion

        #region 编辑
        private RelayCommand<long> _Updata_Command;
        public RelayCommand<long> Updata_Command
        {
            get
            {
                if (_Updata_Command == null)
                {
                    _Updata_Command = new RelayCommand<long>(_Updata);
                }
                return _Updata_Command;
            }
            set { _Updata_Command = value; }
        }
        /// <summary>
        /// 编辑
        /// </summary>
        private void _Updata(long userid)
        {
            WindowManager.ShowDialog("UserManage", new UserMange_ViewModel("Edit", userid));
            DataQuery(1, 50);
        }
        #endregion

        #region 修改个人密码
        private RelayCommand _ChangePassword_Command;
        public RelayCommand ChangePassword_Command
        {
            get
            {
                if (_ChangePassword_Command == null)
                {
                    _ChangePassword_Command = new RelayCommand(_ChangePassword);
                }
                return _ChangePassword_Command;
            }
            set { _ChangePassword_Command = value; }
        }
        /// <summary>
        /// 修改个人密码
        /// </summary>
        private void _ChangePassword()
        {
            #region 验证原密码
            LoginDialogData result = _dialogCoordinator.ShowModalLoginExternal(this,"密码验证","请输入原密码！",new LoginDialogSettings { 
            AffirmativeButtonText = "确认",
            NegativeButtonText = "取消",
            NegativeButtonVisibility = System.Windows.Visibility.Visible,
            ShouldHideUsername = true,
            PasswordWatermark = "请输入原密码",
            EnablePasswordPreview = true,
            });
            if (result == null)
            {
                return;
            }

            var userInfo = SqlSugarHelper.mDB.Queryable<SqlSugarModel.User_Table>().
                Where(a => a.UserID == UserInfo.LogedUserInfo.UserID);
            SqlSugarModel.User_Table info = userInfo.First();
            if (!PasswordStorage.VerifyPassword(result.Password, info.Password))
            {
                ShowMessage("提示！", "用户名或密码错误！"); logger.Info("用户名或密码错误！"); return;
            }
            #endregion

            #region 设置新密码
            LoginDialogData result_New = _dialogCoordinator.ShowModalLoginExternal(this, "修改密码", "请输入新密码", new LoginDialogSettings
            {
                AffirmativeButtonText = "确认",
                NegativeButtonText = "取消",
                NegativeButtonVisibility = System.Windows.Visibility.Visible,
                ShouldHideUsername = true,
                PasswordWatermark = "请输入新密码",
                EnablePasswordPreview = true,
            });

            if (result_New == null)
            {
                return;
            }

            if (result_New.Password.Trim() == string.Empty)
            { ShowMessage("提示！", "请输入新密码！"); return ; }

            string newPassword = PasswordStorage.HashPassword(result_New.Password);
            SqlSugarHelper.mDB.Updateable<SqlSugarModel.User_Table>()
                .SetColumns(a=>a.Password == newPassword)
                .Where(a=>a.UserID == UserInfo.LogedUserInfo.UserID)
                .ExecuteCommand();
            ShowMessage("提示！", "密码修改成功！");
            #endregion
        }
        #endregion
    }
}
