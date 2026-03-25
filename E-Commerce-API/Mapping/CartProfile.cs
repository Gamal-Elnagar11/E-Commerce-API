using AutoMapper;
using E_Commerce_API.DTO.CartDTO;
using E_Commerce_API.Models;

namespace E_Commerce_API.Mapping
{
    public class CartProfile : Profile
    {
        
            public CartProfile()
            {
                CreateMap<CartItem, CartItemDTO>()
                    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Products.Name))
                    .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Products.ImageUrl))
                    .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice))
                    .ForSourceMember(src => src.Cart, opt => opt.DoNotValidate()); // يتجاهل الـ Cart داخل CartItem

                //CreateMap<Cart, ResponseCartDTO>()
                //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                //    .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.CartItems.Sum(i => i.UnitPrice * i.Quantity)));

            CreateMap<Cart, ResponseCartDTO>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems)) // ← مهم جدًا
        .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.CartItems.Sum(i => i.UnitPrice * i.Quantity)));

        }
        
    }
}
