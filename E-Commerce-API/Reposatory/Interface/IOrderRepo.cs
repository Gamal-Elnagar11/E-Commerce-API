using E_Commerce_API.Models;
using E_Commerce_API.Static;

namespace E_Commerce_API.Reposatory.Interface
{
    public interface IOrderRepo
    { 
            // إضافة طلب جديد
            Task AddOrder(Order order);

            // جلب طلب واحد بالـ Id
            Task<Order> GetOrderById(int orderId);

            // جلب كل الطلبات لمستخدم محدد
            Task<List<Order>> GetOrdersByUserId(string userId);

            // جلب كل الطلبات (Admin)
            Task<List<Order>> GetAllOrders();

            // تعديل حالة الطلب
            Task UpdateOrderStatus(int orderId, OrderStatus status);

            
        
    }
}
