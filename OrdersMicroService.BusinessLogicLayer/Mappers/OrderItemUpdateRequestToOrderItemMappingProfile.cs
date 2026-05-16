using AutoMapper;

namespace OrdersMicroService.BusinessLogicLayer.Mappers;

public class OrderItemUpdateRequestToOrderItemMappingProfile : Profile
{
    public OrderItemUpdateRequestToOrderItemMappingProfile()
    {
        CreateMap<DTOs.OrderItemUpdateRequest, DataAccessLayer.Entities.OrderItem>()
            .ForMember(dest => dest._id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore());
    }
}
