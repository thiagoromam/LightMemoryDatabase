using System;
using System.Collections.Generic;
using System.Linq;
using LightMemoryDatabase.Database;
using LightMemoryDatabase.Database.IO;
using LightMemoryDatabase.Database.Sources;

namespace LightMemoryDatabase.Api
{
    public static class DependencyRegistry
    {
        private static readonly Dictionary<Type, object> Types;

        static DependencyRegistry()
        {
            Types = new Dictionary<Type, object>();

            RegisterSingleton<MemorySourceContext>();
            RegisterSingleton<IContextService, ContextService>();
            Register<DatabaseReader>();
            Register<DatabaseWriter>();
            RegisterSingleton<DatabaseInfo>();
        }

        private static void Register(Type interfaceType, Type objectType)
        {
            var constructorParameters = objectType.GetConstructors().First().GetParameters();
            Func<object> createInstance;
            if (constructorParameters.Any())
            {
                createInstance = () =>
                {
                    var parameters = constructorParameters.Select(p => Resolve(p.ParameterType)).ToArray();
                    return Activator.CreateInstance(objectType, parameters);
                };
            }
            else
            {
                createInstance = () => Activator.CreateInstance(objectType);
            }

            Types[interfaceType] = createInstance;
        }
        public static void Register<TType>() where TType : class
        {
            Register<TType, TType>();
        }
        public static void Register<TInterface, TType>() where TType : class, TInterface
        {
            Register(typeof(TInterface), typeof(TType));
        }
        public static void RegisterSingleton<TType>() where TType : class
        {
            RegisterSingleton<TType, TType>();
        }
        public static void RegisterSingleton<T>(T instance) where T : class
        {
            Types[typeof(T)] = instance;
        }
        public static void RegisterSingleton<TInterface, TType>() where TType : class, TInterface
        {
            Register<TInterface, TType>();

            var createInstanceFunction = (Func<object>)Types[typeof(TInterface)];
            Func<object> createSingletonInstance = () =>
            {
                var instance = createInstanceFunction();
                Types[typeof(TInterface)] = instance;
                return instance;
            };

            Types[typeof(TInterface)] = createSingletonInstance;
        }

        public static TInterface Resolve<TInterface>() where TInterface : class
        {
            return (TInterface)Resolve(typeof(TInterface));
        }
        private static object Resolve(Type type)
        {
            var obj = Types[type];
            var createInstanceFunction = obj as Func<object>;
            return createInstanceFunction != null ? createInstanceFunction() : obj;
        }
    }
}
