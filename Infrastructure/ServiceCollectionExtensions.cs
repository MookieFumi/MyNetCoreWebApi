using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using mywebapi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace mywebapi.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider ConfigureContainer(this IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);

            //Register services
            builder.RegisterType<ValuesService>().As<IValuesService>().EnableInterfaceInterceptors();

            //Interceptors
            builder.Register(c =>
                {
                    var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
                    return new CallLoggerInterceptor(loggerFactory);
                }).Named<IInterceptor>("log-calls");


            var container = builder.Build();

            return new AutofacServiceProvider(container);
        }
    }
}