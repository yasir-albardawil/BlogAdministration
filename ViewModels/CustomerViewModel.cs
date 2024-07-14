using PieShop.Models;

namespace PieShop.ViewModels
{
    public class CustomerViewModel
    {
        public IEnumerable<Customer> AllCustomers { get; }

        public CustomerViewModel(IEnumerable<Customer> customers)
        {
            AllCustomers = customers;
        }
    }
}
