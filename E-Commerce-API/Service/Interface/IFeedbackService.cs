using E_Commerce_API.Models;

namespace E_Commerce_API.Service.Interface
{
    public interface IFeedbackService
    {
        public Task AddFeedback(int Rating, string UserId, string Comment, string UserName);
        public Task<List<Feedback>> GetAllFeedback();
        public Task<Feedback> DeleteFeedback(int id);
    }
}
