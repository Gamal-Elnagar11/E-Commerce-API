using E_Commerce_API.DTO.CartDTO;
using E_Commerce_API.Models;

namespace E_Commerce_API.Service.Interface
{
    public interface ICartService
    {
        public Task<Cart> GetCartByUserId();
        public Task<List<Cart>> GetAllCarts();

        public Task<Cart> GetOrCreateCart();
         public Task<Cart> ClearCart(Cart cart);

        public  Task<Product> GetProductByIdAsync(int id);

        public Task<CartItem> AddItemCart( int productid, int quantity);
        public Task<CartItem> UpdateItemCartQuantity(int productid, int newquantity);
        public Task<CartItem> DeleteItemFromCart(int productid);
    }
}
