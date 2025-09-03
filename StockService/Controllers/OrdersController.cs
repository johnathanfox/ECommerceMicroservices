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
    public class OrdesController : ControllerBase
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
            // validação de estoque chamando o stockservice 
            var stockServiceUrl = "http://localhost:5001/api/stock/";
            var response = await _httpClient.GetAsync($"{stockServiceUrl}/{orderDto.ProductId}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Produto não encontrado no estoque.");
            }
        }
    }
}