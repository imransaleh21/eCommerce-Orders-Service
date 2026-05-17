using MongoDB.Driver;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using OrdersMicroService.DataAccessLayer.Entities;

namespace OrdersMicroService.BusinessLogicLayer.ServicesContract;
public interface IOrdersService
{
    Task<List<OrderResponse?>> GetOrders();
    Task<List<OrderResponse?>> GetOrdersByCondition(FilterDefinition<Order> filter);
    Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order> filter);
    Task<OrderResponse?> AddOrder(OrderAddRequest orderAddRequest);
    Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest);
    Task<bool> DeleteOrder(Guid orderID);

}
