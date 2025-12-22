using System;
using System.Collections.Generic;
using System.Linq;
using MahApps.Metro.IconPacks;
using Signet.Mvvm;
using Signet.View;
using MahApps.Metro.Controls;
using MenuItem = Signet.ViewModel.MenuItem;
using System.Windows;
using System.Windows.Navigation;
using GalaSoft.MvvmLight;
using Signet.Model;
using Signet.Common;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using NLog;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Modbus.Data;
using Modbus.Device;
using Modbus.Serial;
using Modbus.Utility;
using NLog.Targets;
using System.IO;
namespace Signet.ViewModel
{
    public class MainWindow_ViewModel : ViewModelBase//BindableBase
    {
        //创建新的 Logger 的开销很小，因为它必须获取锁并分配对象。
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ObservableCollection<MenuItem> Menu { get; } = new ObservableCollection<MenuItem>();

        public ObservableCollection<MenuItem> OptionsMenu { get; } = new ObservableCollection<MenuItem>();
        public string Title { get; } = SoftwareInfoHelper.SoftwareName;

        public Main_Model _mMain_Model;
        public Main_Model mMain_Model
        {
            get { return _mMain_Model; }
            set { _mMain_Model = value; RaisePropertyChanged(() => mMain_Model); }
        }

        //[Obsolete]
        public MainWindow_ViewModel()
        {
            mMain_Model = new Main_Model()
            {
                Company = SoftwareInfoHelper.Company,
                SoftwareLogoIconPath = SoftwareInfoHelper.SoftwareLogoIconPath,
                UserName = UserInfo.LogedUserInfo.UserName,
            };

            _dialogCoordinator = DialogCoordinator.Instance;

            #region 添加菜单
            // Build the menus
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.HouseMedicalSolid },
                Label = "主页",
                NavigationType = typeof(Home_View),
                NavigationDestination = new Uri("View/Home_View.xaml", UriKind.RelativeOrAbsolute)
            });


            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.UserInjuredSolid },
                Label = "用户管理",
                NavigationType = typeof(Users_View),
                NavigationDestination = new Uri("View/Users_View.xaml", UriKind.RelativeOrAbsolute)
            });

            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.SquarePersonConfinedSolid},
                Label = "权限配置",
                NavigationType = typeof(AuthorityCfg_View),
                NavigationDestination = new Uri("View/AuthorityCfg_View.xaml", UriKind.RelativeOrAbsolute)
            });


            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.FileLinesSolid},
                Label = "日志查询",
                NavigationType = typeof(Log_View),
                NavigationDestination = new Uri("View/Log_View.xaml", UriKind.RelativeOrAbsolute)
            });



            this.OptionsMenu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.GearSolid },
                Label = "设置",
                NavigationType = typeof(Setting_View),
                NavigationDestination = new Uri("View/Setting_View.xaml", UriKind.RelativeOrAbsolute)
            });

            this.OptionsMenu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.InfoSolid },
                Label = "关于",
                NavigationType = typeof(Setting_View),
                NavigationDestination = new Uri("View/About_View.xaml", UriKind.RelativeOrAbsolute)
            });
            #endregion

            #region 获取地区列表
            //ObservableCollection<SqlSugarModel.Location_Table> locations = 
            //    new ObservableCollection<SqlSugarModel.Location_Table>(
            //    SqlSugarHelper.mDB.Queryable<SqlSugarModel.Location_Table>().ToList());
            //GlobalInfo.Locations = locations;
            #endregion

            #region 读取配置文件
            mMain_Model.UserHeadSculpturePath = ConfigFileHelper.ConfigRead("UserHeadSculpturePath") == string.Empty ?
                SoftwareInfoHelper.UserHeadSculpturePath : ConfigFileHelper.ConfigRead("UserHeadSculpturePath");

            ThemeManager.Current.ChangeThemeBaseColor(System.Windows.Application.Current,
                Theme_Model.Theme = ConfigFileHelper.ConfigRead("Theme"));

            ThemeManager.Current.ChangeThemeColorScheme(System.Windows.Application.Current,
                Theme_Model.Colors = ConfigFileHelper.ConfigRead("Colors"));

            ////如果没填默认北京
            //GlobalInfo.CurLocation = ConfigFileHelper.ConfigRead("Location_ID") == string.Empty ?
            //    locations.Where(a => a.LocationId == "101010100").First() : 
            //    locations.Where(a => a.LocationId == ConfigFileHelper.ConfigRead("Location_ID")).First();
            #endregion

            //StartModbusTcpSlave();//开启modbus服务
            //logger.Info("开启modbus服务成功！");
            logger.Info("系统启动成功！");
        }


        #region 界面开关
        private RelayCommand windowLoaded;

        public RelayCommand WindowLoaded
        {
            get
            {
                if (windowLoaded == null)
                {
                    windowLoaded = new RelayCommand(Loaded);
                }
                return windowLoaded;
            }
            set { windowLoaded = value; }
        }

        private void Loaded()
        {
            //StartModbusTcpSlave();
        }

        private RelayCommand windowClosed;

        public RelayCommand WindowClosed
        {
            get
            {
                if (windowClosed == null)
                {
                    windowClosed = new RelayCommand(Closed);
                }
                return windowClosed;
            }
            set { windowClosed = value; }
        }

        private void Closed()
        {
            logger.Info("系统已退出！");
        }


        private RelayCommand<System.ComponentModel.CancelEventArgs> windowClosing;

        public RelayCommand<System.ComponentModel.CancelEventArgs> WindowClosing
        {
            get
            {
                if (windowClosing == null)
                {
                    windowClosing = new RelayCommand<System.ComponentModel.CancelEventArgs>(Closing);
                }
                return windowClosing;
            }
            set { windowClosing = value; }
        }

        private void Closing(System.ComponentModel.CancelEventArgs e)
        {

        }        
        #endregion

        #region 更换头像
        private RelayCommand _HeadSculptureChange_Command;
        public RelayCommand HeadSculptureChange_Command
        {
            get
            {
                if (_HeadSculptureChange_Command == null)
                {
                    _HeadSculptureChange_Command = new RelayCommand(_HeadSculptureChange);
                }
                return _HeadSculptureChange_Command;
            }
            set { _HeadSculptureChange_Command = value; }
        }

        private void _HeadSculptureChange()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "png (*.png)|*.png|jpg (*.jpg)|*.jpg";                
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    try
                    {
                        string dstPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "Imgs", Path.GetFileName(selectedFilePath));
                        File.Copy(selectedFilePath, dstPath);

                        mMain_Model.UserHeadSculpturePath = dstPath;
                        ConfigFileHelper.ConfigSet("UserHeadSculpturePath", dstPath);
                    }
                    catch
                    {
                        ConfigFileHelper.ConfigSet("UserHeadSculpturePath", string.Empty);
                        FooMessage("提示！","所选文件被占用或者格式不支持!");
                    }
                }
            }
        }

        #endregion

        #region 登录
        private RelayCommand _LogIn_Command;
        public RelayCommand LogIn_Command
        {
            get
            {
                if (_LogIn_Command == null)
                {
                    _LogIn_Command = new RelayCommand(_LogIn);
                }
                return _LogIn_Command;
            }
            set { _LogIn_Command = value; }
        }

        private void _LogIn()
        {
        }
        #endregion

        #region Message
        private readonly IDialogCoordinator _dialogCoordinator;
        // Simple method which can be used on a Button
        public async void FooMessage(string title, string msg)
        {
            await _dialogCoordinator.ShowMessageAsync(this, title, msg);
        }
        #endregion
    }
}
