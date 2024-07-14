using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using PieShop.Models;
using PieShop.ViewModels;

namespace PieShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IPieRepository pieRepository, ILogger<HomeController> logger)
        {
            _pieRepository  = pieRepository ?? throw new ArgumentNullException(nameof(pieRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<IActionResult> IndexAsync()
        {

            try
            {
                var ItemsOfTheWeek = await _pieRepository.GetItemsOfTheWeekAsync();
                if (ItemsOfTheWeek == null)
                {
                    return View("Error", new ErrorViewModel { Message = "Failed to retrieve items from the API." });
                }

                return View(new HomeViewModel(ItemsOfTheWeek));
            }
            catch (HttpRequestException ex)
            {
                // Log the exception or handle as appropriate
                var errorMessage = $"HTTP request failed. {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return View("Error", new ErrorViewModel { Message = errorMessage });
            }
            catch (JsonException ex)
            {
                // Log the exception or handle as appropriate
                var errorMessage = $"JSON serialization or deserialization error. {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return View("Error", new ErrorViewModel { Message = errorMessage });
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while fetching items: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return View("Error", new ErrorViewModel { Message = errorMessage });
            }
        }
    }
}
