using AutoMapper;
using E_Commerce_API.DTO.Identity;
using E_Commerce_API.Models;

namespace E_Commerce_API.Mapping
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<RegisterDTO, User>();
            CreateMap<User, UsersDTO>();
            



 

        }
    }
}
