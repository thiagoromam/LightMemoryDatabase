using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightMemoryDatabase.Api
{
    public interface IPlainObjectCollection : IEnumerable { }
    public interface IPlainObjectCollection<T> : IPlainObjectCollection, IEnumerable<T> where T : class, IPlainObject
    {
        void Store(T plainObject);
        void Store(IEnumerable<T> plainObjects);
        bool Update<TProperty>(T item, Expression<Func<T, TProperty>> propertiesToUpdate);
        void Delete(T plainObject);
        void Delete(IEnumerable<T> plainObjects);
        T Load(int id);
        T Find(int id);
    }
}
