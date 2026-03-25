using AutoMapper;
using E_Commerce_API.DTO.OrderDTO;
using E_Commerce_API.Service.Interface;
using E_Commerce_API.Static;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API.Controllers
{ 
        [Route("api/[controller]")]
        [ApiController]
       [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserOrAdmin")]

    public class OrdersController : ControllerBase
        {
            private readonly IOrderService _orderService;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _contextAccessor;

            public OrdersController(IOrderService orderService, IMapper mapper, IHttpContextAccessor contextAccessor)
            {
                _orderService = orderService;
                _mapper = mapper;
                _contextAccessor = contextAccessor;
            }

            [HttpPost("Checkout->Create-Order")]
            public async Task<IActionResult> Checkout([FromBody] CheckoutDTO dto)
            {
                try
                {
                    var userId = _contextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return BadRequest("User Id Not Found");
                    var order = await _orderService.Checkout(userId, dto.PhoneNumber, dto.City, dto.Address, dto.PaymentMethod);


                 if (!Enum.IsDefined(typeof(Payment), dto.PaymentMethod))
                    throw new Exception("Invalid payment method");

                var response = _mapper.Map<ResponseOrderDTO>(order);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
         
        
           [HttpGet("My-Orders")]
            public async Task<IActionResult> GetMyOrders()
            {
                try
                {
                    var userId = _contextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var orders = await _orderService.GetOrdersByUser(userId);

                    var response = _mapper.Map<List<ResponseOrderDTO>>(orders);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
         
        
            [HttpGet("{orderId}")]
         public async Task<IActionResult> GetOrderById(int orderId)
            {
                try
                {
                    var order = await _orderService.GetOrderById(orderId);
                    var response = _mapper.Map<ResponseOrderDTO>(order);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

         
            [HttpGet("AllOrders")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
            {
                try
                {
                    var orders = await _orderService.GetAllOrders();
                    var response = _mapper.Map<List<ResponseOrderDTO>>(orders);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
         
        
        
            [HttpPut("{orderId}/StatusOrder")]
           [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] OrderStatus status)
            {
                try
                {
                    var order = await _orderService.UpdateOrderStatus(orderId, status);
                    var response = _mapper.Map<ResponseOrderDTO>(order);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }


            [HttpGet("Payment-Methods")]
            public IActionResult GetPaymentMethods()
            {
                var methods = Enum.GetValues(typeof(Payment))
                                  .Cast<Payment>()
                                  .Select(m => m.ToString())
                                  .ToList();
                return Ok(methods);
            }
   
    
    
    }
    
}
