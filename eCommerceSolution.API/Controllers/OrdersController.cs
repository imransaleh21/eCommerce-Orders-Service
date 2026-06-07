using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using OrdersMicroService.BusinessLogicLayer.ServicesContract;
using OrdersMicroService.DataAccessLayer.Entities;

namespace eCommerceSolution.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        private readonly IValidator<OrderAddRequest> _orderAddValidator;
        private readonly IValidator<OrderUpdateRequest> _orderUpdateValidator;
        public OrdersController(IOrdersService ordersService,
            IValidator<OrderAddRequest> orderAddRequestValidator, IValidator<OrderUpdateRequest> orderUpdateRequestValidator)
        {
            _ordersService = ordersService;
            _orderAddValidator = orderAddRequestValidator;
            _orderUpdateValidator = orderUpdateRequestValidator;
        }

        [HttpGet]
        public async Task<IEnumerable<OrderResponse?>> Get()
        {
            List<OrderResponse?> orders = await _ordersService.GetOrders();
            return orders;
        }

        [HttpGet("/search/productid/{productId}")]
        public async Task<IEnumerable<OrderResponse?>> GetOrdersByProductId(Guid productId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.ElemMatch(o => o.OrderItems,
                Builders<OrderItem>.Filter.Eq(oi => oi.ProductID, productId));

            List<OrderResponse?> orders = await _ordersService.GetOrdersByCondition(filter);
            return orders;
        }
        [HttpGet("/search/orderid/{orderId}")]
        public async Task<IEnumerable<OrderResponse?>> GetOrdersByOrderId(Guid orderId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(o => o.OrderID, orderId);
            List<OrderResponse?> orders = await _ordersService.GetOrdersByCondition(filter);
            return orders;
        }
        [HttpGet("/search/orderdate/{orderDate}")]
        public async Task<IEnumerable<OrderResponse?>> GetOrdersByOrderDate(DateTime orderDate)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(o => o.OrderDate.ToString("yyyy-MM-dd"), orderDate.ToString("yyyy-MM-dd"));
            List<OrderResponse?> orders = await _ordersService.GetOrdersByCondition(filter);
            return orders;
        }
        [HttpGet("/search/userid/{userId}")]
        public async Task<IEnumerable<OrderResponse?>> GetOrdersByUserId(Guid userId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(o => o.UserID, userId);
            List<OrderResponse?> orders = await _ordersService.GetOrdersByCondition(filter);
            return orders;
        }
        [HttpPost]
        public async Task<ActionResult<OrderResponse?>> Post(OrderAddRequest orderAddRequest)
        {
            if (orderAddRequest == null) throw new ArgumentNullException(nameof(orderAddRequest));
            ValidationResult validationResult = await _orderAddValidator.ValidateAsync(orderAddRequest);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }
            try
            {
                OrderResponse? addedOrder = await _ordersService.AddOrder(orderAddRequest);
                if (addedOrder is null) return BadRequest("Failed to add order.");
                return CreatedAtAction(nameof(GetOrdersByOrderId), new { orderId = addedOrder.OrderID }, addedOrder);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{orderId}")]
        public async Task<ActionResult<OrderResponse?>> Put(Guid orderId, OrderUpdateRequest orderUpdateRequest)
        {
            if (orderUpdateRequest == null) throw new ArgumentNullException(nameof(orderUpdateRequest));
            if(orderId != orderUpdateRequest.OrderID) return BadRequest("Order ID in URL does not match Order ID in request body.");

            ValidationResult validationResult = await _orderUpdateValidator.ValidateAsync(orderUpdateRequest);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }
            try
            {
                OrderResponse? updatedOrder = await _ordersService.UpdateOrder(orderUpdateRequest);
                if (updatedOrder is null) return BadRequest("Failed to update order.");
                return Ok(updatedOrder);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{orderId}")]
        public async Task<ActionResult> Delete(Guid orderId)
        {
            if(orderId == Guid.Empty) return BadRequest("Order ID can't be empty.");
            try
            {
                bool deleted = await _ordersService.DeleteOrder(orderId);
                if (!deleted) return BadRequest("Failed to delete order.");
                return Ok();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
