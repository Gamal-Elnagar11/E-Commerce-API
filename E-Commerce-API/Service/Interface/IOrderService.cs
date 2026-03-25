using E_Commerce_API.Models;
using E_Commerce_API.Static;

namespace E_Commerce_API.Service.Interface
{
    public interface IOrderService
    {
        // Checkout: تحويل Cart → Order
        Task<Order> Checkout(string userId, string phone, string city, string address, Payment paymentMethod);

        // جلب كل طلبات مستخدم
        Task<List<Order>> GetOrdersByUser(string userId);

        // جلب طلب واحد
        Task<Order> GetOrderById(int orderId);

        // جلب كل الطلبات (Admin)
        Task<List<Order>> GetAllOrders();

        // تغيير حالة طلب (Admin)
        Task<Order> UpdateOrderStatus(int orderId, OrderStatus status);
    }
}

