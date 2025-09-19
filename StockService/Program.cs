using Microsoft.EntityFrameworkCore;
using Serilog;
using StockService.Data;
using StockService.Services;

// Vamos configurar os logs para acompanhar o que está acontecendo
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Mostra no terminal
    .WriteTo.File("logs/stockservice-.txt", rollingInterval: RollingInterval.Day)  // Salva em arquivo
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Usar o Serilog para logs mais bonitos
builder.Host.UseSerilog();

// Configurar os serviços básicos da API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "API do Estoque", 
        Version = "v1",
        Description = "Aqui você gerencia todos os produtos da loja"
    });
    // Se tiver documentação XML, vai aparecer no Swagger
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "StockService.xml"));
});

// Configurar o banco de dados
builder.Services.AddDbContext<StockDbContext>(options =>
{
    // Por enquanto usando banco em memória (mais fácil para testar)
    options.UseInMemoryDatabase("StockDb");
    
    // Para usar SQL Server de verdade, descomente a linha abaixo:
    // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Registrar nossos serviços customizados
builder.Services.AddScoped<IProductService, ProductService>();

// Este cara fica escutando as mensagens do RabbitMQ
builder.Services.AddHostedService<MessageConsumer>();

// Definir que vai rodar na porta 5001
builder.WebHost.UseUrls("http://localhost:5001");

var app = builder.Build();

// Criar o banco de dados se não existir
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StockDbContext>();
    context.Database.EnsureCreated();  // Cria as tabelas e popula com dados iniciais
}

// Configurar o pipeline de requisições
if (app.Environment.IsDevelopment())
{
    // Só mostrar o Swagger em desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API do Estoque V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Log.Information("🚀 Serviço de Estoque rodando na porta 5001! Acesse http://localhost:5001/swagger");

app.Run();