using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MyWebApi.Controllers.api
{
    [Route("api/[controller]")]
    public class EnvironmentController : Controller
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public EnvironmentController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        [HttpGet]
        public IActionResult GetAllRoutes()
        {
<<<<<<< HEAD

            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .Select(ad => new
                {
                    Action = ad.RouteValues["action"],
                    Controller = ad.RouteValues["controller"]
                }).Distinct().ToList();

            return Ok(routes);
=======
            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items.Distinct().ToList();

            return Ok(routes.Select(ad => new
            {
                Action = ad.RouteValues["action"],
                Controller = ad.RouteValues["controller"]
            }));
>>>>>>> Added environment controller
        }
    }
}