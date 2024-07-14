
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO.Pipelines;
using System.Text;

namespace PieShop.Models
{
    public class MockOrderRepository : IOrderRepository
    {
        private readonly IShoppingCart _shoppingCart;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MockOrderRepository(IShoppingCart shoppingCart, HttpClient httpClient, IConfiguration configuration)
        {
            _shoppingCart = shoppingCart ?? throw new ArgumentNullException(nameof(shoppingCart));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/Orders");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(jsonData);
                return orders ?? new List<Order>();
            }

            return new List<Order>();
        }

        public async Task<Order> CreateAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            try
            {
                var json = JsonConvert.SerializeObject(order);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var baseUrl = _configuration["baseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new InvalidOperationException("Base URL configuration is missing or empty.");
                }

                var response = await _httpClient.PostAsync($"{baseUrl.TrimEnd('/')}/orders", data);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var createdOrder = JsonConvert.DeserializeObject<Order>(jsonData);
                    return createdOrder ?? throw new Exception("Deserialized item is null."); ;
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


        public async Task<Order> UpdateAsync(Order order)
        {
            var json = JsonConvert.SerializeObject(order);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var baseUrl = _configuration["baseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("Base URL configuration is missing or empty.");
            }

            var requestUrl = $"{baseUrl.TrimEnd('/')}/orders/{order.OrderId}";

            try
            {
                var response = await _httpClient.PutAsync(requestUrl, data);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var itemResult = JsonConvert.DeserializeObject<Order>(jsonData);
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
            var response = await _httpClient.DeleteAsync($"{_configuration["BaseUrl"]}/orders/{id}");
        }

        public async Task<Order?> FindAsync(int? id)
        {
            var response = await _httpClient.GetAsync($"{_configuration["BaseUrl"]}/orders/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Order>();
            }
            return null;
        }
    }
}
