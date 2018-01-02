namespace MyWebApi.Features.Items.ViewModels
{
    public class ItemViewModel
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string EanCode { get; set; }
        public decimal SalePrice{ get; set; }
        public string Release { get; set; }
        public string Notes { get; set; }
        public string Image { get; set; }
    }
}
