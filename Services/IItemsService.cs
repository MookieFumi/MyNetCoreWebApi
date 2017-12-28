using System.Collections.Generic;
using MyWebApi.Features.Items.ViewModels;

namespace MyWebApi.Services
{
    public interface IItemsService
    {
        PaginatedResult<ItemViewModel> GetItems(PaginatedConfiguration pagination);
    }
}