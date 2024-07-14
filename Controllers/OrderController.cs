using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PieShop.Models;
using PieShop.Models.Repositories;

namespace PieShop.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShoppingCart _shoppingCart;

        public OrderController(IOrderRepository orderRepository, IShoppingCart shoppingCart)
        {
            _orderRepository = orderRepository;
            _shoppingCart = shoppingCart;
        }

        public  async Task<IActionResult> ListAsync(string search, string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";

            IEnumerable<Order> orders;
            orders = await _orderRepository.GetAllOrdersAsync();

            if (!string.IsNullOrEmpty(search))
            {

                orders = orders.Where(o =>
                        o.FirstName.Contains(search) ||
                        o.Email.Contains(search) ||
                        o.Country.Contains(search) ||
                        o.City.Contains(search)
        );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    orders = orders.OrderByDescending(p => p.FirstName);
                    break;
                case "id_desc":
                    orders = orders.OrderByDescending(p => p.OrderId);
                    break;
                default:
                    orders = orders.OrderBy(p => p.FirstName);
                    break;
            }

            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pie = await _orderRepository.FindAsync(id);
            if (pie == null)
            {
                return NotFound();
            }

            return View(pie);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {

            var order = await _orderRepository.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete"), Authorize(Roles = "SuperAdmin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAsync(int id)
        {
            var order = await _orderRepository.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }
            await _orderRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Order));
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            if (_shoppingCart.ShoppingCartItems.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty, add some items first");
            }

            if (ModelState.IsValid)
            {
                _orderRepository.CreateAsync(order);
                _shoppingCart.ClearCart();
                return RedirectToAction("CheckoutComplete");
            }

            return View(order);
        }

        public IActionResult CheckoutComplete()
        {
            ViewBag.CheckoutCompleteMessage = "Thanks for your order. You'll soon enjoy our delicious items!";
            return View();
        }
    }
}
