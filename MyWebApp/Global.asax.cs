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
            SetFilePaths();
            LoadData();
        }

        private void SetDateFormat(string dateFormat)
        {
            CultureInfo ci = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = dateFormat;
            ci.DateTimeFormat.ShortTimePattern = "HH:mm";
            ci.DateTimeFormat.DateSeparator = "/";
            ci.DateTimeFormat.TimeSeparator = ":";
            Thread.CurrentThread.CurrentCulture = ci;
        }

        private void SetFilePaths()
        {
            Users.FilePath = Server.MapPath("~/App_Data/Korisnik.txt");
            FitnessCenters.FilePath = Server.MapPath("~/App_Data/FitnesCentar.txt");
            GroupTrainings.FilePath = Server.MapPath("~/App_Data/GrupniTrening.txt");
            Comments.FilePath = Server.MapPath("~/App_Data/Komentar.txt");
            Users.FitnessCenterOwnerFilePath = Server.MapPath("~/App_Data/VlasnikFitnesCentar.txt");
            FitnessCenters.OwnerFilePath = Server.MapPath("~/App_Data/VlasnikFitnesCentar.txt");
            Users.FitnessCenterTrainerFilePath = Server.MapPath("~/App_Data/FitnesCentarTrener.txt");
            Users.GroupTrainingVisitorFilePath = Server.MapPath("~/App_Data/GrupniTreningPosetilac.txt");
            Users.GroupTrainingTrainerFilePath = Server.MapPath("~/App_Data/GrupniTreningTrener.txt");
            GroupTrainings.FitnessCenterGroupTrainingFilePath = Server.MapPath("~/App_Data/FitnesCentarGrupniTrening.txt");
            GroupTrainings.GroupTrainingVisitorFilePath = Server.MapPath("~/App_Data/GrupniTreningPosetilac.txt");
            Comments.VisitorFilePath = Server.MapPath("~/App_Data/PosetilacKomentar.txt");
            Comments.RelatedFitnessCenterFilePath = Server.MapPath("~/App_Data/FitnesCentarKomentar.txt");
        }

        private void LoadData()
        {
            Users.LoadInitialUsers();
            FitnessCenters.LoadInitialFitnessCenters();
            GroupTrainings.LoadInitialGroupTrainings();
            Comments.LoadInitialComments();
            Users.FinishLoading();
            FitnessCenters.FinishLoading();
            GroupTrainings.FinishLoading();
            Comments.FinishLoading();
        }
    }
}
