namespace PieShop.Models
{
    public class MockCustomerRepository : ICustomerRepository
    {

        private readonly PieShopDBContext _pieShopDbContext;

        public MockCustomerRepository(PieShopDBContext pieShopDBContext)
        {
            _pieShopDbContext = pieShopDBContext;
        }

        public IEnumerable<Customer> Customers
        {
            get
            {
                return _pieShopDbContext.Customers;
            }
        }

        public Customer? Get(int id)
        {
            return _pieShopDbContext.Customers.FirstOrDefault(c => c.Id == id);
        }
    }
}
