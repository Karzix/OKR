using ConsumerWeightUpdate.Repository;
using Microsoft.Extensions.Options;
using OKR.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OKR.Models.Entity;
using Microsoft.AspNetCore.SignalR.Client;

namespace ConsumerWeightUpdate
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly HubConnection _hubConnection;
        private IConfiguration _config;

        public RabbitMQConsumer(IOptions<RabbitMQSettings> rabbitMQSettings, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _rabbitMQSettings = rabbitMQSettings.Value;
            _serviceScopeFactory = serviceScopeFactory;
            _config = configuration;

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
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_config["signalr:url"])
                .Build();
            _hubConnection.StartAsync().Wait();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received: {message}");
                var weightUpdate = JsonConvert.DeserializeObject<MessageWeightUpdate>(message);
                // Tạo một scope mới để sử dụng các scoped services
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var keyResultRepository = scope.ServiceProvider.GetRequiredService<IKeyResultRepository>();
                    var objectivesRepository = scope.ServiceProvider.GetRequiredService<IObjectivesRepository>();
                    var progressUpdatesRepository = scope.ServiceProvider.GetRequiredService<IProgressUpdatesRepository>();

                    // Xử lý message với các repository
                    try
                    {
                        SaveWeightUpdate(keyResultRepository, objectivesRepository, progressUpdatesRepository, weightUpdate);
                        await _hubConnection.InvokeAsync("ReceiveMessageWeightUpdate", weightUpdate.ConnectionId ,"OK");
                    }
                    catch (Exception ex)
                    {
                        await _hubConnection.InvokeAsync("ReceiveMessageWeightUpdate", weightUpdate.ConnectionId, ex.Message + " -> " + ex.Message);
                    }
                    
                }
            };

            _channel.BasicConsume(queue: RabbitMQQueue.QueueWeightUpdate,
                                 autoAck: true,
                                 consumer: consumer);

            return Task.CompletedTask;
        }

        private async void SaveWeightUpdate(
            IKeyResultRepository _keyResultRepository,
            IObjectivesRepository _objectivesRepository,
            IProgressUpdatesRepository _progressUpdatesRepository,
            MessageWeightUpdate message)
        {
            //var test = _objectivesRepository.AsQueryable().First();
            //Console.WriteLine(test.ToString());

            var keyresult = _keyResultRepository.AsQueryable().Where(x=>x.Id == message.KeyresultId).First();
            var progressUpdates = new ProgressUpdates();
            progressUpdates.CreatedBy = message.CreateBy;
            progressUpdates.CreatedOn = DateTime.UtcNow;
            progressUpdates.Note = message.Note;
            progressUpdates.KeyResultId = keyresult.Id;
            progressUpdates.OldPoint = keyresult.CurrentPoint;
            progressUpdates.NewPoint = keyresult.CurrentPoint + message.AddedPoints;

            keyresult.CurrentPoint = (int)(keyresult.CurrentPoint + message.AddedPoints);
            _keyResultRepository.Edit(keyresult);
            progressUpdates.KeyresultCompletionRate = _keyResultRepository.caculatePercentKeyResults(keyresult);
            Dictionary<Guid, int> op = _objectivesRepository.caculatePercentObjectives(_objectivesRepository.AsQueryable().Where(x => x.Id == keyresult.ObjectivesId));
            progressUpdates.ObjectivesCompletionRate = op.ContainsKey(keyresult.ObjectivesId) ? op[keyresult.ObjectivesId] : 0;
            _progressUpdatesRepository.Add(progressUpdates);
        }
    }
}
