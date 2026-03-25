using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_API.Models
{
    public class Cart
    {
        public int Id { get; set; }


        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public bool isDeleted { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    }
}
