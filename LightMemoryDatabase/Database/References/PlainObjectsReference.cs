using System.Collections.Generic;
using System.Linq;
using LightMemoryDatabase.Api;
using LightMemoryDatabase.Database.Sources;
using LightMemoryDatabase.Helpers;

namespace LightMemoryDatabase.Database.References
{
    internal sealed class PlainObjectsReference<T> : LazyObject<IList<T>>, IPlainObjectsReference<T> where T : class, IPlainObject
    {
        private readonly bool _cascadeDelete;

        public PlainObjectsReference(bool cascadeDelete)
        {
            _cascadeDelete = cascadeDelete;
        }
        public PlainObjectsReference(bool cascadeDelete, MemorySourceContext context, IEnumerable<int> ids)
            : base(() => LazyValueFactory(context, ids))
        {
            _cascadeDelete = cascadeDelete;
        }

        object IReference.Value
        {
            get { return Value; }
        }

        public override IList<T> Value
        {
            get { return base.Value; }
            set
            {
                if (value == null)
                {
                    if (_cascadeDelete && base.Value != null)
                        base.Value.Clear();
                }
                else
                {
                    if (!(value is PlainObjectsReferenceList<T>))
                        value = new PlainObjectsReferenceList<T>(value);
                }

                base.Value = value;
            }
        }

        private static PlainObjectsReferenceList<T> LazyValueFactory(MemorySourceContext context, IEnumerable<int> ids)
        {
            var references = context.GetCollectionSource(typeof(T)).Where(t => ids.Contains(t.Id)).Cast<T>();
            var list = ObjectHelper.CreateNewGenericInstance(typeof (PlainObjectsReferenceList<>), typeof (T), references);
            return (PlainObjectsReferenceList<T>)list;
        }
    }
}
