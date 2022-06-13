using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public enum EUserType { POSETILAC,TRENER,VLASNIK}

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public EUserType UserType { get; set; }
        public List<GroupTraining> VisitingGroupTrainings { get; set; }
        public List<GroupTraining> TrainingGroupTrainings { get; set; }
        public FitnessCenter FitnessCenterTrainer { get; set; }
        public List<FitnessCenter> FitnessCentersOwned { get; set; }

        public User() { }

        public User(User u)
        {
            this.Username = u.Username;
            this.Password = u.Password;
            this.Name = u.Name;
            this.LastName = u.LastName;
            this.Gender = u.Gender;
            this.Email = u.Email;
            this.BirthDate = u.BirthDate;
            this.UserType = u.UserType;
        }

    }
}