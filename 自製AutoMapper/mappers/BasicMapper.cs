using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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