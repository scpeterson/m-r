using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using SimpleCQRS.Api.App_Start;

namespace SimpleCQRS.Api
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            MvcConfig.RegisterRoutes(RouteTable.Routes);
            MvcConfig.RegisterGlobalFilters(GlobalFilters.Filters);

        }

    }
}