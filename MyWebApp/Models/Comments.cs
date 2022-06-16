using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class Comments
    {
        public static string FilePath { get; set; }

        public static string VisitorFilePath { get; set; }

        public static string RelatedFitnessCenterFilePath { get; set; }

        public static List<Comment> CommentsList { get; set; }

        private static int GenerateId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }

        public static Comment FindById(int id)
        {
            return CommentsList.Find(item => item.Id == id);
        }

        public static List<Comment> FindAllByFitnessCenterId(int fitnessId)
        {
            return CommentsList.FindAll(item => item.RelatedFitnessCenter.Id == fitnessId);
        }

        public static void LoadInitialComments()
        {
            CommentsList = new List<Comment>();
            StreamReader sr = new StreamReader(FilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                Comment c = GetSingleComment(line);
                CommentsList.Add(c);
            }
            sr.Close();
        }

        public static void FinishLoading()
        {
            LoadCreators();
            LoadRelatedFitnessCenters();
        }

        private static Comment GetSingleComment(string line)
        {
            Comment c = new Comment();
            string[] values = line.Split(';');
            c.Id = int.Parse(values[0]);
            c.Text = values[1];
            c.Rating = int.Parse(values[2]);
            return c;
        }

        private static void LoadCreators()
        {
            StreamReader sr = new StreamReader(VisitorFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                AssignCreator(line);
            }
            sr.Close();
        }

        private static void AssignCreator(string line)
        {
            int userId = int.Parse(line.Split(';')[0]);
            int id = int.Parse(line.Split(';')[1]);
            Comment c = FindById(id);
            c.Creator = new User(Users.FindById(userId));
        }

        private static void LoadRelatedFitnessCenters()
        {
            StreamReader sr = new StreamReader(RelatedFitnessCenterFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                AssignRelatedFitnessCenter(line);
            }
            sr.Close();
        }

        private static void AssignRelatedFitnessCenter(string line)
        {
            int fitnessCenterId = int.Parse(line.Split(';')[0]);
            int commentId = int.Parse(line.Split(';')[1]);
            Comment c = FindById(commentId);
            c.RelatedFitnessCenter = new FitnessCenter(FitnessCenters.FindById(fitnessCenterId));
        }
    }
}