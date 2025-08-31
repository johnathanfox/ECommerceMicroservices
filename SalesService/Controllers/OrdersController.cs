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
        private static List<Order> orders = new List<Order>();
        private static int nextOrderId = 1;
        public OrdersController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [httpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            // Checa o estoque no StockService
            var stockServiceUrl = "http://localhost:5001/api/stock/"; 
            var response = await _httpClient.GetAsync($"{stockServiceUrl}/{orderDto.ProductId}");

            if (!stockResponse.IsSuccessStatusCode)
            {
                return BadRequest("Produto não encontrado no estoque ou serviço de estoque indisponivel.");
            }

            var stockContent = await stockResponse.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject(stockResponse);

            if (product.Quantity < orderDto.Quantity)
            {
                return BadRequest("Estoque insuficiente.");
            }

            // Cria a ordem 
            var order = new Order
            {
                Id = _nextOrderId++,
                ProductId = orderDto.ProductId,
                Quantity = orderDto.Quantity,
                OrderDate = System.DateTime.Now,
                Status = "Confirmado"
            };

            _orders.Add(newOrder);


            return Ok(newOrder);
        }
    }
}