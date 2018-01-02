using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MyWebApi.Infrastructure;
using MyWebApi.Infrastructure.DI.Modules;

namespace Microsoft.Extensions.DependencyInjection
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