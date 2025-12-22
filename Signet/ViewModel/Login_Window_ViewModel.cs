using Signet.Common;
using Signet.Model;
using Signet.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using NLog.Targets;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Signet.ViewModel
{
    public class Login_Window_ViewModel: ViewModelBase
    {

        private Login_Window_Model _mLogin_Window_Model;

        public Login_Window_Model mLogin_Window_Model
        {
            get { return _mLogin_Window_Model; }
            set { _mLogin_Window_Model = value; RaisePropertyChanged(() => mLogin_Window_Model); }
        }

        //创建新的 Logger 的开销很小，因为它必须获取锁并分配对象。
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Login_Window_ViewModel()
        {
            #region 热生成NLOG
            var databaseTarget = (DatabaseTarget)LogManager.Configuration.FindTargetByName("SqlServer_Log");
            databaseTarget.ConnectionString = Common.SqlSugarHelper.ConnectionString;
            LogManager.ReconfigExistingLoggers();
            #endregion

            #region 窗口注册
            WindowManager.Register<MainWindow>("MainWindow");
            WindowManager.Register<UserManage_Window>("UserManage");
            #endregion

            _dialogCoordinator = DialogCoordinator.Instance;

           //MappedDiagnosticsLogicalContext.Set("userId", string.Empty);

            mLogin_Window_Model = new Login_Window_Model()
            {
                SoftTitle = string.Join(" ",SoftwareInfoHelper.SoftwareName.ToCharArray()),
                UserCode = "C0001",
                Password = "123456",
            };

            //RegisterHelper.GenerateCryptographFile();
        }

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


        #region 登录
        public class UserInfoResult
        {
            public int uid { get; set; }
            public string ucode { get; set; }
            public string uname { get; set; }
            public string uphonenum { get; set; }
            public string uemail { get; set; }
            public DateTime ubirthday { get; set; }
            public int udeparid { get; set; }
            public string udeparname { get; set; }
            public int uroleid { get; set; }
            public string urolecode { get; set; }
            public string urolename { get; set; }
            public string atauthority { get; set; }
        }

        private RelayCommand _Login_Command;
        public RelayCommand Login_Command
        {
            get
            {
                if (_Login_Command == null)
                {
                    _Login_Command = new RelayCommand(_Login);
                }
                return _Login_Command;
            }
            set { _Login_Command = value; }
        }

        private void _Login()
        {
            #region 卫语句
            //bool needLicense = false;
            ////软件授权检测
            //if (RegisterHelper.ExistLicenseFile())
            //{
            //    string mes = string.Empty;

            //    if (RegisterHelper.CheckLicense(RegisterHelper.ReadLicenseFile(), out mes))
            //    {
            //        needLicense = false;
            //    }
            //    else
            //    {
            //        needLicense = true;
            //        ShowMessage("提示！", mes);
            //        logger.Info(mes);
            //    }
            //}
            //else
            //{
            //    needLicense = true;
            //}
            //GlobalInfo.registerState = !needLicense;

            //if (needLicense)
            //{
            //    #region 输入证书弹窗
            //    var result = _dialogCoordinator.ShowModalLoginExternal(this, "未激活", "请输入您的激活码！",
            //    new LoginDialogSettings(((Login_Window)(Application.Current.MainWindow)).MetroDialogOptions)
            //    {
            //        InitialUsername = RegisterHelper.ReadCryptographFile(),
            //        UsernameWatermark = "设备编码",
            //        EnablePasswordPreview = true,
            //        PasswordWatermark = "请输入激活码",
            //        AffirmativeButtonText = "激活",
            //    });
            //    #endregion

            //    string mes = string.Empty;
            //    GlobalInfo.registerState =
            //    RegisterHelper.CheckLicense(result.Password, out mes);
            //    ShowMessage("提示！", mes);
            //    logger.Info(mes);
            //    if (!GlobalInfo.registerState)
            //    {
            //        return;
            //    }
            //}

            if (mLogin_Window_Model.UserCode == string.Empty
                || mLogin_Window_Model.Password == string.Empty
                || mLogin_Window_Model.UserCode == null
                || mLogin_Window_Model.Password == null)
            { ShowMessage("提示！", "用户名或密码不能为空！"); logger.Info("用户名或密码不能为空！"); return; }

            var userInfo = SqlSugarHelper.mDB.Queryable<SqlSugarModel.User_Table>().
                Where(a => a.UserCode == mLogin_Window_Model.UserCode);
            if (userInfo.Count() == 0 || userInfo == null)
            {
                ShowMessage("提示！", "用户名或密码错误！"); logger.Info("用户名或密码错误！"); return;
            }

            SqlSugarModel.User_Table info = userInfo.First();
            if (!PasswordStorage.VerifyPassword(mLogin_Window_Model.Password, info.Password))
            {
                ShowMessage("提示！", "用户名或密码错误！"); logger.Info("用户名或密码错误！"); return;
            }
            #endregion

            var query = @"
            SELECT 
                u.UserID as uid,
                u.UserCode as ucode,
                u.UserName as uname,
                u.PhoneNum as uphonenum,
                u.Email as uemail,
                u.Birthday as ubirthday,
                d.DepartID as udeparid,
                d.Depart_Name AS udeparname,
                r.Role_ID as uroleid,
                r.Role_Code AS urolecode,
                r.Role_Name AS urolename,
                STUFF((
                    SELECT ',' + a.Authority_Name
                    FROM Authorities_Table a
                    INNER JOIN Role_Auth_Relationship_Table rar ON a.Authorities_ID = rar.Authority_ID
                    WHERE rar.Role_ID = r.Role_ID
                    AND a.IsEffect = 1
                    FOR XML PATH('')
                ), 1, 1, '') AS atauthority
            FROM User_Table u
            INNER JOIN Depart_Table d ON u.UserDeparID = d.DepartID
            INNER JOIN Role_Table r ON u.UserRoleID = r.Role_ID
            WHERE u.UserCode = @UserCode;";

            var uinfo = SqlSugarHelper.mDB.Ado.SqlQuery<UserInfoResult>(query, new { UserCode = info.UserCode }).ToList().FirstOrDefault();

            UserInfo.LogedUserInfo = new UserInfo_Model
            {
                UserID = uinfo.uid,
                UserCode = uinfo.ucode,
                UserName = uinfo.uname,
                PhoneNum = uinfo.uphonenum,
                Email = uinfo.uemail,
                Birthday = uinfo.ubirthday,
                DepartID = uinfo.udeparid,
                DepartmentName = uinfo.udeparname,
                RoleID = uinfo.uroleid,
                RoleCode = uinfo.urolecode,
                RoleName = uinfo.urolename,
                Authorities = uinfo.atauthority == null ? new List<string>() : uinfo.atauthority.Split(',').ToList(),
            };

            AuthorizationManager.Instance.LoadUserPermission(UserInfo.LogedUserInfo.RoleID);

            //传参给nlog配置文件
            MappedDiagnosticsLogicalContext.Set("userId", info.UserID);
            MappedDiagnosticsLogicalContext.Set("userName", info.UserName);

            logger.Info("登录成功！");

            //关闭本窗口打开新窗口
            WindowManager.Show("MainWindow", new MainWindow_ViewModel());
            mLogin_Window_Model.FormVisibility = Visibility.Hidden;

            //ToClose = true;
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
