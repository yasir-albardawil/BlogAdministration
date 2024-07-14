using PieShop.Models;
using PieShop.Models.Repositories;

namespace PieShop.Models.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        public IEnumerable<Category> AllCategories =>
            new List<Category>
            {
            new Category{CategoryId=1, CategoryName="Fruit items", Description="All-fruity items"},
            new Category{CategoryId=2, CategoryName="Cheese cakes", Description="Cheesy all the way"},
            new Category{CategoryId=3, CategoryName="Seasonal items", Description="Get in the mood for a seasonal pie"}
            };
    }
}
