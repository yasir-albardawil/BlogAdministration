using System.Collections.Generic;
using System.Threading.Tasks;

namespace PieShop.Models
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryById(int? id);
        Task AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
        Task<Category?> FindAsync(int? id);
    }
}
