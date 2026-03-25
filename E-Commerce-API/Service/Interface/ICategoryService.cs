using E_Commerce_API.Models;
using E_Commerce_API.UnitOfWork;

namespace E_Commerce_API.Service.Interface
{
    public interface ICategoryService
    {
        public Task<List<Category>> GetAllCateory();
        public Task <List<Category>> GetAllCateoryWithProducts();
        public Task<Category> GetCategoryByIdAsync(int id);
        public Task<Category> AddCategoryAsync(Category category);
        public Task<Category> UpdateCategoryAsync(int id ,Category category);
        public Task<Category> DeleteCategoryAsync(int id);
        public Task<List<Category>>  Search (string name);
        public Task<List<Category>> SearchWithProducts(string name);  
    }
}
