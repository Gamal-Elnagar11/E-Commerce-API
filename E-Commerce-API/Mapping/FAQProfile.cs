using AutoMapper;
using E_Commerce_API.DTO.FAQDTO;
using E_Commerce_API.Models;

namespace E_Commerce_API.Mapping
{
    public class FAQProfile : Profile
    {
        public FAQProfile()
        {
            CreateMap<FAQ,FAQsDTO>().ReverseMap();
            CreateMap<FAQ, AnswerDTO>();
            CreateMap<FAQ, GetAllQuastionDTO>();
        }
    }
}
