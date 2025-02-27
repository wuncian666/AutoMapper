using System;
using System.Reflection;
using 自製AutoMapper.mappers;

namespace AutoMapper.Mappers
{
    public class BasicMapper<TSource, TTarget> :
        AbstractMapper<TSource, TTarget> where TTarget : new()
    {
        public override TTarget AssignValue(
            TSource source,
            PropertyInfo property,
            Type targetType)
        {
            var sourcePropName = property.Name;
            var targetProp = targetType.GetProperty(sourcePropName);

            TTarget target = new TTarget();
            targetProp.SetValue(target, property.GetValue(source));

            return target;
        }
    }
}