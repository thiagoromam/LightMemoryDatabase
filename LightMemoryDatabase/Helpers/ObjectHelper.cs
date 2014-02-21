using System;
using System.Linq;
using System.Linq.Expressions;

namespace LightMemoryDatabase.Helpers
{
    internal static class ObjectHelper
    {
        public static bool Merge<T, TProperty>(T to, T from, Expression<Func<T, TProperty>> propertiesToMerge)
        {
            var valuesByProperties = propertiesToMerge.GetValues(from);
            var updatesToPerform = valuesByProperties.Where(update => !Equals(update.Key.GetValue(to), update.Value)).ToList();

            foreach (var update in updatesToPerform)
                update.Key.SetValue(to, update.Value);

            return updatesToPerform.Any();
        }

        public static object CreateNewGenericInstance(Type objectType, Type argumentType, params object[] args)
        {
            var instanceType = objectType.MakeGenericType(argumentType);
            return Activator.CreateInstance(instanceType, args);
        }
    }
}
