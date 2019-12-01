/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using DeepCombined.Properties;

namespace DeepCombined.Forms.Converter
{
    [ValueConversion(typeof(Enum), typeof(string))]
    public sealed class EnumToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            CheckSourceType(typeof(Enum), value);
            CheckTargetType(typeof(string), targetType, true);

            var valueType = value.GetType();
            var fieldInfo = valueType.GetField(value.ToString(), BindingFlags.Static | BindingFlags.Public);

            if (fieldInfo == null) throw new ArgumentException(Resources.BitFieldsNotSupported, "value");

            var attributes = (LocalizedDescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(LocalizedDescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            return fieldInfo.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            CheckSourceType(typeof(string), value);
            CheckTargetType(typeof(Enum), targetType, false);

            var str = (string) value;

            foreach (var fieldInfo in targetType.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (fieldInfo.Name == str) return fieldInfo.GetValue(null);

                var attributes = (LocalizedDescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(LocalizedDescriptionAttribute), false);

                foreach (var attribute in attributes)
                {
                    if (attribute.Description == str) return fieldInfo.GetValue(null);
                }
            }

            throw new ArgumentException(string.Format(Resources.EnumValueNotFound, str), "value");
        }

        #endregion

        #region Private Methods

        private static void CheckSourceType(Type supportedSourceType, object value)
        {
            if (!supportedSourceType.IsInstanceOfType(value)) throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ValueNotOfType, supportedSourceType.Name), "value");
        }

        private static void CheckTargetType(Type supportedTargeType, Type requestedTargetType, bool covariance)
        {
            if (covariance)
            {
                if (!requestedTargetType.IsAssignableFrom(supportedTargeType))
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.TargetNotExtendingType, requestedTargetType.Name, supportedTargeType.Name), "targetType");
            }
            else
            {
                if (!supportedTargeType.IsAssignableFrom(requestedTargetType))
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.TargetNotExtendingType, requestedTargetType.Name, supportedTargeType.Name), "targetType");
            }
        }

        #endregion
    }
}