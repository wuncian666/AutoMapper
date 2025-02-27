using System;
using System.Reflection;
using 自製AutoMapper.mappers;

namespace AutoMapper.Mappers
{
    internal class EnumToEnumMapper<TSource, TTarget> :
        AbstractMapper<TSource, TTarget> where TTarget : new()
    {
        public override TTarget AssignValue(
            TSource source,
            PropertyInfo property,
            Type targetType)
        {
            TTarget target = new TTarget();

            var sourcePropName = property.Name;
            var targetProp = targetType.GetProperty(sourcePropName);
            var sourceValue = property.GetValue(source);

            if (targetProp.PropertyType == typeof(string))
            {
                targetProp.SetValue(target, sourceValue.ToString());
            }
            else if (targetProp.PropertyType == typeof(int))
            {
                var enumToInt = (int)(sourceValue);
                targetProp.SetValue(target, enumToInt);
            }

            return target;
        }
    }
}