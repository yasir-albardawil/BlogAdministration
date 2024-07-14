using Microsoft.AspNetCore.Mvc;
using PieShop.Models;
using PieShop.ViewModels;

namespace PieShop.Components
{
    public class OrderMenu : ViewComponent
    {
        private readonly IOrderRepository _orderRepository;

        public OrderMenu(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return View(orders);
        }
    }
}
