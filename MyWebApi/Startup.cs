using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                .AddFeatureFolders();

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

        private static RequestLocalizationOptions GetRequestLocalizationOptions()
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

            var requestProvider = new RouteDataRequestCultureProvider();
            options.RequestCultureProviders.Insert(0, requestProvider);

            return options;
        }

        private static List<CultureInfo> GetSupportedCultures()
        {
            return new List<CultureInfo>
            {
                new CultureInfo("es"),
                new CultureInfo("es-ES"),
                new CultureInfo("en"),
                new CultureInfo("en-US")
            };
        }
    }
}
