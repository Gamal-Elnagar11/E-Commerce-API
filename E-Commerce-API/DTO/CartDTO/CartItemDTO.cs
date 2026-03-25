namespace E_Commerce_API.DTO.CartDTO
{
    public class CartItemDTO
    {
        
            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ImageUrl { get; set; }

            public int Quantity { get; set; }

            public decimal UnitPrice { get; set; }

            public decimal TotalPrice { get; set; }
        
    }
}
