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

        // 可配置的阈值
        public double LowThreshold { get; set; } = 10;
        public double HighThreshold { get; set; } = 100;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NumericToColorConverter : IValueConverter
    {
        // 可配置的阈值
        public double LowThreshold { get; set; } = 0;
        public double HighThreshold { get; set; } = 100;

        // 可配置的颜色
        public Brush LowValueBrush { get; set; } = Brushes.Red;      // 低值：红色
        public Brush NormalValueBrush { get; set; } = Brushes.Yellow; // 正常值：黄色
        public Brush HighValueBrush { get; set; } = null;            // 高值：不设置特殊颜色（使用默认）

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return NormalValueBrush;

            double numericValue;
            if (value is IConvertible convertible)
            {
                numericValue = convertible.ToDouble(culture);
            }
            else
            {
                return NormalValueBrush;
            }

            if (numericValue < LowThreshold)
                return LowValueBrush;        // 低于阈值：红色
            else if (numericValue <= HighThreshold)
                return NormalValueBrush;     // 正常范围：黄色
            else
                return HighValueBrush;       // 高于阈值：返回null（使用控件默认颜色）
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
