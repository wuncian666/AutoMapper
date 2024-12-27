using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
            //1.他是類別下的某一個欄位
            //2.他是普通字串或數值
            //3.他是欄位+字串 (背後其實是一個函數)
            //4.若上述情況都不能轉換則噴出一個Exception告訴使用者該欄位無法被轉型

            var sourceBody = (MemberExpression)source.Body;
            string sourceMemberName = sourceBody.Member.Name;
            string targetMemberName = null;
            Type sourceType = typeof(DProperty);
            Type targetType = typeof(TProperty);

            MappingConditions conditions = null;

            // 能夠找到對應類別中的物件名稱
            if (target.Body is MemberExpression targetExprssion)
            {
                targetMemberName = targetExprssion.Member.Name;
                conditions = new MappingConditions(sourceMemberName, targetMemberName, ObjType.IS_MEMBER);
                this.mapping.Add(targetMemberName, conditions);
            }
            // 直接指派一個數值給目標
            else if (target.Body is ConstantExpression constant && sourceType == targetType)
            {
                var targetValue = constant.Value;
                conditions = new MappingConditions(sourceMemberName, targetValue, ObjType.IS_VALUE);
                this.mapping.Add(sourceMemberName, conditions);
            }
            else if (target.Body is MethodCallExpression method)
            {
                object temp = method.Method.Invoke(method, this.GetMethodArguments(method));
                conditions = new MappingConditions(sourceMemberName, temp, ObjType.IS_COMBINATION);
                this.mapping.Add(sourceMemberName, conditions);
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