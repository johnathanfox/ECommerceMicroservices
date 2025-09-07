// Este serviço é responsável por publicar mensagens para o RabbitMQ.
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace SalesService.Services
{
    // A classe agora implementa IDisposable para permitir o uso em uma instrução 'using'.
    public class MessageProducer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageProducer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _channel.QueueDeclare(queue: "order_stock_queue",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
        }

        public void SendMessage<T>(T message)
        {
            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);
            
            _channel.BasicPublish(exchange: "",
                                routingKey: "order_stock_queue",
                                basicProperties: null,
                                body: body);
        }

        // Implementação do método Dispose para liberar os recursos do RabbitMQ.
        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
