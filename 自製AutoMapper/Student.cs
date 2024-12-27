using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 自製AutoMapper.Enums;

namespace 自製AutoMapper
{
    internal class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string StuEmail { get; set; }

        public string StuPhone { get; set; }

        public Parent Parent { get; set; }

        public Children[] Childrens { get; set; }

        public int Sex { get; set; }
    }
}