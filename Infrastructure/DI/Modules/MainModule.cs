using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Castle.DynamicProxy;
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
                var stopwatchInterceptor = new StopwatchAsyncInterceptor(loggerFactory);
                return stopwatchInterceptor;
            });

            //De forma individual....
            //builder.RegisterType<ValuesService>().As<IValuesService>()
            //.EnableInterfaceInterceptors();

            //Para no tener que incluir uno por uno todos los servicios
            builder.RegisterAssemblyTypes(typeof(IValuesService).GetTypeInfo().Assembly)
                .OnActivating(args =>
                {
                    var interceptor = new StopwatchAsyncInterceptor(args.Context.Resolve<ILoggerFactory>());
                    var service = args.Component.Services.First();
                    var proxy = new ProxyGenerator().CreateInterfaceProxyWithTargetInterface(Type.GetType(service.Description), args.Instance, interceptor);

                    args.ReplaceInstance(proxy);

                }).AsImplementedInterfaces();
        }
    }
}