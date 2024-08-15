using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OKR.DTO;
using OKR.Infrastructure.Enum;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Repository.Implementation;
using OKR.Service.Contract;
using System;
using System.Data.Entity;
using System.Diagnostics;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace OKR.Service.Implementation
{
    public class ProgressUpdatesService : IProgressUpdatesService
    {
        private IMapper _mapper;
        private IProgressUpdatesRepository _progressUpdatesRepository;
        private IHttpContextAccessor _httpContextAccessor;
        private UserManager<ApplicationUser> _userManager;
        private IHttpContextAccessor _contextAccessor;
        private IDepartmentObjectivesRepository _departmentObjectivesRepository;
        private IDepartmentRepository _departmentRepository;

        public ProgressUpdatesService(IMapper mapper, IProgressUpdatesRepository progressUpdatesRepository,
            IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor,
            IDepartmentObjectivesRepository departmentObjectivesRepository, IDepartmentRepository departmentRepository)
        {
            _mapper = mapper;
            _progressUpdatesRepository = progressUpdatesRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _departmentObjectivesRepository = departmentObjectivesRepository;
            _departmentRepository = departmentRepository;
        }


        public AppResponse<SearchResponse<ProgressUpdatesDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ProgressUpdatesDto>>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _progressUpdatesRepository.CountRecordsByPredicate(query);
                var progress = _progressUpdatesRepository.FindByPredicate(query);
                if (request.SortBy != null)
                {
                    progress = _progressUpdatesRepository.addSort(progress, request.SortBy);
                }
                else
                {
                    progress = progress.OrderByDescending(x => x.CreatedOn);
                }
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var progressList = progress.Skip(startIndex).Take(pageSize).Include(x=>x.KeyResults);
                var dtoList = progressList.Select(x => new ProgressUpdatesDto
                {
                    Id = x.Id,
                    CreateBy = x.CreatedBy,
                    CreateOn = x.CreatedOn,
                    KeyResultId = x.KeyResultId,
                    NewPoint = x.NewPoint,
                    Note = x.Note,
                    OldPoint = x.OldPoint,
                    Unit = x.KeyResults.Unit,
                }).ToList();
                var searchResult = new SearchResponse<ProgressUpdatesDto>
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

        private ExpressionStarter<ProgressUpdates> BuildFilterExpression(List<Filter> Filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<ProgressUpdates>(true);


                if (Filters != null)
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "targetType":
                                {
                                    predicate = BuildFilterTargetType(predicate, Filters);
                                    break;
                                }
                            case "createBy":
                                {
                                    var tg = Filters.Where(x => x.FieldName == "targetType").FirstOrDefault();
                                    if (tg != null && tg.Value.Contains("0")){
                                        predicate = predicate.And(x => x.CreatedBy == filter.Value);
                                    }
                                    break;
                                }
                            case "keyresultId":
                                {
                                    predicate = predicate.And(x=>x.KeyResultId.Equals(filter.Value));
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (Filters.Where(x => x.FieldName == "targetType").Count() == 0)
                {
                    predicate = predicate.And(x => x.KeyResults.Objectives.TargetType == TargetType.individual);
                    if (Filters.Where(x => x.FieldName == "createBy").Count() == 0)
                    {
                        predicate = predicate.And(x => x.CreatedBy.Equals(userName));
                    }
                }

                predicate = predicate.And(x => x.KeyResults.Objectives.IsDeleted != true);
                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private ExpressionStarter<ProgressUpdates> BuildFilterTargetType(ExpressionStarter<ProgressUpdates> predicate, List<Filter> Filters)
        {
            var filter = Filters.Where(x => x.FieldName == "targetType").First();
            var emumN = int.Parse(filter.Value);
            TargetType targetType = (TargetType)emumN;
            predicate = predicate.And(x => x.KeyResults.Objectives.TargetType == targetType);
            if (targetType == TargetType.individual)
            {
                return predicate;
            }
            var filterUserName = Filters.Where(x => x.FieldName == "createBy").FirstOrDefault();
            ApplicationUser user;
            if (filterUserName != null)
            {
                user = _userManager.Users.Where(x => x.UserName == filterUserName.Value).FirstOrDefault();
            }
            else
            {
                user = _userManager.Users.Where(x => x.UserName == _contextAccessor.HttpContext.User.Identity.Name).FirstOrDefault();
            }
            var department = _departmentRepository.GetParentOfChildDepartment(emumN, user.DepartmentId.Value);
            var departmentObjectiveIds = _departmentObjectivesRepository.AsQueryable()
                 .Where(doj => doj.DepartmentId == department.Id)
                 .Select(doj => doj.ObjectivesId);

            predicate = predicate.And(x => departmentObjectiveIds.Contains(x.KeyResults.Objectives.Id));
            

            return predicate;
        }

        public AppResponse<List<DataChart>> GetDataChart(SearchRequest request)
        {
            var result = new AppResponse<List<DataChart>>();
            try
            {
                var filter = request.Filters.FirstOrDefault(x=>x.FieldName == "objectivesId" || x.FieldName == "keyresultsId");
                if (filter == null)
                {
                    return result.BuildError("");
                }
                IQueryable<ProgressUpdates> QueryProgresses = _progressUpdatesRepository.AsQueryable();
                if(filter.FieldName == "objectivesId")
                {
                    QueryProgresses = _progressUpdatesRepository.AsQueryable().Where(x=>x.KeyResults.ObjectivesId.Equals(Guid.Parse(filter.Value))).AsQueryable();
                }
                else if(filter.FieldName == "keyresultsId")
                {
                    QueryProgresses = _progressUpdatesRepository.AsQueryable().Where(x => x.KeyResultId.Equals(Guid.Parse(filter.Value))).AsQueryable();
                }
                var progresses = QueryProgresses.OrderBy(x=>x.CreatedOn).Select(x=> new DataChart
                {
                    Date = x.CreatedOn,
                    Label = x.Note,
                    NewPoint = x.NewPoint,
                    OldPoint = x.OldPoint,
                    UserName = x.CreatedBy,
                    KeyresultCompletionRate = x.KeyresultCompletionRate,
                    ObjectivesCompletionRate = x.ObjectivesCompletionRate,
                }).ToList();
                result.BuildResult(progresses);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message+ " " + ex.StackTrace);
            }
            return result;
        }
    }
}
