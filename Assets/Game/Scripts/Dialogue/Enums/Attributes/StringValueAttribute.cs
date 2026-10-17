using System;

namespace Assets.Game.Scripts.Dialogue.Enums.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class StringValueAttribute : Attribute
    {
        public string Value { get; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }

        public static string GetStringValue(Enum enumValue)
        {
            var value = enumValue.GetAttributeOfType<StringValueAttribute>();
            if (value == null)
            {
                return enumValue.ToString();
            }
            return value.Value;
        }
    }

    public static class EnumUtils
    {
        public static T GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
        {
            var type = enumValue.GetType();
            var memInfo = type.GetMember(enumValue.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
    }
}
