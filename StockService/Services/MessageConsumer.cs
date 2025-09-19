using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using StockService.Models;
using Microsoft.AspNetCore.Mvc;

namespace StockService.Services
{
    public class MessageConsumer : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MessageConsumer> _logger;

        public MessageConsumer(IServiceProvider serviceProvider, ILogger<MessageConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            
            // Conectar no RabbitMQ (nosso "correio" de mensagens)
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            // Criar a fila onde vamos receber as notificações de vendas
            _channel.QueueDeclare(queue: "order_stock_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            
            // Este é o cara que fica "escutando" as mensagens
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    // Pegar a mensagem que chegou
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation("📨 Chegou uma mensagem: {Message}", message);

                    // Transformar a mensagem em dados que conseguimos usar
                    var orderData = JsonSerializer.Deserialize<JsonElement>(message);

                    var productId = orderData.GetProperty("productId").GetInt32();
                    var quantity = orderData.GetProperty("quantity").GetInt32();
                    var orderId = orderData.GetProperty("orderId").GetInt32();

                    // Atualizar o estoque usando nosso serviço
                    using var scope = _serviceProvider.CreateScope();
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                    var success = await productService.UpdateStockAsync(productId, quantity);

                    if (success)
                    {
                        _logger.LogInformation("✅ Estoque atualizado! Pedido {OrderId}, produto {ProductId}, tirei {Quantity} do estoque", 
                            orderId, productId, quantity);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Ops! Não consegui atualizar o estoque do pedido {OrderId}, produto {ProductId}, quantidade {Quantity}", 
                            orderId, productId, quantity);
                        // Aqui poderia avisar o serviço de vendas que deu problema
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 Deu ruim ao processar a mensagem!");
                }
            };

            // Começar a escutar as mensagens
            _channel.BasicConsume(queue: "order_stock_queue", autoAck: true, consumer: consumer);
            _logger.LogInformation("👂 Estou escutando as mensagens de venda...");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return Task.CompletedTask;
        }
    }
}