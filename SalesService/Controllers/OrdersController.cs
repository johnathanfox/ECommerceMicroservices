using Microsoft.AspNetCore.Mvc;
using SalesService.Dtos;
using SalesService.Services;

namespace SalesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Criar um novo pedido na loja
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                // Verificar se os dados estão corretos
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _orderService.CreateOrderAsync(createOrderDto);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                // Algo deu errado na validação (ex: sem estoque)
                _logger.LogWarning(ex, "Ops! Problema na validação do pedido");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eita! Erro inesperado ao criar pedido");
                return StatusCode(500, new { message = "Algo deu errado aqui, tenta de novo!" });
            }
        }

        /// <summary>
        /// Ver todos os pedidos da loja
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problema ao buscar os pedidos");
                return StatusCode(500, new { message = "Não consegui pegar os pedidos, tenta de novo!" });
            }
        }

        /// <summary>
        /// Buscar um pedido específico pelo número
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound(new { message = $"Não achei nenhum pedido com o número {id}" });
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao procurar pedido {OrderId}", id);
                return StatusCode(500, new { message = "Deu pau na busca, tenta de novo!" });
            }
        }

        /// <summary>
        /// Obtém pedidos de um cliente específico
        /// </summary>
        [HttpGet("customer/{email}")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrdersByCustomer(string email)
        {
            try
            {
                var orders = await _orderService.GetOrdersByCustomerAsync(email);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos do cliente {CustomerEmail}", email);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualiza o status de um pedido
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<OrderResponseDto>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _orderService.UpdateOrderStatusAsync(id, statusDto.Status);
                if (order == null)
                {
                    return NotFound(new { message = $"Pedido com ID {id} não encontrado" });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do pedido {OrderId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        // Manter compatibilidade com código existente
        [HttpPost("legacy")]
        public async Task<IActionResult> CreateOrderLegacy([FromBody] OrderDto orderDto)
        {
            try
            {
                var createOrderDto = new CreateOrderDto
                {
                    ProductId = orderDto.ProductId,
                    Quantity = orderDto.Quantity,
                    CustomerName = "Cliente Padrão",
                    CustomerEmail = "cliente@exemplo.com"
                };

                var order = await _orderService.CreateOrderAsync(createOrderDto);
                return Ok(new { message = "Order created and is being processed.", order = order });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido legacy");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}