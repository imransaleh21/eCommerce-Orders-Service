using FluentValidation;
using OrdersMicroService.BusinessLogicLayer.DTOs;

namespace OrdersMicroService.BusinessLogicLayer.Validators;
public class OrderItemAddRequestValidator : AbstractValidator<OrderItemAddRequest>
{
    public OrderItemAddRequestValidator()
    {
        RuleFor(orderItemAddRequest => orderItemAddRequest.ProductID)
            .NotEmpty().WithErrorCode("ProductID can't be blank");
        RuleFor(orderItemAddRequest => orderItemAddRequest.Quantity)
            .NotEmpty().WithErrorCode("Quantity can't be blank")
            .GreaterThan(0).WithErrorCode("Quantity must be greater than 0");
        RuleFor(orderItemAddRequest => orderItemAddRequest.UnitPrice)
            .NotEmpty().WithErrorCode("UnitPrice can't be blank")
            .GreaterThan(0).WithErrorCode("UnitPrice must be greater than 0");
    }
}
