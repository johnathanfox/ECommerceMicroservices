using Microsoft.AspNetCore.Mvc;
using SalesService.Dtos;
using SalesService.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SalesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private static readonly List<Order> _orders = new List<Order>();
        private static int _nextOrderId = 1;

        public OrdersController()
        {
            _httpClient = new HttpClient();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            var stockServiceUrl = "http://localhost:5001/api/stock";
            var response = await _httpClient.GetAsync($"{stockServiceUrl}/{orderDto.ProductId}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Product not found or stock service unavailable.");
            }

            var stockResponse = await response.Content.ReadAsStringAsync();
            dynamic stockData = JsonConvert.DeserializeObject(stockResponse);

            if (stockData.quantity < orderDto.Quantity)
            {
                return BadRequest("Insufficient stock.");
            }
            
            var newOrder = new Order
            {
                Id = _nextOrderId++,
                ProductId = orderDto.ProductId,
                Quantity = orderDto.Quantity,
                OrderDate = System.DateTime.Now,
                Status = "Completed"
            };

            _orders.Add(newOrder);

            return Ok(newOrder);
        }
    }
}