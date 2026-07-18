using AutoMapper;
using eCommerce.Core.HttpClients;
using FluentValidation;
using FluentValidation.Results;
using MongoDB.Driver;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using OrdersMicroService.BusinessLogicLayer.HttpClients;
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
    private readonly UsersMicroserviceClient _usersMicroserviceClient;
    private readonly ProductsMicroserviceClient _productsMicroserviceClient;
    public OrdersService(IOrdersRepository orderRepository, IMapper mapper,
        IValidator<OrderAddRequest> orderAddRequestValidator, IValidator<OrderUpdateRequest> orderUpdateRequestValidator,
        UsersMicroserviceClient usersMicroserviceClient, ProductsMicroserviceClient productsMicroserviceClient)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _orderAddValidator = orderAddRequestValidator;
        _orderUpdateValidator = orderUpdateRequestValidator;
        _usersMicroserviceClient = usersMicroserviceClient;
        _productsMicroserviceClient = productsMicroserviceClient;
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
        UserDTO? userID = await _usersMicroserviceClient.GetUserByIdAsync(orderAddRequest.UserID);
        if (userID == null)
        {
            throw new ValidationException($"User with ID {orderAddRequest.UserID} does not exist in Users Table.");
        }

        // OrderAddRequest to Order mapping
        Order order = _mapper.Map<Order>(orderAddRequest);
        var productMap = new Dictionary<Guid, ProductDTO>();
        foreach (var orderItem in order.OrderItems)
        {
            // Logic to Validate ProductID in Products microservice can be added here
            ProductDTO? product = await _productsMicroserviceClient.GetProductByIdAsync(orderItem.ProductID);
            if (product == null)
            {
                throw new ValidationException($"Product with ID {orderItem.ProductID} does not exist in Products Table.");
            }
            productMap[orderItem.ProductID] = product;
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        order.TotalBill = order.OrderItems.Sum(oi => oi.TotalPrice);

        // Add order to database
        Order? addedOrder = await _orderRepository.AddOrder(order);
        if (addedOrder is null) return null;
        OrderResponse orderResponse = _mapper.Map<OrderResponse>(addedOrder);
        return PopulateProductDetailsFromMap(orderResponse, productMap);
    }

    public async Task<bool> DeleteOrder(Guid orderID)
    {
        FilterDefinition<Order> filterDefinition = Builders<Order>.Filter.Eq(o => o.OrderID, orderID);
        Order? existingOrder = await _orderRepository.GetOrderByCondition(filterDefinition);
        if (existingOrder is null) return false;
        return await _orderRepository.DeleteOrder(orderID);
    }

    public async Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order> filter)
    {
        Order? order = await _orderRepository.GetOrderByCondition(filter);
        if (order is null) return null;
        OrderResponse orderResponse = _mapper.Map<OrderResponse>(order);
        return await PopulateProductDetailsAsync(orderResponse);
    }

    public async Task<List<OrderResponse?>> GetOrders()
    {
        IEnumerable<Order> orders = await _orderRepository.GetOrders();
        var mapped = orders.Select(o => o == null ? null : _mapper.Map<OrderResponse>(o));
        return await PopulateProductDetailsListAsync(mapped);
    }

    public async Task<List<OrderResponse?>> GetOrdersByCondition(FilterDefinition<Order> filter)
    {
        IEnumerable<Order?> orders = await _orderRepository.GetOrdersByCondition(filter);
        var mapped = orders.Select(o => o == null ? null : _mapper.Map<OrderResponse>(o));
        return await PopulateProductDetailsListAsync(mapped);
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
        UserDTO? userID = await _usersMicroserviceClient.GetUserByIdAsync(orderUpdateRequest.UserID);
        if (userID == null)
        {
            throw new ValidationException($"User with ID {orderUpdateRequest.UserID} does not exist in User Table.");
        }

        Order order = _mapper.Map<Order>(orderUpdateRequest);
        var productMap = new Dictionary<Guid, ProductDTO>();
        foreach (var orderItem in order.OrderItems)
        {
            // Logic to Validate ProductID in Products microservice can be added here
            ProductDTO? product = await _productsMicroserviceClient.GetProductByIdAsync(orderItem.ProductID);
            if (product == null)
            {
                throw new ValidationException($"Product with ID {orderItem.ProductID} does not exist in Products Table.");
            }
            productMap[orderItem.ProductID] = product;
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        order.TotalBill = order.OrderItems.Sum(oi => oi.TotalPrice);
        Order? updatedOrder = await _orderRepository.UpdateOrder(order);
        if (updatedOrder is null) return null;
        
        OrderResponse orderResponse = _mapper.Map<OrderResponse>(updatedOrder);
        return PopulateProductDetailsFromMap(orderResponse, productMap);
    }

    private OrderResponse PopulateProductDetailsFromMap(OrderResponse orderResponse, Dictionary<Guid, ProductDTO> productMap)
    {
        if (orderResponse.OrderItems is null) return orderResponse;

        var updatedOrderItems = orderResponse.OrderItems.Select(item =>
            productMap.TryGetValue(item.ProductID, out var product)
                ? item with { ProductName = product.ProductName, ProductCategory = product.ProductCategory }
                : item
        ).ToList();

        return orderResponse with { OrderItems = updatedOrderItems };
    }

    private async Task<OrderResponse?> PopulateProductDetailsAsync(OrderResponse? orderResponse)
    {
        if (orderResponse is null || orderResponse.OrderItems is null) return orderResponse;

        var updatedOrderItems = new List<OrderItemResponse>();
        foreach (var item in orderResponse.OrderItems)
        {
            ProductDTO? product = await _productsMicroserviceClient.GetProductByIdAsync(item.ProductID);
            if (product is not null)
            {
                updatedOrderItems.Add(item with { ProductName = product.ProductName, ProductCategory = product.ProductCategory });
            }
            else
            {
                updatedOrderItems.Add(item);
            }
        }
        return orderResponse with { OrderItems = updatedOrderItems };
    }

    private async Task<List<OrderResponse?>> PopulateProductDetailsListAsync(IEnumerable<OrderResponse?> orderResponses)
    {
        var updatedResponses = new List<OrderResponse?>();
        foreach (var res in orderResponses)
        {
            updatedResponses.Add(await PopulateProductDetailsAsync(res));
        }
        return updatedResponses;
    }
}
