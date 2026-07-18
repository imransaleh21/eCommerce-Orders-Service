using AutoMapper;
using OrdersMicroService.BusinessLogicLayer.DTOs;

namespace OrdersMicroService.BusinessLogicLayer.Mappers;

public class UserDTOToOrderResponseMappingProfile : Profile
{
    public UserDTOToOrderResponseMappingProfile()
    {
        CreateMap<UserDTO, OrderResponse>();
    }
}
