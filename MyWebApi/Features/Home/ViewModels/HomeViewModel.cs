using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Features.Home.ViewModels
{
    public class HomeViewModel
    {
        [Display(Name = nameof(Author))]
        public string Author { get; set; }
    }
}