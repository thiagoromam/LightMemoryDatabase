using System;

namespace LightMemoryDatabase.Database.References
{
    internal class LazyObject<T>
    {
        private bool _callFactoryInNextCall;
        private T _value;
        private readonly Func<T> _valueFactory;

        public LazyObject()
        {
            _callFactoryInNextCall = false;
        }
        public LazyObject(Func<T> valueFactory)
        {
            _callFactoryInNextCall = true;
            _valueFactory = valueFactory;
        }

        public virtual T Value
        {
            get
            {
                if (_callFactoryInNextCall)
                    _value = _valueFactory();

                return _value;
            }
            set
            {
                _value = value;
                _callFactoryInNextCall = false;
            }
        }
    }
}
