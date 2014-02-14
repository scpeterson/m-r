﻿using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleCQRS.Api
{
    public static class MvcConfig
    {

        public static void RegisterRoutes(RouteCollection routes)
        {
            // ignore
            routes.IgnoreRoute("{file}.js");
            routes.IgnoreRoute("{file}.html");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

    }
}