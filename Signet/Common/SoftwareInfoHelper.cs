using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
namespace Signet.Common
{
    public class SoftwareInfoHelper
    {
        //软件名称
        public static readonly string SoftwareName = "XXX管理软件";
        //icon路径
        public static readonly string SoftwareLogoIconPath = "pack://application:,,,/Signet;component/Images/Logo.ico";
        //图标路径
        public static readonly string SoftwareLogoPath = "pack://application:,,,/Signet;component/Images/Logo.png";
        //public static BitmapImage SoftwareLogoImg = 
        //    new BitmapImage(new Uri(SoftwareLogoPath));
        //版权
        public static readonly string Copyright = "版权所有 © 浙江XXX科技股份有限公司";
        //联系方式
        public static readonly string ContactInfo = "联系方式：嘉兴市南湖区凌公塘路3439号";
        //公司名称
        public static readonly string Company = "浙江XXX科技股份有限公司";
        //版本号
        public static readonly string Version = ConfigFileHelper.ConfigRead("Ver 1.00");
        //默认头像路径
        public static string UserHeadSculpturePath = Application.StartupPath + @"\Imgs\Material-AccountTie.png";
    }
}
