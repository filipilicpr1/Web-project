using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public FitnessCenter RelatedFitnessCenter { get; set; }
        public User Creator { get; set; }

        public Comment() { }

        public Comment(Comment c)
        {
            this.Id = c.Id;
            this.Text = c.Text;
            this.Rating = c.Rating;
        }
    }
}