using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.Extensions.Logging;
using mywebapi.Services;

namespace mywebapi.Infrastructure.DI.Modules
{
    public class MainModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            //Interceptores
            builder.Register(c =>
                    {
                        var loggerFactory = c.Resolve<ILoggerFactory>();
                        return new StopwatchInterceptor(loggerFactory);
                    });

            //De forma individual....
            // builder.RegisterType<ValuesService>().As<IValuesService>()
            // .EnableInterfaceInterceptors();

            //Para no tener que incluir uno por uno todos los servicios
            builder.RegisterAssemblyTypes(typeof(IValuesService).GetTypeInfo().Assembly)
                   .AsImplementedInterfaces()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(StopwatchInterceptor));
        }
    }
}