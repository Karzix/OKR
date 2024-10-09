﻿using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class EntityObjectivesService: IEntityObjectivesService
    {
        private IHttpContextAccessor _contextAccessor;
        private IObjectivesRepository _objectiveRepository;
        private IMapper _mapper;
        private IKeyResultRepository _keyResultRepository;
        private ISidequestsRepository _questsRepository;
        private UserManager<ApplicationUser> _userManager;
        private IDepartmentRepository _departmentRepository;
        private IDepartmentObjectivesRepository _departmentObjectivesRepository;
        private IUserObjectivesRepository _userObjectivesRepository;

        public EntityObjectivesService(IHttpContextAccessor contextAccessor, IObjectivesRepository objectiveRepository, 
            IMapper mapper, IKeyResultRepository keyResultRepository, ISidequestsRepository questsRepository,
            UserManager<ApplicationUser> userManager, IDepartmentRepository departmentRepository, 
            IDepartmentObjectivesRepository departmentObjectivesRepository, IUserObjectivesRepository userObjectivesRepository)
        {
            _contextAccessor = contextAccessor;
            _objectiveRepository = objectiveRepository;
            _mapper = mapper;
            _keyResultRepository = keyResultRepository;
            _questsRepository = questsRepository;
            _userManager = userManager;
            _departmentRepository = departmentRepository;
            _departmentObjectivesRepository = departmentObjectivesRepository;
            _userObjectivesRepository = userObjectivesRepository;
        }

        public AppResponse<SearchResponse<EntityObjectivesDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<EntityObjectivesDto>>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _userObjectivesRepository.CountRecordsByPredicate(query);
                var model = _userObjectivesRepository.FindByPredicate(query);
                if (request.SortBy != null)
                {
                    model = _userObjectivesRepository.addSort(model, request.SortBy);
                }
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * (int)pageSize;

                model = model.Skip(startIndex).Take(pageSize);
                var objectId_point = _userObjectivesRepository.caculatePercentObjectives(model);
                var List = model.Include(x => x.Objectives)
                    .Select(x => new EntityObjectivesDto
                    {
                        Id = x.Id,
                        Name = x.Objectives.Name,
                        Deadline = x.Objectives.Deadline,
                        StartDay = x.Objectives.StartDay,
                        TargetType = x.Objectives.TargetType,
                        TargetTypeName = getTargetTypeName(x.Objectives.TargetType),
                        ListKeyResults = _keyResultRepository.AsQueryable().Where(k => k.ObjectivesId == x.ObjectivesId)
                        .Select(k => new KeyResultDto
                        {
                            Active = k.Active,
                            CurrentPoint = k.CurrentPoint,
                            Deadline = k.Deadline,
                            Description = k.Description,
                            Id = k.Id,
                            MaximunPoint = k.MaximunPoint,
                            Unit = k.Unit,
                            Sidequests = _questsRepository.AsQueryable().Where(s => s.KeyResultsId == k.Id).
                            Select(s => new SidequestsDto
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Status = s.Status,
                                KeyResultsId = s.KeyResultsId,
                            }).ToList(),
                            
                        }).ToList(),
                        Point = objectId_point.ContainsKey(x.Id) ? objectId_point[x.Id] : 0,
                        ObjectivesId = x.ObjectivesId,
                        Status = x.status,
                        CreateBy = x.CreatedBy
                    })
                    .ToList();

                var searchUserResult = new SearchResponse<EntityObjectivesDto>
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

        private ExpressionStarter<UserObjectives> BuildFilterExpression(List<Filter> Filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<UserObjectives>(true);


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
                            case "createOn":
                                {
                                    predicate = predicate.And(x => x.CreatedBy == filter.Value);
                                    break;
                                }
                            case "targetType":
                                {
                                    predicate = BuildFilterTargetType(predicate, Filters);
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                if (Filters.Where(x => x.FieldName == "targetType").Count() == 0 || Filters.Where(x => x.FieldName == "targetType").First().Value == "0")
                {
                    predicate = predicate.And(x => x.Objectives.TargetType == TargetType.individual);
                    if (Filters.Where(x => x.FieldName == "createBy").Count() == 0)
                    {
                        predicate = predicate.And(x => x.CreatedBy.Equals(userName));
                    }
                }
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

        private ExpressionStarter<UserObjectives> BuildFilterTargetType(ExpressionStarter<UserObjectives> predicate, List<Filter> Filters)
        {
            var filter = Filters.Where(x => x.FieldName == "targetType").First();
            var enumN = int.Parse(filter.Value);
            TargetType targetType = (TargetType)enumN;
            if (targetType == TargetType.individual)
            {
                return predicate;
            }
            predicate = predicate.And(x => x.Objectives.TargetType == targetType);
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
            if (user.DepartmentId == null)
            {
                throw new Exception("User does not have a department.");
            }
            var department = _departmentRepository.GetParentOfChildDepartment(enumN, user.DepartmentId.Value);
            var departmentObjectiveIds = _departmentObjectivesRepository.AsQueryable()
                 .Where(doj => doj.DepartmentId == department.Id)
                 .Select(doj => doj.ObjectivesId);

            predicate = predicate.And(x => departmentObjectiveIds.Contains(x.Id));

            return predicate;
        }

        public AppResponse<EntityObjectivesDto> Get(Guid id)
        {
            var result = new AppResponse<EntityObjectivesDto>();
            try
            {
                EntityObjectivesDto dto = new EntityObjectivesDto();
                var userObjectivesAsquery = _userObjectivesRepository.AsQueryable().Where(x => x.Id == id);
               
                if (userObjectivesAsquery.Count() > 0)
                {
                    var objectId_point = _userObjectivesRepository.caculatePercentObjectives(userObjectivesAsquery);
                    dto = userObjectivesAsquery.Include(x => x.Objectives).Select(x=> new EntityObjectivesDto
                    {
                        Deadline = x.Objectives.Deadline,
                        Id = x.Id,
                        ListKeyResults = _keyResultRepository.AsQueryable().Where(k => k.ObjectivesId == x.ObjectivesId)
                        .Select(k => new KeyResultDto
                        {
                            Active = k.Active,
                            CurrentPoint = k.CurrentPoint,
                            Deadline = k.Deadline,
                            Description = k.Description,
                            Id = k.Id,
                            MaximunPoint = k.MaximunPoint,
                            Unit = k.Unit,
                            Sidequests = _questsRepository.AsQueryable().Where(s => s.KeyResultsId == k.Id).
                            Select(s => new SidequestsDto
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Status = s.Status,
                                KeyResultsId = s.KeyResultsId,
                            }).ToList(),

                        }).ToList(),
                        Name = x.Objectives.Name,
                        ObjectivesId = x.ObjectivesId,
                        Point = objectId_point.ContainsKey(x.Id) ? objectId_point[x.Id] : 0,
                        StartDay = x.Objectives.StartDay,
                        TargetType = x.Objectives.TargetType,
                        CreateBy = x.Objectives.CreatedBy,
                    }).First();
                    return result.BuildResult(dto);
                }
                var departmentObjectivesAsquery = _departmentObjectivesRepository.AsQueryable().Where(x => x.Id == id);
                if (departmentObjectivesAsquery.Count() > 0)
                {
                    var objectivesAsquery = departmentObjectivesAsquery.Select(x => x.Objectives);
                    var objectId_point = _objectiveRepository.caculatePercentObjectives(objectivesAsquery);
                    dto = departmentObjectivesAsquery.Include(x=>x.Objectives).Select(x => new EntityObjectivesDto
                    {
                        Deadline = x.Objectives.Deadline,
                        Id = x.Id,
                        ListKeyResults = _keyResultRepository.AsQueryable().Where(k => k.ObjectivesId == x.ObjectivesId)
                        .Select(k => new KeyResultDto
                        {
                            Active = k.Active,
                            CurrentPoint = k.CurrentPoint,
                            Deadline = k.Deadline,
                            Description = k.Description,
                            Id = k.Id,
                            MaximunPoint = k.MaximunPoint,
                            Unit = k.Unit,
                            Sidequests = _questsRepository.AsQueryable().Where(s => s.KeyResultsId == k.Id).
                            Select(s => new SidequestsDto
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Status = s.Status,
                                KeyResultsId = s.KeyResultsId,
                            }).ToList(),

                        }).ToList(),
                        Name = x.Objectives.Name,
                        ObjectivesId = x.ObjectivesId,
                        Point = objectId_point.ContainsKey(x.ObjectivesId) ? objectId_point[x.ObjectivesId] : 0,
                        StartDay = x.Objectives.StartDay,
                        TargetType = x.Objectives.TargetType,
                        CreateBy = x.Objectives.CreatedBy,
                    }).First();
                    return result.BuildResult(dto);
                }
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> StatusChange(EntityObjectivesDto dto)
        {
            var result = new AppResponse<string>();
            try
            {
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                var userObjectives = _userObjectivesRepository.AsQueryable().Where(x => x.Id == dto.Id).FirstOrDefault();
                var departmentObjectives = _departmentObjectivesRepository.AsQueryable().Where(x => x.Id == dto.Id).FirstOrDefault();
                if (userObjectives == null && departmentObjectives == null)
                {
                    return result.BuildError("cannot find objectives");
                }
                if (userObjectives != null)
                {
                    
                    userObjectives.status = dto.Status.Value;
                    _userObjectivesRepository.Edit(userObjectives);
                }
                else
                {
                    departmentObjectives.status = dto.Status.Value;
                    _departmentObjectivesRepository.Edit(departmentObjectives);
                }
                result.BuildResult("OK");
                
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
    }
}
