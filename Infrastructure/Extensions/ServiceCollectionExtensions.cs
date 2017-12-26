using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using mywebapi.Infrastructure.DI.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace mywebapi.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider GetAutofacServiceProvider(this IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            //Services
            builder.Populate(services);

            //Register custom modules
            builder.RegisterModule<MainModule>();

            return new AutofacServiceProvider(builder.Build());
        }

        public static IMvcBuilder AddFeatureFolders(this IMvcBuilder services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddRazorOptions(o =>
            {
                o.ViewLocationExpanders.Add(new FeatureFolderViewLocationExpander());
            });

            return services;
        }
    }
}