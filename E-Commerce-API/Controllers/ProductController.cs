using System.Linq.Expressions;
using System.Security.AccessControl;
using AutoMapper;
using E_Commerce_API.DTO.ProductDTO;
using E_Commerce_API.Models;
using E_Commerce_API.Service.Interface;
using E_Commerce_API.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductController : ControllerBase
    {

        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        [HttpGet("Products")]
        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get ALl Products")]
        [EndpointDescription("Get All Products From System")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProduct()
        {

            try
            {


                var resutl1 = await _productService.GetAllProducts();
                var result2 = _mapper.Map<List<ResponseProduct>>(resutl1);
                return Ok(result2);
            }
            catch(Exception ex)
            {
                return BadRequest(  ex.Message );
            }
        }


        [HttpGet("Search")]
        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Search in Products")]
        [EndpointDescription("Get All Products From System That Contain Value In Search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string? name)
        {

            if (string.IsNullOrEmpty(name))
                return BadRequest("Search Value is required"); 

            var getproduct = await _productService.Search(name);
                var result = _mapper.Map<List<ProductBaseDTO>>(getproduct);
                return Ok(result);
            
        }


        [HttpGet("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get Product By Id")]
        [EndpointDescription("Get Any Product From System That Contain Value In Search")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var findid = await _productService.GetProductByIdAsync(id);
                if (findid == null)
                    return NotFound("Product ID Not Found");

                var resutl = _mapper.Map<ResponseProduct>(findid);

                return Ok(resutl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("AddProduct")]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Add New Product")]
        [EndpointDescription("Add a New Product To System Specific By Admin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromForm]AddProductDTO productDTO)
        {
            try
            {
                var map = _mapper.Map<Product>(productDTO);
                 var result =  await _productService.AddProductAsync(map);

                  var endresult = _mapper.Map<ResponseProduct>(result);
 
                return Ok(endresult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }



        [HttpPut("{id}")]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Update Product By Id")]
        [EndpointDescription("Update a Product From System Specific By Admin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromRoute]int id , [FromForm] UpdateProductDTO updateDTO)
        {
            try
            {
                var findid = await _productService.GetProductByIdAsync(id);
                if (findid == null)
                    return NotFound("Product ID Not Found");

                var findCategory = await _productService.CategoryExistsAsync(updateDTO.CategoryId);
                if (!findCategory)
                    return NotFound("Category ID Not Found");

                _mapper.Map(updateDTO , findid);

                var result = await _productService.UpdateProductAsync(id, findid);

                 var map = _mapper.Map<ResponseProduct>(result);

                return Ok(map);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpPut("/stock/{id}")]
        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Update Stock By Id")]
        [EndpointDescription("Update a Stock Prduct From System Specific By Admin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateQuantity(int id , int stock)
        {
            try
            {
                await _productService.UpdateStock(id, stock);
                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Delete Product By Id")]
        [EndpointDescription("Delete Prduct From System Specific By Admin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var findid = await _productService.GetProductByIdAsync(id);
            if (findid == null)
                return NotFound("ID For Product Not Found");

            var result = await _productService.DeleteProductAsync(id);
            var result2 = _mapper.Map<ResponseProduct>(result);
            return Ok(result2);


        }


    }
}
