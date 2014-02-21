using System.Collections.Generic;
using LightMemoryDatabase.Api;
using System.Collections;
using System;
using System.Linq;
using System.Linq.Expressions;
using LightMemoryDatabase.Database.Sources;
using LightMemoryDatabase.Helpers;

namespace LightMemoryDatabase.Database
{
    internal class PlainObjectCollection<T> : IPlainObjectCollection<T> where T : class, IPlainObject
    {
        private readonly ICollectionSource _source;

        public PlainObjectCollection(ICollectionSource source)
        {
            _source = source;
        }

        public void Store(T plainObject)
        {
            _source.Store(plainObject);
        }
        public void Store(IEnumerable<T> plainObjects)
        {
            _source.Store(plainObjects);
        }

        public bool Update<TProperty>(T plainObject, Expression<Func<T, TProperty>> propertiesToUpdate)
        {
            var plainObjectFromSource = Load(plainObject.Id);
            var merged = ObjectHelper.Merge(plainObjectFromSource, plainObject, propertiesToUpdate);

            return merged;
        }

        public void Delete(T plainObject)
        {
            _source.Delete(plainObject);
        }
        public void Delete(IEnumerable<T> plainObjects)
        {
            _source.Delete(plainObjects);
        }

        public void Delete(int id)
        {
            _source.Delete(id);
        }
        
        public T Load(int id)
        {
            return (T)_source.Load(id);
        }
        public T Find(int id)
        {
            return (T)_source.Find(id);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.Cast<T>().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
