using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PieShop.Models;
using PieShop.ViewModels;

namespace PieShop.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoryController> _logger;
        private readonly IConfiguration _configuration;

        public CategoryController(ICategoryRepository categoryRepository, HttpClient httpClient, ILogger<CategoryController> logger, IConfiguration configuration)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IActionResult> List(string search, string sortOrder)
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                if (categories == null)
                {
                    return View("Error", new ErrorViewModel { Message = "Failed to retrieve categories from the API." });
                }

                categories = FilterAndSortCategories(categories, search, sortOrder);
                return View(new CategoryViewModel(categories));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP request failed: {ex.Message}");
                return View("Error", new ErrorViewModel { Message = $"HTTP request failed. {ex.Message}" });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"JSON error: {ex.Message}");
                return View("Error", new ErrorViewModel { Message = $"JSON error. {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred: {ex.Message}");
                return View("Error", new ErrorViewModel { Message = $"An error occurred. {ex.Message}" });
            }
        }

        private IEnumerable<Category> FilterAndSortCategories(IEnumerable<Category> categories, string search, string sortOrder)
        {
            if (!string.IsNullOrEmpty(search))
            {
                categories = categories.Where(i => i.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            return sortOrder switch
            {
                "name_desc" => categories.OrderByDescending(i => i.CategoryName),
                _ => categories
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
                var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/Categories/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = $"API call failed with status code: {response.StatusCode}, reason: {response.ReasonPhrase}";
                    _logger.LogError(errorMessage);
                    return View("Error", new ErrorViewModel { Message = errorMessage });
                }

                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Category>(data);
                return View(result ?? new Category());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred: {ex.Message}");
                return View("Error", new ErrorViewModel { Message = $"An error occurred: {ex.Message}" });
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                TempData["SuccessMessage"] = "Category successfully created.";
                return RedirectToAction(nameof(List));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryRepository.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,Description")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryRepository.UpdateAsync(category);
                    TempData["SuccessMessage"] = "Category successfully updated.";
                    return RedirectToAction(nameof(List));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _categoryRepository.CategoryExistsAsync(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(category);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryRepository.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryRepository.FindAsync(id);
            if (category != null)
            {


                var hasItems = await _categoryRepository.DeleteAsync(category.CategoryId);

                if (hasItems)
                {
                    ModelState.AddModelError(string.Empty, "The category cannot be deleted because it has associated items.");
                    return View(category);
                }
                TempData["SuccessMessage"] = "Category successfully deleted.";
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Category not found.");
                return RedirectToAction(nameof(List));
            }

            return RedirectToAction(nameof(List));
        }

    }
}
