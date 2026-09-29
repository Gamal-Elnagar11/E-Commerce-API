using E_Commerce_API.Models;
using E_Commerce_API.Static;

namespace E_Commerce_API.Reposatory.Interface
{
    public interface IOrderRepo
    { 
             Task AddOrder(Order order);

             Task<Order> GetOrderById(int orderId);

             Task<List<Order>> GetOrdersByUserId(string userId);
             Task<List<Order>> GetAllOrders();

             Task UpdateOrderStatus(int orderId, OrderStatus status);
            Task DeleteOrder (int  orderId);

            
        
    }
}
