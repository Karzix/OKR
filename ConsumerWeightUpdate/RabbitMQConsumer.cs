using ConsumerWeightUpdate.Repository;
using Microsoft.Extensions.Options;
using OKR.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ConsumerWeightUpdate
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQConsumer(IOptions<RabbitMQSettings> rabbitMQSettings, IServiceScopeFactory serviceScopeFactory)
        {
            _rabbitMQSettings = rabbitMQSettings.Value;
            _serviceScopeFactory = serviceScopeFactory;

            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMQSettings.HostName,
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password,
                Port = _rabbitMQSettings.Port
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: RabbitMQQueue.QueueWeightUpdate,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received: {message}");

                // Tạo một scope mới để sử dụng các scoped services
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var keyResultRepository = scope.ServiceProvider.GetRequiredService<IKeyResultRepository>();
                    var objectivesRepository = scope.ServiceProvider.GetRequiredService<IObjectivesRepository>();
                    var progressUpdatesRepository = scope.ServiceProvider.GetRequiredService<IProgressUpdatesRepository>();

                    // Xử lý message với các repository
                    SaveWeightUpdate(keyResultRepository, objectivesRepository, progressUpdatesRepository, message);
                }
            };

            _channel.BasicConsume(queue: RabbitMQQueue.QueueWeightUpdate,
                                 autoAck: true,
                                 consumer: consumer);

            return Task.CompletedTask;
        }

        private void SaveWeightUpdate(
            IKeyResultRepository _keyResultRepository,
            IObjectivesRepository _objectivesRepository,
            IProgressUpdatesRepository _progressUpdatesRepository,
            string message)
        {
            var test = _objectivesRepository.AsQueryable().First();
            Console.WriteLine(test.ToString());
        }
    }
}
