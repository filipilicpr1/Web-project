using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class Users
    {
        public static string FilePath { get; set; }

        public static string FitnessCenterOwnerFilePath { get; set; }

        public static string GroupTrainingVisitorFilePath { get; set; }

        public static string GroupTrainingTrainerFilePath { get; set; }

        public static string FitnessCenterTrainerFilePath { get; set; }

        public static List<User> UsersList { get; set; }


        public static User FindByUsername(string username)
        {
            return UsersList.Find(item => String.Equals(item.Username,username));
        }

        public static void LoadInitialUsers()
        {
            Users.UsersList = new List<User>();
            StreamReader sr = new StreamReader(FilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                User u = GetSingleUser(line);
                Users.UsersList.Add(u);
            }
            sr.Close();
        }

        public static void FinishLoading()
        {
            LoadOwnedFitnessCenters();
            LoadFitnessCenterTrainers();
            LoadVisitingGroupTrainings();
            LoadTrainingGroupTrainings();
        }

        private static User GetSingleUser(string line)
        {
            User u = new User();
            string[] values = line.Split('-');
            u.Username = values[0];
            u.Password = values[1];
            u.Name = values[2];
            u.LastName = values[3];
            u.Gender = values[4];
            u.Email = values[5];
            u.BirthDate = DateTime.Parse(values[6]);
            switch (values[7])
            {
                case "Posetilac":
                    u.UserType = EUserType.POSETILAC;
                    u.VisitingGroupTrainings = new List<GroupTraining>();
                    break;
                case "Trener":
                    u.UserType = EUserType.TRENER;
                    u.TrainingGroupTrainings = new List<GroupTraining>();
                    break;
                case "Vlasnik":
                    u.UserType = EUserType.VLASNIK;
                    u.FitnessCentersOwned = new List<FitnessCenter>();
                    break;
            }
            return u;
        }

        private static void LoadOwnedFitnessCenters()
        {
            StreamReader sr = new StreamReader(FitnessCenterOwnerFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                AssignOwnedFitnessCenter(line);
            }
            sr.Close();
        }

        private static void AssignOwnedFitnessCenter(string line)
        {
            string username = line.Split('-')[0];
            int id = int.Parse(line.Split('-')[1]);
            User u = FindByUsername(username);
            u.FitnessCentersOwned.Add(new FitnessCenter(FitnessCenters.FindById(id)));
        }

        private static void LoadVisitingGroupTrainings()
        {
            StreamReader sr = new StreamReader(GroupTrainingVisitorFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                AssignVisitingGroupTrainings(line);
            }
            sr.Close();
        }

        private static void AssignVisitingGroupTrainings(string line)
        {
            int id = int.Parse(line.Split('-')[0]);
            string username = line.Split('-')[1];
            User u = FindByUsername(username);
            u.VisitingGroupTrainings.Add(new GroupTraining(GroupTrainings.FindById(id)));
        }

        private static void LoadTrainingGroupTrainings()
        {
            StreamReader sr = new StreamReader(GroupTrainingTrainerFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                AssignTrainingGroupTrainings(line);
            }
            sr.Close();
        }

        private static void AssignTrainingGroupTrainings(string line)
        {
            int id = int.Parse(line.Split('-')[0]);
            string username = line.Split('-')[1];
            User u = FindByUsername(username);
            u.TrainingGroupTrainings.Add(new GroupTraining(GroupTrainings.FindById(id)));
        }

        private static void LoadFitnessCenterTrainers()
        {
            StreamReader sr = new StreamReader(FitnessCenterTrainerFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                AssignFitnessCenterTrainer(line);
            }
            sr.Close();
        }

        private static void AssignFitnessCenterTrainer(string line)
        {
            int id = int.Parse(line.Split('-')[0]);
            string username = line.Split('-')[1];
            User u = FindByUsername(username);
            u.FitnessCenterTrainer = new FitnessCenter(FitnessCenters.FindById(id));
        }

    }
}