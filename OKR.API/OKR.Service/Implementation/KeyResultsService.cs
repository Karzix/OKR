using AutoMapper;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OKR.DTO;
using OKR.Infrastructure.Enum;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;
using System.Data.Entity;
using System.Security.Claims;
using static OKR.Infrastructure.Enum.helperQuarter;

namespace OKR.Service.Implementation
{
    public class KeyResultsService : IKeyResultsService
    {
        private IKeyResultRepository _keyResultRepository;
        private IHttpContextAccessor _contextAccessor;
        private IMapper _mapper;
        private IProgressUpdatesRepository _progressUpdatesRepository;
        private IObjectivesRepository _objectivesRepository;
        //private readonly IModel _channel;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDepartmentRepository _departmentRepository;
        private IDepartmentProgressApprovalRepository _progressApprovalRepository;
        public KeyResultsService(IKeyResultRepository keyResultRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IProgressUpdatesRepository progressUpdatesRepository, IObjectivesRepository objectivesRepository,
            IConfiguration configuration, IDepartmentProgressApprovalRepository departmentProgressApprovalRepository,
            UserManager<ApplicationUser> userManager, IDepartmentRepository departmentRepository)
        {
            _keyResultRepository = keyResultRepository;
            _contextAccessor = httpContextAccessor;
            _mapper = mapper;
            _progressUpdatesRepository = progressUpdatesRepository;
            _objectivesRepository = objectivesRepository;
            //_channel = model;
            //_config = configuration;
            //_hubConnection = new HubConnectionBuilder()
            //    .WithUrl(_config["signalr:url"])
            //    .Build();
            //_hubConnection.StartAsync().Wait();
            _progressApprovalRepository = departmentProgressApprovalRepository;
            _userManager = userManager;
            _departmentRepository = departmentRepository;
        }

        public async Task<AppResponse<KeyResultRespone>> UpdateWeight(KeyResultRequest request)
        {
            var result = new AppResponse<KeyResultRespone>();
            try
            {   
                
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                var now = DateTime.UtcNow;
                var keyresult = _keyResultRepository.Get(request.Id.Value);
                var objectivesAsQueryable = _objectivesRepository.AsQueryable().Where(x=>x.Id == request.ObjectivesId);
                if(!objectivesAsQueryable.Any())
                {
                    return result.BuildError("Cannot find objectives");
                }
                var objectives = objectivesAsQueryable.First();
                var currentUser = await _userManager.FindByNameAsync(userName);
                if (GetCurrentUserRole() == "Teamleader" || currentUser.Id == objectives.ApplicationUserId
                        || (GetCurrentUserRole() == "Admin" && objectives.TargetType == Infrastructure.Enum.TargetType.company)
                    )
                {
                    var progress = new ProgressUpdates();
                    progress.Id = Guid.NewGuid();
                    progress.CreatedOn = now;
                    progress.CreatedBy = userName;
                    progress.Note = GetUpdateString(request, keyresult);
                    progress.KeyResultId = keyresult.Id;
                    progress.OldPoint = keyresult.CurrentPoint;
                    progress.NewPoint = keyresult.CurrentPoint + request.AddedPoints ?? 0;

                    keyresult.CurrentPoint += request.AddedPoints ?? 0;
                    keyresult.IsCompleted = (bool)request.IsCompleted;
                    if(keyresult.Unit == TypeUnitKeyResult.CompletedOrNotCompleted)
                    {
                        progress.OldPoint = keyresult.IsCompleted ? 0 : 1;
                        progress.NewPoint = keyresult.IsCompleted ? 1 : 0;
                    }
                    _keyResultRepository.Edit(keyresult);
                    progress.ObjectivesCompletionRate = _objectivesRepository.caculatePercentObjectivesById(keyresult.ObjectivesId);
                    _progressUpdatesRepository.Add(progress);
                }
                else
                {
                    var departmentProgressApproval = new DepartmentProgressApproval();
                    departmentProgressApproval.Id = Guid.NewGuid();
                    departmentProgressApproval.CreatedOn = now;
                    departmentProgressApproval.CreatedBy = userName;
                    departmentProgressApproval.Note = GetUpdateString(request, keyresult);
                    departmentProgressApproval.AddedPoints = (int)request.AddedPoints;
                    departmentProgressApproval.KeyResultsId = keyresult.Id;
                    departmentProgressApproval.Note = GetUpdateString(request, keyresult);
                    departmentProgressApproval.Unit = (TypeUnitKeyResult)request.Unit;
                    departmentProgressApproval.IsCompleted = (bool)request.IsCompleted;
                    _progressApprovalRepository.Add(departmentProgressApproval);
                }

                result.BuildResult(_mapper.Map<KeyResultRespone>(keyresult));
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private string GetUpdateString(KeyResultRequest NewKeyResult, KeyResults CurKeyResults)
        {
            if(!string.IsNullOrEmpty(NewKeyResult.Note))
            {
                return NewKeyResult.Note;
            }
            string content = _contextAccessor.HttpContext.User.Identity.Name + " ";
            if(CurKeyResults.Unit == Infrastructure.Enum.TypeUnitKeyResult.CompletedOrNotCompleted)
            {
                content += "change status of " + CurKeyResults.Description + " to " + ((bool)NewKeyResult.IsCompleted ? "COMPLETED" : "NOT COMPLETED");
                return content; 
            }
            if ((CurKeyResults.CurrentPoint + NewKeyResult.AddedPoints) != CurKeyResults.CurrentPoint)
            {
                content += "update weights " + NewKeyResult.Description +" from " + CurKeyResults.CurrentPoint + " to " + (CurKeyResults.CurrentPoint + NewKeyResult.AddedPoints)+ "; ";
            }
            return content;
        }

        private void Update_Save(KeyResults keyresult, KeyResultRequest request)
        {
            var updateString = request.Note.IsNullOrEmpty() ? GetUpdateString(request, keyresult) : request.Note;
            var userName = _contextAccessor.HttpContext.User.Identity.Name;
            var progressUpdates = new ProgressUpdates();
            progressUpdates.CreatedBy = userName;
            progressUpdates.CreatedOn = DateTime.UtcNow;
            progressUpdates.Note = updateString;
            progressUpdates.KeyResultId = keyresult.Id; 
            progressUpdates.OldPoint = keyresult.CurrentPoint;
            progressUpdates.NewPoint = keyresult.CurrentPoint + request.AddedPoints;

            keyresult.CurrentPoint = (int)(keyresult.CurrentPoint + request.AddedPoints);
            _keyResultRepository.Edit(keyresult);
            _progressUpdatesRepository.Add(progressUpdates);
        }
        private string GetCurrentUserRole()
        {
            var user = _contextAccessor.HttpContext.User;

            // Kiểm tra nếu người dùng đã đăng nhập
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                // Lấy tất cả các vai trò của người dùng
                var roles = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                // Trả về vai trò đầu tiên (có thể điều chỉnh nếu người dùng có nhiều vai trò)
                return roles.First();
            }

            return "";
        }

        public AppResponse<KeyResultRespone> Get(Guid Id)
        {
            var result = new AppResponse<KeyResultRespone>();
            try
            {
                var dto = _keyResultRepository.AsQueryable().Where(x=>x.Id == Id).Include(x=>x.Objectives).Select(x=> new KeyResultRespone
                {
                    IsCompleted = x.IsCompleted,
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    CurrentPoint = x.CurrentPoint,
                    EndDay = x.Objectives.EndDay,
                    MaximunPoint = x.MaximunPoint,
                    Description = x.Description,
                    Id = x.Id,
                    StartDay = x.Objectives.StartDay,
                    Status = x.Status,
                    //LastProgressUpdate = _progressUpdatesRepository.FindBy(pr => pr.KeyResultId == Id)
                    //.OrderByDescending(x => x.CreatedOn).Select(x => x.CreatedOn).FirstOrDefault(),
                    //Percentage = x.Percentage,
                    ObjectivesId = x.ObjectivesId,
                    Percentage = x.Percentage,
                    Unit = x.Unit,
                    
                    
                }).First();
                dto.LastProgressUpdate = _progressUpdatesRepository.AsQueryable()
                    .Where(x => x.KeyResultId == Id).OrderByDescending(x => x.CreatedOn)
                    .Select(x => x.CreatedOn)
                    .FirstOrDefault();
                //var dto = _mapper.Map<KeyResultDto>(keyresult);

                dto.ProgressUpdates = _progressUpdatesRepository.AsQueryable().Where(x => x.KeyResultId == dto.Id).OrderByDescending(x=>x.CreatedOn)
                    .Select(x => new ProgressUpdatesRespone
                    {
                        CreateBy = x.CreatedBy,
                        CreateOn = x.CreatedOn,
                        Id = x.Id,
                        KeyResultId = x.KeyResultId,
                        NewPoint = x.NewPoint,
                        Note = x.Note,
                        OldPoint = x.OldPoint,
                    }).ToList();

                result.BuildResult(dto);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

    }
}
