using E_Commerce_API.Static;

namespace E_Commerce_API.DTO.OrderDTO
{
    public class CheckoutDTO
    {
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public Payment PaymentMethod { get; set; }
    }
}
