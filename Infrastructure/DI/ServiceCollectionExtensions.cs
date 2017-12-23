using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using mywebapi.Infrastructure.DI.Modules;

namespace mywebapi.Infrastructure.DI
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
    }
}