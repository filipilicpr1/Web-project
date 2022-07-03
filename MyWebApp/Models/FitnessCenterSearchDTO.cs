using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class FitnessCenterSearchDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int MinYear { get; set; }
        public int MaxYear { get; set; }
    }
}