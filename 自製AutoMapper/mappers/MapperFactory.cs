using AutoMapper.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 自製AutoMapper.Enums;

namespace 自製AutoMapper.mappers
{
    public class MapperFactory
    {
        public MapperFactory()
        { }

        public static AbstractMapper<TSource, TTarget>
            Create<TSource, TTarget>(PropertyType type) where TTarget : new()
        {
            AbstractMapper<TSource, TTarget> mapper = null;
            switch (type)
            {
                case PropertyType.Basic:
                    mapper = new BasicMapper<TSource, TTarget>();
                    break;

                case PropertyType.Array:
                    mapper = new ArrayMapper<TSource, TTarget>();
                    break;

                case PropertyType.Enum:
                    mapper = new EnumToEnumMapper<TSource, TTarget>();
                    break;

                case PropertyType.Class:
                    mapper = new MultiObjectMapper<TSource, TTarget>();
                    break;
            }

            return mapper;
        }
    }
}