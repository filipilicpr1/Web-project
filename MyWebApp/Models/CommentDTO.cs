using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class CommentDTO
    {
        public string Text { get; set; }
        public int Rating { get; set; }
        public int FitnessCenterId { get; set; }
    }
}