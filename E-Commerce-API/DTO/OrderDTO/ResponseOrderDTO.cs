using E_Commerce_API.Static;

namespace E_Commerce_API.DTO.OrderDTO
{
    public class ResponseOrderDTO
    {
        
            public int Id { get; set; }
            public string UserName { get; set; }
            public decimal TotalPrice { get; set; }
            public OrderStatus Status { get; set; }
            public Payment PaymentMethod { get; set; }
            public DateTime Date { get; set; } = DateTime.UtcNow;
            public string PhoneNumber { get; set; }
            public string City { get; set; }
            public string Address { get; set; }
            public List<OrderItemDTO> Items { get; set; } = new();
        
    }
}
