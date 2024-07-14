using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PieShop.Models;
using PieShop.ViewModels;

namespace PieShop.Components
{
    public class PieMenu : ViewComponent
    {
        private readonly IPieRepository _pieRepository;

        public PieMenu(IPieRepository pieRepository, ILogger<PieMenu> logger)
        {
            _pieRepository = pieRepository ?? throw new ArgumentNullException(nameof(pieRepository));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
                var allItems = await _pieRepository.GetAllItemsAsync();
                var items = allItems.OrderBy(p => p.Name);
                return View(items);
        }
    }
}
