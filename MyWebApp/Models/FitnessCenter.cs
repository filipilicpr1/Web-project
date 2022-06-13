using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class FitnessCenter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int YearCreated { get; set; }
        public User Owner { get; set; }
        public int MonthlySubscription { get; set; }
        public int YearlySubscription { get; set; }
        public int TrainingCost { get; set; }
        public int GroupTrainingCost { get; set; }
        public int PersonalTrainingCost { get; set; }

        public FitnessCenter() {}

        public FitnessCenter(FitnessCenter fc)
        {
            this.Id = fc.Id;
            this.Name = fc.Name;
            this.Address = fc.Address;
            this.YearCreated = fc.YearCreated;
            this.MonthlySubscription = fc.MonthlySubscription;
            this.YearlySubscription = fc.YearlySubscription;
            this.TrainingCost = fc.TrainingCost;
            this.GroupTrainingCost = fc.GroupTrainingCost;
            this.PersonalTrainingCost = fc.PersonalTrainingCost;
        }
    }
}