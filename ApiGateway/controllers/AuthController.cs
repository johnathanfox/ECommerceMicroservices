using Microsoft.AspNetCore.Mvc;
using ApiGateway.Dtos;
using ApiGateway.Services; // Adicionado para usar o JwtHelper

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (loginDto.Username == "user" && loginDto.Password == "password")
            {
                var token = JwtHelper.GenerateToken(_config);
                return Ok(new { token });
            }

            return Unauthorized();
        }
    }
}
