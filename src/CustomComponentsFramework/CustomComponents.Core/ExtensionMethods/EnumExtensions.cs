using System;
using System.ComponentModel;

namespace CustomComponents.Core.ExtensionMethods
{
    public static class EnumExtensions
    {
        /// <summary>
        ///     If enum is annotated with the Description attribute, returns the description,
        ///     otherwise returns the ToString()
        /// </summary>
        public static string Description(this Enum value)
        {
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute),
                                                       false);
            return attributes.Length == 0
                ? value.ToString()
                : ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}