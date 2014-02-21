using System;
using System.Collections.Generic;
using System.Linq;

namespace LightMemoryDatabase.Serialization
{
    internal abstract class JsonSerialization
    {
        private readonly Dictionary<Type, JsonClassInfo> _classesInfo;

        protected JsonSerialization()
        {
            _classesInfo = new Dictionary<Type, JsonClassInfo>();
        }

        protected JsonClassInfo GetClassInfo(Type type)
        {
            if (!_classesInfo.ContainsKey(type))
                _classesInfo.Add(type, new JsonClassInfo(type));

            return _classesInfo[type];
        }
        public JsonClassInfo GetClassInfo(string name)
        {
            return _classesInfo.Values.Single(c => c.Name == name);
        }

        public void Register(JsonClassInfo classInfo)
        {
            _classesInfo[classInfo.ClassType] = classInfo;
        }
        public void Register(IEnumerable<JsonClassInfo> classesInfo)
        {
            foreach (var classInfo in classesInfo)
                Register(classInfo);
        }
    }
}
