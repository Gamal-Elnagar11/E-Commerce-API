using E_Commerce_API.DTO.FAQDTO;
using E_Commerce_API.Models;
using E_Commerce_API.Service.Interface;
using E_Commerce_API.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace E_Commerce_API.Service.Implementation
{
    public class FAQService : IFAQService
    {


        private readonly IUnitOfWork _unitOfWork;

        public FAQService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FAQ> AddFAQ(FAQ fAQ)
        {
           var result =  await _unitOfWork.Repositoey<FAQ>().AddAsync(fAQ);
            await _unitOfWork.CompleteAsync();
            return result;
        }

        public async Task<FAQ> DeleteFAQ(int id)
        {
            var getid = await _unitOfWork.Repositoey<FAQ>().GetByIdAsync(id);
            if (getid == null)
                throw new KeyNotFoundException($" FAQ With Id {id} Not Found");

              _unitOfWork.Repositoey<FAQ>().Delete(getid);
             await _unitOfWork.CompleteAsync();
            return getid;

         }

        public async Task <List<FAQ>> GetAllQuastion()
        {
             return await _unitOfWork.Repositoey<FAQ>().GetAll().ToListAsync();
        }

        public async Task<FAQ> GetAnswerById(int id)
        {
            var getid = await _unitOfWork.Repositoey<FAQ>().GetByIdAsync(id);
            if (getid == null)
                throw new KeyNotFoundException($"FAQ with id {id} not found");

             return getid;
        }






        public async Task<FAQ> UpdateFAQ(FAQtest fAQ)
        {
            var getid = await _unitOfWork.Repositoey<FAQ>().GetByIdAsync(fAQ.Id);
            if (getid == null)
                throw new KeyNotFoundException($"FAQ with id {fAQ.Id} not found");

            getid.Question = fAQ.Question;
            getid.Answer = fAQ.Answer;

               _unitOfWork.Repositoey<FAQ>().Update(getid);
             await _unitOfWork.CompleteAsync();
            return getid;
        }
    }
}
