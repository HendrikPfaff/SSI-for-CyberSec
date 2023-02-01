using SSI_Holder.Models;
using System.Globalization;

namespace SSI_Holder.Converters
{
    internal class ConnectionStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is ConnectionState state)
            {
                return Enum.GetName(state);
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
