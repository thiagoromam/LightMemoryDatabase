using System;
using System.Collections.Generic;
using LightMemoryDatabase.Api;
using LightMemoryDatabase.Database.Sources;

namespace LightMemoryDatabase.Database.References
{
    internal class ReferenceManager
    {
        private readonly MemorySourceContext _context;
        private readonly Dictionary<Type, Type> _createNormalInstanceMethods;
        private readonly Dictionary<Type, Type> _createListInstanceMethods;
        private readonly Type _normalInstanceType;
        private readonly Type _listInstanceType;

        public ReferenceManager(MemorySourceContext context)
        {
            _context = context;
            _createNormalInstanceMethods = new Dictionary<Type, Type>();
            _createListInstanceMethods = new Dictionary<Type, Type>();
            _normalInstanceType = typeof(PlainObjectReference<>);
            _listInstanceType = typeof(PlainObjectsReference<>);
        }

        public IPlainObjectReference Resolve(Type type, int id, bool isCascadeDelete)
        {
            var instanceType = GetFullInstanceType(type, _normalInstanceType, _createNormalInstanceMethods);
            return (IPlainObjectReference) CreateInstance(instanceType, isCascadeDelete, id);
        }
        public IPlainObjectsReference Resolve(Type type, IEnumerable<int> ids, bool isCascadeDelete)
        {
            var instanceType = GetFullInstanceType(type, _listInstanceType, _createListInstanceMethods);
            return (IPlainObjectsReference)CreateInstance(instanceType, isCascadeDelete, ids);
        }

        private object CreateInstance(Type fullInstanceType, bool isCascadeDelete, object valueReference)
        {
            return Activator.CreateInstance(fullInstanceType, new[] { isCascadeDelete, _context, valueReference });
        }
        private static Type GetFullInstanceType(Type type, Type instanceType, IDictionary<Type, Type> createInstanceMethods)
        {
            type = type.GetGenericArguments()[0];
            if (!createInstanceMethods.ContainsKey(type))
                createInstanceMethods[type] = instanceType.MakeGenericType(type);

            return createInstanceMethods[type];
        }
    }
}