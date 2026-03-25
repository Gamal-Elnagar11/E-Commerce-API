using E_Commerce_API.DTO.CartDTO;
using E_Commerce_API.Models;
using E_Commerce_API.Reposatory.Implementation;
using E_Commerce_API.Service.Interface;
using E_Commerce_API.UnitOfWork;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce_API.Service.Implementation
{
    public class CartService : ICartService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public CartService(UserManager<User> userManager, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _contextAccessor = httpContextAccessor;
        }

        //private string GetUserId()
        //{
        //    var principle = _contextAccessor.HttpContext.User;
        //    var userid = _userManager.GetUserId(principle);
        //    return userid;
        //}
        private string GetUserId()
        {
            var user = _contextAccessor.HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
                throw new ArgumentException("User not logged in"); // أو throw استثناء لو تحب

            // حاول تجيب الـ UserId من claim "sub" أو "NameIdentifier"
            var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? user.FindFirst("sub")?.Value;

            return userId;
        }
        public async Task<List<Cart>> GetAllCarts()
        {
            return await _unitOfWork.CartRepo.GetAllCarts();



        }

        public async Task<Cart> GetCartByUserId()
        {
            var userid = GetUserId();
             var result = await _unitOfWork.CartRepo.GetCartByUserId(userid);
            if (result == null)
                throw new ArgumentException("Can not found cart to this userid");

            return result;



        }

        public async Task<Cart> GetOrCreateCart()
        {
 
            var userid = GetUserId();
            if (string.IsNullOrEmpty(userid))
                throw new UnauthorizedAccessException("User must be logged in.");

            var cart = await _unitOfWork.CartRepo.GetCartByUserId(userid);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userid
                };

                await _unitOfWork.CartRepo.AddCart(cart);
                await _unitOfWork.CompleteAsync();
            }


            //var cartDto = new ResponseCartDTO
            //{
            //    Id = cart.Id,
            //    Items = cart.CartItems.Select(i => new CartItemDTO
            //    {
            //        ProductId = i.ProductId,
            //        ProductName = i.Products.Name,
            //        ImageUrl = i.Products.ImageUrl,
            //        Quantity = i.Quantity,
            //        UnitPrice = i.UnitPrice,
            //        TotalPrice = i.Quantity * i.UnitPrice
            //    }).ToList(),

            //    TotalPrice = cart.CartItems.Sum(i => i.UnitPrice * i.Quantity)
            //};
             
            return cart;
        }
        public async Task<Cart> ClearCart(Cart cart)
        {
            if (cart == null)
                throw new ArgumentException("Cart Not Found");
            cart.CartItems.Clear();
            await _unitOfWork.CompleteAsync();
            return cart;
        }

 













        public async Task<CartItem> AddItemCart( int productid, int quantity)
        {
            var userid = GetUserId();
            var cart = await _unitOfWork.CartRepo.GetCartByUserId(userid);
            if(cart == null)
            {
                cart = new Cart
                {
                    UserId = userid
                };
                await _unitOfWork.CartRepo.AddCart(cart);
                await _unitOfWork.CompleteAsync();

            }

            var product = await _unitOfWork.ProductRepo.GetProductsByIdAsync(productid);
            if (product == null)
                throw new ArgumentException("Product not found");
            var existproduct = cart.CartItems
                .FirstOrDefault(a => a.ProductId == productid);

            if (existproduct != null)
            {
                existproduct.Quantity += quantity;
                await _unitOfWork.CompleteAsync();
                return existproduct;
            }
            var newitem = new CartItem
            {
                CartId = cart.Id,
                 Quantity = quantity,
                UnitPrice = product.Price,
                ProductId = product.Id,
                

            };
            cart.CartItems = cart.CartItems ?? new List<CartItem>();
            cart.CartItems.Add(newitem);
              await _unitOfWork.CompleteAsync();
              return newitem;

        }


        public async Task<CartItem> UpdateItemCarrQuantity(Cart cart, int productid, int newquantity)
        {
         
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productid);

            if (cartItem == null)
                throw new ArgumentException("Product not found in cart");

            cartItem.Quantity = newquantity;
            await _unitOfWork.CompleteAsync();
 
            return cartItem;
        }        

        public async Task<CartItem> DeleteItemFromCart(Cart cart, int productid)
        { 
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productid);

            if (cartItem == null)
                throw new ArgumentException("Product not found in cart");

            cart.CartItems.Remove(cartItem);

            await _unitOfWork.CompleteAsync();
            return cartItem;
        }
    


         


    }
}
