using Microsoft.AspNetCore.Mvc;
using PieShop.Models;
using PieShop.ViewModels;
using System.IO.Pipelines;

namespace PieShop.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
       
        public IActionResult List()
        {
            CustomerViewModel customerViewModel = new CustomerViewModel(_customerRepository.Customers);

            return View(customerViewModel);
        }

        public IActionResult Detail(int id)
        {
            var customer = _customerRepository.Get(id);


            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
    }
}
