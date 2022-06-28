using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class GroupTrainings
    {
        public static string FilePath { get; set; }

        public static string FitnessCenterGroupTrainingFilePath { get; set; }

        public static string GroupTrainingVisitorFilePath { get; set; }

        public static List<GroupTraining> GroupTrainingsList { get; set;}

        private static int GenerateId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }

        public static GroupTraining FindById(int id)
        {
            return GroupTrainingsList.Find(item => item.Id == id);
        }

        public static List<GroupTraining> FindAllUpcomingByFitnessCenterId(int fitnessId)
        {
            return GroupTrainingsList.FindAll(item => (item.FitnessCenterLocation.Id == fitnessId) && item.Upcoming && !item.Deleted);
        }

        public static void RegisterUserForTraining(User u, GroupTraining gt)
        {
            if(gt.VisitorCapacity == gt.VisitorCount)
            {
                return;
            }
            u.VisitingGroupTrainings.Add(new GroupTraining(gt));
            gt.Visitors.Add(new User(u));
            gt.VisitorCount++;
            // ne treba da se zapamte treninzi, jer u tom fajlu nista nije izmenjeno
            SaveGroupTrainingVisitors();
        }

        public static List<GroupTraining> FindVisitedGroupTrainings(User u)
        {
            // mora ovako jer svaki grupni trening u VisitingGRoupTraining za FitnessCenterLocation ima null
            var list = u.VisitingGroupTrainings.FindAll(item => !item.Upcoming && !item.Deleted);
            List<GroupTraining> retVal = new List<GroupTraining>();
            foreach (var item in list)
            {
                retVal.Add(GroupTrainings.FindById(item.Id));
            }
            return retVal;
        }

        public static List<GroupTraining> FindCompletedTrainingsByTrainer(User u)
        {
            var list = u.TrainingGroupTrainings.FindAll(item => !item.Upcoming && !item.Deleted);
            List<GroupTraining> retVal = new List<GroupTraining>();
            foreach (var item in list)
            {
                retVal.Add(GroupTrainings.FindById(item.Id));
            }
            return retVal;
        }

        public static List<GroupTraining> FindAllTrainingsByTrainer(User u)
        {
            List<GroupTraining> retVal = new List<GroupTraining>();
            foreach (var item in u.TrainingGroupTrainings)
            {
                if (item.Deleted)
                {
                    continue;
                }
                retVal.Add(GroupTrainings.FindById(item.Id));
            }
            return retVal;
        }

        public static void AddGroupTraining(GroupTraining gt,User trainer)
        {
            gt.Id = GenerateId();
            gt.Upcoming = true;
            gt.Deleted = false;
            gt.VisitorCount = 0;
            gt.FitnessCenterLocation = new FitnessCenter(FitnessCenters.FindById(trainer.FitnessCenterTrainer.Id));
            GroupTrainingsList.Add(gt);
            trainer.TrainingGroupTrainings.Add(new GroupTraining(gt));
            SaveGroupTrainings();
            SaveFitnessCenterGroupTraining();
            Users.SaveGroupTrainingTrainer();
        }

        public static void UpdateGroupTraining(GroupTraining gt)
        {
            GroupTraining originalGt = FindById(gt.Id);
            if (originalGt == null)
            {
                return;
            }
            originalGt.Name = gt.Name;
            originalGt.TrainingType = gt.TrainingType;
            originalGt.Duration = gt.Duration;
            originalGt.VisitorCapacity = gt.VisitorCapacity;
            originalGt.DateOfTraining = gt.DateOfTraining;
            originalGt.Upcoming = CheckDate(originalGt.DateOfTraining);
            SaveGroupTrainings();
        }

        public static void DeleteGroupTraining(GroupTraining gt, User u)
        {
            gt.Deleted = true;
            foreach(var item in u.TrainingGroupTrainings)
            {
                if(item.Id == gt.Id)
                {
                    item.Deleted = true;
                    break;
                }
            }
            SaveGroupTrainings();
        }

        public static void LoadInitialGroupTrainings()
        {
            GroupTrainingsList = new List<GroupTraining>();
            StreamReader sr = new StreamReader(FilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                GroupTraining gt = GetSingleGroupTraining(line);
                GroupTrainingsList.Add(gt);
            }
            sr.Close();
            // za svaki trening odredi da li je u buducnosti ili u proslosti
            UpdateGroupTrainings();
        }

        private static GroupTraining GetSingleGroupTraining(string line)
        {
            GroupTraining gt = new GroupTraining();
            string[] values = line.Split(';');
            gt.Id = int.Parse(values[0]);
            gt.Name = values[1];
            gt.TrainingType = values[2];
            gt.Duration = int.Parse(values[3]);
            gt.DateOfTraining = DateTime.Parse(values[4]);
            gt.VisitorCapacity = int.Parse(values[5]);
            gt.Deleted = bool.Parse(values[6]);
            return gt;
        }

        public static void UpdateGroupTrainings()
        {
            foreach(var gt in GroupTrainingsList)
            {
                gt.Upcoming = CheckDate(gt.DateOfTraining);
            }
        }

        private static bool CheckDate(DateTime gtDate)
        {
            int gtYear = gtDate.Year;
            int gtMonth = gtDate.Month;
            int gtDay = gtDate.Day;
            int gtHour = gtDate.Hour;
            int gtMinute = gtDate.Minute;
            DateTime currentDate = DateTime.Now;
            int currYear = currentDate.Year;
            int currMonth = currentDate.Month;
            int currDay = currentDate.Day;
            int currHour = currentDate.Hour;
            int currMinute = currentDate.Minute;

            if(gtYear > currYear)
            {
                return true;
            }

            if(gtYear < currYear)
            {
                return false;
            }

            if(gtMonth > currMonth)
            {
                return true;
            }

            if(gtMonth < currMonth)
            {
                return false;
            }

            if(gtDay > currDay)
            {
                return true;
            }

            if(gtDay < currDay)
            {
                return false;
            }

            if(gtHour > currHour)
            {
                return true;
            }

            if(gtHour < currHour)
            {
                return false;
            }

            if(gtMinute > currMinute)
            {
                return true;
            }

            if(gtMinute < currMinute)
            {
                return false;
            }

            return true;
        }

        public static void FinishLoading()
        {
            LoadFitnessCenterLocations();
            LoadFitnessGroupTrainingVisitors();
        }

        private static void LoadFitnessCenterLocations()
        {
            StreamReader sr = new StreamReader(FitnessCenterGroupTrainingFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                AssignFitnessCenter(line);
            }
            sr.Close();
        }

        private static void AssignFitnessCenter(string line)
        {
            int fitnessCenterId = int.Parse(line.Split(';')[0]);
            int groupTrainingId = int.Parse(line.Split(';')[1]);
            GroupTraining gt = GroupTrainings.FindById(groupTrainingId);
            gt.FitnessCenterLocation = new FitnessCenter(FitnessCenters.FindById(fitnessCenterId));
        }

        private static void LoadFitnessGroupTrainingVisitors()
        {
            StreamReader sr = new StreamReader(GroupTrainingVisitorFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                AssignGroupTrainingVisitor(line);
            }
            sr.Close();
        }

        private static void AssignGroupTrainingVisitor(string line)
        {
            int id = int.Parse(line.Split(';')[0]);
            int userId = int.Parse(line.Split(';')[1]);
            GroupTraining gt = GroupTrainings.FindById(id);
            gt.Visitors.Add(new User(Users.FindById(userId)));
            gt.VisitorCount = gt.Visitors.Count;
        }

        private static void SaveGroupTrainings()
        {
            StreamWriter sw = new StreamWriter(FilePath);
            foreach (GroupTraining gt in GroupTrainingsList)
            {
                string text = gt.Id + ";" + gt.Name + ";" + gt.TrainingType + ";" + gt.Duration + ";" + gt.DateOfTraining.ToString("dd/MM/yyyy HH:mm") + ";" + gt.VisitorCapacity + ";" + gt.Deleted;
                sw.WriteLine(text);
            }
            sw.Close();
        }

        private static void SaveGroupTrainingVisitors()
        {
            StreamWriter sw = new StreamWriter(GroupTrainingVisitorFilePath);
            foreach (GroupTraining gt in GroupTrainingsList)
            {
                foreach(User u in gt.Visitors)
                {
                    string text = gt.Id + ";" + u.Id;
                    sw.WriteLine(text);
                }
            }
            sw.Close();
        }

        private static void SaveFitnessCenterGroupTraining()
        {
            StreamWriter sw = new StreamWriter(FitnessCenterGroupTrainingFilePath);
            foreach (GroupTraining gt in GroupTrainingsList)
            {
                string text = gt.FitnessCenterLocation.Id + ";" + gt.Id;
                sw.WriteLine(text);
            }
            sw.Close();
        }
    }
}