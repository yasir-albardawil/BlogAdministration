namespace PieShop.Models
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> Customers { get; }

        Customer? Get(int id);
    }
}
