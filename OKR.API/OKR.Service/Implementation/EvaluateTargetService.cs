using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace OKR.Service.Implementation
{
    public class EvaluateTargetService : IEvaluateTargetService
    {
        private IEvaluateTargetRepository _evaluateTargetRepository;
        private IDepartmentObjectivesRepository _departmentObjectivesRepository;
        private IUserRepository _userRepository;
        private IHttpContextAccessor _contextAccessor;
        private IMapper _mapper;
        private IUserObjectivesRepository _userObjectivesRepository;
        public EvaluateTargetService(IEvaluateTargetRepository evaluateTargetRepository, IDepartmentObjectivesRepository departmentObjectivesRepository,
            IUserRepository userRepository, IHttpContextAccessor contextAccessor, IMapper mapper, IUserObjectivesRepository userObjectivesRepository)
        {
            _evaluateTargetRepository = evaluateTargetRepository;
            _departmentObjectivesRepository = departmentObjectivesRepository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _userObjectivesRepository = userObjectivesRepository;
        }

        public AppResponse<EvaluateTargetDto> Create(EvaluateTargetDto request)
        {
            var result = new AppResponse<EvaluateTargetDto>();
            try
            {
                if(request.DepartmentObjectivesId == null && request.UserObjectivesId == null)
                {
                    return result.BuildError("Cannot find Objectives");
                }
                var typeObjectives = "";
                if(request.UserObjectivesId != null)
                {
                    typeObjectives = "user";
                    var userObjectives = _userObjectivesRepository.FindByPredicate(x=>x.Id == request.UserObjectivesId);
                    if(userObjectives.Count() == 0)
                    {
                        return result.BuildError("cannot find user objectives");
                    }
                }
                else
                {
                    typeObjectives = "department";
                    var departmentObjectives = _departmentObjectivesRepository.FindByPredicate(x => x.Id == request.DepartmentObjectivesId);
                    if (departmentObjectives.Count() == 0)
                    {
                        return result.BuildError("cannot find user objectives");
                    }
                }
                var evaluateTarget = _mapper.Map<EvaluateTarget>(request);
                evaluateTarget.CreatedBy = _contextAccessor.HttpContext.User.Identity.Name;
                evaluateTarget.CreatedOn = DateTime.UtcNow;
                _evaluateTargetRepository.Add(evaluateTarget);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;  
        }

        public AppResponse<string> Delete(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var userNamme = _contextAccessor.HttpContext.User.Identity.Name;
                var evaluateTarget = _evaluateTargetRepository.Get(id);
                if (evaluateTarget.CreatedBy != userNamme)
                {
                    return result.BuildError("you are not the creator");
                }
                evaluateTarget.IsDeleted = true;
                _evaluateTargetRepository.Edit(evaluateTarget);
                return result.BuildResult("Delete OK");

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<EvaluateTargetDto> Edit(EvaluateTargetDto request)
        {
            var result = new AppResponse<EvaluateTargetDto>();
            try
            {
                var userNamme = _contextAccessor.HttpContext.User.Identity.Name;
                var evaluateTarget = _evaluateTargetRepository.Get(request.Id.Value);
                if (evaluateTarget.CreatedBy != userNamme)
                {
                    return result.BuildError("you are not the creator\r\n");
                }
                evaluateTarget.Content = request.Content;
                evaluateTarget.ModifiedOn = DateTime.UtcNow;
                evaluateTarget.Modifiedby = _contextAccessor.HttpContext.User.Identity.Name;
                _evaluateTargetRepository.Edit(evaluateTarget);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<SearchResponse<EvaluateTargetDto>> Search(SearchRequest request)
        { 
            var result = new AppResponse<SearchResponse<EvaluateTargetDto>>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _evaluateTargetRepository.CountRecordsByPredicate(query);
                var evaluatarget = _evaluateTargetRepository.FindByPredicate(query);
                //if (request.SortBy != null)
                //{
                //    users = _evaluateTargetRepository.addSort(users, request.SortBy);
                //}
                //else
                //{
                    evaluatarget = evaluatarget.OrderByDescending(x => x.CreatedOn);
                //}
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var UserList = evaluatarget.Skip(startIndex).Take(pageSize);
                var dtoList = UserList.Select(x => new EvaluateTargetDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    CreateBy = x.CreatedBy,
                    CreateOn = x.CreatedOn,
                    DepartmentObjectivesId = x.DepartmentObjectivesId,
                    UserObjectivesId = x.UserObjectivesId,
                    Modifiedby = x.Modifiedby,
                    ModifiedOn = x.ModifiedOn,
                }).ToList();
                var searchResult = new SearchResponse<EvaluateTargetDto>
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

        private ExpressionStarter<EvaluateTarget> BuildFilterExpression(List<Filter> Filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<EvaluateTarget>(true);


                if (Filters != null)
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "objectivesId":
                                predicate = predicate.And(x => x.DepartmentObjectives.ObjectivesId == Guid.Parse(filter.Value) || x.UserObjectives.ObjectivesId == Guid.Parse(filter.Value));
                                break;
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
    }
}
