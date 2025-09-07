// Imports
// ...
using SalesService.Services;
using SalesService.Models;
using SalesService.Dtos;
using Microsoft.AspNetCore.Mvc;
namespace SalesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private static readonly List<Order> _orders = new List<Order>();
        private static int _nextOrderId = 1;

        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderDto orderDto)
        {
         
            var newOrder = new Order
            {
                Id = _nextOrderId++,
                ProductId = orderDto.ProductId,
                Quantity = orderDto.Quantity,
                OrderDate = System.DateTime.Now,
                Status = "Processing" // O status inicial agora é "Processing"
            };

            _orders.Add(newOrder);

            // Envie a mensagem para o RabbitMQ
            using (var producer = new MessageProducer())
            {
                producer.SendMessage(new { orderId = newOrder.Id, productId = orderDto.ProductId, quantity = orderDto.Quantity });
            }

            return Ok(new { message = "Order created and is being processed.", order = newOrder });
        }
    }
}