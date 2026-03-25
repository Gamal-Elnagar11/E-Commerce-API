using E_Commerce_API.Data;
using E_Commerce_API.Models;
using E_Commerce_API.Reposatory.Interface;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_API.Reposatory.Implementation
{
    public class GetUserIdInCart : IGetUserCart
    {

        private readonly Application _db;

        public GetUserIdInCart(Application db)
        {
            _db = db;
        }

        public async Task<Cart> GetUserIdInCartAsync(string userid)
        {
             return await _db.Carts.
                Include( a => a.CartItems)
                .ThenInclude(a => a.Products)
                .FirstOrDefaultAsync(a =>  a.UserId == userid);
        }
    }
}
