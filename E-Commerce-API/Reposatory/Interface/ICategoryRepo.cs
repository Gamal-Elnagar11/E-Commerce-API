using E_Commerce_API.Models;

namespace E_Commerce_API.Reposatory.Interface
{
    public interface ICategoryRepo
    {
        public Task<List<Category>> GetAllCategoriesAsync();
        public Task<List<Category>> GetAllCategoriesWithProducts();
        public Task<Category> GeCategoriesByIdAsync(int id);
        public Task<Category> AddAsync(Category category);
        public void UpdateCategory(Category category);
        public void DeleteCategory(Category category);
        public Task<List<Category>> Search(string name);
        public Task<List<Category>> SearchwithProducts(string name);
    }
}
