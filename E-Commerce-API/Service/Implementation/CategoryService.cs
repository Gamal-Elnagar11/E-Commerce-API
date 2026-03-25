using E_Commerce_API.Models;
using E_Commerce_API.Service.Interface;
using E_Commerce_API.UnitOfWork;

namespace E_Commerce_API.Service.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
 



        public async Task<Category> AddCategoryAsync(Category category)
        {
            try
            {


                await _unitOfWork.CategoryRepo.AddAsync(category);
                await _unitOfWork.CompleteAsync();
                return category;
            }
            catch (Exception ex)
            {
                throw new Exception("Add Category has Proble",ex);
            }
        }

        public async Task<Category> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepo.GeCategoriesByIdAsync(id);
            if (category == null)
                throw new ArgumentException("Category ID Not Found");

            if (category.Products.Any())
                throw new ArgumentException("This Category has Products");


            // _unitOfWork.CategoryRepo.DeleteCategory(findid);
            category.IsDeleted = true;
              await _unitOfWork.CompleteAsync();
            return category;

        }

        
         
        public  async Task<List<Category>> GetAllCateory()
        {
             return  await _unitOfWork.CategoryRepo.GetAllCategoriesAsync();
         }

        public async Task<List<Category>> GetAllCateoryWithProducts()
        {
            return await _unitOfWork.CategoryRepo.GetAllCategoriesWithProducts();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var result = await _unitOfWork.CategoryRepo.GeCategoriesByIdAsync(id);
            if (result == null)
                throw new ArgumentException("Category ID Not Found");
            if (result.IsDeleted == true)
                throw new ArgumentException("This Category was Deleted");


            return result;
        }

        public async Task<List<Category>> Search(string name)
        {
            return await _unitOfWork.CategoryRepo.Search(name);
        }

        public async Task<List<Category>> SearchWithProducts(string name)
        {
            return await _unitOfWork.CategoryRepo.SearchwithProducts(name);
        }

        public async Task<Category> UpdateCategoryAsync(int id, Category category)
        {
            var findid = await _unitOfWork.CategoryRepo.GeCategoriesByIdAsync(id);
            if (findid == null)
                throw new ArgumentException("Category ID Not Found");

            findid.Name = category.Name;
            

              _unitOfWork.CategoryRepo.UpdateCategory(findid);
             await _unitOfWork.CompleteAsync();
            return findid;

        }

       
    }
}
