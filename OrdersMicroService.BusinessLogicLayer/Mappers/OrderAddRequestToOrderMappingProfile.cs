using AutoMapper;
using OrdersMicroService.DataAccessLayer.Entities;

namespace OrdersMicroService.BusinessLogicLayer.Mappers;

public class OrderAddRequestToOrderMappingProfile : Profile
{
    public OrderAddRequestToOrderMappingProfile()
    {
        CreateMap<DTOs.OrderAddRequest, Order>()
            .ForMember(dest => dest._id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderID, opt => opt.Ignore())
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserID))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.TotalBill, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
    }
}
