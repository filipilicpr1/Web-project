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

        public static User FindById(int id)
        {
            return UsersList.Find(item => item.Id == id);
        }

        private static int GenerateId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }

        public static User FindByUsername(string username)
        {
            return UsersList.Find(item => String.Equals(item.Username,username));
        }

        public static void AddUser(User user)
        {
            user.Id = GenerateId();
            user.Blocked = false;
            user.UserType = EUserType.POSETILAC;
            user.LoggedIn = false;
            user.VisitingGroupTrainings = new List<GroupTraining>();
            UsersList.Add(user);
            SaveUsers();
        }

        public static void UpdateUser(User user)
        {
            User existingUser = FindById(user.Id);
            if(existingUser == null)
            {
                return;
            }
            existingUser.Username = user.Username;
            existingUser.Password = user.Password;
            existingUser.Name = user.Name;
            existingUser.LastName = user.LastName;
            existingUser.BirthDate = user.BirthDate;
            existingUser.Email = user.Email;
            existingUser.Gender = user.Gender;
            SaveUsers();
            Comments.UpdateCommentCreator(existingUser);
            FitnessCenters.UpdateFitnessCenterOwner(existingUser);
            GroupTrainings.UpdateGroupTrainingVisitor(existingUser);
        }

        public static List<User> FindEligibleTrainers()
        {
            return UsersList.FindAll(item => (item.UserType == EUserType.POSETILAC) && item.VisitingGroupTrainings.Count == 0);
        }

        public static void RegisterTrainer(User newTrainer, FitnessCenter fc)
        {
            newTrainer.UserType = EUserType.TRENER;
            newTrainer.TrainingGroupTrainings = new List<GroupTraining>();
            newTrainer.FitnessCenterTrainer = new FitnessCenter(fc);
            SaveUsers();
            SaveFitnessCenterTrainer();
        }

        public static List<User> FindFitnessCenterTrainers(FitnessCenter fc)
        {
            return UsersList.FindAll(item => (item.UserType == EUserType.TRENER) && !item.Blocked && (item.FitnessCenterTrainer.Id == fc.Id));
        }

        public static void BlockTrainer(User trainer)
        {
            trainer.Blocked = true;
            SaveUsers();
        }

        public static void BlockAllFitnessCenterTrainers(FitnessCenter fc)
        {
            foreach(User u in UsersList)
            {
                if(u.UserType != EUserType.TRENER)
                {
                    continue;
                }
                if (u.Blocked)
                {
                    continue;
                }
                if(u.FitnessCenterTrainer.Id == fc.Id)
                {
                    u.FitnessCenterTrainer.Deleted = true;
                    u.Blocked = true;
                }
            }
            SaveUsers();
        }

        public static void UpdateFitnessCenterTrainer(FitnessCenter fitnessCenter)
        {
            for(int i = 0; i < UsersList.Count; i++)
            {
                if(UsersList[i].UserType != EUserType.TRENER)
                {
                    continue;
                }
                if (UsersList[i].Blocked)
                {
                    continue;
                }
                if(UsersList[i].FitnessCenterTrainer.Id == fitnessCenter.Id)
                {
                    UsersList[i].FitnessCenterTrainer = new FitnessCenter(fitnessCenter);
                }
            }
        }

        public static void UpdateOwnedFitnessCenter(FitnessCenter fitnessCenter)
        {
            for (int i = 0; i < UsersList.Count; i++)
            {
                if (UsersList[i].UserType != EUserType.VLASNIK)
                {
                    continue;
                }
                for(int j = 0; j < UsersList[i].FitnessCentersOwned.Count; j++)
                {
                    if(UsersList[i].FitnessCentersOwned[j].Id == fitnessCenter.Id)
                    {
                        UsersList[i].FitnessCentersOwned[j] = new FitnessCenter(fitnessCenter);
                    }
                }
            }
        }

        public static void UpdateVisitingGroupTrainings(GroupTraining groupTraining)
        {
            for(int i = 0; i < UsersList.Count; i++)
            {
                if(UsersList[i].UserType != EUserType.POSETILAC)
                {
                    continue;
                }
                for(int j = 0; j < UsersList[i].VisitingGroupTrainings.Count; j++)
                {
                    if(UsersList[i].VisitingGroupTrainings[j].Id == groupTraining.Id)
                    {
                        UsersList[i].VisitingGroupTrainings[j] = new GroupTraining(groupTraining);
                    }
                }
            }
        }

        public static void UpdateTrainingGroupTrainings(GroupTraining groupTraining)
        {
            for(int i = 0; i < UsersList.Count; i++)
            {
                if(UsersList[i].UserType != EUserType.TRENER)
                {
                    continue;
                }
                if (UsersList[i].Blocked)
                {
                    continue;
                }
                for (int j = 0; j < UsersList[i].TrainingGroupTrainings.Count; j++)
                {
                    if(UsersList[i].TrainingGroupTrainings[j].Id == groupTraining.Id)
                    {
                        UsersList[i].TrainingGroupTrainings[j] = new GroupTraining(groupTraining);
                    }
                }
            }
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

        private static void SaveUsers()
        {
            StreamWriter sw = new StreamWriter(FilePath);
            foreach(User u in UsersList)
            {
                string text = u.Id + ";" + u.Username + ";" + u.Password + ";" + u.Name + ";" + u.LastName + ";" + u.Gender + ";" + u.Email + ";" + u.BirthDate.ToString("dd/MM/yyyy") + ";" + u.UserType.ToString() + ";" + u.Blocked;
                sw.WriteLine(text);
            }
            sw.Close();
        }

        public static void SaveGroupTrainingTrainer()
        {
            StreamWriter sw = new StreamWriter(GroupTrainingTrainerFilePath);
            foreach (User u in UsersList)
            {
                if(u.UserType != EUserType.TRENER)
                {
                    continue;
                }
                foreach(GroupTraining gt in u.TrainingGroupTrainings)
                {
                    string text = gt.Id + ";" + u.Id;
                    sw.WriteLine(text);
                }
            }
            sw.Close();
        }

        public static void SaveFitnessCenterTrainer()
        {
            StreamWriter sw = new StreamWriter(FitnessCenterTrainerFilePath);
            foreach (User u in UsersList)
            {
                if (u.UserType != EUserType.TRENER)
                {
                    continue;
                }
                string text = u.FitnessCenterTrainer.Id + ";" + u.Id;
                sw.WriteLine(text);
            }
            sw.Close();
        }

        private static User GetSingleUser(string line)
        {
            User u = new User();
            string[] values = line.Split(';');
            u.Id = int.Parse(values[0]);
            u.Username = values[1];
            u.Password = values[2];
            u.Name = values[3];
            u.LastName = values[4];
            u.Gender = values[5];
            u.Email = values[6];
            u.BirthDate = DateTime.Parse(values[7]);
            switch (values[8])
            {
                case "POSETILAC":
                    u.UserType = EUserType.POSETILAC;
                    u.VisitingGroupTrainings = new List<GroupTraining>();
                    break;
                case "TRENER":
                    u.UserType = EUserType.TRENER;
                    u.TrainingGroupTrainings = new List<GroupTraining>();
                    break;
                case "VLASNIK":
                    u.UserType = EUserType.VLASNIK;
                    u.FitnessCentersOwned = new List<FitnessCenter>();
                    break;
            }
            u.Blocked = bool.Parse(values[9]);
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
            int userId = int.Parse(line.Split(';')[0]);
            int id = int.Parse(line.Split(';')[1]);
            User u = FindById(userId);
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
            int id = int.Parse(line.Split(';')[0]);
            int userId = int.Parse(line.Split(';')[1]);
            User u = FindById(userId);
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
            int id = int.Parse(line.Split(';')[0]);
            int userId = int.Parse(line.Split(';')[1]);
            User u = FindById(userId);
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
            int id = int.Parse(line.Split(';')[0]);
            int userId = int.Parse(line.Split(';')[1]);
            User u = FindById(userId);
            u.FitnessCenterTrainer = new FitnessCenter(FitnessCenters.FindById(id));
        }

    }
}