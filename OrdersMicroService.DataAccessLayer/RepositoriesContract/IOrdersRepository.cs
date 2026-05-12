using MongoDB.Driver;
using OrdersMicroService.DataAccessLayer.Entities;

namespace OrdersMicroService.DataAccessLayer.RepositoriesContract;

internal interface IOrdersRepository
{
    /// <summary>
    /// Asynchronously retrieves a collection of orders.
    /// </summary>
    /// <returns>The task result contains an enumerable collection of <see cref="Order"/> objects. The collection is empty if no orders are found.</returns>
    Task<IEnumerable<Order>> GetOrders();
    /// <summary>
    /// Asynchronously retrieves all orders that match the specified filter condition.
    /// </summary>
    /// <param name="predicate">A filter definition that specifies the criteria used to select orders. Cannot be null.</param>
    /// <returns>The task result contains a collection of orders that satisfy the filter condition. The collection will be empty if no orders match.</returns>
    Task<IEnumerable<Order?>> GetOrdersByCondition(FilterDefinition<Order> predicate);
    /// <summary>
    /// Asynchronously retrieves the first order that matches the specified filter condition.
    /// </summary>
    /// <param name="predicate">A filter definition that specifies the condition used to select the order. Cannot be null.</param>
    /// <returns>The task result contains the first matching order if found; otherwise, null.</returns>
    Task<Order?> GetOrderByCondition(FilterDefinition<Order> predicate);
    /// <summary>
    /// Adds a new order to the system asynchronously.
    /// </summary>
    /// <param name="order">The order to add. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added order if the operation succeeds; otherwise, null.</returns>
    Task<Order?> AddOrder(Order order);
    /// <summary>
    /// Updates the specified order with new information asynchronously.
    /// </summary>
    /// <param name="order">The order to update. Must not be null. The order's identifier determines which order is updated.</param>
    /// <returns>The task result contains the updated order if the update is successful; otherwise, null.</returns>
    Task<Order?> UpdateOrder(Order order);
    /// <summary>
    /// Deletes the order with the specified identifier asynchronously.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order to delete.</param>
    /// <returns>The task result is <see langword="true"/> if the order was deleted successfully; otherwise, <see langword="false"/>.</returns>
    Task<bool> DeleteOrder(Guid orderId);
}
