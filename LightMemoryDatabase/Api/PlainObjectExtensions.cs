using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LightMemoryDatabase.Database.References;
using LightMemoryDatabase.Serialization;

namespace LightMemoryDatabase.Api
{
    public static class PlainObjectExtensions
    {
        private static readonly IDictionary<Type, JsonClassInfo> ClassesInfos;

        static PlainObjectExtensions()
        {
            ClassesInfos = new Dictionary<Type, JsonClassInfo>();
        }

        private static JsonClassInfo GetClassInfo<T>()
        {
            var type = typeof(T);
            if (!ClassesInfos.ContainsKey(type))
                ClassesInfos.Add(type, new JsonClassInfo(type));

            return ClassesInfos[type];
        }

        private static JsonFieldInfo GetClassField<T, TProperty>(Expression<Func<T, TProperty>> field)
        {
            var fieldName = ((MemberExpression)field.Body).Member.Name;

            return GetClassInfo<T>().ReferenceFields.Single(f => f.Name == fieldName);
        }

        public static IPlainObjectReference<TProperty> CreateNewReference<T, TProperty>(this T obj, Expression<Func<T, IPlainObjectReference<TProperty>>> field)
            where T : class, IPlainObject
            where TProperty : class, IPlainObject
        {
            var classField = GetClassField(field);
            return new PlainObjectReference<TProperty>(classField.IsCascadeDelete);
        }

        public static IPlainObjectsReference<TProperty> CreateNewReference<T, TProperty>(this T obj, Expression<Func<T, IPlainObjectsReference<TProperty>>> field)
            where T : class, IPlainObject
            where TProperty : class, IPlainObject
        {
            var classField = GetClassField(field);
            return new PlainObjectsReference<TProperty>(classField.IsCascadeDelete);
        }
    }
}
