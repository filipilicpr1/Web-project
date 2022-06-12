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

        public static List<FitnessCenter> FitnessCentersList { get; set; }

        public static List<FitnessCenter> FindByName(string name)
        {
            return FitnessCentersList.FindAll(item => String.Equals(item.Name, name));
        }

        public static FitnessCenter FindByAddress(string address)
        {
            return FitnessCentersList.Find(item => String.Equals(item.Address, address));
        }

        public static void LoadFitnessCenters()
        {
            if(FitnessCentersList == null)
            {
                FitnessCentersList = new List<FitnessCenter>();
            }

            StreamReader sr = new StreamReader(FilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                FitnessCenter fc = GetSingleFitnessCenter(line);
                FitnessCentersList.Add(fc);
            }
            sr.Close();
        }

        private static FitnessCenter GetSingleFitnessCenter(string line)
        {
            FitnessCenter fc = new FitnessCenter();
            string[] values = line.Split('-');
            fc.Name = values[0];
            fc.Address = values[1];
            fc.YearCreated = int.Parse(values[2]);
            fc.Owner = Users.FindByUsername(values[3]);
            fc.MonthlySubscription = int.Parse(values[4]);
            fc.YearlySubscription = int.Parse(values[5]);
            fc.TrainingCost = int.Parse(values[6]);
            fc.GroupTrainingCost = int.Parse(values[7]);
            fc.PersonalTrainingCost = int.Parse(values[8]);
            return fc;
        }
    }
}