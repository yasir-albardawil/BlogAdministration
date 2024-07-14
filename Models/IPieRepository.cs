using System.Collections.Generic;
using System.Threading.Tasks;

namespace PieShop.Models
{
    public interface IPieRepository
    {
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task<IEnumerable<Item>> GetItemsOfTheWeekAsync();
        Task<Item?> GetPieByIdAsync(int? id);
        Task<Item> CreateAsync(Item item);
        Task<Item> UpdateAsync(Item item);
        Task DeleteAsync(int id);
        Task<Item?> FindAsync(int? id);
        Task<bool> PieExistsAsync(int id);
    }
}
