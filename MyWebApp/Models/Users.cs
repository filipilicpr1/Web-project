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

        public static List<User> UsersList { get; set; }


        public static User FindByUsername(string username)
        {
            return UsersList.Find(item => String.Equals(item.Username,username));
        }

        public static void LoadUsers()
        {
            if (Users.UsersList == null)
            {
                Users.UsersList = new List<User>();
            }

            StreamReader sr = new StreamReader(FilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                User u = GetSingleUser(line);
                Users.UsersList.Add(u);
            }
            sr.Close();
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
            switch (values[6])
            {
                case "Posetilac":
                    u.UserType = EUserType.POSETILAC;
                    break;
                case "Trener":
                    u.UserType = EUserType.TRENER;
                    break;
                case "Vlasnik":
                    u.UserType = EUserType.VLASNIK;
                    break;
            }
            return u;
        }
    }
}