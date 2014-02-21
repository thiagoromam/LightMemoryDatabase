using System;
using System.Linq;
using System.Reflection;

namespace LightMemoryDatabase.Helpers
{
    internal static class MemberHelper
    {
        public static object GetValue(this PropertyInfo member, object obj)
        {
            return member.GetValue(obj, null);
        }

        public static void SetValue(this PropertyInfo member, object obj, object value)
        {
            member.SetValue(obj, value, null);
        }

        public static bool IsBackingField(string fieldName)
        {
            return fieldName.EndsWith(">k__BackingField") || fieldName.EndsWith(">k__OriginalField");
        }
        public static bool IsBackingField(this FieldInfo field)
        {
            return IsBackingField(field.Name);
        }
        public static bool IsBackingField(this FieldInfo field, out PropertyInfo property)
        {
            var isBackingField = field.IsBackingField();

            property = null;

            if (isBackingField)
            {
                var propertyName = field.Name.Substring(1, field.Name.IndexOf('>') - 1);
                property = field.ReflectedType.GetProperty(propertyName);
            }

            return isBackingField;
        }

        public static T GetAttribute<T>(this MemberInfo member, bool inherit = true) where T : Attribute
        {
            var attributes = member.GetCustomAttributes(typeof(T), inherit);
            return (T)attributes.SingleOrDefault();
        }
        public static bool HasAttribute<T>(this MemberInfo member, bool inherit = true) where T : Attribute
        {
            return member.GetCustomAttributes(typeof(T), inherit).Any();
        }

        public static bool IsNormalValueType(this Type type)
        {
            return type.IsPrimitive || type.IsClass && type == typeof(string);
        }
        public static bool HasInterface<T>(this Type type)
        {
            return type.HasInterface(typeof(T));
        }
        public static bool HasInterface(this Type type, params Type[] interfaceType)
        {
            return type.GetInterfaces().Any(interfaceType.Contains);
        }
    }
}
