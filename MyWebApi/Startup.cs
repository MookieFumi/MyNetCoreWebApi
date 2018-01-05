using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace MyWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddFeatureFolders()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        return factory.Create(typeof(SharedResource));
                    };
                });

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            var container = services.GetAutofacServiceProvider();
            return container;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRequestResponseLoggingMiddleware();

            app.UseRequestLocalization(GetRequestLocalizationOptions());
            app.UseMvcWithDefaultRoute();
        }

        private RequestLocalizationOptions GetRequestLocalizationOptions()
        {
            var supportedCultures = GetSupportedCultures();
            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures.First()),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            var cookieProvider = options.RequestCultureProviders
                .OfType<CookieRequestCultureProvider>()
                .First();
            cookieProvider.CookieName = "Culture";

            RemoveAcceptLanguageProvider(options);

            //var routeDataRequestCultureProvider = new RouteDataRequestCultureProvider { Options = options };
            //options.RequestCultureProviders.Insert(0, routeDataRequestCultureProvider);

            //Eliminamos todos los proveedores
            //options.RequestCultureProviders.Clear();
            //options.RequestCultureProviders.Add(new MyRequestCultureProvider());

            return options;
        }

        private static void RemoveAcceptLanguageProvider(RequestLocalizationOptions options)
        {
            var acceptLanguageProvider = options.RequestCultureProviders
                .OfType<AcceptLanguageHeaderRequestCultureProvider>()
                .First();
            options.RequestCultureProviders.Remove(acceptLanguageProvider);
        }

        private static List<CultureInfo> GetSupportedCultures()
        {
            return new List<CultureInfo>
            {
                new CultureInfo("es"),
                new CultureInfo("es-ES"),
                new CultureInfo("en"),
                new CultureInfo("en-US"),
                new CultureInfo("en-GB")
            };
        }
    }

    public class MyRequestCultureProvider : CookieRequestCultureProvider
    {
        public MyRequestCultureProvider()
        {
            CookieName = "Culture";
        }

        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var result = await base.DetermineProviderCultureResult(httpContext);
            IList<StringSegment> cultures = result.Cultures;

            var companyHeader = httpContext.Request.Cookies.SingleOrDefault(p => p.Key == "Company");
            if (companyHeader.Key != null && companyHeader.Value == "666")
            {
                cultures.Clear();
                cultures.Add("en-US");
            }

            return await Task.FromResult(new ProviderCultureResult(result.Cultures, result.UICultures));
        }
    }
}
