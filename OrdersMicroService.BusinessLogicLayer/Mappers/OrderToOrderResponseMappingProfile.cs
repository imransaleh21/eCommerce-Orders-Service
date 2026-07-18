using AutoMapper;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using OrdersMicroService.DataAccessLayer.Entities;

namespace OrdersMicroService.BusinessLogicLayer.Mappers;
public class OrderToOrderResponseMappingProfile : Profile
{
    public OrderToOrderResponseMappingProfile()
    {
        CreateMap<Order, OrderResponse>();
    }
}
