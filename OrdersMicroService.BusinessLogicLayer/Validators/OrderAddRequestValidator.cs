using FluentValidation;
using OrdersMicroService.BusinessLogicLayer.DTOs;

namespace OrdersMicroService.BusinessLogicLayer.Validators;
public class OrderAddRequestValidator : AbstractValidator<OrderAddRequest>
{
    public OrderAddRequestValidator()
    {
        RuleFor(orderAddRequest => orderAddRequest.UserID)
            .NotEmpty().WithErrorCode("UserID can't be blank");
        RuleFor(orderAddRequest => orderAddRequest.OrderDate)
            .NotEmpty().WithErrorCode("OrderDate can't be blank");
        RuleForEach(orderAddRequest => orderAddRequest.OrderItems)
           .SetValidator(new OrderItemAddRequestValidator());
    }
}
