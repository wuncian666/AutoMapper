using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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