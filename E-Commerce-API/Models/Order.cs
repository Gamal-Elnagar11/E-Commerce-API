using System.ComponentModel.DataAnnotations.Schema;
using E_Commerce_API.Static;

namespace E_Commerce_API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public Payment PaymentMethod { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Address { get; set; }


        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
