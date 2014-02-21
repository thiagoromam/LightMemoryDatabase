using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightMemoryDatabase.Api;

namespace LightMemoryDatabase.Database.References
{
    public class PlainObjectsReferenceList<T> : IList<T> where T : IPlainObject
    {
        private readonly IList<T> _source;

        public int Count
        {
            get { return _source.Count; }
        }
        public bool IsReadOnly
        {
            get { return ((IList<T>)_source).IsReadOnly; }
        }
        public T this[int index]
        {
            get { return _source[index]; }
            set { _source[index] = value; }
        }

        public PlainObjectsReferenceList(IEnumerable<T> source)
        {
            _source = source as IList<T> ?? source.ToList();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _source.Add(item);
        }
        public void Clear()
        {
            _source.Clear();
        }
        public bool Contains(T item)
        {
            return _source.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            _source.CopyTo(array, arrayIndex);
        }
        public bool Remove(T item)
        {
            return _source.Remove(item);
        }
        public int IndexOf(T item)
        {
            return _source.IndexOf(item);
        }
        public void Insert(int index, T item)
        {
            _source.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            _source.RemoveAt(index);
        }
    }
}
