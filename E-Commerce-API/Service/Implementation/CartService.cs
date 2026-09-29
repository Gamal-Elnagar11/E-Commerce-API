using E_Commerce_API.DTO.CartDTO;
using E_Commerce_API.Models;
using E_Commerce_API.Reposatory.Implementation;
using E_Commerce_API.Service.Interface;
using E_Commerce_API.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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





        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Repositoey<Product>()
                                 .GetAll()                     // IQueryable
                                 .Include(p => p.Category)     // Include Relations
                                 .FirstOrDefaultAsync(p => p.Id == id);

            return product;


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












        public async Task<CartItem> AddItemCart(int productid, int quantity)
        {
            var userid = GetUserId();
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be at least 1");
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // 1. جلب المنتج (تأكد أن الـ Repository لا يستخدم AsNoTracking)
                    var product = await _unitOfWork.ProductRepo.GetProductsByIdAsync(productid);
                    if (product == null) throw new ArgumentException("Product not found");
                   
                       // 2. جلب السلة
                    var cart = await _unitOfWork.CartRepo.GetCartByUserId(userid);
                    if (cart == null)
                    {
                        cart = new Cart { UserId = userid };
                        await _unitOfWork.CartRepo.AddCart(cart);
                        // لا تستدعي CompleteAsync هنا، انتظر للنهاية
                    }


                    var existproduct = cart.CartItems?.FirstOrDefault(a => a.ProductId == productid);
                    int currentInCart = existproduct?.Quantity ?? 0;

                    // هنا نتحقق: هل المخزون المتاح يكفي (الكمية الحالية في السلة + الكمية الجديدة)؟
                    if (product.Stock < (currentInCart + quantity))
                        throw new ArgumentException("Product Quantity Not Found");
  
                    // 5. تحديث السلة
                    if (existproduct != null)
                    {
                        existproduct.Quantity += quantity;
                    }
                    else
                    {
                        var newitem = new CartItem
                        {
                            CartId = cart.Id,
                            Quantity = quantity,
                            UnitPrice = product.Price,
                            ProductId = product.Id
                        };
                        cart.CartItems.Add(newitem);
                        existproduct = newitem; // للعودة به في النهاية
                    }

                    // 6. حفظ الكل (الخصم + الإضافة) في عملية واحدة
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    return existproduct;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw; // أعد رمي الخطأ ليعرف الـ Controller أن العملية فشلت
                }
            }
        }

        public async Task<CartItem> UpdateItemCartQuantity(int productid, int newquantity)
        {
            var userid = GetUserId();
            if (newquantity <= 0)
                throw new ArgumentException("Quantity must be at least 1");
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // 1. جلب المنتج المحدث (للتأكد من المخزن الحقيقي)
                    var product = await _unitOfWork.ProductRepo.GetProductsByIdAsync(productid);
                    if (product == null) throw new ArgumentException("Product not found");

                    // 2. جلب السلة
                    var cart = await _unitOfWork.CartRepo.GetCartByUserId(userid);
                    var cartItem = cart?.CartItems?.FirstOrDefault(ci => ci.ProductId == productid);
                    if (cartItem == null) throw new ArgumentException("Product not found in cart");

                    
                    if (newquantity > product.Stock)
                        throw new ArgumentException("Product Quantity Not Found");
                     
                    // 5. تحديث السلة
                    cartItem.Quantity = newquantity;

                    // 6. الحفظ
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    return cartItem;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<CartItem> DeleteItemFromCart(int productid)
        {
            var userid = GetUserId();

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // 1. جلب المنتج والسلة
                    var product = await _unitOfWork.ProductRepo.GetProductsByIdAsync(productid);
                    var cart = await _unitOfWork.CartRepo.GetCartByUserId(userid);
                    var cartItem = cart?.CartItems?.FirstOrDefault(ci => ci.ProductId == productid);

                    if (cartItem == null)
                        throw new ArgumentException("Product not found in cart");
 
                    // 3. إزالة المنتج من السلة
                    cart.CartItems.Remove(cartItem);

                    // 4. حفظ التغييرات
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    return cartItem;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }





    }
}
