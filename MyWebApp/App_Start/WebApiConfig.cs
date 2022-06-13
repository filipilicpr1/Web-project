﻿using System;
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
                name: "SearchFitnessCenters",
                routeTemplate: "api/{controller}/{name}/{address}/{minYear}/{maxYear}",
                defaults: new {name = "", address = "", minYear = 0, maxYear = 10000 }
            );

        }
    }
}
