using AutoMapper;
using E_Commerce_API.DTO.CategoryDTO;
using E_Commerce_API.DTO.ProductDTO;
using E_Commerce_API.Models;

namespace E_Commerce_API.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoriesDTO>().ReverseMap();
             CreateMap<Product, ProductBaseDTO>();

            CreateMap<Category, CategorywithProductDTO>()
                .ForMember(dest => dest.Products,
                           opt => opt.MapFrom(src => src.Products));
            CreateMap<Category, CategoryName>().ReverseMap();

        }
    }
    
}
