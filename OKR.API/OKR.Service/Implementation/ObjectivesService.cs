using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using OKR.DTO;
using OKR.Infrastructure.Enum;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;
using System.Data.Entity;
using static Maynghien.Infrastructure.Helpers.SearchHelper;
using static OKR.Infrastructure.Enum.FormatTargetType;

namespace OKR.Service.Implementation
{
    public class ObjectiveService : IObjectivesService
    {
        private IHttpContextAccessor _contextAccessor;
        private IObjectivesRepository _objectiveRepository;
        private IMapper _mapper;
        private IKeyResultRepository _keyResultRepository;
        private ISidequestsRepository _questsRepository;

        public ObjectiveService(IHttpContextAccessor contextAccessor, IObjectivesRepository objectiveRepository, IMapper mapper, IKeyResultRepository keyResultRepository, ISidequestsRepository sidequestsRepository)
        {
            _contextAccessor = contextAccessor;
            _objectiveRepository = objectiveRepository;
            _mapper = mapper;
            _keyResultRepository = keyResultRepository;
            _questsRepository = sidequestsRepository;
        }

        public AppResponse<ObjectiveDto> Create(ObjectiveDto request)
        {
            var result = new AppResponse<ObjectiveDto>();
            try
            {
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                
                if(request.ListKeyResults == null || request.ListKeyResults.Count == 0)
                {
                    return result.BuildError("requires at least one keyResult");
                }
                foreach(var item in request.ListKeyResults)
                {
                    if(item.Unit == TypeUnitKeyResult.Checked)
                    {
                        if(item.Sidequests.Count() == 0)
                        {
                            result.BuildError(item.Description + " requires at least one Sidequests");
                        }
                    }
                    else
                    {
                        if(item.MaximunPoint == 0)
                        {
                            return result.BuildError("MaximunPoint need to be greater than 0");
                        }
                        if(item.CurrentPoint < 0)
                        {
                            return result.BuildError("CurrentPoint cannot be negative");
                        }
                    }
                }

                var objective =_mapper.Map<Objectives>(request);
                objective.Id = Guid.NewGuid();
                objective.CreatedBy = userName;
                var keyResults = new List<KeyResults>();
                var ListKeyResultsDtoTemp = new List<KeyResultDto>();
                foreach(var k in request.ListKeyResults)
                {
                    var newKeyResult = _mapper.Map<KeyResults>(k);
                    newKeyResult.Id = Guid.NewGuid();
                    newKeyResult.CreatedBy = userName;
                    keyResults.Add(newKeyResult);

                    var newKeyResultDto =_mapper.Map<KeyResultDto>(newKeyResult);
                    newKeyResultDto.Sidequests = k.Sidequests;
                    ListKeyResultsDtoTemp.Add(newKeyResultDto);
                }

                var ListSidequests = new List<Sidequests>();
                foreach (var k in ListKeyResultsDtoTemp)
                {
                    foreach (var s in k.Sidequests != null ? k.Sidequests : [])
                    {
                        ListSidequests.Add(new Sidequests()
                        {
                            CreatedBy = userName,
                            CreatedOn = DateTime.Now,
                            Id = Guid.NewGuid(),
                            KeyResultsId = k.Id.Value,
                            Name = s.Name,
                        });
                    }
                }
                _objectiveRepository.Add(objective,keyResults, ListSidequests);

                request.Id = objective.Id;
                request.ListKeyResults = _mapper.Map<List<KeyResultDto>>(keyResults);


                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<string> Delete(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var objective = _objectiveRepository.Get(Id);
                objective.IsDeleted = true;
                _objectiveRepository.Edit(objective);
                result.BuildResult("OK");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<ObjectiveDto> Get(Guid Id)
        {
            var result = new AppResponse<ObjectiveDto>();
            try
            {
                var ojective = _objectiveRepository.Get(Id);
                var listKeyResult = _keyResultRepository.FindBy(x=>x.ObjectivesId == Id).ToList();
                var data = _mapper.Map<ObjectiveDto>(ojective);
                data.ListKeyResults = _mapper.Map<List<KeyResultDto>>(listKeyResult);

                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<SearchResponse<ObjectiveDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ObjectiveDto>>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _objectiveRepository.CountRecordsByPredicate(query);
                var model = _objectiveRepository.FindByPredicate(query);
                if (request.SortBy != null)
                {
                    model = _objectiveRepository.addSort(model, request.SortBy);
                }
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                
                model = model.Skip(startIndex).Take(pageSize);
                var objectId_point = _objectiveRepository.caculatePercentObjectives(model);
                var List = model.Include(x=>x.TargetType)
                    .Select(x => new ObjectiveDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Deadline = x.Deadline,
                        StartDay = x.StartDay,
                        TargetType = x.TargetType,
                        TargetTypeName = getTargetTypeName(x.TargetType),
                        ListKeyResults = _keyResultRepository.GetAll().Where(k=>k.ObjectivesId == x.Id)
                        .Select(k=> new KeyResultDto
                        {
                            Active = k.Active,
                            CurrentPoint = k.CurrentPoint,
                            Deadline = k.Deadline,
                            Description = k.Description,
                            Id = k.Id,
                            MaximunPoint = k.MaximunPoint,
                            Unit = k.Unit,
                            Sidequests = _questsRepository.GetAll().Where(s => s.KeyResultsId == k.Id).
                            Select(s=> new SidequestsDto
                            {
                                Id=s.Id,
                                Name=s.Name,
                                Status = s.Status,
                                KeyResultsId = s.KeyResultsId,
                            }).ToList(),
                        }).ToList(),
                        Point = objectId_point.ContainsKey(x.Id) ? objectId_point[x.Id] : 0
                    })
                    .ToList();


                var searchUserResult = new SearchResponse<ObjectiveDto>
                {
                    TotalRows = numOfRecords,
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    CurrentPage = pageIndex,
                    Data = List,
                };
                result.BuildResult(searchUserResult);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
        private ExpressionStarter<Objectives> BuildFilterExpression(List<Filter> Filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<Objectives>(true);


                if (Filters != null)
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "createBy":
                                {
                                    predicate = predicate.And(x => x.CreatedBy.Equals(filter.Value));
                                    break;
                                }

                            default:
                                break;
                        }
                    }
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                if (Filters.Where(x => x.FieldName == "createBy").Count() == 0)
                {
                    predicate = predicate.And(x => x.CreatedBy.Equals(userName));
                }
                predicate = predicate.And(x => x.IsDeleted != true);
                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public AppResponse<int> CaculateOveralProgress(SearchRequest request)
        {
            var result = new AppResponse<int>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _objectiveRepository.CountRecordsByPredicate(query);
                var model = _objectiveRepository.FindByPredicate(query);
                var point = _objectiveRepository.caculateOveralProgress(model);
                result.BuildResult(point);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }


        public AppResponse<ObjectiveDto> Edit(ObjectiveDto request)
        {
            var result = new AppResponse<ObjectiveDto>();
            try
            {
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                //edit
                var objectives = _objectiveRepository.Get(request.Id.Value);
                var listKeyResults = _keyResultRepository.FindBy(x => x.ObjectivesId == objectives.Id).ToList();
                var listkrID = listKeyResults.Select(x=>x.Id).ToList();
                var listSidequests = _questsRepository.GetAll().Where(x => listkrID.Contains(x.KeyResultsId)).ToList();

                objectives.StartDay = request.StartDay.Value;
                objectives.Deadline = request.Deadline.Value;
                objectives.Name = request.Name;
                objectives.TargetType = (TargetType)request.TargetType;

                foreach (var item in listKeyResults)
                {
                    item.IsDeleted = true;
                }
                foreach(var item in listSidequests)
                {
                    item.IsDeleted = true;
                }
                foreach (var item in request.ListKeyResults)
                {
                    var kr = listKeyResults.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (kr == null)
                    {
                        continue;
                    }
                    kr.IsDeleted = false;
                    kr.ModifiedOn = DateTime.Now;
                    kr.Modifiedby = _contextAccessor.HttpContext.User.Identity.Name;
                    kr.MaximunPoint = item.MaximunPoint.Value;
                    kr.Deadline = item.Deadline.Value;
                    kr.Active = item.Active.Value;
                    kr.CurrentPoint = item.CurrentPoint.Value;
                    kr.Description = item.Description;
                    kr.Unit = item.Unit.Value;
                    foreach (var sqdto in item.Sidequests)
                    {
                        var sq = listSidequests.Where(x=>x.Id == sqdto.Id).First();
                        sq.IsDeleted = false;
                        sq.ModifiedOn = DateTime.Now;
                        sq.Modifiedby = _contextAccessor.HttpContext.User.Identity.Name;
                        sq.Name = sqdto.Name;
                        sq.Status = (bool)sqdto.Status;
                        sq.KeyResultsId = kr.Id;
                    }
                }

                //create new
                var ListNewKeyresultsDto = request.ListKeyResults.Where(x => listKeyResults.All(kr => kr.Id != x.Id)).ToList();
                var ListNewKeyresults = new List<KeyResults>();
                var ListNewSidequests = new List<Sidequests>();
                ListNewKeyresultsDto.ForEach(X =>
                {
                    var newKR = _mapper.Map<KeyResults>(X);
                    newKR.Id = Guid.NewGuid();
                    newKR.CreatedBy = userName;
                    newKR.CreatedOn = DateTime.Now;
                    newKR.ObjectivesId = objectives.Id;
                    ListNewKeyresults.Add(newKR);
                    X.Sidequests.ForEach(sq =>
                    {
                        var newSQ = _mapper.Map<Sidequests>(sq);
                        newSQ.Id = Guid.NewGuid();
                        newSQ.CreatedBy = userName;
                        newSQ.KeyResultsId = newKR.Id;
                        newSQ.CreatedOn = DateTime.Now;
                        ListNewSidequests.Add(newSQ);
                    });
                });

                listKeyResults.AddRange(ListNewKeyresults);
                listSidequests.AddRange(ListNewSidequests);

                _objectiveRepository.Edit(objectives, listKeyResults, listSidequests);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
