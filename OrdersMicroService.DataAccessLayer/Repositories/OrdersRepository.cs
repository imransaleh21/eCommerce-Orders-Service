using MongoDB.Driver;
using OrdersMicroService.DataAccessLayer.Entities;
using OrdersMicroService.DataAccessLayer.RepositoriesContract;

namespace OrdersMicroService.DataAccessLayer.Repositories;

public class OrdersRepository : IOrdersRepository
{
    private readonly IMongoCollection<Order> _orders;
    private readonly string collateralName = "orders";
    public OrdersRepository(IMongoDatabase database)
    {
        _orders = database.GetCollection<Order>(collateralName);
    }
     public async Task<Order?> AddOrder(Order order)
    {
        order.OrderID = Guid.NewGuid();
        order._id = order.OrderID;

        foreach (var orderItem in order.OrderItems)
        {
            orderItem._id = Guid.NewGuid();   
        }

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
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(o => o.OrderID, order.OrderID);
        Order? existingOrder = (await _orders.FindAsync(filter)).FirstOrDefault();
        if(existingOrder == null) return null; 

        order._id = existingOrder._id;
        var result = await _orders.ReplaceOneAsync(filter, order);
        return result.ModifiedCount > 0 ? order : null;
    }
}
