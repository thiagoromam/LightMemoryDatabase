using System;
using System.Collections.Generic;
using LightMemoryDatabase.Api;

namespace LightMemoryDatabase.Database.Sources
{
    internal interface ISimpleCollectionSource : IEnumerable<IPlainObject>
    {
        Type ElementType { get; }
        bool Store(IPlainObject plainObject);
        bool Delete(IPlainObject plainObject);
        IPlainObject Load(int id);
        IPlainObject Find(int id);
    }

    internal interface ICollectionSource : ISimpleCollectionSource
    {
        bool Delete(int id);
        void Store(IEnumerable<IPlainObject> plainObjects);
        void Delete(IEnumerable<IPlainObject> plainObjects);
    }
}