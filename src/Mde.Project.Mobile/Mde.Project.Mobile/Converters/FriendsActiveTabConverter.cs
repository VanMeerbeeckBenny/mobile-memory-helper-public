using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mde.Project.Mobile.Converters
{
    public class FriendsActiveTabConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType() == typeof(bool))
            {
                bool isActive = (bool)value;
                return isActive ? "#90d6e8" : "#578a96";
            }
            else
            {
                throw new ArgumentException("Expected bool as value", "value");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
