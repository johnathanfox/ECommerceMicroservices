using Microsoft.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
var key = Enconding.ASCII.GetBytes("my_secret_key_12345"); // Chave secreta para assinar o token JWT

builder.Services.AddAuthentication(x =>
{ 
    x.DefaultScheme
}


)