using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 自製AutoMapper.Enums;

namespace 自製AutoMapper
{
    internal class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public Parent Parent { get; set; }

        public Children[] Childrens { get; set; }

        public Sex Sex { get; set; }
    }
}