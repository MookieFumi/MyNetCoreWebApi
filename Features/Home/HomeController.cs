using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MyWebApi.Features.Home.ViewModels;
using MyWebApi.Services;

namespace MyWebApi.Features.Home
{
    public class HomeController : Controller
    {
        private ILogger<HomeController> _logger;
        private readonly IValuesService _valuesService;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public HomeController(ILogger<HomeController> logger, IValuesService valuesService,
            IStringLocalizer<HomeController> localizer,
        IStringLocalizer<SharedResource> sharedLocalizer
        )
        {
            _logger = logger;
            _valuesService = valuesService;
            _localizer = localizer;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET home/index
        [HttpGet]
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
                Values = _valuesService.GetValues().ToList(),
                Home = _localizer["Home"],
                Title = _sharedLocalizer[SharedResourceKeys.Title]
            };

            return View(viewModel);
        }

    }
}
