using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 自製AutoMapper
{
    internal class MappingConditions
    {
        public string SourcePropertyName { get; set; }

        public object TargetData { get; set; } // target propertyName, target value, target function

        public ObjType ObjType { get; set; }

        public MappingConditions(string sourcePropertyName, object targetData, ObjType type)
        {
            this.SourcePropertyName = sourcePropertyName;
            this.TargetData = targetData;
            this.ObjType = type;
        }
    }
}