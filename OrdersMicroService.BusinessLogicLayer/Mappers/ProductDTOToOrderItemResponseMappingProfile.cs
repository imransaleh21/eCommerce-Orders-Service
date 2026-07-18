using AutoMapper;
using OrdersMicroService.BusinessLogicLayer.DTOs;

namespace OrdersMicroService.BusinessLogicLayer.Mappers;

public class ProductDTOToOrderItemResponseMappingProfile : Profile
{
    public ProductDTOToOrderItemResponseMappingProfile()
    {
        CreateMap<ProductDTO, OrderItemResponse>();
    }
}