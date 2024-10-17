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
using OKR.Repository.Implementation;
using OKR.Service.Contract;
using RabbitMQ.Client;
using System.Data.Entity;
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
        //private readonly HubConnection _hubConnection;
        //private IConfiguration _config;
        private IDepartmentProgressApprovalRepository _progressApprovalRepository;
        public KeyResultsService(IKeyResultRepository keyResultRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IProgressUpdatesRepository progressUpdatesRepository, IObjectivesRepository objectivesRepository, IModel model,
            IConfiguration configuration, IDepartmentProgressApprovalRepository departmentProgressApprovalRepository)
        {
            _keyResultRepository = keyResultRepository;
            _contextAccessor = httpContextAccessor;
            _mapper = mapper;
            _progressUpdatesRepository = progressUpdatesRepository;
            _objectivesRepository = objectivesRepository;
            _channel = model;
            //_config = configuration;
            //_hubConnection = new HubConnectionBuilder()
            //    .WithUrl(_config["signalr:url"])
            //    .Build();
            //_hubConnection.StartAsync().Wait();
            _progressApprovalRepository = departmentProgressApprovalRepository;
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
                var objectives = _objectivesRepository.AsQueryable()
                    .Where(x=>x.Id == keyresult.ObjectivesId)
                    .Include(x=>x.UserObjectives).First();
                var updateString = request.Note.IsNullOrEmpty() ? GetUpdateString(request, keyresult) : request.Note;
                if(objectives.UserObjectives != null && objectives.UserObjectives.Any())
                {
                    var progressUpdates = new ProgressUpdates();
                    progressUpdates.CreatedBy = userName;
                    progressUpdates.CreatedOn = DateTime.UtcNow;
                    progressUpdates.Note = updateString;
                    progressUpdates.KeyResultId = keyresult.Id;
                    progressUpdates.OldPoint = keyresult.CurrentPoint;
                    progressUpdates.NewPoint = keyresult.CurrentPoint + request.AddedPoints;

                    keyresult.CurrentPoint = (int)(keyresult.CurrentPoint + request.AddedPoints);
                    _keyResultRepository.Edit(keyresult);
                    progressUpdates.KeyresultCompletionRate = _keyResultRepository.caculatePercentKeyResults(keyresult);
                    Dictionary<Guid, int> op = _objectivesRepository.caculatePercentObjectives(_objectivesRepository.AsQueryable().Where(x => x.Id == keyresult.ObjectivesId));
                    progressUpdates.ObjectivesCompletionRate = op.ContainsKey(keyresult.ObjectivesId) ? op[keyresult.ObjectivesId] : 0;
                    _progressUpdatesRepository.Add(progressUpdates);
                }
                else
                {
                    var departmentProgressApproval = new DepartmentProgressApproval
                    {
                        Id = Guid.NewGuid(),
                        CreatedBy = userName,
                        CreatedOn = DateTime.UtcNow,
                        KeyResultsId = keyresult.Id,
                        Note = updateString,
                        AddedPoints = (int)request.AddedPoints
                    };
                    _progressApprovalRepository.Add(departmentProgressApproval);
                }

                result.BuildResult(request);
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
