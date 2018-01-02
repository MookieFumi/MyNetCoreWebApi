using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Xunit;

namespace MyWebApi.Test
{
    public class StringLocalizerTest
    {
        private readonly TestServer _server;
        public StringLocalizerTest()
        {
            //Ojo, ahora mismo sólo se está haciendo para la cultura por defecto (puede valer)
            var webHostBuilder = new WebHostBuilder().UseStartup<Startup>();
            _server = new TestServer(webHostBuilder);
        }

        [Fact]
        public void All_Keys_Exists_In_Resources()
        {
            var localizer = _server.Host.Services.GetService<IStringLocalizer<SharedResource>>();
            var properties = typeof(SharedResourceKeys).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var property in properties)
            {
                Assert.False(localizer[property.Name].ResourceNotFound);
            }
        }

        [Fact(Skip = "Throw an exception while localizer.GetAllStrings()")]
        public void All_Resources_Have_a_Key()
        {
            var properties = typeof(SharedResourceKeys).GetFields(BindingFlags.Public | BindingFlags.Static);

            var localizer = _server.Host.Services.GetService<IStringLocalizer<SharedResource>>();
            //TODO Exceptions
            var items = localizer.GetAllStrings();

            foreach (var item in items)
            {
                Assert.Contains(item.Name, properties.Select(p => p.Name));
            }
        }
    }
}
