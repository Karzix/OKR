using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Repository.Implementation;
using OKR.Service.Contract;
using System.Security.Claims;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace OKR.Service.Implementation
{
    public class DepartmentProgressApprovalService : IDepartmentProgressApprovalService
    { 
        private IDepartmentProgressApprovalRepository _departmentProgressApprovalRepository;
        private IMapper _mapper;
        private IHttpContextAccessor _contextAccessor;
        private IDepartmentRepository _departmentRepository;
        private IDepartmentObjectivesRepository _departmentObjectivesRepository;
        private IUserRepository _userRepository;
        private IKeyResultRepository _keyResultRepository;
        private IProgressUpdatesRepository _progressUpdatesRepository;
        private IObjectivesRepository _objectivesRepository;

        public DepartmentProgressApprovalService(IDepartmentProgressApprovalRepository departmentProgressApprovalRepository,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, IDepartmentRepository departmentRepository,
            IDepartmentObjectivesRepository departmentObjectivesRepository, IUserRepository userRepository,
            IKeyResultRepository keyResultRepository, IProgressUpdatesRepository progressUpdatesRepository, IObjectivesRepository objectivesRepository)
        {
            _departmentProgressApprovalRepository = departmentProgressApprovalRepository;
            _mapper = mapper;
            _contextAccessor = httpContextAccessor;
            _departmentRepository = departmentRepository;
            _departmentObjectivesRepository = departmentObjectivesRepository;
            _userRepository = userRepository;
            _keyResultRepository = keyResultRepository;
            _progressUpdatesRepository = progressUpdatesRepository;
            _objectivesRepository = objectivesRepository;
        }
        public AppResponse<SearchResponse<DepartmentProgressApprovalDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<DepartmentProgressApprovalDto>>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _departmentProgressApprovalRepository.CountRecordsByPredicate(query);
                var users = _departmentProgressApprovalRepository.FindByPredicate(query);
       
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var UserList = users.Skip(startIndex).Take(pageSize);
                var dtoList = UserList.Select(x => new DepartmentProgressApprovalDto
                {
                    Id = x.Id,
                    AddedPoints = x.AddedPoints,
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    KeuresultID = x.KeyResultsId,
                    Note = x.Note,
                }).ToList();
                var searchResult = new SearchResponse<DepartmentProgressApprovalDto>
                {
                    TotalRows = numOfRecords,
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    CurrentPage = pageIndex,
                    Data = dtoList,
                };

                result.Data = searchResult;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
        private ExpressionStarter<DepartmentProgressApproval> BuildFilterExpression(List<Filter> Filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<DepartmentProgressApproval>(true);


                if (Filters != null)
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "KeuresultId":
                                {
                                    predicate = predicate.And(x => x.KeyResultsId == Guid.Parse(filter.Value));
                                    break;
                                }
                            case "entityObjectivesId":
                                {
                                    predicate = BuildFilterEntityObjectives(predicate,Filters);
                                    break;
                                }
                            case "user":
                                {
                                    predicate = BuildFilterUser(predicate, Filters);
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                predicate = predicate.And(x => x.IsDeleted == false);
                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private ExpressionStarter<DepartmentProgressApproval> BuildFilterEntityObjectives(ExpressionStarter<DepartmentProgressApproval> predicate, List<Filter> Filters)
        {
            var filter = Filters.Where(x=>x.FieldName == "entityObjectivesId").First();
            var entityObjectivesId = Guid.Parse(filter.Value);
            predicate = predicate.And(dpa =>
                   dpa.KeyResults.Objectives.UserObjectives.Any(uo => uo.Id == entityObjectivesId) 
                   || dpa.KeyResults.Objectives.DepartmentObjectives.Any(dpo => dpo.Id == entityObjectivesId));

            return predicate;
        }
        private ExpressionStarter<DepartmentProgressApproval> BuildFilterUser(ExpressionStarter<DepartmentProgressApproval> predicate, List<Filter> Filters)
        {
            var userRole = GetCurrentUserRole();
            var managerId = GetCurrentUserId();
            if (userRole == "TeamLead" || userRole == "BranchManagement")
            {
                var managedDepartments = _userRepository.AsQueryable()
                    .Where(user => user.Id == managerId.ToString()) 
                    .Select(user => user.DepartmentId) 
                    .ToList();
                var objectivesIds = _departmentObjectivesRepository.AsQueryable()
                    .Where(deptObj => managedDepartments.Contains(deptObj.DepartmentId))
                    .Select(deptObj => deptObj.ObjectivesId)
                    .ToList();
                predicate = predicate.And(dpa => objectivesIds.Contains(dpa.KeyResults.ObjectivesId));
            }
            return predicate;
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
        private Guid GetCurrentUserId()
        {
            var user = _contextAccessor.HttpContext.User;
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(userId, out var parsedGuid) ? parsedGuid : Guid.Empty;
            }
            return Guid.Empty;
        }

        public AppResponse<DepartmentProgressApprovalDto> Confirm(DepartmentProgressApprovalDto dept)
        {
            var result = new AppResponse<DepartmentProgressApprovalDto>();
            try
            {
                var departmentProgressApproval = _departmentProgressApprovalRepository.Get(dept.Id.Value);
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                if (dept.IsApproved)
                {
                    var keyresult = _keyResultRepository.Get(departmentProgressApproval.KeyResultsId);
                    var progressUpdates = new ProgressUpdates();
                    progressUpdates.CreatedBy = userName;
                    progressUpdates.CreatedOn = DateTime.UtcNow;
                    progressUpdates.Note = departmentProgressApproval.Note;
                    progressUpdates.KeyResultId = departmentProgressApproval.KeyResultsId;
                    progressUpdates.OldPoint = keyresult.CurrentPoint;
                    progressUpdates.NewPoint = keyresult.CurrentPoint + departmentProgressApproval.AddedPoints;

                    keyresult.CurrentPoint = (int)(keyresult.CurrentPoint + departmentProgressApproval.AddedPoints);
                    _keyResultRepository.Edit(keyresult);
                    progressUpdates.KeyresultCompletionRate = _keyResultRepository.caculatePercentKeyResults(keyresult);
                    Dictionary<Guid, int> op = _objectivesRepository.caculatePercentObjectives(_objectivesRepository.AsQueryable().Where(x => x.Id == keyresult.ObjectivesId));
                    progressUpdates.ObjectivesCompletionRate = op.ContainsKey(keyresult.ObjectivesId) ? op[keyresult.ObjectivesId] : 0;
                    _progressUpdatesRepository.Add(progressUpdates);
                }
                departmentProgressApproval.IsDeleted = true;
                _departmentProgressApprovalRepository.Edit(departmentProgressApproval);
                result.BuildResult(dept);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }



    }
}
