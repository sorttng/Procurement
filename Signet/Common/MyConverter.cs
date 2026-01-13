using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using Signet.Model;
using System.Windows.Media;
namespace Signet.Common
{
    public class AuthConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (UserInfo.LogedUserInfo.Authorities == null)
                return false;
            string machAu = value.ToString();
            if (machAu == string.Empty)
                return true;//如果该控件权限为空的话则全部有权限访问

            if (UserInfo.LogedUserInfo.Authorities.Contains(machAu))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AlbumInventoryConverter : IValueConverter
    {
        // 可配置的阈值
        //public int LowThreshold { get; set; } = GlobalInfo.InventoryThreshold;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                return false;
            int val = (int)value;
            if(val<= GlobalInfo.InventoryThreshold)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
