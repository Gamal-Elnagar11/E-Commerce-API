using AutoMapper;
using E_Commerce_API.DTO.OrderDTO;
using E_Commerce_API.Models;

namespace E_Commerce_API.Mapping
{
    public class OrderProfile : Profile
    {
         
        public OrderProfile()
        {
            // -----------------------------------
            // OrderItem → OrderItemDTO
            // -----------------------------------
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.Price));

             

            CreateMap<OrderItem, OrderItemDTO>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));

            CreateMap<Order, ResponseOrderDTO>()
           .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                //.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "UnKnownUser"))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DateTime.ToString("yyyy-MM-dd HH:mm:ss")));

 
        }
    }
}

