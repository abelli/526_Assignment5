﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using MessageParser.Models;
using WebMatrix.WebData;

namespace CIS726_Assignment2
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            WebSecurity.InitializeDatabaseConnection("AccountsDBContext", "Users", "ID", "username", autoCreateTables: false);

            ObjectMessageQueue.InitializeQueue(ObjectMessageQueue.AUTH_RESPONSE);
            ObjectMessageQueue.InitializeQueue(ObjectMessageQueue.DB_RESPONSE);
        }
    }
}