using E_Commerce_API.Data;
using E_Commerce_API.Models;
using E_Commerce_API.Reposatory.Interface;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_API.Reposatory.Implementation
{
    public class ProductRepo : IProductRepo
    {
        private readonly Application _db;

        public ProductRepo(Application db)
        {
            _db = db;
        }

         
        public Task<List<Product>> GetAllProductsAsync()
        {
            return _db.Products.Include(a => a.Category).ToListAsync();
        }


        public async Task<Product> GetProductsByIdAsync(int id)
        {
            return await _db.Products.FindAsync(id);
        }



        public async Task<Product> AddAsync(Product product)
        {
                 await _db.Products.AddAsync(product);
                return product;
        }



        public void UpdateProduct(Product product)
        {
             _db.Update(product);
        }


        public void DeleteProduct(Product product)
        {
                _db.Products.Remove(product);
              
        }


        
    }
}
