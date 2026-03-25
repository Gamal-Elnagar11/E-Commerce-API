using E_Commerce_API.Models;

namespace E_Commerce_API.Reposatory.Interface
{
    public interface IProductRepo
    {
        public Task<List<Product>> GetAllProductsAsync();
       public Task<Product> GetProductsByIdAsync(int id);
       public Task<Product> AddAsync(Product product);
       public void  UpdateProduct(Product product);
       public void DeleteProduct(Product product);

    }
}
