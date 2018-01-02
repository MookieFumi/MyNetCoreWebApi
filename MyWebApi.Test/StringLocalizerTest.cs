using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Xunit;

namespace MyWebApi.Test
{
    public class StringLocalizerTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public StringLocalizerTest()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>();
            _server = new TestServer(webHostBuilder);
            _client = _server.CreateClient();
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

        [Fact]
        public async Task A()
        {
            var response = await _client.GetAsync("/api/environment");
            var a = await response.Content.ReadAsStringAsync();
        }
    }
}
