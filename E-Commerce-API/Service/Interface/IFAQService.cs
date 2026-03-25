using E_Commerce_API.DTO.FAQDTO;
using E_Commerce_API.Models;

namespace E_Commerce_API.Service.Interface
{
    public interface IFAQService
    {
        public Task<List<FAQ>> GetAllQuastion();
        public Task<FAQ> AddFAQ(FAQ fAQ);
        public Task<FAQ> UpdateFAQ(FAQ fAQ);
        public Task<FAQ> DeleteFAQ(int id);
        public Task<FAQ> GetAnswerById(int id);

    }
}
