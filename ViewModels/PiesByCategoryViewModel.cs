using PieShop.Models;

namespace PieShop.ViewModels
{
    public class PiesByCategoryViewModel
    {
        public IEnumerable<Item> ALlPiesByCategory { get; }

        public string? CurrentCategory { get; }

        public PiesByCategoryViewModel(IEnumerable<Item> items, string? currentCategory)
        {
            ALlPiesByCategory = items;
            CurrentCategory = currentCategory;
        }
    }
}
