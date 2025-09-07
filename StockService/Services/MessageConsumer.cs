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

        public MessageConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" }; // Ou o endereço do seu RabbitMQ
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "order_stock_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var orderData = JsonSerializer.Deserialize<dynamic>(message);

                // Lógica para validar e atualizar o estoque
                var product = _products.FirstOrDefault(p => p.Id == (int)orderData.GetProperty("productId").GetInt32());

                if (product != null && product.Quantity >= (int)orderData.GetProperty("quantity").GetInt32())
                {
                    product.Quantity -= (int)orderData.GetProperty("quantity").GetInt32();
                    Console.WriteLine($"Estoque do produto {product.Name} atualizado. Nova quantidade: {product.Quantity}");
                    // Aqui você persistiria a mudança no banco de dados
                }
                else
                {
                    Console.WriteLine($"Estoque insuficiente ou produto não encontrado para o pedido.");
                    // Lógica para notificar o SalesService sobre a falha (opcional, mas importante em um sistema real)
                }
            };

            _channel.BasicConsume(queue: "order_stock_queue", autoAck: true, consumer: consumer);

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