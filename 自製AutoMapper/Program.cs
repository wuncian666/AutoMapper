using System.Collections.Generic;

namespace 自製AutoMapper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parent parent = new Parent()
            {
                Father = "Jason",
                Mother = "Lisa"
            };

            Children[] childrens =
                {
                new Children()
                {
                    Name = "Lucy"
                },
                new Children()
                {
                    Name = "Susan"
                }
            };

            User user = new User()
            {
                Id = 1,
                Name = "John",
                Email = "John@gmail.com",
                Password = "123456",
                Phone = "0912345678",
                Parent = parent,
                Childrens = childrens,
                Sex = Enums.Sex.Man
            };

            Dictionary<string, string> map =
                new Dictionary<string, string>() {
                    { "Email", "StuEmail" } ,
                    { "Phone", "StuPhone" }
                };

            Student student = Mapper.Map<User, Student>(user);
        }

        private static string Add(string str1, string str2)
        {
            return str1 + str2;
        }
    }
}