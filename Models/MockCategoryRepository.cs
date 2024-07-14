using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PieShop.Models
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public MockCategoryRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
                var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/categories");

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<IEnumerable<Category>>(jsonData);
                    return items ?? new List<Category>();
                }

                return Enumerable.Empty<Category>();
        }

        public async Task<Category?> GetCategoryById(int? id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Category>();
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw new HttpRequestException($"Failed to retrieve category: {response.StatusCode}, {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetCategoryById: {ex.Message}");
                throw;
            }
        }

        public async Task AddAsync(Category category)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_configuration["BaseUrl"]}/categories", category);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AddAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var json = JsonConvert.SerializeObject(category);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var baseUrl = _configuration["baseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("Base URL configuration is missing or empty.");
            }

            var requestUrl = $"{baseUrl.TrimEnd('/')}/categories/{category.CategoryId}";

            try
            {
                var response = await _httpClient.PutAsync(requestUrl, data);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var categoryResult = JsonConvert.DeserializeObject<Category>(jsonData);
                    return categoryResult ?? throw new Exception("Deserialized category is null.");
                }
                else
                {
                    throw new HttpRequestException($"Failed to update category: {response.StatusCode}, {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Log exception or handle as appropriate
                throw new HttpRequestException("HTTP request failed.", ex);
            }
            catch (JsonException ex)
            {
                // Log exception or handle as appropriate
                throw new JsonException("JSON serialization or deserialization error.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_configuration["BaseUrl"]}/categories/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                // Handle the specific case where the status code is 409 Conflict
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = System.Text.Json.JsonDocument.Parse(responseContent);
                if (jsonResponse.RootElement.TryGetProperty("message", out var messageElement))
                {
                    var message = messageElement.GetString();
                    if (message == "The category cannot be deleted because it has associated items.")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/categories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<bool>();
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CategoryExistsAsync: {ex.Message}");
                throw;
            }
        }


        public async Task<Category?> FindAsync(int? id)
        {
            var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Category>();
            }
            return new Category();
        }

        public async Task<bool> PieExistsAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/Items/Exists/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }
    }
}
