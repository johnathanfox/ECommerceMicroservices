using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StockService.Data;
using StockService.Dtos;
using StockService.Services;
using Xunit;

namespace StockService.Tests
{
    public class ProductServiceTests : IDisposable
    {
        private readonly StockDbContext _context;
        private readonly ProductService _productService;
        private readonly Mock<ILogger<ProductService>> _mockLogger;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<StockDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new StockDbContext(options);
            _mockLogger = new Mock<ILogger<ProductService>>();
            _productService = new ProductService(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldCreateProduct_WhenValidData()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Name = "Produto Teste",
                Description = "Descrição do produto teste",
                Price = 99.99m,
                Quantity = 10
            };

            // Act
            var result = await _productService.CreateProductAsync(createProductDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createProductDto.Name, result.Name);
            Assert.Equal(createProductDto.Description, result.Description);
            Assert.Equal(createProductDto.Price, result.Price);
            Assert.Equal(createProductDto.Quantity, result.Quantity);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Name = "Produto Teste",
                Description = "Descrição do produto teste",
                Price = 99.99m,
                Quantity = 10
            };

            var createdProduct = await _productService.CreateProductAsync(createProductDto);

            // Act
            var result = await _productService.GetProductByIdAsync(createdProduct.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdProduct.Id, result.Id);
            Assert.Equal(createdProduct.Name, result.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Act
            var result = await _productService.GetProductByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateStockAsync_ShouldUpdateStock_WhenSufficientQuantity()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Name = "Produto Teste",
                Description = "Descrição do produto teste",
                Price = 99.99m,
                Quantity = 10
            };

            var createdProduct = await _productService.CreateProductAsync(createProductDto);

            // Act
            var result = await _productService.UpdateStockAsync(createdProduct.Id, 5);

            // Assert
            Assert.True(result);
            
            var updatedProduct = await _productService.GetProductByIdAsync(createdProduct.Id);
            Assert.Equal(5, updatedProduct?.Quantity);
        }

        [Fact]
        public async Task UpdateStockAsync_ShouldReturnFalse_WhenInsufficientStock()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Name = "Produto Teste",
                Description = "Descrição do produto teste",
                Price = 99.99m,
                Quantity = 5
            };

            var createdProduct = await _productService.CreateProductAsync(createProductDto);

            // Act
            var result = await _productService.UpdateStockAsync(createdProduct.Id, 10);

            // Assert
            Assert.False(result);
            
            var product = await _productService.GetProductByIdAsync(createdProduct.Id);
            Assert.Equal(5, product?.Quantity); // Quantidade não deve ter mudado
        }

        [Fact]
        public async Task CheckStockAvailabilityAsync_ShouldReturnTrue_WhenStockAvailable()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Name = "Produto Teste",
                Description = "Descrição do produto teste",
                Price = 99.99m,
                Quantity = 10
            };

            var createdProduct = await _productService.CreateProductAsync(createProductDto);

            // Act
            var result = await _productService.CheckStockAvailabilityAsync(createdProduct.Id, 5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckStockAvailabilityAsync_ShouldReturnFalse_WhenStockInsufficient()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Name = "Produto Teste",
                Description = "Descrição do produto teste",
                Price = 99.99m,
                Quantity = 5
            };

            var createdProduct = await _productService.CreateProductAsync(createProductDto);

            // Act
            var result = await _productService.CheckStockAvailabilityAsync(createdProduct.Id, 10);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
