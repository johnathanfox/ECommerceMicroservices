using Microsoft.EntityFrameworkCore;
using SalesService.Data;
using SalesService.Dtos;
using SalesService.Models;
using System.Text.Json;

namespace SalesService.Services
{
    public class OrderService : IOrderService
    {
        private readonly SalesDbContext _context;
        private readonly ILogger<OrderService> _logger;
        private readonly HttpClient _httpClient;

        public OrderService(SalesDbContext context, ILogger<OrderService> logger, HttpClient httpClient)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            try
            {
                // Validar estoque antes de criar o pedido
                var stockAvailable = await ValidateStockAsync(createOrderDto.ProductId, createOrderDto.Quantity);
                if (!stockAvailable)
                {
                    throw new InvalidOperationException("Estoque insuficiente para o produto solicitado");
                }

                // Buscar informações do produto para calcular preço
                var productInfo = await GetProductInfoAsync(createOrderDto.ProductId);
                if (productInfo == null)
                {
                    throw new InvalidOperationException("Produto não encontrado");
                }

                var order = new Order
                {
                    ProductId = createOrderDto.ProductId,
                    Quantity = createOrderDto.Quantity,
                    CustomerName = createOrderDto.CustomerName,
                    CustomerEmail = createOrderDto.CustomerEmail,
                    UnitPrice = productInfo.Price,
                    TotalPrice = productInfo.Price * createOrderDto.Quantity,
                    OrderDate = DateTime.UtcNow,
                    Status = "Processing",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Enviar mensagem para o RabbitMQ
                using (var producer = new MessageProducer())
                {
                    producer.SendMessage(new { 
                        orderId = order.Id, 
                        productId = order.ProductId, 
                        quantity = order.Quantity 
                    });
                }

                _logger.LogInformation("Pedido criado com sucesso: {OrderId} para cliente {CustomerName}", 
                    order.Id, order.CustomerName);

                return MapToResponseDto(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido para produto {ProductId}", createOrderDto.ProductId);
                throw;
            }
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _context.Orders
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
                
                return orders.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os pedidos");
                throw;
            }
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                return order != null ? MapToResponseDto(order) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido {OrderId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByCustomerAsync(string customerEmail)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.CustomerEmail == customerEmail)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
                
                return orders.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos do cliente {CustomerEmail}", customerEmail);
                throw;
            }
        }

        public async Task<OrderResponseDto?> UpdateOrderStatusAsync(int id, string status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                    return null;

                order.Status = status;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Status do pedido {OrderId} atualizado para {Status}", id, status);

                return MapToResponseDto(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do pedido {OrderId}", id);
                throw;
            }
        }

        public async Task<bool> ValidateStockAsync(int productId, int quantity)
        {
            try
            {
                // Chamar o StockService para verificar disponibilidade
                var response = await _httpClient.GetAsync($"http://localhost:5001/api/stock/{productId}/availability?quantity={quantity}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var stockInfo = JsonSerializer.Deserialize<JsonElement>(content);
                    return stockInfo.GetProperty("isAvailable").GetBoolean();
                }

                _logger.LogWarning("Falha ao validar estoque para produto {ProductId}. Status: {StatusCode}", 
                    productId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar estoque para produto {ProductId}", productId);
                return false;
            }
        }

        private async Task<ProductInfo?> GetProductInfoAsync(int productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5001/api/stock/{productId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var productData = JsonSerializer.Deserialize<JsonElement>(content);
                    
                    return new ProductInfo
                    {
                        Id = productData.GetProperty("id").GetInt32(),
                        Name = productData.GetProperty("name").GetString() ?? "",
                        Price = productData.GetProperty("price").GetDecimal()
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar informações do produto {ProductId}", productId);
                return null;
            }
        }

        private static OrderResponseDto MapToResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                UnitPrice = order.UnitPrice,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };
        }

        private class ProductInfo
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
        }
    }
}
