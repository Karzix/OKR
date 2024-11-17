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
        private IUserRepository _userRepository;
        private IHttpContextAccessor _contextAccessor;
        private IMapper _mapper;
        private IObjectivesRepository _objectivesRepository;
        public EvaluateTargetService(IEvaluateTargetRepository evaluateTargetRepository, 
            IUserRepository userRepository, IHttpContextAccessor contextAccessor, IMapper mapper
            , IObjectivesRepository objectivesRepository)
        {
            _evaluateTargetRepository = evaluateTargetRepository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _objectivesRepository = objectivesRepository;
        }

        public AppResponse<EvaluateTargetRespone> Create(EvaluateTargetRequest request)
        {
            var result = new AppResponse<EvaluateTargetRespone>();
            try
            {
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                if (request.ObjectivesId == null)
                {
                    result.BuildError("objectives null");
                }
                var objectivesAsQueryable = _objectivesRepository.FindBy(x=>x.Id == request.ObjectivesId);
                if(objectivesAsQueryable.Count() == 0)
                {
                    return result.BuildError("cannot find objectives");
                }
                var evaluaTarget = _mapper.Map<EvaluateTarget>(request);
                evaluaTarget.Id = Guid.NewGuid();
                evaluaTarget.CreatedBy = userName;
                _evaluateTargetRepository.Add(evaluaTarget);
                request.Id = evaluaTarget.Id;
                result.BuildResult(_mapper.Map<EvaluateTargetRespone>(evaluaTarget));
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

        public AppResponse<EvaluateTargetRespone> Edit(EvaluateTargetRequest request)
        {
            var result = new AppResponse<EvaluateTargetRespone>();
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
                result.BuildResult(_mapper.Map<EvaluateTargetRespone>(evaluateTarget));
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<SearchResponse<EvaluateTargetRespone>> Search(SearchRequest request)
        { 
            var result = new AppResponse<SearchResponse<EvaluateTargetRespone>>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _evaluateTargetRepository.CountRecordsByPredicate(query);
                var evaluatarget = _evaluateTargetRepository.FindByPredicate(query);
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var list = evaluatarget.Skip(startIndex).Take(pageSize);

                var data = list.Select(x=> new EvaluateTargetRespone
                {
                    Content = x.Content,
                    CreateBy = x.CreatedBy,
                    CreateOn = x.CreatedOn,
                    ObjectivesId = x.ObjectivesId,
                    Id = x.Id,
                    Modifiedby = x.Modifiedby,
                    ModifiedOn = x.ModifiedOn,
                }).ToList();

                var searchResult = new SearchResponse<EvaluateTargetRespone>
                {
                    TotalRows = numOfRecords,
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    CurrentPage = pageIndex,
                    Data = data,
                };
                result.BuildResult(searchResult);
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
                                predicate = predicate.And(x => x.ObjectivesId == Guid.Parse(filter.Value));
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
