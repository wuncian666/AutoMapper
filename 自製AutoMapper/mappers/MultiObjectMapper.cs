using System;
using System.Reflection;
using 自製AutoMapper;
using 自製AutoMapper.mappers;

namespace AutoMapper.Mappers
{
    internal class MultiObjectMapper<TSource, TTarget> :
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

            MethodInfo findMapMethod = typeof(Mapper).GetMethod("Map");
            MethodInfo mapMethod =
                findMapMethod.MakeGenericMethod(
                property.PropertyType,
                targetProp.PropertyType);
            object temp = mapMethod.Invoke(null, new object[] { sourceValue, null });

            targetProp.SetValue(target, temp);

            return target;
        }
    }
}