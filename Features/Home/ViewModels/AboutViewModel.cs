using System.Collections.Generic;

namespace mywebapi.Features.Home.ViewModels
{
    public class AboutViewModel
    {
        public string Author { get; set; }
        public int Year { get; set; }
        public ICollection<int> Values { get; set; }
    }
}
