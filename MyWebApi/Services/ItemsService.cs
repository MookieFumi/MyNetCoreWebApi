using System.Collections.Generic;
using System.Linq;
using MyWebApi.Features.Items.ViewModels;

namespace MyWebApi.Services
{
    public class ItemsService : IItemsService
    {
        public PaginatedResult<ItemViewModel> GetItems(PaginatedConfiguration pagination)
        {
            return new PaginatedResult<ItemViewModel>(pagination.PageIndex, pagination.PageSize, GetItemsFromMemory().Take(pagination.PageSize), 666);
        }

        private IEnumerable<ItemViewModel> GetItemsFromMemory()
        {
            return new List<ItemViewModel>
            {
                new ItemViewModel
                {
                    Name = "Big Box Crash Bandicoot",
                    Manufacturer = "Exquisite gaming",
                    EanCode = "5060525890222",
                    SalePrice = 29.99m,
                    Release = "TBC",
                    Notes = "Bandolera, Vaso, Cartera.",
                    Image = "https://media.game.es/COVERV2/3D_L/143/143768.png"
                },
                new ItemViewModel
                {
                    Name = "Big Box Call of Duty WWII",
                    Manufacturer = "Exquisite gaming",
                    EanCode = "5060525890222",
                    SalePrice = 29.99m,
                    Release = "TBC",
                    Notes = "Mini Figura Soldado WWII Cable Guy 5’’, Gorro, 2 Posavasos.",
                    Image = "https://media.game.es/COVERV2/3D_L/143/143767.png"
                },
                new ItemViewModel
                {
                    Name = "Big Box Crash Bandicoot",
                    Manufacturer = "Exquisite gaming",
                    EanCode = "5060525890222",
                    SalePrice = 29.99m,
                    Release = "TBC",
                    Notes = "Bandolera, Vaso, Cartera.",
                    Image = "https://media.game.es/COVERV2/3D_L/143/143768.png"
                },
                new ItemViewModel
                {
                    Name = "Big Box Call of Duty WWII",
                    Manufacturer = "Exquisite gaming",
                    EanCode = "5060525890222",
                    SalePrice = 29.99m,
                    Release = "TBC",
                    Notes = "Mini Figura Soldado WWII Cable Guy 5’’, Gorro, 2 Posavasos.",
                    Image = "https://media.game.es/COVERV2/3D_L/143/143767.png"
                },
                new ItemViewModel
                {
                    Name = "Big Box Crash Bandicoot",
                    Manufacturer = "Exquisite gaming",
                    EanCode = "5060525890222",
                    SalePrice = 29.99m,
                    Release = "TBC",
                    Notes = "Bandolera, Vaso, Cartera.",
                    Image = "https://media.game.es/COVERV2/3D_L/143/143768.png"
                },
                new ItemViewModel
                {
                    Name = "Big Box Call of Duty WWII",
                    Manufacturer = "Exquisite gaming",
                    EanCode = "5060525890222",
                    SalePrice = 29.99m,
                    Release = "TBC",
                    Notes = "Mini Figura Soldado WWII Cable Guy 5’’, Gorro, 2 Posavasos.",
                    Image = "https://media.game.es/COVERV2/3D_L/143/143767.png"
                },
                new ItemViewModel
                {
                    Name = "Big Box Crash Bandicoot",
                    Manufacturer = "Exquisite gaming",
                    EanCode = "5060525890222",
                    SalePrice = 29.99m,
                    Release = "TBC",
                    Notes = "Bandolera, Vaso, Cartera.",
                    Image = "https://media.game.es/COVERV2/3D_L/143/143768.png"
                },
                new ItemViewModel
                {
                    Name = "Big Box Call of Duty WWII",
                    Manufacturer = "Exquisite gaming",
                    EanCode = "5060525890222",
                    SalePrice = 29.99m,
                    Release = "TBC",
                    Notes = "Mini Figura Soldado WWII Cable Guy 5’’, Gorro, 2 Posavasos.",
                    Image = "https://media.game.es/COVERV2/3D_L/143/143767.png"
                }
            };
        }

    }
}