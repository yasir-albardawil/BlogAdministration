using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace PieShop.Models
{
    public class MockPieRepository : IPieRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MockPieRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/Items");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<IEnumerable<Item>>(jsonData);
                return items ?? new List<Item>();
            }

            return new List<Item>();
        }

        public async Task<IEnumerable<Item>> GetItemsOfTheWeekAsync()
        {
            var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/Items?includeItemsOfTheWeak=true");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<IEnumerable<Item>>(jsonData);
                return items ?? new List<Item>();
            }

            return new List<Item>();
        }

        public async Task<Item?> GetPieByIdAsync(int? id)
        {
            var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/Items/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Item>();
            }
            return null;
        }

        public async Task<Item> CreateAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            try
            {
                var json = JsonConvert.SerializeObject(item);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var baseUrl = _configuration["baseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new InvalidOperationException("Base URL configuration is missing or empty.");
                }

                var response = await _httpClient.PostAsync($"{baseUrl.TrimEnd('/')}/items", data);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var createdItem = JsonConvert.DeserializeObject<Item>(jsonData);
                    return createdItem ?? throw new Exception("Deserialized item is null."); ;
                }
                else
                {
                    // Handle non-success status codes here
                    throw new HttpRequestException($"Failed to create item: {response.StatusCode}, {response.ReasonPhrase}");
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


        public async Task<Item> UpdateAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var json = JsonConvert.SerializeObject(item);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var baseUrl = _configuration["baseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("Base URL configuration is missing or empty.");
            }

            var requestUrl = $"{baseUrl.TrimEnd('/')}/items/{item.Id}";

            try
            {
                var response = await _httpClient.PutAsync(requestUrl, data);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var itemResult = JsonConvert.DeserializeObject<Item>(jsonData);
                    return itemResult ?? throw new Exception("Deserialized item is null.");
                }
                else
                {
                    throw new HttpRequestException($"Failed to update item: {response.StatusCode}, {response.ReasonPhrase}");
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

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_configuration["BaseUrl"]}/Items/{id}");
        }

        public async Task<Item?> FindAsync(int? id)
        {
            var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/Items/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Item>();
            }
            return null;
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
