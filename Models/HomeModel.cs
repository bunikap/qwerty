using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace qwerty.Models
{
    public class HomeModel
    {
        public string title { get; set; }
        public string detail { get; set; }
        public string start { get; set; }
        public string deadline { get; set; }
        public string approver { get; set; }
        public string status { get; set; }
    }
}