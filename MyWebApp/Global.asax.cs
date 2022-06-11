using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MyWebApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SetDateFormat("dd/MM/yyyy");
            LoadUsers(Server.MapPath("~/App_Data/Korisnik.txt"));

        }

        private void SetDateFormat(string dateFormat)
        {
            CultureInfo ci = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = dateFormat;
            ci.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = ci;
        }

        private void LoadUsers(string filePath)
        {
            if(Users.UsersList == null)
            {
                Users.UsersList = new List<User>();
            }

            StreamReader sr = new StreamReader(filePath);
            string line;
            while((line = sr.ReadLine()) != null)
            {
                User u = GetSingleUser(line);
                Users.UsersList.Add(u);
            }
            sr.Close();
        }

        private User GetSingleUser(string line)
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
