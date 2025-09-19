using System;
using System.ComponentModel.DataAnnotations;

namespace SalesService.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantity { get; set; }
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public enum OrderStatus
    {
        Pending,
        Processing,
        Confirmed,
        Cancelled,
        Completed
    }
}