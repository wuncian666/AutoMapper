using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using 自製AutoMapper.Enums;
using 自製AutoMapper.mappers;

namespace 自製AutoMapper
{
    internal class Mapper
    {
        //public static object Map(Type SourceType, Type TargetType)
        //{
        //}

        public static TTarget Map<TSource, TTarget>(
            TSource sourceData,
            Action<MaperExpression<TSource, TTarget>> action = null
            ) where TTarget : new()
        {
            TTarget target = new TTarget();
            Type targetType = target.GetType();

            var sourceProps = sourceData.GetType().GetProperties();

            foreach (var prop in sourceProps)
            {
                var targetProp = targetType.GetProperty(prop.Name);
                if (targetProp != null)
                {
                    var sourceType = prop.GetValue(sourceData).GetType();

                    AbstractMapper<TSource, TTarget> mapper =
                        MapperFactory.Create<TSource, TTarget>(
                            sourceType.GetPropertyTypeEnum());

                    mapper.AssignValue(sourceData, prop, targetType);

                    /*if (sourceType.GetPropertyTypeEnum() == PropertyType.Basic)
                    {
                        targetProp.SetValue(target, prop.GetValue(sourceData));
                    }
                    else if (sourceType.IsArray)
                    {
                        Type targetElementType = targetProp.PropertyType.GetElementType();
                        Array souceArray = (Array)prop.GetValue(sourceData);
                        Array targetArray = Array.CreateInstance(targetElementType, souceArray.Length);

                        MethodInfo findMapMethod = typeof(Mapper).GetMethod("Map");
                        MethodInfo mapMethod = findMapMethod.MakeGenericMethod(
                            sourceType.GetElementType(), targetElementType);
                        for (int i = 0; i < souceArray.Length; i++)
                        {
                            object temp = mapMethod.Invoke(
                                null, new object[] { souceArray.GetValue(i), null });
                            targetArray.SetValue(temp, i);
                        }
                        targetProp.SetValue(target, targetArray);
                    }
                    else if (sourceType.GetPropertyTypeEnum() == PropertyType.Enum)
                    {
                        // source 一定是 enum
                        // int, string
                        if (targetProp.PropertyType == typeof(string))
                        {
                            targetProp.SetValue(target, prop.GetValue(sourceData).ToString());
                        }
                        else if (targetProp.PropertyType == typeof(int))
                        {
                            //var enumToInt = Convert.ToInt32(prop.GetValue(sourceData));
                            var enumToInt = (int)(prop.GetValue(sourceData));
                            targetProp.SetValue(target, enumToInt);
                        }
                    }
                    else if (sourceType.IsClass && (sourceType.Name != "String"))
                    {
                        // stu -> parent, child
                        MethodInfo findMapMethod = typeof(Mapper).GetMethod("Map");
                        MethodInfo mapMethod = findMapMethod.MakeGenericMethod(prop.PropertyType, targetProp.PropertyType);
                        object temp = mapMethod.Invoke(null, new object[] { prop.GetValue(sourceData), null });

                        targetProp.SetValue(target, temp);
                    }
                    */
                }
            }

            if (action == null)
                return target;

            MaperExpression<TSource, TTarget> expression = new MaperExpression<TSource, TTarget>();
            action.Invoke(expression);

            Dictionary<string, MappingConditions> conditionMapping = expression.Build();

            foreach (var condition in conditionMapping)
            {
                var type = condition.Value.ObjType;
                switch (type)
                {
                    // key 是 target 的類別名稱，conditions 裏頭是 source 類別名稱，條件
                    case ObjType.IS_MEMBER:
                        ConditionIsMember(sourceData, targetType, target, condition);
                        break;

                    case ObjType.IS_VALUE:
                        ConditionIsValue(targetType, target, condition);
                        break;

                    case ObjType.IS_COMBINATION:
                        ConditionIsCombination(targetType, target, condition);
                        break;
                }
            }

            //Dictionary<string, string> map = new Dictionary<string, string>();

            //var dataBody = (MemberExpression)source.Body;
            //string dMemName = dataBody.Member.Name;// User
            //var targetBody = (MemberExpression)target.Body;
            //string tMemName = targetBody.Member.Name;// Stu

            //map.Add(dMemName, tMemName);

            //// 3. map
            //tType
            //    //value
            //    .GetProperty(tMemName)
            //    // key
            //    .SetValue(t, data.GetType().GetProperty(dMemName).GetValue(data));

            //Expression為父類別可以回傳整個Func的Expression:x => x.Name
            //子類別:
            //BinaryExpression => (x.Name == "Leo") => 樹狀結構(left,right)
            //MemberExpression => x.Name
            //ConstantExpression=> "Leo"
            //MethodCallExpression =>x.Name.Contains("Leo")

            Func<object, object> func = Map;
            object obj = func.Invoke("AA");

            return target;
        }

        private static void ConditionIsMember<TSource, TTarget>(
            TSource source,
            Type targetType,
            TTarget target,
            KeyValuePair<string, MappingConditions> condition)
        {
            var sourceValue =
                source
                .GetType()
                .GetProperty(condition.Value.SourcePropertyName)
                .GetValue(source);

            targetType
                .GetProperty(condition.Key)
                .SetValue(target, sourceValue);// target.property = sourceValue
        }

        private static void ConditionIsValue<TTarget>(
            Type targetType,
            TTarget target,
            KeyValuePair<string, MappingConditions> condition)
        {
            var targetProp = targetType.GetProperty(condition.Key);

            if (targetProp.PropertyType == condition.Value.TargetData.GetType())
            {
                targetProp.SetValue(target, condition.Value.TargetData);
                return;
            }

            var convertResult = Convert.ChangeType(condition.Value.TargetData, targetProp.PropertyType);
            // 兩邊不同型別時，需轉型
            targetProp.SetValue(target, convertResult);
        }

        private static void ConditionIsCombination<TTarget>(
            Type targetType,
            TTarget target,
            KeyValuePair<string, MappingConditions> condition)
        {
            targetType
                .GetProperty(condition.Key)
                .SetValue(target, condition.Value.TargetData);
        }

        private static object Map(object source)
        {
            return null;
        }
    }
}