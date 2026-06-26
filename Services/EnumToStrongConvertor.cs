namespace WinUI_installer.Services
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value == null ? null : Enum.Parse(targetType, value.ToString());
        }
    }
}
