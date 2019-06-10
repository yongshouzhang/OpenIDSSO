using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using System.Reflection;
using System.Security.Principal;

namespace OpenIDSSO
{
    using Services;
    using Repository;
    using Infrastructure;
    public class AutofacConfig
    {
        public static void RegisterComponent()
        {
            var builder = new ContainerBuilder();
            Assembly current = Assembly.GetExecutingAssembly();
            string assembleyName = current.GetName().Name;

            var interfaceList = current.GetTypes().Where(obj =>
            {
                return obj.IsInterface &&
                  string.Compare(obj.Namespace,
                      string.Join(".", new string[] { assembleyName, "services" }), true) == 0;
            }).Select(obj =>
            {
                return new
                {
                    type = obj,
                    name = obj.Name.ToLower().Replace("service", "")
                };
            });

            var implementList = current.GetTypes().Where(obj =>
            {
                return obj.IsPublic && !obj.IsInterface
                && obj.Name.ToLower().Contains("service")
                && string.Compare(obj.Namespace,
                string.Join(".", new string[]
                {
                    assembleyName,"repository"
                }), true) == 0;
            }).Select(obj =>
            {
                return new { type = obj, name = obj.Name.ToLower().Replace("service", "") };
            });

            builder.RegisterControllers(current);

            builder.RegisterGeneric(typeof(BaseRepository<>))
                .As(typeof(IBaseRepository<>)).SingleInstance();
            builder.RegisterType<UserModelRepositoryService>().As<IUserModelService>();

            builder.RegisterType<LoginLogRepositoryService>().As<ILoginLogService>();

            builder.RegisterType<FormsAuthenticationService>().As<IFormsAuthentication>();

            builder.RegisterType<OpenIDPrincipal>().As<IOpenIDPrincipal>();

            builder.RegisterType<OpenIDIdentity>().As<IOpenIDIdentity>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

    }
}