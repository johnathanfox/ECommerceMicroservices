using Microsoft.EntityFrameworkCore;
using StockService.Data;
using StockService.Dtos;
using StockService.Models;

namespace StockService.Services
{
    public class ProductService : IProductService
    {
        private readonly StockDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(StockDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                return products.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os produtos");
                throw;
            }
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                return product != null ? MapToResponseDto(product) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produto com ID {ProductId}", id);
                throw;
            }
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            try
            {
                var product = new Product
                {
                    Name = createProductDto.Name,
                    Description = createProductDto.Description,
                    Price = createProductDto.Price,
                    Quantity = createProductDto.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Produto criado com sucesso: {ProductName} (ID: {ProductId})", 
                    product.Name, product.Id);

                return MapToResponseDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar produto: {ProductName}", createProductDto.Name);
                throw;
            }
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return null;

                if (!string.IsNullOrEmpty(updateProductDto.Name))
                    product.Name = updateProductDto.Name;

                if (!string.IsNullOrEmpty(updateProductDto.Description))
                    product.Description = updateProductDto.Description;

                if (updateProductDto.Price.HasValue)
                    product.Price = updateProductDto.Price.Value;

                if (updateProductDto.Quantity.HasValue)
                    product.Quantity = updateProductDto.Quantity.Value;

                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Produto atualizado com sucesso: {ProductName} (ID: {ProductId})", 
                    product.Name, product.Id);

                return MapToResponseDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar produto com ID {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return false;

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Produto removido com sucesso: {ProductName} (ID: {ProductId})", 
                    product.Name, product.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover produto com ID {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Tentativa de atualizar estoque de produto inexistente: {ProductId}", productId);
                    return false;
                }

                if (product.Quantity < quantity)
                {
                    _logger.LogWarning("Estoque insuficiente para produto {ProductId}. Disponível: {Available}, Solicitado: {Requested}", 
                        productId, product.Quantity, quantity);
                    return false;
                }

                product.Quantity -= quantity;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Estoque atualizado para produto {ProductId}. Nova quantidade: {NewQuantity}", 
                    productId, product.Quantity);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar estoque do produto {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> CheckStockAvailabilityAsync(int productId, int requiredQuantity)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Produto não encontrado para verificação de estoque: {ProductId}", productId);
                    return false;
                }

                return product.Quantity >= requiredQuantity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar disponibilidade de estoque para produto {ProductId}", productId);
                throw;
            }
        }

        private static ProductResponseDto MapToResponseDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
