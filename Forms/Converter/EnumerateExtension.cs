/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using DeepCombined.Properties;

namespace DeepCombined.Forms.Converter
{
    [MarkupExtensionReturnType(typeof(IEnumerable<object>))]
    public sealed class EnumerateExtension : MarkupExtension
    {
        #region MarkupExtension Members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Type == null)
            {
                throw new InvalidOperationException(Resources.EnumTypeNotSet);
            }

            Type actualType = Nullable.GetUnderlyingType(Type) ?? Type;
            TypeConverter typeConverter;
            ICollection standardValues;

            if ((typeConverter = TypeDescriptor.GetConverter(actualType)) == null ||
                (standardValues = typeConverter.GetStandardValues(serviceProvider as ITypeDescriptorContext)) == null)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.TypeHasNoStandardValues,
                        Type
                    ),
                    "value"
                );
            }

            object[] items = Type == actualType
                ? new object[standardValues.Count]
                : new object[standardValues.Count + 1];
            int index = 0;

            if (Converter == null)
            {
                foreach (object standardValue in standardValues)
                {
                    items[index++] = standardValue;
                }
            }
            else
            {
                CultureInfo culture = ConverterCulture ?? GetCulture(serviceProvider);

                foreach (object standardValue in standardValues)
                {
                    items[index++] = Converter.Convert(standardValue, typeof(object), ConverterParameter, culture);
                }

                if (Type != actualType)
                {
                    items[index] = Converter.Convert(null, typeof(object), ConverterParameter, culture);
                }
            }

            return items;
        }

        #endregion

        #region Private Methods

        private static CultureInfo GetCulture(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget)
                {
                    XmlLanguage language;

                    if (provideValueTarget.TargetObject is DependencyObject targetObject &&
                        (language = (XmlLanguage)targetObject.GetValue(FrameworkElement.LanguageProperty)) != null)
                    {
                        return language.GetSpecificCulture();
                    }
                }
            }

            return null;
        }

        #endregion

        #region Fields

        #endregion

        #region Constructors

        public EnumerateExtension()
        {
        }

        public EnumerateExtension(Type type)
        {
            Type = type ?? throw new ArgumentNullException("type");
        }

        #endregion

        #region Properties

        [ConstructorArgument("type")] public Type Type { get; set; }

        public IValueConverter Converter { get; set; }

        public CultureInfo ConverterCulture { get; set; }

        public object ConverterParameter { get; set; }

        #endregion
    }
}