using AutoMapper;
using E_Commerce_API.DTO.ProductDTO;
using E_Commerce_API.Models;

namespace E_Commerce_API.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<AddProductDTO, Product>()
           .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());  

            CreateMap<GetAllProduct, Product>().ReverseMap();
            CreateMap<ResponseProduct, Product>().ReverseMap();
            CreateMap<UpdateProductDTO, Product>().ReverseMap();
            CreateMap<Product , ResponseProduct>().ReverseMap();
                 


        }
    }
}
