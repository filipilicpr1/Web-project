using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class GroupTraining
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TrainingType { get; set; }
        public FitnessCenter FitnessCenterLocation { get; set; }
        public int Duration { get; set; }
        public DateTime DateOfTraining { get; set; }
        public int VisitorCapacity { get; set; }
        public List<User> Visitors { get; set; }
        public int VisitorCount { get; set; }

        public GroupTraining()
        {
            Visitors = new List<User>();
        }

        public GroupTraining(GroupTraining gt)
        {
            this.Id = gt.Id;
            this.Name = gt.Name;
            this.TrainingType = gt.TrainingType;
            this.Duration = gt.Duration;
            this.VisitorCapacity = gt.VisitorCapacity;
            this.VisitorCount = gt.VisitorCount;
        }
    }
}