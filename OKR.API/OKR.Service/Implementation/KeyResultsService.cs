using AutoMapper;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OKR.DTO;
using OKR.Infrastructure;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace OKR.Service.Implementation
{
    public class KeyResultsService : IKeyResultsService
    {
        private IKeyResultRepository _keyResultRepository;
        private IHttpContextAccessor _contextAccessor;
        private IMapper _mapper;
        private IProgressUpdatesRepository _progressUpdatesRepository;
        private IObjectivesRepository _objectivesRepository;
        private readonly IModel _channel;
        private readonly HubConnection _hubConnection;
        private IConfiguration _config;
        public KeyResultsService(IKeyResultRepository keyResultRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IProgressUpdatesRepository progressUpdatesRepository, IObjectivesRepository objectivesRepository, IModel model,
            IConfiguration configuration)
        {
            _keyResultRepository = keyResultRepository;
            _contextAccessor = httpContextAccessor;
            _mapper = mapper;
            _progressUpdatesRepository = progressUpdatesRepository;
            _objectivesRepository = objectivesRepository;
            _channel = model;
            _config = configuration;
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_config["signalr:url"])
                .Build();
            _hubConnection.StartAsync().Wait();
        }

        public async Task<AppResponse<KeyResultDto>> Update(KeyResultDto request)
        {
            var result = new AppResponse<KeyResultDto>();
            try
            {   
                
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                var keyresult = _keyResultRepository.Get(request.Id.Value);
                if (request.CurrentPoint == null || request.CurrentPoint > keyresult.MaximunPoint)
                {
                    return result.BuildError("current point is invalid");
                }
                var progressUpdates = new ProgressUpdates();
                var weightUpdate = new MessageWeightUpdate();
                var updateString = request.Note.IsNullOrEmpty() ? GetUpdateString(request, keyresult) : request.Note;
            
                weightUpdate.AddedPoints = request.AddedPoints;
                weightUpdate.Note = updateString;
                weightUpdate.CreateBy = userName;
                weightUpdate.KeyresultId = request.Id.Value;
                weightUpdate.ConnectionId = Guid.NewGuid().ToString();
                var message = JsonSerializer.Serialize(weightUpdate);
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(exchange: "",
                                      routingKey: RabbitMQQueue.QueueWeightUpdate,
                                      basicProperties: null,
                                      body: body);

                string respone = "";
                var signalRTaskCompletionSource = new TaskCompletionSource<string>();
                _hubConnection.On<string>(SignalRMessage.WeightUpdate + weightUpdate.ConnectionId, (receivedMessage) =>
                {
                    respone = receivedMessage;
                    signalRTaskCompletionSource.SetResult(receivedMessage);
                });
                await signalRTaskCompletionSource.Task;
                if( respone == "OK")
                {
                    result.BuildResult(request);
                }
                else
                {
                    result.BuildError(respone);
                }

                //result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private string GetUpdateString(KeyResultDto NewKeyResult, KeyResults CurKeyResults)
        {
            string content = _contextAccessor.HttpContext.User.Identity.Name + " ";
            if(NewKeyResult.Description != CurKeyResults.Description)
            {
                content += "update keyresults name from " + CurKeyResults.Description + " to " + NewKeyResult.Description + "; ";
            }
            if ((CurKeyResults.CurrentPoint + NewKeyResult.AddedPoints) != CurKeyResults.CurrentPoint)
            {
                content += "update weights " + NewKeyResult.Description +" from " + CurKeyResults.CurrentPoint + " to " + (CurKeyResults.CurrentPoint + NewKeyResult.AddedPoints)+ "; ";
            }
            if(NewKeyResult.Deadline != CurKeyResults.Deadline)
            {
                content += "update deadline from " + CurKeyResults.Deadline.ToString("dd/MM/yyyy") + " to " + NewKeyResult.Deadline.Value.ToString("dd/MM/yyyy") + "; ";
            }
            return content;
        }
    }
}
