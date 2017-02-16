using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ClickTourney
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }

    /// <summary>
    /// Contains global variables for use throughout the application.
    /// </summary>
    public static class Globals
    {
#if !DEBUG
        public const int PASSWORD_MIN_LENGTH = 6;
#else
        public const int PASSWORD_MIN_LENGTH = 2;
#endif
    }//End globals
}
