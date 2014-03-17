using System.Collections.Generic;
using LightMemoryDatabase.Api;

namespace LightMemoryDatabase.Database
{
    public interface ICollectionManager
    {
        IEnumerable<IPlainObjectCollection> Collections { get; }
    }
}