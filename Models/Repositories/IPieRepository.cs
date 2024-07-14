using PieShop.Models;

namespace PieShop.Models.Repositories
{
    public interface IPieRepository
    {
        IEnumerable<Item> AllPies { get; }
    }

}
