using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightMemoryDatabase.Api;

namespace LightMemoryDatabase.Serialization
{
    internal class JsonClassInfo
    {
        private string _classTypeName;
        [Ignore]
        private Type _classType;
        [Ignore]
        private MethodInfo _initializeMethod;
        [Ignore]
        private bool _fieldInitializeMethodLoaded;

        [Ignore]
        public string Name { get; private set; }
        public Type ClassType
        {
            get { return _classType ?? (_classType = Type.GetType(_classTypeName)); }
        }
        [Ignore]
        public List<JsonFieldInfo> Fields { get; private set; }
        [Ignore]
        public List<JsonFieldInfo> ReferenceFields { get; private set; }
        public MethodInfo InitializeMethod
        {
            get
            {
                if (!_fieldInitializeMethodLoaded)
                {
                    var methods = ClassType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    _initializeMethod = methods.SingleOrDefault(m => m.GetCustomAttributes(typeof(InitalizeAttribute), true).Any());
                    _fieldInitializeMethodLoaded = true;
                }

                return _initializeMethod;
            }
        }

        public JsonClassInfo() { }
        public JsonClassInfo(Type type)
        {
            _classTypeName = type.AssemblyQualifiedName;
            _classType = type;
            Initialize();
        }

        [Initalize]
        private void Initialize()
        {
            Name = ClassType.Name;
            Fields = JsonFieldInfo.GetByType(ClassType);
            ReferenceFields = Fields.Where(f => f.IsReferenceType).ToList();
        }
    }
}
