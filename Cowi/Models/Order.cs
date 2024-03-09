namespace Cowi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<OrderItem> Items { get; set; }

        public decimal TotalPrice { get; set; }

        public string UserId { get; set; }
    }
    
}
