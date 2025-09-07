// This file defines the data transfer object for the login request.
namespace ApiGateway.Dtos
{
    public class LoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
