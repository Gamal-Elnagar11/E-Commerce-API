using E_Commerce_API.DTO.OrderDTO;
using E_Commerce_API.Models;
using E_Commerce_API.Reposatory.Interface;
using E_Commerce_API.Service.Interface;
using E_Commerce_API.Static;
using E_Commerce_API.UnitOfWork;

namespace E_Commerce_API.Service.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepo orderRepo, ICartService cartService, IUnitOfWork unitOfWork)
        {
            _orderRepo = orderRepo;
            _cartService = cartService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Order> Checkout(string userId, string phone, string city, string address, Payment paymentMethod)
        {
            var trans = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cart = await _cartService.GetOrCreateCart();

                if (!cart.CartItems.Any())
                    throw new Exception("Cart is empty");

                // 2. إنشاء Order جديد
                var order = new Order
                {
                    UserId = userId,
                    PhoneNumber = phone,
                    City = city,
                    Address = address,

                    PaymentMethod = paymentMethod,
                    Status = paymentMethod == Payment.CreditCard ? OrderStatus.PendingPayment : OrderStatus.Pending,
                    DateTime = DateTime.UtcNow,
                    OrderItems = cart.CartItems.Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        ProductName = ci.Products.Name,
                        Quantity = ci.Quantity,
                        Price = ci.Products.Price,
                        TotalPrice = ci.Quantity * ci.Products.Price
                    }).ToList(),
                    TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Products.Price)
                    // TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Products.Price)
                };
                foreach (var item in cart.CartItems)
                {
                    var product = await _unitOfWork.ProductRepo.GetProductsByIdAsync(item.ProductId);

                    if (product == null)
                        throw new Exception($"Product with id {item.ProductId} not found");

                    if (product.Stock < item.Quantity)
                        throw new Exception($"Not enough stock for product {product.Name}");

                    product.Stock -= item.Quantity;

                    _unitOfWork.ProductRepo.UpdateProduct(product);
                }


                // 3. حفظ الطلب
                await _unitOfWork.OrderRepo.AddOrder(order);
                await _unitOfWork.CompleteAsync();
                await trans.CommitAsync();
                 await _cartService.ClearCart(cart);

                 return await _unitOfWork.OrderRepo.GetOrderById(order.Id);

            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                throw ;
            }
        }



        public async Task<List<Order>> GetOrdersByUser(string userId)
        {
            return await _unitOfWork.OrderRepo.GetOrdersByUserId(userId);
        }

        // -----------------------------------
        // 3️⃣ جلب طلب واحد
        // -----------------------------------
        public async Task<Order> GetOrderById(int orderId)
        {
            var order = await _unitOfWork.OrderRepo.GetOrderById(orderId);
            if (order == null) throw new Exception("Order not found");
            return order;
        }

        // -----------------------------------
        // 4️⃣ جلب كل الطلبات (Admin)
        // -----------------------------------
        public async Task<List<Order>> GetAllOrders()
        {
            return await _unitOfWork.OrderRepo.GetAllOrders();
        }

        // -----------------------------------
        // 5️⃣ تغيير حالة الطلب (Admin)
        // -----------------------------------
        public async Task<Order> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            await _unitOfWork.OrderRepo.UpdateOrderStatus(orderId, status);
            await _unitOfWork.CompleteAsync();

            return await _unitOfWork.OrderRepo.GetOrderById(orderId);
        }

    }
         
 }