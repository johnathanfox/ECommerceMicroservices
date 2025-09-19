using SalesService.Dtos;

namespace SalesService.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
        Task<OrderResponseDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByCustomerAsync(string customerEmail);
        Task<OrderResponseDto?> UpdateOrderStatusAsync(int id, string status);
        Task<bool> ValidateStockAsync(int productId, int quantity);
    }
}
