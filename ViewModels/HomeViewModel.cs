using PieShop.Models;

namespace PieShop.ViewModels
{
    public class HomeViewModel
    {

        public IEnumerable<Item> PiesOfTheWeek { get; }

        public string? CurrentCategory { get; }

        public HomeViewModel(IEnumerable<Item> piesOfTheWeek)
        {
            PiesOfTheWeek = piesOfTheWeek;
        }
    }
}
