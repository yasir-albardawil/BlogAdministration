using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PieShop.Models;
using PieShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PieShop.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ItemController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ItemController> _logger;
        private readonly IConfiguration _configuration;

        public ItemController(IPieRepository pieRepository, ICategoryRepository categoryRepository, IHttpClientFactory httpClientFactory, ILogger<ItemController> logger, IConfiguration configuration)
        {
            _pieRepository = pieRepository ?? throw new ArgumentNullException(nameof(pieRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IActionResult> List(string category, string search, string sortOrder)
        {
            try
            {
                var items = await _pieRepository.GetAllItemsAsync();
                if (items == null)
                {
                    return View("Error", new ErrorViewModel { Message = "Failed to retrieve items from the API." });
                }

                var filteredResults = FilterAndSortItems(items, search, sortOrder);
                return View(new ItemListViewModel(filteredResults, category));
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

        private IEnumerable<Item> FilterAndSortItems(IEnumerable<Item> items, string search, string sortOrder)
        {
            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(i => i.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            return sortOrder switch
            {
                "name_desc" => items.OrderByDescending(i => i.Name),
                "price_asc" => items.OrderBy(i => i.Price),
                "price_desc" => items.OrderByDescending(i => i.Price),
                _ => items.OrderBy(i => i.Name),
            };
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var item = await _pieRepository.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }

                return View(item);
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
                var errorMessage = $"An error occurred while fetching item details: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return View("Error", new ErrorViewModel { Message = errorMessage });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                var pieCreateViewModel = new PieCreateViewModel { Categories = categories };

                return View(pieCreateViewModel);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while fetching categories: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return View("Error", new ErrorViewModel { Message = errorMessage });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PieCreateViewModel pieCreateViewModel)
        {
            if (pieCreateViewModel.Item.Category.CategoryId == -1)
            {
                ModelState.AddModelError(string.Empty, "The Category field is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    pieCreateViewModel.Item.CategoryId = pieCreateViewModel.Item.Category.CategoryId;
                    var item = await _pieRepository.CreateAsync(pieCreateViewModel.Item);
                    if (item != null)
                    {
                        TempData["SuccessMessage"] = "Item successfully created.";
                        return RedirectToAction(nameof(List));
                    }
                    else
                    {
                        var errorMessage = $"API returned unsuccessful status code: 200";
                        _logger.LogWarning(errorMessage);
                        ModelState.AddModelError(string.Empty, "Failed to create item. Please try again later.");
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = $"An error occurred while creating the item: {ex.Message}";
                    _logger.LogError(ex, errorMessage);
                    ModelState.AddModelError(string.Empty, "Failed to create item. Please try again later.");
                }
            }

            pieCreateViewModel.Categories = await _categoryRepository.GetAllCategoriesAsync();
            return View(pieCreateViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _pieRepository.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var pieEditViewModel = new PieEditViewModel { Item = item, Categories = categories };

            return View(pieEditViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PieEditViewModel pieEditViewModel)
        {
            if (id != pieEditViewModel.Item.Id)
            {
                return NotFound();
            }

            if (pieEditViewModel.Item.Category.CategoryId == -1)
            {
                ModelState.AddModelError("Item.Category.CategoryId", "The Category field is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    pieEditViewModel.Item.CategoryId = pieEditViewModel.Item.Category.CategoryId;
                    await _pieRepository.UpdateAsync(pieEditViewModel.Item);
                    TempData["SuccessMessage"] = "Item successfully updated.";
                    return RedirectToAction(nameof(List));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _pieRepository.PieExistsAsync(pieEditViewModel.Item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            pieEditViewModel.Categories = await _categoryRepository.GetAllCategoriesAsync();
            return View(pieEditViewModel);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _pieRepository.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }

                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching item details for deletion.");
                return View("Error", new ErrorViewModel { Message = "Failed to fetch item details for deletion. Please try again later." });
            }
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var item = await _pieRepository.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }

                await _pieRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Item successfully deleted.";
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the item.");
                return View("Error", new ErrorViewModel { Message = "Failed to delete item. Please try again later." });
            }
        }

        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                var viewName = statusCode.ToString();
                return View(viewName);
            }

            return View();
        }
    }
}
