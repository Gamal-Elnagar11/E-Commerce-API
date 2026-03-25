using E_Commerce_API.Models;

namespace E_Commerce_API.Reposatory.Interface
{
    public interface ICartRepo
    {
        public Task<Cart> GetCartByUserId(string userId);
        public Task<List<Cart>> GetAllCarts();
        public Task<Cart> AddCart(Cart cart);
        public void DeleteCart(Cart cart);

         

    }
}
