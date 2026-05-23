using FluentValidation;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using System.Data;

namespace OrdersMicroService.BusinessLogicLayer.Validators;
public class OrderUpdateRequestValidator : AbstractValidator<OrderUpdateRequest>
{
    public OrderUpdateRequestValidator()
    {
        RuleFor(orderUpdateRequest => orderUpdateRequest.OrderID)
            .NotEmpty().WithErrorCode("Order Id can't be blank");
        RuleFor(orderUpdateRequest => orderUpdateRequest.UserID)
            .NotEmpty().WithErrorCode("User Id can't be blank");
        RuleFor(orderUpdateRequest => orderUpdateRequest.OrderDate)
            .NotEmpty().WithErrorCode("Order Date can't be blank");
        RuleForEach(orderUpdateRequest => orderUpdateRequest.OrderItems)
            .SetValidator(new OrderItemUpdateRequestValidator());
    }
}
