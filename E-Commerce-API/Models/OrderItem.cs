using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Products { get; set; }

         
         public string ProductName { get; set; }
         public decimal TotalPrice { get; set; }
    }
}

