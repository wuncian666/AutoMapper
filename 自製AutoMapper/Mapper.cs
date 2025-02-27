using System;
using System.Collections.Generic;
using System.Reflection;
using 自製AutoMapper.mappers;

namespace 自製AutoMapper
{
    internal class Mapper
    {
        public static TTarget Map<TSource, TTarget>(
            TSource sourceData,
            Action<MaperExpression<TSource, TTarget>> action = null
            ) where TTarget : class, new()
        {
            ValidateSource(sourceData);

            TTarget target = new TTarget();
            Type targetType = target.GetType();

            if (action == null)
            {
                MapDefaultPropertis(sourceData, target, targetType);
            }
            else
            {
                MapCustomConditions(sourceData, target, targetType, action);
            }

            return target;
        }

        private static void ValidateSource<TSource>(TSource sourceData)
        {
            if (sourceData == null)
                throw new ArgumentNullException(nameof(sourceData));
        }

        private static void MapDefaultPropertis<TSource, TTarger>(
            TSource sourceData, TTarger target, Type targetType)
            where TTarger : class, new()
        {
            var sourceProps = sourceData.GetType().GetProperties();
            foreach (var item in sourceProps)
            {
                var targetProp = targetType.GetProperty(item.Name);
                if (IsValidTargetProperty(targetProp))
                {
                    MapProperty(sourceData, target, item, targetProp);
                }
            }
        }

        private static void MapCustomConditions<TSource, TTarget>(
            TSource sourceData, TTarget target, Type targetType, Action<MaperExpression<TSource, TTarget>> action)
        {
            var expression = new MaperExpression<TSource, TTarget>();
            action(expression);
            var conditionMapping = expression.Build();

            foreach (var condition in conditionMapping)
            {
                ProcessCondition(sourceData, target, targetType, condition);
            }
        }

        private static bool IsValidTargetProperty(PropertyInfo targetProp)
        {
            return targetProp != null && targetProp.CanWrite;
        }

        private static void MapProperty<TSource, TTarget>(
            TSource sourceData,
            TTarget target,
            PropertyInfo sourceProp,
            PropertyInfo targetProp)
            where TTarget : class, new()
        {
            try
            {
                var mapper = MapperFactory.Create<TSource, TTarget>(
                    sourceProp.PropertyType.GetPropertyTypeEnum());
                if (mapper != null)
                {
                    mapper.AssignValue(sourceData, sourceProp, targetProp.DeclaringType);
                }
                else if (targetProp.PropertyType == sourceProp.PropertyType)
                {
                    targetProp.SetValue(target, sourceProp.GetValue(sourceData));
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException(
                    $"Cannot map source filed '{sourceProp.Name}' to target field '{targetProp.Name}'.");
            }
        }

        private static void ProcessCondition<TSource, TTarget>(
            TSource sourceData, TTarget target, Type targetType, KeyValuePair<string, MappingConditions> condition)
        {
            if (condition.Value == null)
            {
                throw new InvalidOperationException($"Cannot map source filed '{condition.Key}' to target field.");
            }
            var type = condition.Value.ObjType;
            switch (type)
            {
                case ObjType.IS_MEMBER:
                    ConditionIsMember(sourceData, targetType, target, condition);
                    break;

                case ObjType.IS_VALUE:
                    ConditionIsValue(targetType, target, condition);
                    break;

                case ObjType.IS_COMBINATION:
                    ConditionIsCombination(targetType, target, condition);
                    break;

                default:
                    throw new InvalidOperationException($"Cannot map source filed '{condition.Key}' to target field.");
            }
        }

        private static void ConditionIsMember<TSource, TTarget>(
            TSource source,
            Type targetType,
            TTarget target,
            KeyValuePair<string, MappingConditions> condition)
        {
            var sourceProp =
                source
                .GetType()
                .GetProperty(condition.Value.SourcePropertyName)
                .GetValue(source);
            if (sourceProp != null)
            {
                var sourceValue = sourceProp.GetType().GetProperty(condition.Value.SourcePropertyName).GetValue(sourceProp);
                SetPropertyValue(targetType, target, condition.Key, sourceValue);
            }
        }

        private static void ConditionIsValue<TTarget>(
            Type targetType,
            TTarget target,
            KeyValuePair<string, MappingConditions> condition)
        {
            SetPropertyValue(targetType, target, condition.Key, condition.Value.TargetData);
        }

        private static void ConditionIsCombination<TTarget>(
            Type targetType,
            TTarget target,
            KeyValuePair<string, MappingConditions> condition)
        {
            SetPropertyValue(targetType, target, condition.Key, condition.Value.TargetData);
        }

        private static void SetPropertyValue<TTarget>(Type targetType, TTarget target, string propertyName, object value)
        {
            var prop = targetType.GetProperty(propertyName);
            if (prop != null)
            {
                try
                {
                    var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                    prop.SetValue(target, convertedValue);
                }
                catch (Exception)
                {
                    throw new InvalidOperationException(
                        $"Cannot map source filed '{propertyName}' to target field '{prop.Name}'.");
                }
            }
        }
    }
}