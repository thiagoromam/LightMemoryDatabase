using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightMemoryDatabase.Api;
using LightMemoryDatabase.Serialization;

namespace LightMemoryDatabase.Database.Transactions
{
    internal class PlainObjectStateValue
    {
        private readonly JsonFieldInfo _field;
        private readonly IPlainObject _plainObject;
        private readonly object _stateValue;
        private bool? _hasChanged;

        public bool HasChanged
        {
            get { return _hasChanged ?? (_hasChanged = ValidateChange()).Value; }
        }

        public PlainObjectStateValue(JsonFieldInfo field, IPlainObject plainObject)
        {
            _field = field;
            _plainObject = plainObject;
            _stateValue = field.GetValue(_plainObject);

            if (field.IsReferenceType)
                _stateValue = ((IReference)_stateValue).Value;

            if (field.IsEnumerable && _stateValue != null)
                _stateValue = (((IEnumerable) _stateValue).Cast<object>()).ToList();
        }

        public void Restore()
        {
            if (!HasChanged) return;

            if (_field.IsReferenceType)
            {
                var reference = (IReference)_field.GetValue(_plainObject);
                reference.Value = _stateValue;
            }
            else
            {
                _field.SetValue(_plainObject, _stateValue);   
            }
        }

        private bool ValidateChange()
        {
            var value = _field.GetValue(_plainObject);

            if (_field.IsReferenceType)
                value = ((IReference)value).Value;

            if (_field.IsEnumerable)
            {
                if (value != null && _stateValue != null)
                {
                    var enumerable = ((IEnumerable)value).Cast<object>().ToList();
                    return !enumerable.SequenceEqual((List<object>)_stateValue);
                }
            }

            return !Equals(value, _stateValue);
        }
    }
}
