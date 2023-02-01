using System.Globalization;

namespace SSI_Holder.Converters
{
    internal class AttributeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = value.ToString();

            switch (value as string)
            {
                case "student_name":
                    result = "Name des Studenten";
                    break;
                case "abschlussnote":
                    result = "Abschlussnote";
                    break;
                case "studiengang":
                    result = "Studiengang";
                    break;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
