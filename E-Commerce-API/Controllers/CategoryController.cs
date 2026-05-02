using AutoMapper;
using E_Commerce_API.DTO.CategoryDTO;
using E_Commerce_API.Models;
using E_Commerce_API.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryService categoryService , IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }



        [HttpGet("{id}", Name ="GetById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
              var getid = await _categoryService.GetCategoryByIdAsync(id);
                var map = _mapper.Map<CategorywithProductDTO>(getid);
                return Ok(map);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpGet("Get All Category With Products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllWithProducts()
        {
            try
            {
               var resutl = await _categoryService.GetAllCateoryWithProducts();
               var map = _mapper.Map<List<CategorywithProductDTO>>(resutl);
                return Ok(map);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get All Category")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategory()
        {
            var allcategory = await _categoryService.GetAllCateory();
            var result = _mapper.Map<List<CategoriesDTO>>(allcategory);

            return Ok(result);
        }


        [HttpGet("Search Category")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string? name)
        {
            if (name == null)
                 return BadRequest("Search value empty");
            var categories = await _categoryService.Search(name);
            var result = _mapper.Map<List<CategoryName>>(categories);
            return Ok(result);
        }


        [HttpGet("Search With Products")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchWithProducts(string? name)
        {
            if (name == null)
                return BadRequest("Search value empty");
            var categories = await _categoryService.SearchWithProducts(name);
            var map = _mapper.Map<List<CategorywithProductDTO>>(categories);
            return Ok(map);
        }


        [HttpPost("AddCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddCategory(CategoryName categoriesDTO)
        {
            try
            {
                var map = _mapper.Map<Category>(categoriesDTO);
                var category = await _categoryService.AddCategoryAsync(map);
                var result = _mapper.Map<CategoryName>(category);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
 
 
        }


        [HttpPut("{id}", Name = "UpdateCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdatCategory(int id , CategoryName categoryName)
        {
            try
            {
                var map = _mapper.Map<Category>(categoryName);
                var result = await _categoryService.UpdateCategoryAsync(id, map);
                var endresult = _mapper.Map<CategoryName>(result);
                return Ok(endresult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}", Name = "DeleteCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
