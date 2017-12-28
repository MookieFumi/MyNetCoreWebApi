using Microsoft.AspNetCore.Mvc;
using MyWebApi.Features.Items.ViewModels;
using MyWebApi.Services;

namespace MyWebApi.Features.Items
{
    [Route("[controller]")]
    public class ItemsController : Controller
    {
        private readonly IItemsService _itemsService;
        

        public ItemsController(IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        // GET items/index
        [HttpGet]
        [HttpGet("index")]
        public IActionResult Index(PaginatedConfiguration pagination)
        {
            var result = Get(pagination);
            return View(result);
        }


        // GET api/values
        [HttpGet("~/api/[controller]")]
        public PaginatedResult<ItemViewModel> Get(PaginatedConfiguration pagination)
        {
            var items = _itemsService.GetItems(pagination);
            return items;
        }

    }
}
