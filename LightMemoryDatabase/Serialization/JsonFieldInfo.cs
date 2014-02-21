using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using LightMemoryDatabase.Api;
using LightMemoryDatabase.Helpers;

namespace LightMemoryDatabase.Serialization
{
    internal class JsonFieldInfo
    {
        private readonly Func<object, object> _getValue;
        private readonly Action<object, object> _setValue;

        public string Name { get; set; }
        public bool IsValueType { get; private set; }
        public Type FieldType { get; private set; }
        public Type ReferenceType { get; private set; }
        public bool IsReferenceType { get; private set; }
        public bool IsEnumerable { get; private set; }
        public bool IsCascadeDelete { get; private set; }

        public JsonFieldInfo(FieldInfo field)
        {
            Name = Regex.Replace(field.Name, "<|>.+", string.Empty);
            FieldType = field.FieldType;
            IsValueType = FieldType.IsNormalValueType();

            if (!IsValueType)
            {
                IsEnumerable = FieldType.HasInterface(typeof(IEnumerable), typeof(IPlainObjectsReference));
                IsReferenceType = FieldType.HasInterface<IReference>();

                if (IsReferenceType)
                {
                    var plainObjectType = FieldType.GetGenericArguments()[0];
                    IsReferenceType = plainObjectType.HasInterface<IPlainObject>();
                    ReferenceType = IsReferenceType ? plainObjectType : null;
                }
            }

            IsCascadeDelete = field.HasAttribute<CascadeDeleteAttribute>();

            _getValue = field.GetValue;
            _setValue = field.SetValue;
        }

        public object GetValue(object obj)
        {
            return _getValue(obj);
        }
        public void SetValue(object obj, object value)
        {
            _setValue(obj, value);
        }

        public static List<JsonFieldInfo> GetByType(Type type)
        {
            List<FieldInfo> fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();
            fields.RemoveAll(f => f.IsInitOnly || f.GetCustomAttributes(typeof(IgnoreAttribute), true).Any());

            foreach (var field in fields.ToList())
            {
                PropertyInfo property;
                if (field.IsBackingField(out property) && property.HasAttribute<IgnoreAttribute>())
                    fields.Remove(field);
            }

            return new List<JsonFieldInfo>(fields.Select(f => new JsonFieldInfo(f)));
        }
    }
}
