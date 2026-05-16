using AutoMapper;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using OrdersMicroService.DataAccessLayer.Entities;

namespace OrdersMicroService.BusinessLogicLayer.Mappers;
public class OrderToOrderResponseMappingProfile : Profile
{
    public OrderToOrderResponseMappingProfile()
    {
        CreateMap<Order, OrderResponse>()
            .ConstructUsing(src => new OrderResponse(
                src.OrderID,
                src.UserID,
                src.OrderDate,
                src.TotalBill,
                src.OrderItems.Select(oi => new OrderItemResponse(
                    oi.ProductID,
                    oi.Quantity,
                    oi.UnitPrice,
                    oi.TotalPrice
                )).ToList()
            ));
    }
}
