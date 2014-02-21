using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ComputerBeacon.Json;
using LightMemoryDatabase.Database.References;
using LightMemoryDatabase.Helpers;

namespace LightMemoryDatabase.Serialization
{
    internal class JsonDeserializer : JsonSerialization
    {
        private readonly ReferenceManager _referenceManager;
        private readonly Dictionary<Type, Type> _normalListTypes;

        public JsonDeserializer(ReferenceManager referenceManager)
        {
            _referenceManager = referenceManager;
            _normalListTypes = new Dictionary<Type, Type>();
        }

        public T Deserialize<T>(string json) where T : class
        {
            return (T)Deserialize(typeof(T), json);
        }
        public object Deserialize(Type type, string json)
        {
            var jsonObj = type.HasInterface<IEnumerable>() ? new JsonArray(json) : (object)new JsonObject(json);

            return Deserialize(type, jsonObj);
        }
        private object Deserialize(Type type, object json)
        {
            if (json is string)
                return json;

            var array = json as JsonArray;
            if (array != null)
                return DeserializeArray(type, array);

            return DeserializeObject(type, (JsonObject)json);
        }
        private object DeserializeObject(Type type, JsonObject json)
        {
            if (json == null)
                return null;

            var obj = Activator.CreateInstance(type);
            var classInfo = GetClassInfo(type);
            foreach (var item in json)
            {
                var value = item.Value;
                var field = classInfo.Fields.Single(f => f.Name == item.Key);

                if (field.IsReferenceType)
                {
                    value = field.IsEnumerable
                        ? _referenceManager.Resolve(field.FieldType, ((IEnumerable)value).Cast<int>(), field.IsCascadeDelete)
                        : (object)_referenceManager.Resolve(field.FieldType, (int)value, field.IsCascadeDelete);
                }
                else if (!field.IsValueType)
                {
                    value = Deserialize(field.FieldType, value);
                }

                field.SetValue(obj, value);
            }

            if (classInfo.InitializeMethod != null)
                classInfo.InitializeMethod.Invoke(obj, null);

            return obj;
        }
        private object DeserializeArray(Type type, JsonArray json)
        {
            type = type.GetGenericArguments()[0];
            if (!_normalListTypes.ContainsKey(type))
                _normalListTypes.Add(type, typeof(List<>).MakeGenericType(type));

            var values = type.IsNormalValueType() ? json : json.Select(j => Deserialize(type, j));

            var obj = (IList)Activator.CreateInstance(_normalListTypes[type]);
            foreach (var value in values)
                obj.Add(value);

            return obj;
        }
    }
}
