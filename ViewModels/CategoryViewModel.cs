using PieShop.Models;

namespace PieShop.ViewModels
{
    public class CategoryViewModel
    {
        public IEnumerable<Category> AllCategory { get; }

        public CategoryViewModel(IEnumerable<Category> categories)
        {
            AllCategory = categories;
        }

    }
}
