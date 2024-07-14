using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PieShop.Models;
using PieShop.ViewModels;

namespace PieShop.Components
{
    public class CategoryMenu : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryMenu(ICategoryRepository categoryRepository, ILogger<CategoryMenu> logger)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            categories = categories.OrderBy(c => c.CategoryName);
            return View(categories);
        }
    }
}
