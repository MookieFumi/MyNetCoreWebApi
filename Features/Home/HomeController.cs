using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApi.Features.Home.ViewModels;
using MyWebApi.Services;

namespace MyWebApi.Features.Home
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private ILogger<HomeController> _logger;
        private readonly IValuesService _valuesService;

        public HomeController(ILogger<HomeController> logger, IValuesService valuesService)
        {
            _logger = logger;
            _valuesService = valuesService;
        }

        // GET home/index
        [HttpGet]
        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }

        // GET home/about
        [HttpGet("about")]
        public IActionResult About()
        {
            var viewModel = new AboutViewModel
            {
                Author = "MookieFumi",
                Year = DateTime.UtcNow.Year,
                Values = _valuesService.GetValues().ToList()
            };

            return View(viewModel);
        }

    }
}
