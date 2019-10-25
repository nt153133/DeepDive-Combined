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
using Deep.Properties;

namespace Deep.Forms.Converter
{
    [MarkupExtensionReturnType(typeof(IEnumerable<object>))]
    public sealed class EnumerateExtension : MarkupExtension
    {
        #region MarkupExtension Members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Type == null) throw new InvalidOperationException(Resources.EnumTypeNotSet);

            var actualType = Nullable.GetUnderlyingType(Type) ?? Type;
            TypeConverter typeConverter;
            ICollection standardValues;

            if ((typeConverter = TypeDescriptor.GetConverter(actualType)) == null ||
                (standardValues = typeConverter.GetStandardValues(serviceProvider as ITypeDescriptorContext)) == null)
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.TypeHasNoStandardValues,
                        Type
                    ),
                    "value"
                );

            var items = Type == actualType
                ? new object[standardValues.Count]
                : new object[standardValues.Count + 1];
            var index = 0;

            if (Converter == null)
            {
                foreach (var standardValue in standardValues)
                {
                    items[index++] = standardValue;
                }
            }
            else
            {
                var culture = ConverterCulture ?? GetCulture(serviceProvider);

                foreach (var standardValue in standardValues)
                {
                    items[index++] = Converter.Convert(standardValue, typeof(object), ConverterParameter, culture);
                }

                if (Type != actualType) items[index] = Converter.Convert(null, typeof(object), ConverterParameter, culture);
            }

            return items;
        }

        #endregion

        #region Private Methods

        private static CultureInfo GetCulture(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

                if (provideValueTarget != null)
                {
                    var targetObject = provideValueTarget.TargetObject as DependencyObject;
                    XmlLanguage language;

                    if ((targetObject = provideValueTarget.TargetObject as DependencyObject) != null &&
                        (language = (XmlLanguage) targetObject.GetValue(FrameworkElement.LanguageProperty)) != null)
                        return language.GetSpecificCulture();
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
            if (type == null) throw new ArgumentNullException("type");

            Type = type;
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