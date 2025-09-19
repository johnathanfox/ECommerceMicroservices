using Microsoft.AspNetCore.Mvc;
using StockService.Dtos;
using StockService.Services;

namespace StockService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<StockController> _logger;

        public StockController(IProductService productService, ILogger<StockController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os produtos do catálogo
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os produtos");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém um produto específico por ID
        /// </summary>
        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int productId)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    return NotFound(new { message = $"Produto com ID {productId} não encontrado" });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produto {ProductId}", productId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Cadastra um novo produto
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction(nameof(GetProduct), new { productId = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar produto");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualiza um produto existente
        /// </summary>
        [HttpPut("{productId}")]
        public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int productId, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = await _productService.UpdateProductAsync(productId, updateProductDto);
                if (product == null)
                {
                    return NotFound(new { message = $"Produto com ID {productId} não encontrado" });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar produto {ProductId}", productId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Remove um produto
        /// </summary>
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                var success = await _productService.DeleteProductAsync(productId);
                if (!success)
                {
                    return NotFound(new { message = $"Produto com ID {productId} não encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover produto {ProductId}", productId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Verifica disponibilidade de estoque
        /// </summary>
        [HttpGet("{productId}/availability")]
        public async Task<ActionResult<object>> CheckStockAvailability(int productId, [FromQuery] int quantity = 1)
        {
            try
            {
                var isAvailable = await _productService.CheckStockAvailabilityAsync(productId, quantity);
                var product = await _productService.GetProductByIdAsync(productId);
                
                if (product == null)
                {
                    return NotFound(new { message = $"Produto com ID {productId} não encontrado" });
                }

                return Ok(new 
                { 
                    productId = productId,
                    productName = product.Name,
                    requestedQuantity = quantity,
                    availableQuantity = product.Quantity,
                    isAvailable = isAvailable
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar disponibilidade do produto {ProductId}", productId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}