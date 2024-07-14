using Microsoft.AspNetCore.Mvc;
using PieShop.Models;
using PieShop.ViewModels;

namespace PieShop.Components
{
    public class CategorySelect : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategorySelect(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            categories = categories.OrderBy(c => c.CategoryName);

            return View(categories);
        }
    }
}
