using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MongoDB.Driver;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using OrdersMicroService.BusinessLogicLayer.ServicesContract;
using OrdersMicroService.DataAccessLayer.Entities;
using OrdersMicroService.DataAccessLayer.RepositoriesContract;
namespace OrdersMicroService.BusinessLogicLayer.Services;

public class OrdersService : IOrdersService
{
    private readonly IOrdersRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<OrderAddRequest> _orderAddValidator;
    private readonly IValidator<OrderUpdateRequest> _orderUpdateValidator;
    private readonly IValidator<OrderItemAddRequest> _orderItemAddValidator;
    private readonly IValidator<OrderItemUpdateRequest> _orderItemUpdateValidator;
    public OrdersService(IOrdersRepository orderRepository, IMapper mapper,
        IValidator<OrderItemAddRequest> orderItemAddValidator, IValidator<OrderItemUpdateRequest> orderItemUpdateRequestValidator,
        IValidator<OrderAddRequest> orderAddRequestValidator, IValidator<OrderUpdateRequest> orderUpdateRequestValidator)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _orderAddValidator = orderAddRequestValidator;
        _orderUpdateValidator = orderUpdateRequestValidator;
        _orderItemAddValidator = orderItemAddValidator;
        _orderItemUpdateValidator = orderItemUpdateRequestValidator;
    }
    public async Task<OrderResponse?> AddOrder(OrderAddRequest orderAddRequest)
    {
        if (orderAddRequest is null) throw new ArgumentNullException(nameof(orderAddRequest));
        ValidationResult validationResult = await _orderAddValidator.ValidateAsync(orderAddRequest);
        if (!validationResult.IsValid)
        {
            string errors = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(errors);
        }
        // Logic for validate UserID in Users microservice
        // ...

        // OrderAddRequest to Order mapping
        Order order = _mapper.Map<Order>(orderAddRequest);
        foreach (var orderItem in order.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        order.TotalBill = order.OrderItems.Sum(oi => oi.TotalPrice);

        // Add order to database
        Order? addedOrder = await _orderRepository.AddOrder(order);
        if (addedOrder is null) return null;
        return _mapper.Map<OrderResponse>(addedOrder);
    }

    public Task<bool> DeleteOrder(Guid orderID)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order> filter)
    {
        throw new NotImplementedException();
    }

    public Task<List<OrderResponse?>> GetOrders()
    {
        throw new NotImplementedException();
    }

    public Task<List<OrderResponse?>> GetOrdersByCondition(FilterDefinition<Order> filter)
    {
        throw new NotImplementedException();
    }

    public async Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
    {
        if (orderUpdateRequest is null) throw new ArgumentNullException(nameof(orderUpdateRequest));
        ValidationResult validationResult = await _orderUpdateValidator.ValidateAsync(orderUpdateRequest);
        if (!validationResult.IsValid)
        {
            string errors = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(errors);
        }
        // Logic for validate UserID in Users microservice
        // ...
        Order order = _mapper.Map<Order>(orderUpdateRequest);
        foreach (var orderItem in order.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        order.TotalBill = order.OrderItems.Sum(oi => oi.TotalPrice);
        Order? updatedOrder = await _orderRepository.UpdateOrder(order);
        if (updatedOrder is null) return null;
        return _mapper.Map<OrderResponse>(updatedOrder);
    }
}
