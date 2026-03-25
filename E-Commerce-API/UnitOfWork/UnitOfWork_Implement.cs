using E_Commerce_API.Data;
using E_Commerce_API.GenaricRepo;
using E_Commerce_API.Models;
using E_Commerce_API.Reposatory.Implementation;
using E_Commerce_API.Reposatory.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace E_Commerce_API.UnitOfWork
{
    public class UnitOfWork_Implement : IUnitOfWork
    {

        private readonly Application _db;
        private IDbContextTransaction _transaction;
         public IGetUserCart getuseridincart { get; }
        private readonly Dictionary<Type, object> _repository = new Dictionary<Type, object>();

        private IProductRepo _productRepo;
        private ICategoryRepo _categoryRepo;  
        private ICartRepo _cartRepo;
        private IOrderRepo _orderRepo;


        public UnitOfWork_Implement( Application db)
        { 
             _db = db;
            getuseridincart = new GetUserIdInCart(_db);   // here get userid
        }

 
        public  ICategoryRepo CategoryRepo
        {
            get
            {
                if (_categoryRepo == null)
                    _categoryRepo = new CategoryRepo(_db);
                return _categoryRepo;
            }
        }

         public IProductRepo ProductRepo
        {
            get
            {
                if(_productRepo == null)
                    _productRepo = new ProductRepo(_db);
                return _productRepo;
            }
        }

        public ICartRepo CartRepo
        {
            get
            {
                if(_cartRepo == null)
                    _cartRepo = new CartRepo(_db);
                return _cartRepo;
            }
        }

        public IOrderRepo OrderRepo
        {
            get
            {
                if(_orderRepo == null)
                    _orderRepo = new OrderRepo(_db);
                return _orderRepo;
            }
        }

        //public async Task<Cart> GetCartByUserIdAsync(string userId)
        //{
        //    return await   getuseridincart.GetUserIdInCartAsync(userId);
        //}




        public void Dispose()
        {
           _db.Dispose();
        }


        public IGenaricRepo<T> Repositoey<T>() where T : class
        {
            if(_repository.ContainsKey(typeof(T)))
            {
                return _repository[typeof(T)] as IGenaricRepo<T>;
            }
            var repository = new GenaricRepo<T>(_db);
            _repository[typeof(T)] = repository;
            return repository;
        }





        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
            return _transaction;
         }

        public async Task CommmetTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _transaction.CommitAsync();
        }
          

        public async Task RollebackAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _transaction.RollbackAsync();
        }

        public async Task<int> CompleteAsync()
        {
            return await _db.SaveChangesAsync();
        }



    }
}
