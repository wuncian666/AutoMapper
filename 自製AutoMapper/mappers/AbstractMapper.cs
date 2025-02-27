using System;
using System.Reflection;

namespace 自製AutoMapper.mappers
{
    public abstract class AbstractMapper<TSource, TTarget>
    {
        public abstract TTarget AssignValue(
            TSource source,
            PropertyInfo property,
            Type targetType);
    }
}