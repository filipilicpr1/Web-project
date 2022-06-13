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

        public static List<FitnessCenter> FindByAddress(string address)
        {
            return FitnessCentersList.FindAll(item => String.Equals(item.Address, address));
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
            string[] values = line.Split('-');
            fc.Id = int.Parse(values[0]);
            fc.Name = values[1];
            fc.Address = values[2];
            fc.YearCreated = int.Parse(values[3]);
            fc.MonthlySubscription = int.Parse(values[4]);
            fc.YearlySubscription = int.Parse(values[5]);
            fc.TrainingCost = int.Parse(values[6]);
            fc.GroupTrainingCost = int.Parse(values[7]);
            fc.PersonalTrainingCost = int.Parse(values[8]);
            return fc;
        }

        private static void AssignOwner(string line)
        {
            string username = line.Split('-')[0];
            int id = int.Parse(line.Split('-')[1]);
            FitnessCenter fc = FindById(id);
            fc.Owner = new User(Users.FindByUsername(username));
        }
    }
}