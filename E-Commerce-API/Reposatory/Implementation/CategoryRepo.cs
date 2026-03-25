using E_Commerce_API.Data;
using E_Commerce_API.Models;
using E_Commerce_API.Reposatory.Implementation;
using E_Commerce_API.Reposatory.Interface;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_API.Reposatory.Implementation
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly Application _db;

        public CategoryRepo(Application db)
        {
            _db = db;
        }

        public async Task<Category> AddAsync(Category category)
        {
              await _db.Categories.AddAsync(category);
            return category;
        }

        public void DeleteCategory(Category product)
        {
            _db.Categories.Remove(product);
        }

        public async Task<Category> GeCategoriesByIdAsync(int id)
        {
            return await _db.Categories.Include(a => a.Products).FirstOrDefaultAsync( a=> a.Id == id);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _db.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<List<Category>> GetAllCategoriesWithProducts()
        {
            return await _db.Categories.Include(c => c.Products).ToListAsync();
        }

        public async Task<List<Category>> Search(string name)
        {
             return await _db.Categories.Where(a => a.Name.Contains(name)).ToListAsync();
        }

        public async Task<List<Category>> SearchwithProducts(string name)
        {
             return await _db.Categories.Include(a => a.Products).Where(a => a.Name.Contains(name)).ToListAsync();
        }

        public void UpdateCategory(Category product)
        {
               
            _db.Update(product);
        }
    }
} 