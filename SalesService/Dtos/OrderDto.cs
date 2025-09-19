using System.ComponentModel.DataAnnotations;

namespace SalesService.Dtos
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "ID do produto é obrigatório")]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantity { get; set; }
        
        [Required(ErrorMessage = "Nome do cliente é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string CustomerName { get; set; } = string.Empty;
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
        public string CustomerEmail { get; set; } = string.Empty;
    }
    
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    // Manter compatibilidade com código existente
    public class OrderDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}