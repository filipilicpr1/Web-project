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

        public static List<GroupTraining> FindAllByFitnessCenterId(int fitnessId)
        {
            return GroupTrainingsList.FindAll(item => item.FitnessCenterLocation.Id == fitnessId);
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
        }

        public static void FinishLoading()
        {
            LoadFitnessCenterLocations();
            LoadFitnessGroupTrainingVisitors();
        }

        private static GroupTraining GetSingleGroupTraining(string line)
        {
            GroupTraining gt = new GroupTraining();
            string[] values = line.Split('-');
            gt.Id = int.Parse(values[0]);
            gt.Name = values[1];
            gt.TrainingType = values[2];
            gt.Duration = int.Parse(values[3]);
            gt.DateOfTraining = DateTime.Parse(values[4]);
            gt.VisitorCapacity = int.Parse(values[5]);
            return gt;
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
            int fitnessCenterId = int.Parse(line.Split('-')[0]);
            int groupTrainingId = int.Parse(line.Split('-')[1]);
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
            int id = int.Parse(line.Split('-')[0]);
            string username = line.Split('-')[1];
            GroupTraining gt = GroupTrainings.FindById(id);
            gt.Visitors.Add(new User(Users.FindByUsername(username)));
            gt.VisitorCount = gt.Visitors.Count;
        }
    }
}