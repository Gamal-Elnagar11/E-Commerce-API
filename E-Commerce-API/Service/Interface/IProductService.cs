using E_Commerce_API.Models;

namespace E_Commerce_API.Service.Interface
{
    public interface IProductService
    {
       public IQueryable<Product> GetAllProducts();
       public Task<Product> GetProductByIdAsync(int id);
       public Task<Product> UpdateProductAsync(int id , Product product);
        public Task<Product> UpdateStock(int id ,int stock);
       public Task<Product> AddProductAsync(Product product);
       public Task<Product> DeleteProductAsync(int id);
        public Task<List<Product>> Search(string name);

        public Task<bool> CategoryExistsAsync(int categoryId);

       


    }
}
