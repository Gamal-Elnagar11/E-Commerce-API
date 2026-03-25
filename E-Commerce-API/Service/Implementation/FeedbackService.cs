using E_Commerce_API.Data;
using E_Commerce_API.Models;
using E_Commerce_API.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_API.Service.Implementation
{
    public class FeedbackService : IFeedbackService
    {

        private readonly Application _db;
        public FeedbackService(Application db)
        {
            _db = db;
        }
        public async Task AddFeedback(int Rating, string UserId, string Comment, string UserName)
        {

            if (Rating < 1 || Rating > 5)
                throw new ArgumentException(" Rating Must be betwen 1 and 5");
            var feedback = new Feedback
            {
                Comment = Comment,
                UserId = UserId,
                UserName = UserName,
                Rating = Rating,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Feedbacks.AddAsync(feedback);
            await _db.SaveChangesAsync();
        }



        public async Task<Feedback> DeleteFeedback(int id)
        {
            var result = await _db.Feedbacks.FindAsync(id);
            if (result == null)
                throw new ArgumentException($"this id {id} Not found");

              _db.Feedbacks.Remove(result);
             await  _db.SaveChangesAsync();
             return result;
        }

        public async Task<List<Feedback>> GetAllFeedback()
        {
            var allfeed = await _db.Feedbacks.ToListAsync();
            return allfeed;
        }
    }
}
