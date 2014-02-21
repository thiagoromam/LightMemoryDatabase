using LightMemoryDatabase.Api;
using LightMemoryDatabase.Database.Sources;

namespace LightMemoryDatabase.Database.References
{
    internal sealed class PlainObjectReference<T> : LazyObject<T>, IPlainObjectReference<T> where T : class, IPlainObject
    {
        private readonly bool _cascadeDelete;

        public PlainObjectReference(bool cascadeDelete)
        {
            _cascadeDelete = cascadeDelete;
        }
        public PlainObjectReference(bool cascadeDelete, MemorySourceContext context, int id)
            : base(() => LazyValueFactory(context, id))
        {
            _cascadeDelete = cascadeDelete;
        }

        object IReference.Value
        {
            get { return Value; }
        }

        public override T Value
        {
            get { return base.Value; }
            set
            {
                if (_cascadeDelete && value != base.Value && base.Value != null)
                {
                    // TODO remove object from database
                }

                base.Value = value;
            }
        }

        private static T LazyValueFactory(MemorySourceContext context, int id)
        {
            return (T)context.GetCollectionSource(typeof(T)).Load(id);
        }
    }
}
