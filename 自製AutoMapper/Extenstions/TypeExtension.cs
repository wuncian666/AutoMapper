using System;
using System.Collections;
using 自製AutoMapper.Enums;

namespace 自製AutoMapper
{
    internal static class TypeExtension
    {
        public static PropertyType GetPropertyTypeEnum(this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            if (underlyingType == typeof(string))
                return PropertyType.Basic;

            if (underlyingType.IsArray)
                return PropertyType.Array;

            if (underlyingType.IsEnum)
                return PropertyType.Enum;

            if (underlyingType.IsClass)
                return PropertyType.Class;

            if (typeof(IEnumerable).IsAssignableFrom(underlyingType))
                return PropertyType.Collection;

            return PropertyType.Basic;
        }
    }
}