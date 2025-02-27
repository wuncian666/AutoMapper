using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace 自製AutoMapper
{
    internal class MaperExpression<TSource, TTarget>
    {
        private readonly Dictionary<string, MappingConditions> mapping =
            new Dictionary<string, MappingConditions>();

        public MaperExpression<TSource, TTarget> ForMember<DProperty, TProperty>(
            Expression<Func<TSource, DProperty>> source,
            Expression<Func<TTarget, TProperty>> target)
        {
            var sourceBody = (MemberExpression)source.Body;
            string sourceMemberName = sourceBody.Member.Name;
            string targetMemberName = null;
            Type sourceType = typeof(DProperty);
            Type targetType = typeof(TProperty);
            if (target.Body is MemberExpression targetExprssion)
            {
                targetMemberName = targetExprssion.Member.Name;
                _ = new MappingConditions(sourceMemberName, targetMemberName, ObjType.IS_MEMBER);
                this.mapping.Add(targetMemberName, null);
            }
            else if (target.Body is ConstantExpression constant && sourceType == targetType)
            {
                var targetValue = constant.Value;
                _ = new MappingConditions(sourceMemberName, targetValue, ObjType.IS_VALUE);
                this.mapping.Add(sourceMemberName, null);
            }
            else if (target.Body is MethodCallExpression method)
            {
                object temp = method.Method.Invoke(method, this.GetMethodArguments(method));
                _ = new MappingConditions(sourceMemberName, temp, ObjType.IS_COMBINATION);
                this.mapping.Add(sourceMemberName, null);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Cannot map source filed '{sourceMemberName}' of type '{sourceType}'" +
                    $" to target field '{targetMemberName}' of type '{targetType}'.");
            }

            return this;
        }

        public Dictionary<string, MappingConditions> Build()
        {
            return mapping;
        }

        private object[] GetMethodArguments(MethodCallExpression method)
        {
            List<object> arguments = new List<object>();
            foreach (var arg in method.Arguments)
            {
                object temp = typeof(ConstantExpression).GetProperty("Value").GetValue(arg);
                arguments.Add(temp);
            }
            return arguments.ToArray();
        }
    }
}