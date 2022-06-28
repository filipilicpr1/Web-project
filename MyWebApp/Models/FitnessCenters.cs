using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class FitnessCenters
    {
        public static string FilePath { get; set; }

        public static string OwnerFilePath { get; set; }

        public static List<FitnessCenter> FitnessCentersList { get; set; }

        private static int GenerateId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }

        public static FitnessCenter FindById(int id)
        {
            return FitnessCentersList.Find(item => item.Id == id);
        }

        public static List<FitnessCenter> FindByName(string name)
        {
            return FitnessCentersList.FindAll(item => String.Equals(item.Name, name));
        }

        public static List<FitnessCenter> SearchAllByName(string name)
        {
            return FitnessCentersList.FindAll(item => item.Name.ToLower().Contains(name.ToLower()) && !item.Deleted);
        }

        public static List<FitnessCenter> FindByAddress(string address)
        {
            return FitnessCentersList.FindAll(item => String.Equals(item.Address, address));
        }

        public static List<FitnessCenter> SearchAllByAddress(string address)
        {
            return FitnessCentersList.FindAll(item => item.Address.ToLower().Contains(address.ToLower()) && !item.Deleted);
        }

        public static List<FitnessCenter> FindAllOwned(User u)
        {
            List<FitnessCenter> retVal = new List<FitnessCenter>();
            foreach(var item in u.FitnessCentersOwned)
            {
                if (item.Deleted)
                {
                    continue;
                }
                retVal.Add(FitnessCenters.FindById(item.Id));
            }
            return retVal;
        }

        public static void AddFitnessCenter(FitnessCenter fc, User u)
        {
            fc.Id = GenerateId();
            fc.Deleted = false;
            fc.Owner = new User(u);
            FitnessCentersList.Add(fc);
            u.FitnessCentersOwned.Add(new FitnessCenter(fc));
            SaveFitnessCenters();
            SaveFitnessCenterOwner();
        }

        public static void UpdateFitnessCenter(FitnessCenter fc)
        {
            FitnessCenter originalFc = FindById(fc.Id);
            if(fc == null)
            {
                return;
            }
            originalFc.Name = fc.Name;
            originalFc.Address = fc.Address;
            originalFc.YearCreated = fc.YearCreated;
            originalFc.MonthlySubscription = fc.MonthlySubscription;
            originalFc.YearlySubscription = fc.YearlySubscription;
            originalFc.TrainingCost = fc.TrainingCost;
            originalFc.GroupTrainingCost = fc.GroupTrainingCost;
            originalFc.PersonalTrainingCost = fc.PersonalTrainingCost;
            SaveFitnessCenters();
        }

        public static void LoadInitialFitnessCenters()
        {
            FitnessCentersList = new List<FitnessCenter>();
            StreamReader sr = new StreamReader(FilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                FitnessCenter fc = GetSingleFitnessCenter(line);
                FitnessCentersList.Add(fc);
            }
            sr.Close();
        }

        public static void FinishLoading()
        {
            StreamReader sr = new StreamReader(OwnerFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                AssignOwner(line);
            }
            sr.Close();
        }

        private static FitnessCenter GetSingleFitnessCenter(string line)
        {
            FitnessCenter fc = new FitnessCenter();
            string[] values = line.Split(';');
            fc.Id = int.Parse(values[0]);
            fc.Name = values[1];
            fc.Address = values[2];
            fc.YearCreated = int.Parse(values[3]);
            fc.MonthlySubscription = int.Parse(values[4]);
            fc.YearlySubscription = int.Parse(values[5]);
            fc.TrainingCost = int.Parse(values[6]);
            fc.GroupTrainingCost = int.Parse(values[7]);
            fc.PersonalTrainingCost = int.Parse(values[8]);
            fc.Deleted = bool.Parse(values[9]);
            return fc;
        }

        private static void AssignOwner(string line)
        {
            int userId = int.Parse(line.Split(';')[0]);
            int id = int.Parse(line.Split(';')[1]);
            FitnessCenter fc = FindById(id);
            fc.Owner = new User(Users.FindById(userId));
        }

        private static void SaveFitnessCenters()
        {
            StreamWriter sw = new StreamWriter(FilePath);
            foreach (FitnessCenter fc in FitnessCentersList)
            {
                string text = fc.Id + ";" + fc.Name + ";" + fc.Address + ";" + fc.YearCreated + ";" + fc.MonthlySubscription + ";" + fc.YearlySubscription + ";" + fc.TrainingCost + ";" + fc.GroupTrainingCost + ";" + fc.PersonalTrainingCost + ";" + fc.Deleted;
                sw.WriteLine(text);
            }
            sw.Close();
        }

        private static void SaveFitnessCenterOwner()
        {
            StreamWriter sw = new StreamWriter(OwnerFilePath);
            foreach (FitnessCenter fc in FitnessCentersList)
            {
                string text = fc.Owner.Id + ";" + fc.Id;
                sw.WriteLine(text);
            }
            sw.Close();
        }
    }
}