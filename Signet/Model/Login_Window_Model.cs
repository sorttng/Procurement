using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
namespace Signet.Model
{
    public class Login_Window_Model: ObservableObject
    {
        private string _UserCode;
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空！")]
        public string UserCode
        {
            get { return _UserCode; }
            set { _UserCode = value; RaisePropertyChanged(() => UserCode); }
        }

        private string _Password=string.Empty;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set { _Password = value; RaisePropertyChanged(() => Password); }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(this.UserCode) && this.UserCode == string.Empty)
                {
                    return "Number is not greater than 10!";
                }

                return null;
            }
        }


        private Visibility _FormVisibility = Visibility.Visible;
        /// <summary>
        /// 窗体是否可见
        /// </summary>
        public Visibility FormVisibility
        {
            get { return _FormVisibility; }
            set { _FormVisibility = value; RaisePropertyChanged(() => FormVisibility); }
        }

        private string _SoftTitle = string.Empty;
        /// <summary>
        /// 软件标题
        /// </summary>
        public string SoftTitle
        {
            get { return _SoftTitle; }
            set { _SoftTitle = value; RaisePropertyChanged(() => SoftTitle); }
        }

    }
}
