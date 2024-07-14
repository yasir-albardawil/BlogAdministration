using PieShop.Models;

namespace PieShop.ViewModels
{
    public class ItemListViewModel
    {

        public IEnumerable<Item> AllItems { get; }

        public string? CurrentCategory { get; }

        public ItemListViewModel(IEnumerable<Item> items, string? currentCategory)
        {
            AllItems = items;
            CurrentCategory = currentCategory;
        }
    }
}
