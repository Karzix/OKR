using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using OKR.DTO;
using OKR.Infrastructure.Enum;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Repository.Implementation;
using OKR.Service.Contract;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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
        private IUserRepository _userRepository;
        private IKeyResultRepository _keyResultRepository;
        private IProgressUpdatesRepository _progressUpdatesRepository;
        private IObjectivesRepository _objectivesRepository;

        public DepartmentProgressApprovalService(IDepartmentProgressApprovalRepository departmentProgressApprovalRepository,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, IDepartmentRepository departmentRepository,
           IUserRepository userRepository,
            IKeyResultRepository keyResultRepository, IProgressUpdatesRepository progressUpdatesRepository, IObjectivesRepository objectivesRepository)
        {
            _departmentProgressApprovalRepository = departmentProgressApprovalRepository;
            _mapper = mapper;
            _contextAccessor = httpContextAccessor;
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
            _keyResultRepository = keyResultRepository;
            _progressUpdatesRepository = progressUpdatesRepository;
            _objectivesRepository = objectivesRepository;
        }
        public AppResponse<SearchResponse<DepartmentProgressApprovalRespone>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<DepartmentProgressApprovalRespone>>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _departmentProgressApprovalRepository.CountRecordsByPredicate(query);
                var users = _departmentProgressApprovalRepository.FindByPredicate(query);
       
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var dpa = users.Skip(startIndex).Take(pageSize);
                var dtoList = dpa.Select(x => new DepartmentProgressApprovalRespone
                {
                    Id = x.Id,
                    AddedPoints = x.AddedPoints,
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    KeyresultID = x.KeyResultsId,
                    Note = x.Unit == TypeUnitKeyResult.CompletedOrNotCompleted 
                        ? "change status of " + x.KeyResults.Description + " to " + ((bool)x.IsCompleted ? "COMPLETED" : "NOT COMPLETED")
                        : "add " + x.AddedPoints + " for " + x.KeyResults.Description + " ",
                    IsCompleted = x.IsCompleted
                }).ToList();
                var searchResult = new SearchResponse<DepartmentProgressApprovalRespone>
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
                            case "objectivesId":
                                {
                                    predicate = predicate.And(x=>x.KeyResults.ObjectivesId == Guid.Parse(filter.Value));
                                    break;
                                }
                            case "user":
                                {
                                    //predicate = BuildFilterUser(predicate, Filters);
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

        public AppResponse<DepartmentProgressApprovalRespone> Confirm(DepartmentProgressApprovalRequest dept)
        {
            var result = new AppResponse<DepartmentProgressApprovalRespone>();
            try
            {
                var departmentProgressApproval = _departmentProgressApprovalRepository.Get(dept.Id.Value);
                departmentProgressApproval.KeyResults = _keyResultRepository.Get(departmentProgressApproval.KeyResultsId);
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                if (dept.IsApproved)
                {
                    var keyresult = _keyResultRepository.Get(departmentProgressApproval.KeyResultsId);
                    if(keyresult.Unit != TypeUnitKeyResult.CompletedOrNotCompleted)
                    {
                        var progress = new ProgressUpdates();
                        progress.Id = Guid.NewGuid();
                        progress.CreatedOn = DateTime.UtcNow;
                        progress.CreatedBy = userName;
                        progress.Note = GetUpdateString(departmentProgressApproval);
                        progress.OldPoint = keyresult.CurrentPoint;
                        progress.NewPoint = keyresult.CurrentPoint + departmentProgressApproval.AddedPoints;
                        progress.KeyResultId = departmentProgressApproval.KeyResultsId;

                        keyresult.CurrentPoint += (int)departmentProgressApproval.AddedPoints!;
                        _keyResultRepository.Edit(keyresult);
                        progress.ObjectivesCompletionRate = _objectivesRepository.caculatePercentObjectivesById(keyresult.ObjectivesId);
                        _progressUpdatesRepository.Add(progress);
                    }
                    else
                    {
                        var progress = new ProgressUpdates();
                        progress.Id = Guid.NewGuid();
                        progress.CreatedOn = DateTime.UtcNow;
                        progress.CreatedBy = userName;
                        progress.Note = GetUpdateString(departmentProgressApproval);
                        progress.KeyResultId = departmentProgressApproval.KeyResultsId;
                        progress.OldPoint = departmentProgressApproval.IsCompleted ?  0 : 1;
                        progress.NewPoint = departmentProgressApproval.IsCompleted ? 1 : 0;
                        keyresult.IsCompleted = departmentProgressApproval.IsCompleted;
                        _keyResultRepository.Edit(keyresult);
                        progress.ObjectivesCompletionRate = _objectivesRepository.caculatePercentObjectivesById(keyresult.ObjectivesId);
                        _progressUpdatesRepository.Add(progress);
                    }
                    
                }
                departmentProgressApproval.IsDeleted = true;
                _departmentProgressApprovalRepository.Edit(departmentProgressApproval);
                result.BuildResult(_mapper.Map<DepartmentProgressApprovalRespone>(departmentProgressApproval));
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private string GetUpdateString(DepartmentProgressApproval dpa)
        {
            if (!string.IsNullOrEmpty(dpa.Note))
            {
                return dpa.Note;
            }
            string content = dpa.CreatedBy + " ";
            if (dpa.Unit == Infrastructure.Enum.TypeUnitKeyResult.CompletedOrNotCompleted)
            {
                content += "change status of " + dpa.KeyResults.Description + " to " + ((bool)dpa.IsCompleted ? "COMPLETED" : "NOT COMPLETED");
                return content;
            }
            else if ((dpa.KeyResults.CurrentPoint + dpa.AddedPoints) != dpa.KeyResults.CurrentPoint)
            {
                content += "update weights " + dpa.KeyResults.Description + " from " + dpa.KeyResults.CurrentPoint + " to " + (dpa.KeyResults.CurrentPoint + dpa.AddedPoints) + "; ";
            }
            return content;
        }

    }
}
