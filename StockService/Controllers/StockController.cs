using Microsoft.AspNetCore.Mvc;
using StockService.Models;
using System.Collections.Generic;
using System.Linq;

namespace StockService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private static List<Product> products = new List<Product>
        {
            new Product { Id = 1, Name = "Product A", Quantity = 100 },
            new Product { Id = 2, Name = "Product B", Quantity = 200 }
        };

        [HttpGe("{productId}")]

        public IActionResult GetStock(int productId)
        {
            var product = products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }

}