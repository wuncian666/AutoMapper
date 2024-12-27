using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using 自製AutoMapper;
using 自製AutoMapper.mappers;

namespace AutoMapper.Mappers
{
    internal class ArrayMapper<TSource, TTarget> :
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
            var sourceType = property.GetValue(source).GetType();

            Type targetElementType = targetProp.PropertyType.GetElementType();
            Array sourceArray = (Array)property.GetValue(source);
            Array targetArray = Array.CreateInstance(targetElementType, sourceArray.Length);

            MethodInfo findMapMethod = typeof(Mapper).GetMethod("Map");
            MethodInfo mapMethod = findMapMethod.MakeGenericMethod(
                sourceType.GetElementType(), targetElementType);
            for (int i = 0; i < sourceArray.Length; i++)
            {
                object temp = mapMethod.Invoke(
                    null, new object[] { sourceArray.GetValue(i), null });
                targetArray.SetValue(temp, i);
            }
            targetProp.SetValue(target, targetArray);
            return target;
        }
    }
}