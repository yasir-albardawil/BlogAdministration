namespace PieShop.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int Id { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
    }
}
