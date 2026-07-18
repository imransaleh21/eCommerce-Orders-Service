namespace OrdersMicroService.BusinessLogicLayer.DTOs;

public record UserDTO(
Guid UserId,
string Email,
string PersonName,
string Gender
);
