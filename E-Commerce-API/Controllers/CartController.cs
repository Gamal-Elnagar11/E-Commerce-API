using AutoMapper;
using E_Commerce_API.DTO.CartDTO;
using E_Commerce_API.Models;
using E_Commerce_API.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme ,Policy = "UserOrAdmin")]

    public class CartController : ControllerBase
    {
          private readonly ICartService _cartService;
        private readonly IMapper _mapper;

            public CartController(ICartService cartService, IMapper mapper)
            {
                _cartService = cartService;
                _mapper = mapper;
            }

        
        [HttpGet("MyCart")]
            public async Task<IActionResult> GetMyCart()
            {
                try
                {
                    var cart = await _cartService.GetOrCreateCart(); 
                    var map = _mapper.Map<ResponseCartDTO>(cart);
                    return Ok(map);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
      
        
        //   [HttpGet("AllCart")]
        //  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        //public async Task<IActionResult> GetAllCarts()
        //    {
        //        try
        //        {
        //            var carts = await _cartService.GetAllCarts();
        //            var map = _mapper.Map<List<ResponseCartDTO>>(carts);
        //             return Ok(map);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }
       
        
           [HttpPost("add-item-to-cart")]
            public async Task<IActionResult> AddItemToCart(int productId, int quantity)
            {
                try
                {
                    var item = await _cartService.AddItemCart(productId, quantity);
                     var map = _mapper.Map<CartItemDTO>(item);
                     return Ok(map);  
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
         
        
          [HttpPut("update-item-in-cart")]
            public async Task<IActionResult> UpdateItemQuantity(int productId, int newQuantity)
            {
                try
                {
                    
                    var cart = await _cartService.GetOrCreateCart();
                     var item = await _cartService.UpdateItemCarrQuantity(cart, productId, newQuantity);
                   var map = _mapper.Map<CartItemDTO>(item);
                    return Ok(map);  
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }


              [HttpDelete("remove-item-from-cart")]
            public async Task<IActionResult> RemoveItemFromCart(int productId)
            {
                try
                {
                    var cart = await _cartService.GetOrCreateCart();
                     var item = await _cartService.DeleteItemFromCart(cart, productId);
                  var map = _mapper.Map<CartItemDTO>(item);
                    return Ok(map);  
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }


             [HttpDelete("clear")]
            public async Task<IActionResult> ClearCart()
            {
                try
                {
                    var cart = await _cartService.GetOrCreateCart();
                     var clearedCart = await _cartService.ClearCart(cart);
                var map = _mapper.Map<ResponseCartDTO>(clearedCart);

                    return Ok(map); 
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        

    }
}

