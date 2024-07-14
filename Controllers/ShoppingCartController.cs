using Microsoft.AspNetCore.Mvc;
using PieShop.Models;
using PieShop.ViewModels;

namespace PieShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly IShoppingCart _shoppingCart;

        public ShoppingCartController(IPieRepository pieRepository, IShoppingCart shoppingCart)
        {
            _pieRepository = pieRepository;
            _shoppingCart = shoppingCart;
        }
        public ViewResult Index()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var shoppingCartViewModel = new ShoppingCartViewModel(_shoppingCart, _shoppingCart.GetShoppingCartTotal());

            return View(shoppingCartViewModel);
        }

        public async Task<IActionResult> AddToShoppingCart(int id)
        {
            var allItems = await _pieRepository.GetAllItemsAsync();
            var selectedPie = allItems.FirstOrDefault(p => p.Id == id);

            if (selectedPie != null)
            {
                _shoppingCart.AddToCart(selectedPie);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveFromShoppingCart(int id)
        {
            var allItems = await _pieRepository.GetAllItemsAsync();
            var selectedPie = allItems.FirstOrDefault(p => p.Id == id);

            if (selectedPie != null)
            {
                _shoppingCart.RemoveFromCart(selectedPie);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ClearShoppingCart()
        {
             if (_shoppingCart.GetShoppingCartItems().Count() > 0)
                _shoppingCart.ClearCart();

            return RedirectToAction("index");
        }
    }
}
