using FluentValidation;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using System.Data;

namespace OrdersMicroService.BusinessLogicLayer.Validators;

public class OrderItemUpdateRequestValidator : AbstractValidator<OrderItemUpdateRequest>
{
    public OrderItemUpdateRequestValidator()
    {
        RuleFor(orderItemUpdateRequest => orderItemUpdateRequest.ProductID)
            .NotEmpty().WithErrorCode("Order Item Id can't be blank");
        RuleFor(orderItemUpdateRequest => orderItemUpdateRequest.Quantity)
            .NotEmpty().WithErrorCode("Quantity can't be blank")
            .GreaterThan(0).WithErrorCode("Quantity must be greater than 0");
        RuleFor(orderItemUpdateRequest => orderItemUpdateRequest.UnitPrice)
            .NotEmpty().WithErrorCode("UnitPrice can't be blank")
            .GreaterThan(0).WithErrorCode("UnitPrice must be greater than 0");
    }
}
