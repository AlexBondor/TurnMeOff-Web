using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models.UserModel
{
    public class UserModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public List<string> Interests { get; set; }

    }
}