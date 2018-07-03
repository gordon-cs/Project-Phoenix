using Autofac;
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

            builder
                .RegisterType<AdminDashboardService>()
                .As<IAdminDashboardService>();

            builder
                .RegisterType<RciInputService>()
                .As<IRciInputService>();

            builder
                .RegisterType<AdminDashboardController>()
                .InstancePerRequest();

            builder
                .RegisterType<RciInputController>()
                .InstancePerRequest();

            Container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
        }
    }
}