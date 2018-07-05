﻿using Autofac;
using Autofac.Integration.Mvc;
using Phoenix.Controllers;
using Phoenix.DapperDal;
using Phoenix.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Phoenix
{
    public static class DependencyInjection
    {
        private static IContainer Container { get; set; }

        public static void Initialize()
        {
            var builder = new ContainerBuilder();

            builder
                .RegisterType<SqlConnectionFactory>()
                .As<IDbConnectionFactory>()
                .WithParameter("connectionString", ConfigurationManager.ConnectionStrings["RCIDatabase"].ConnectionString);

            builder
                .RegisterType<DapperDal.DapperDal>()
                .As<IDal>();

            // Register services
            builder
                .RegisterType<AdminDashboardService>()
                .As<IAdminDashboardService>();

            builder
                .RegisterType<RciInputService>()
                .As<IRciInputService>();

            builder
                .RegisterType<LoginService>()
                .As<ILoginService>();

            //Register Controllers
            builder
                .RegisterType<AdminDashboardController>()
                .InstancePerRequest();

            builder
                .RegisterType<RciInputController>()
                .InstancePerRequest();

            builder
                .RegisterType<LoginController>()
                .InstancePerRequest();

            builder
                .RegisterType<RciCheckoutController>()
                .InstancePerRequest();

            Container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
        }
    }
}