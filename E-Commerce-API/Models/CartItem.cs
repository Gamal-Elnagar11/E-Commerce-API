using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_API.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Products { get; set; }


        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }



    }
}
