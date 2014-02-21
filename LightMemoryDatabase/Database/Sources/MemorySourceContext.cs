using System;
using System.Collections;
using System.Collections.Generic;
using LightMemoryDatabase.Api;
using LightMemoryDatabase.Serialization;

namespace LightMemoryDatabase.Database.Sources
{
    internal class MemorySourceContext
    {
        private readonly Dictionary<Type, ICollectionSource> _collectionSources;
        private DatabaseInfo _databaseInfo;

        public IEnumerable<Type> CollectionTypes
        {
            get { return _collectionSources.Keys; }
        }

        public MemorySourceContext(DatabaseInfo databaseInfo)
        {
            _databaseInfo = databaseInfo;
            _collectionSources = new Dictionary<Type, ICollectionSource>();
        }

        public ICollectionSource GetCollectionSource(Type type)
        {
            lock (_collectionSources)
            {
                if (!_collectionSources.ContainsKey(type))
                    AddCollectionSource(type, null);
            }

            return _collectionSources[type];
        }

        public void AddCollectionSource(Type type, IEnumerable<IPlainObject> source)
        {
            _collectionSources.Add(type, new GlobalCollectionManager(this, _databaseInfo, new CollectionSource(type, source)));
        }

        #region GlobalCollectionManager class

        internal class GlobalCollectionManager : ICollectionSource
        {
            private readonly MemorySourceContext _context;
            private readonly DatabaseInfo _databaseInfo;
            private readonly ISimpleCollectionSource _collectionSource;
            private readonly JsonClassInfo _classInfo;

            public Type ElementType
            {
                get { return _collectionSource.ElementType; }
            }

            public GlobalCollectionManager(MemorySourceContext context, DatabaseInfo databaseInfo, ISimpleCollectionSource collectionSource)
            {
                _context = context;
                _databaseInfo = databaseInfo;
                _collectionSource = collectionSource;
                _classInfo = new JsonClassInfo(collectionSource.ElementType);
            }

            public bool Store(IPlainObject plainObject)
            {
                if (_collectionSource.Store(plainObject))
                {
                    plainObject.Id = NextId();
                    StoreReferences(plainObject);
                    return true;
                }

                return false;
            }
            public void Store(IEnumerable<IPlainObject> plainObjects)
            {
                foreach (var plainObject in plainObjects)
                    Store(plainObject);
            }
            public bool Delete(IPlainObject plainObject)
            {
                if (_collectionSource.Delete(plainObject))
                {
                    DeleteReferences(plainObject);
                    return true;
                }

                return false;
            }
            public void Delete(IEnumerable<IPlainObject> plainObjects)
            {
                foreach (var plainObject in plainObjects)
                    Delete(plainObject);
            }
            public bool Delete(int id)
            {
                return Delete(Load(id));
            }
            public IPlainObject Load(int id)
            {
                return _collectionSource.Load(id);
            }
            public IPlainObject Find(int id)
            {
                return _collectionSource.Find(id);
            }
            public IEnumerator<IPlainObject> GetEnumerator()
            {
                return _collectionSource.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private void StoreReferences(IPlainObject plainObject)
            {
                foreach (var referenceField in _classInfo.ReferenceFields)
                {
                    var collectionContext = _context.GetCollectionSource(referenceField.ReferenceType);
                    var value = ((IReference)referenceField.GetValue(plainObject)).Value;
                    if (referenceField.IsEnumerable)
                        collectionContext.Store((IEnumerable<IPlainObject>)value);
                    else
                        collectionContext.Store((IPlainObject)value);
                }
            }
            private void DeleteReferences(IPlainObject plainObject)
            {
                foreach (var referenceField in _classInfo.ReferenceFields)
                {
                    if (!referenceField.IsCascadeDelete)
                        continue;

                    var collectionContext = _context.GetCollectionSource(referenceField.ReferenceType);
                    var value = ((IReference)referenceField.GetValue(plainObject)).Value;
                    if (referenceField.IsEnumerable)
                        collectionContext.Delete((IEnumerable<IPlainObject>)value);
                    else
                        collectionContext.Delete((IPlainObject)value);
                }
            }

            private int NextId()
            {
                var currentId = _databaseInfo.CurrentPlainObjectId;
                _databaseInfo.CurrentPlainObjectId += 1;
                return currentId;
            }
        }

        #endregion
    }
}
