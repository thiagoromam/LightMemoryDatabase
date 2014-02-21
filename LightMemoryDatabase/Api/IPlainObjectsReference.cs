using System.Collections.Generic;

namespace LightMemoryDatabase.Api
{
    public interface IPlainObjectsReference : IReference
    {
    }

    public interface IPlainObjectsReference<T> : IPlainObjectsReference where T : class, IPlainObject
    {
        new IList<T> Value { get; set; }
    }
}
