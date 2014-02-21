using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ComputerBeacon.Json;
using LightMemoryDatabase.Api;

namespace LightMemoryDatabase.Serialization
{
    internal class JsonSerializer : JsonSerialization
    {
        public string Serialize<T>(T obj)
        {
            return _Serialize(obj).ToString();
        }
        private object _Serialize(object obj)
        {
            var enumerable = obj as IEnumerable;
            if (enumerable != null)
                return SerializeArray(enumerable);

            return SerializeObject(obj);
        }
        private JsonObject SerializeObject(object obj)
        {
            if (obj == null)
                return null;

            var json = new JsonObject();

            var fields = GetClassInfo(obj.GetType()).Fields;
            foreach (var field in fields)
            {
                var value = field.GetValue(obj);

                if (field.IsReferenceType)
                {
                    value = ((IReference)value).Value;

                    value = field.IsEnumerable
                        ? new JsonArray(((IEnumerable<IPlainObject>)value).Select(i => (object)i.Id).ToArray())
                        : (object)((IPlainObject)value).Id;
                }
                else if (!field.IsValueType)
                {
                    value = _Serialize(value);
                }

                json.Add(field.Name, value);
            }

            return json;
        }
        private JsonArray SerializeArray(IEnumerable enumerable)
        {
            var strings = enumerable as IEnumerable<string>;
            if (strings != null)
                return new JsonArray(strings.Cast<object>().ToArray());

            var json = new JsonArray();
            foreach (var item in enumerable)
                json.Add(_Serialize(item));

            return json;
        }
    }
}
