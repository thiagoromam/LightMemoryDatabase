using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LightMemoryDatabase.Helpers
{
    internal static class ExpressionHelper
    {
        public static IDictionary<PropertyInfo, object> GetValues<T, TProperty>(this Expression<Func<T, TProperty>> expression, T obj)
        {
            IEnumerable<MemberExpression> expressions = null;

            var newExpression = expression.Body as NewExpression;
            if (newExpression != null)
                expressions = newExpression.Arguments.Cast<MemberExpression>();
            else
            {
                var memberExpression = expression.Body as MemberExpression;
                if (memberExpression != null)
                    expressions = new List<MemberExpression> { memberExpression };
            }

            if (expressions == null)
                throw new ArgumentOutOfRangeException("expression");

            var dictionary = new Dictionary<PropertyInfo, object>();
            foreach (var e in expressions)
            {
                var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(e, typeof(object)), (ParameterExpression)e.Expression);
                dictionary.Add((PropertyInfo)e.Member, lambda.Compile()(obj));
            }
            return dictionary;
        }
    }
}
