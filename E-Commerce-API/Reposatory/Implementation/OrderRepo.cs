using E_Commerce_API.Models;
using System;
using E_Commerce_API.Reposatory.Interface;
using E_Commerce_API.Static;
using E_Commerce_API.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_API.Reposatory.Implementation
{
    public class OrderRepo : IOrderRepo
    {

        private readonly Application _db;

        public OrderRepo(Application db)
        {
            _db = db;
        }

            public async Task AddOrder(Order order)
            {
                await _db.Orders.AddAsync(order);
            }

            public async Task<Order> GetOrderById(int orderId)
            {
                return await _db.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(a => a.Products)
                    .Include( a => a.User)
                    .FirstOrDefaultAsync(o => o.Id == orderId);
            }

            public async Task<List<Order>> GetOrdersByUserId(string userId)
            {
                return await _db.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(a => a.Products)
                    .Include( a => a.User)
                    .Where(o => o.UserId == userId)
                    .ToListAsync();
            }

            public async Task<List<Order>> GetAllOrders()
            {
                return await _db.Orders
                    .Include(o => o.OrderItems)
                    .Include(a => a.User)
                    .ToListAsync();
            }

            public async Task UpdateOrderStatus(int orderId, OrderStatus status)
            {
                var order = await _db.Orders.FindAsync(orderId);
                if (order == null) throw new Exception("Order not found");
                order.Status = status;
            }

             
        
    }
}
