using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MyWebApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "SearchVisitedGroupTrainings",
                routeTemplate: "api/{controller}/{fitnessCenter}/{trainingType}/{name}",
                defaults: new { fitnessCenter = "", name = "", trainingType = "" }
            );

            config.Routes.MapHttpRoute(
                name: "SearchFitnessCenters",
                routeTemplate: "api/{controller}/{name}/{address}/{minYear}/{maxYear}",
                defaults: new {name = "", address = "", minYear = 0, maxYear = 10000 }
            );

            config.Routes.MapHttpRoute(
                name: "SearchGroupTrainingsByFitnessCenterId",
                routeTemplate: "api/{controller}/{fitnessId}",
                defaults: new { fitnessId = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "LoginUser",
                routeTemplate: "api/{controller}/{username}/{password}",
                defaults: new { username = "", password = ""}
            );
        }
    }
}
