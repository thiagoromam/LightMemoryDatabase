namespace LightMemoryDatabase.Api
{
    public interface IPlainObjectReference : IReference
    {
    }

    public interface IPlainObjectReference<T> : IPlainObjectReference where T : class, IPlainObject
    {
        new T Value { get; set; }
    }
}
