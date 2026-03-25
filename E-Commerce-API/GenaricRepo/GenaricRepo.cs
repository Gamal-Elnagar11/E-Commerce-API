using E_Commerce_API.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_API.GenaricRepo
{
    public class GenaricRepo<T> : IGenaricRepo<T> where T : class
    {

        private readonly Application _db;
        private readonly DbSet<T> _Set;

        public GenaricRepo(Application db)
        {
            _db = db; 
            _Set = _db.Set<T>();
        }


        public IQueryable<T> GetAll()
        {
             
            return _Set;
        }



        public async Task<T> GetByIdAsync(int id)
        {
             return await _Set.FindAsync(id);
        }




        public async Task<T> AddAsync(T value)
        {
            await _Set.AddAsync(value);
            return value;
        }





        public void Update(T value)
        {
            _Set.Update(value);
        }


        public void Delete(T value)
        {
            _Set.Remove(value);
        }

    }
}

