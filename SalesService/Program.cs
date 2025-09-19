using Microsoft.EntityFrameworkCore;
using Serilog;
using SalesService.Data;
using SalesService.Services;

// Configurar logs para acompanhar as vendas
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Aparece no terminal
    .WriteTo.File("logs/salesservice-.txt", rollingInterval: RollingInterval.Day)  // Salva em arquivo diário
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Usar logs mais bonitos com Serilog
builder.Host.UseSerilog();

// Configurar a API de vendas
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "API de Vendas", 
        Version = "v1",
        Description = "Aqui você gerencia todos os pedidos da loja"
    });
    // Documentação XML aparece no Swagger se existir
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SalesService.xml"));
});

// Configurar banco de dados dos pedidos
builder.Services.AddDbContext<SalesDbContext>(options =>
{
    // Usando banco em memória para facilitar os testes
    options.UseInMemoryDatabase("SalesDb");
    
    // Para usar SQL Server real, descomente:
    // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// HttpClient para conversar com o serviço de estoque
builder.Services.AddHttpClient<IOrderService, OrderService>();

// Registrar nosso serviço de pedidos
builder.Services.AddScoped<IOrderService, OrderService>();

// Vai rodar na porta 5000
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

// Preparar o banco de dados
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
    context.Database.EnsureCreated();  // Cria as tabelas necessárias
}

// Configurar como a API vai funcionar
if (app.Environment.IsDevelopment())
{
    // Swagger só em desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Vendas V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Log.Information("🛒 Serviço de Vendas rodando na porta 5000! Acesse http://localhost:5000/swagger");

app.Run();
