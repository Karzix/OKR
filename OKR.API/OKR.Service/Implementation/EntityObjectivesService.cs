using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OKR.DTO;
using OKR.Infrastructure.Enum;
using OKR.Models.Entity;
using OKR.Models.Migrations;
using OKR.Repository.Contract;
using OKR.Service.Contract;
using System.Data.Entity;
using static Maynghien.Infrastructure.Helpers.SearchHelper;
using static OKR.Infrastructure.Enum.FormatTargetType;
using static OKR.Infrastructure.Enum.helperQuarter;

namespace OKR.Service.Implementation
{
    public class EntityObjectivesService : IEntityObjectivesService
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
        private IDepartmentProgressApprovalRepository _departmentProgressApprovalRepository;

        public EntityObjectivesService(IHttpContextAccessor contextAccessor, IObjectivesRepository objectiveRepository,
            IMapper mapper, IKeyResultRepository keyResultRepository, ISidequestsRepository questsRepository,
            UserManager<ApplicationUser> userManager, IDepartmentRepository departmentRepository,
            IDepartmentObjectivesRepository departmentObjectivesRepository, IUserObjectivesRepository userObjectivesRepository,
            IDepartmentProgressApprovalRepository departmentProgressApprovalRepository)
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
            _departmentProgressApprovalRepository = departmentProgressApprovalRepository;
        }

        public AppResponse<SearchResponse<EntityObjectivesDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<EntityObjectivesDto>>();
            try
            {
                TargetType targetType;
                var filtertargetType = request.Filters.FirstOrDefault(x => x.FieldName == "targetType");
                if (filtertargetType != null)
                    targetType = (TargetType)int.Parse(filtertargetType.Value);
                else
                    targetType = TargetType.individual;

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * pageSize;

                var data = new List<EntityObjectivesDto>();
                int numOfRecords = 0;

                if (targetType == TargetType.individual)
                {
                    (data, numOfRecords) = GetUserObjectivesData(request.Filters, startIndex, pageSize);
                }
                else
                {
                    (data, numOfRecords) = GetDepartmentObjectivesData(request.Filters, startIndex, pageSize);
                }

                var searchUserResult = new SearchResponse<EntityObjectivesDto>
                {
                    TotalRows = numOfRecords,
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    CurrentPage = pageIndex,
                    Data = data,
                };
                result.BuildResult(searchUserResult);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message );
            }
            return result;
        }

        private ExpressionStarter<UserObjectives> BuildFilterUserObjectives(List<Filter> Filters)
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
                                    string[] dateStrings = filter.Value.Split(',');
                                    var dayStart = DateTime.ParseExact(dateStrings[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    //if (filter.Value != "")
                                    predicate = predicate.And(m => m.CreatedOn.Value >= dayStart);
                                    if (dateStrings[1] != null)
                                    {
                                        var dayEnd = DateTime.ParseExact(dateStrings[1], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                        predicate = predicate.And(m => m.CreatedOn.Value <= dayEnd);
                                    }
                                    break;
                                }
                            case "targetType":
                                {
                                    //predicate = BuildFilterTargetType(predicate, Filters);
                                    break;
                                }
                            //case "startDay":
                            //    {
                            //        string[] dateStrings = filter.Value.Split(',');
                            //        var dayStart = DateTime.ParseExact(dateStrings[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //        //if (filter.Value != "")
                            //        predicate = predicate.And(m => m.Objectives.StartDay >= dayStart);
                            //        if (dateStrings[1] != null)
                            //        {
                            //            var dayEnd = DateTime.ParseExact(dateStrings[1], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //            predicate = predicate.And(m => m.Objectives.StartDay <= dayEnd);
                            //        }
                            //        break;
                            //    }
                            //case "deadline":
                            //    {
                            //        string[] dateStrings = filter.Value.Split(',');
                            //        var dayStart = DateTime.ParseExact(dateStrings[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //        //if (filter.Value != "")
                            //        predicate = predicate.And(m => m.Objectives.Deadline >= dayStart);
                            //        if (dateStrings[1] != null)
                            //        {
                            //            var dayEnd = DateTime.ParseExact(dateStrings[1], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //            predicate = predicate.And(m => m.Objectives.Deadline <= dayEnd);
                            //        }
                            //        break;
                            //    }
                            case "status":
                                {
                                    if (filter.Value.IsNullOrEmpty())
                                    {
                                        break;
                                    }
                                    var enumN = int.Parse(filter.Value);
                                    StatusObjectives statusObjectives = (StatusObjectives)enumN;
                                    predicate = predicate.And(x => x.Objectives.status == statusObjectives);
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
                //if (Filters.Where(x => x.FieldName == "createBy").Count() == 0)
                //{
                //    predicate = predicate.And(x => x.CreatedBy.Equals(userName));
                //}
                predicate = predicate.And(x => x.IsDeleted != true);
                return predicate;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private ExpressionStarter<DepartmentObjectives> BuildFilterDepartmentObjectives(List<Filter> Filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<DepartmentObjectives>(true);


                if (Filters != null)
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "createBy":
                                {
                                    //predicate = predicate.And(x => x.CreatedBy.Equals(filter.Value));
                                    break;
                                }
                            case "createOn":
                                {
                                    string[] dateStrings = filter.Value.Split(',');
                                    var dayStart = DateTime.ParseExact(dateStrings[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    //if (filter.Value != "")
                                    predicate = predicate.And(m => m.CreatedOn.Value >= dayStart);
                                    if (dateStrings[1] != null)
                                    {
                                        var dayEnd = DateTime.ParseExact(dateStrings[1], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                        predicate = predicate.And(m => m.CreatedOn.Value <= dayEnd);
                                    }
                                    break;
                                }
                            case "targetType":
                                {
                                    predicate = BuildFilterTargetType(predicate, Filters);
                                    break;
                                }
                            //case "startDay":
                            //    {
                            //        string[] dateStrings = filter.Value.Split(',');
                            //        var dayStart = DateTime.ParseExact(dateStrings[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //        //if (filter.Value != "")
                            //        predicate = predicate.And(m => m.Objectives.StartDay >= dayStart);
                            //        if (dateStrings[1] != null)
                            //        {
                            //            var dayEnd = DateTime.ParseExact(dateStrings[1], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //            predicate = predicate.And(m => m.Objectives.StartDay <= dayEnd);
                            //        }
                            //        break;
                            //    }
                            //case "deadline":
                            //    {
                            //        string[] dateStrings = filter.Value.Split(',');
                            //        var dayStart = DateTime.ParseExact(dateStrings[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //        //if (filter.Value != "")
                            //        predicate = predicate.And(m => m.Objectives.Deadline >= dayStart);
                            //        if (dateStrings[1] != null)
                            //        {
                            //            var dayEnd = DateTime.ParseExact(dateStrings[1], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //            predicate = predicate.And(m => m.Objectives.Deadline <= dayEnd);
                            //        }
                            //        break;
                            //    }
                            case "status":
                                {
                                    if (filter.Value.IsNullOrEmpty())
                                    {
                                        break;
                                    }
                                    var enumN = int.Parse(filter.Value);
                                    StatusObjectives statusObjectives = (StatusObjectives)enumN;
                                    predicate = predicate.And(x => x.Objectives.status == statusObjectives);
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
                //if (Filters.Where(x => x.FieldName == "createBy").Count() == 0)
                //{
                //    predicate = predicate.And(x => x.CreatedBy.Equals(userName));
                //}
                predicate = predicate.And(x => x.IsDeleted != true);
                return predicate;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private ExpressionStarter<DepartmentObjectives> BuildFilterTargetType(ExpressionStarter<DepartmentObjectives> predicate, List<Filter> Filters)
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

            predicate = predicate.And(x => departmentObjectiveIds.Contains(x.ObjectivesId));

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
                    dto = userObjectivesAsquery.Include(x => x.Objectives).Select(x => new EntityObjectivesDto
                    {
                        Quarter = x.Objectives.Quarter,
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
                        Year = x.Objectives.Year,
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
                    dto = departmentObjectivesAsquery.Include(x => x.Objectives).Select(x => new EntityObjectivesDto
                    {
                        Quarter = x.Objectives.Quarter,
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
                        Year = x.Objectives.Year,
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
                var now = DateTime.UtcNow;
                //var userObjectives = _userObjectivesRepository.AsQueryable().Where(x => x.Id == dto.Id).FirstOrDefault();
                //var departmentObjectives = _departmentObjectivesRepository.AsQueryable().Where(x => x.Id == dto.Id).FirstOrDefault();

                var objectives = _objectiveRepository.AsQueryable().Where(x => x.Id == dto.ObjectivesId).First();
                if (objectives.Year <= now.Year || objectives.Quarter != GetCurrentQuarter())
                {
                    return result.BuildError("cannot be changed because the deadline has passed");
                }
                objectives.status = dto.Status.Value;
                _objectiveRepository.Edit(objectives);
                result.BuildResult("OK");

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        private (List<EntityObjectivesDto>, int) GetUserObjectivesData(IEnumerable<Filter> filters, int startIndex, int pageSize)
        {
            var filterList = filters.ToList();
            var queryUserObjectives = BuildFilterUserObjectives(filterList);
            var model = _userObjectivesRepository.FindByPredicate(queryUserObjectives);
            int numOfRecords = _userObjectivesRepository.CountRecordsByPredicate(queryUserObjectives);
            model = model.OrderByDescending(x=>x.CreatedOn).Skip(startIndex).Take(pageSize);
            var objectIdPoint = _userObjectivesRepository.caculatePercentObjectives(model);

            var data = model.Include(x => x.Objectives)
                .Select(x => new EntityObjectivesDto
                {
                    Id = x.Id,
                    Name = x.Objectives.Name,
                    Quarter = x.Objectives.Quarter,
                    Year = x.Objectives.Year,
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
                    Point = objectIdPoint.ContainsKey(x.Id) ? objectIdPoint[x.Id] : 0,
                    ObjectivesId = x.ObjectivesId,
                    Status = x.Objectives.status,
                    CreateBy = x.CreatedBy,
                    NumberOfPendingUpdates = 0
                })
                .ToList();

            return (data, numOfRecords);
        }

        private (List<EntityObjectivesDto>, int) GetDepartmentObjectivesData(IEnumerable<Filter> filters, int startIndex, int pageSize)
        {
            var filterList = filters.ToList();
            var queryDepartmentObjectives = BuildFilterDepartmentObjectives(filterList);
            var model = _departmentObjectivesRepository.FindByPredicate(queryDepartmentObjectives);
            int numOfRecords = _departmentObjectivesRepository.CountRecordsByPredicate(queryDepartmentObjectives);
            model = model.OrderByDescending(x => x.CreatedOn).Skip(startIndex).Take(pageSize);
            var objectIdPoint = _departmentObjectivesRepository.caculatePercentObjectives(model);

            var data = model.Include(x => x.Objectives)
                .Select(x => new EntityObjectivesDto
                {
                    Id = x.Id,
                    Name = x.Objectives.Name,
                    Quarter = x.Objectives.Quarter,
                    Year = x.Objectives.Year,
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
                    Point = objectIdPoint.ContainsKey(x.Id) ? objectIdPoint[x.Id] : 0,
                    ObjectivesId = x.ObjectivesId,
                    Status = x.Objectives.status,
                    CreateBy = x.CreatedBy,
                    NumberOfPendingUpdates = _departmentProgressApprovalRepository.AsQueryable()
                                                .Where(dpa => dpa.KeyResults.ObjectivesId == x.ObjectivesId && !dpa.IsDeleted).Count(),
                })
                .ToList();

            return (data, numOfRecords);
        }

        public AppResponse<string> DeleteEntityObjectives(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var dpo = _departmentObjectivesRepository.AsQueryable().Where(x=>x.Id == Id).FirstOrDefault();
                if (dpo != null)
                {
                    dpo.IsDeleted = true;
                    _departmentObjectivesRepository.Edit(dpo);
                }
                else
                {
                    var uo = _userObjectivesRepository.AsQueryable().Where(x => x.Id == Id).FirstOrDefault();
                    if (uo != null)
                    {
                        uo.IsDeleted = true;
                        _userObjectivesRepository.Edit(uo);
                    }
                }
                //var objectives = _objectiveRepository.Get(dpo.ObjectivesId);
                //objectives.IsDeleted = true;
                //_objectiveRepository.Edit(objectives);
                result.BuildResult("delete OK");
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
