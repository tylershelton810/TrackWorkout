using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.Models
{
    public class NewUser
    {

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime Birthday { get; set; }

        public int WeightType { get; set; }

        public decimal Weight { get; set; }

    }
}