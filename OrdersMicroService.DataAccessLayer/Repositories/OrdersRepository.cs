using MongoDB.Driver;
using OrdersMicroService.DataAccessLayer.Entities;
using OrdersMicroService.DataAccessLayer.RepositoriesContract;

namespace OrdersMicroService.DataAccessLayer.Repositories;

public class OrdersRepository : IOrdersRepository
{
    private readonly IMongoCollection<Order> _orders;
    private readonly string collateralName = "Orders";
    public OrdersRepository(IMongoDatabase database)
    {
        _orders = database.GetCollection<Order>(collateralName);
    }
     public async Task<Order?> AddOrder(Order order)
    {
        await _orders.InsertOneAsync(order);
        return order;
    }

    public async Task<bool> DeleteOrder(Guid orderId)
    {
        var result = await _orders.DeleteOneAsync(o => o.OrderID == orderId);
        return result.DeletedCount > 0;
    }

    public async Task<Order?> GetOrderByCondition(FilterDefinition<Order> predicate)
    {
        return await _orders.Find(predicate).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetOrders()
    {
        return await _orders.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Order?>> GetOrdersByCondition(FilterDefinition<Order> predicate)
    {
        return await _orders.Find(predicate).ToListAsync();
    }

    public async Task<Order?> UpdateOrder(Order order)
    {
        var result = await _orders.ReplaceOneAsync(o => o.OrderID == order.OrderID, order);
        return result.ModifiedCount > 0 ? order : null;
    }
}
