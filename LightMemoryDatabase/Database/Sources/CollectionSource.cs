using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightMemoryDatabase.Api;

namespace LightMemoryDatabase.Database.Sources
{
    internal class CollectionSource : ISimpleCollectionSource
    {
        private readonly List<IPlainObject> _source;

        public Type ElementType { get; private set; }

        public CollectionSource(Type elementType, IEnumerable<IPlainObject> source)
        {
            ElementType = elementType;
            _source = source != null ? source as List<IPlainObject> ?? source.ToList() : new List<IPlainObject>();
        }

        public bool Store(IPlainObject plainObject)
        {
            if (_source.Contains(plainObject))
                return false;

            _source.Add(plainObject);
            return true;
        }
        public bool Delete(IPlainObject plainObject)
        {
            return _source.Remove(plainObject);
        }
        public IPlainObject Load(int id)
        {
            return _source.Single(o => o.Id == id);
        }
        public IPlainObject Find(int id)
        {
            return _source.SingleOrDefault(o => o.Id == id);
        }

        public IEnumerator<IPlainObject> GetEnumerator()
        {
            return _source.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}