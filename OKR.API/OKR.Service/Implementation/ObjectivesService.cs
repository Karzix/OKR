﻿using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OKR.DTO;
using OKR.Infrastructure.Enum;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;
using System.Data.Entity;
using System.Security.Claims;
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
        private UserManager<ApplicationUser> _userManager;
        private IDepartmentRepository _departmentRepository;
        private IProgressUpdatesRepository _progressUpdatesRepository;

        public ObjectiveService(IHttpContextAccessor contextAccessor, IObjectivesRepository objectiveRepository, IMapper mapper,
            IKeyResultRepository keyResultRepository, UserManager<ApplicationUser> userManager,
            IDepartmentRepository departmentRepository, IProgressUpdatesRepository progressUpdatesRepository)
        {
            _contextAccessor = contextAccessor;
            _objectiveRepository = objectiveRepository;
            _mapper = mapper;
            _keyResultRepository = keyResultRepository;
            _userManager = userManager;
            _departmentRepository = departmentRepository;
            _progressUpdatesRepository = progressUpdatesRepository;
        }

        public async Task<AppResponse<ObjectiveDto>> Create(ObjectiveDto request)
        {
            var result = new AppResponse<ObjectiveDto>();
            try
            {
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                var now = DateTime.UtcNow;
                var user = await _userManager.FindByNameAsync(userName);
                var objectives = _mapper.Map<Objectives>(request);
                objectives.Id = Guid.NewGuid();
                objectives.KeyResults.Clear();
                objectives.CreatedBy = userName;
                var Day = GetDateRange(request.Period + ":" +request.Year);
                objectives.StartDay = Day.StartDate;
                objectives.EndDay = Day.EndDate;
                if (request.TargetType == TargetType.individual)
                {
                    objectives.ApplicationUserId = user.Id;
                }
                else
                {
                    var role = GetUserRole();
                    if((role == "Teamleader" && request.TargetType == TargetType.company) || (role == "Admin" && request.TargetType == TargetType.team))
                    {
                        return result.BuildError("wrong range!");
                    }
                    objectives.DepartmentId = user.DepartmentId;
                }
                var listKeyresults = _mapper.Map<List<KeyResults>>(request.KeyResults);
                listKeyresults.ForEach(x =>
                {
                    x.CreatedBy = userName;
                    x.CreatedOn = now;
                    x.ObjectivesId = objectives.Id;
                    x.Id = Guid.NewGuid();
                });
                _objectiveRepository.Add(objectives,listKeyresults);
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
                data.KeyResults = _mapper.Map<List<KeyResultDto>>(listKeyResult);
                data.LastProgressUpdate = _progressUpdatesRepository.AsQueryable()
                    .Where(x=>x.KeyResults.ObjectivesId == ojective.Id).OrderByDescending(x => x.CreatedOn)
                    .Select(x=>x.CreatedOn)
                    .FirstOrDefault();
                data.Point = _objectiveRepository.caculatePercentObjectivesById(Id);

                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<SearchResponse<ObjectiveDto>>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ObjectiveDto>>();
            try
            {
                var query = await  BuildFilterExpression(request.Filters);
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
                //var objectId_point = _objectiveRepository.caculatePercentObjectives(model);
                var List = model.Include(x=>x.TargetType)
                    .Select(x => new ObjectiveDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        TargetType = x.TargetType,
                        TargetTypeName = getTargetTypeName(x.TargetType),
                        KeyResults = _keyResultRepository.AsQueryable().Where(k=>k.ObjectivesId == x.Id)
                        .Select(k=> new KeyResultDto
                        {
                            Active = k.Active,
                            CurrentPoint = k.CurrentPoint,
                            //Deadline = k.Deadline,
                            Description = k.Description,
                            Id = k.Id,
                            MaximunPoint = k.MaximunPoint,
                            Unit = k.Unit,
                            Status = k.Status
                        }).ToList(),
                        //Point = objectId_point.ContainsKey(x.Id) ? objectId_point[x.Id] : 0
                        ApplicationUserId = x.ApplicationUserId,
                        DepartmentId = x.DepartmentId,
                        EndDay = x.EndDay,
                        IsPublic = x.IsPublic,
                        IsUserObjectives = x.IsUserObjectives,
                        StartDay = x.StartDay,
                        status = x.status,
                        Period = x.Period,
                        Year = x.Year,
                        CreatedBy = x.CreatedBy,
                        CreatedOn = x.CreatedOn
                    })
                    .ToList();
                foreach (var item in List)
                {
                    item.Point = _objectiveRepository.caculatePercentObjectivesById((Guid)item.Id);
                }
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
        private async Task<ExpressionStarter<Objectives>> BuildFilterExpression(List<Filter> Filters)
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
                            case "status":
                                {
                                    if (filter.Value.IsNullOrEmpty())
                                    {
                                        break;
                                    }
                                    var enumN = int.Parse(filter.Value);
                                    StatusObjectives statusObjectives = (StatusObjectives)enumN;
                                    predicate = predicate.And(x => x.status == statusObjectives);
                                    break;
                                }
                            case "period":
                                {
                                    //var parts = filter.Value.Split(':');
                                    //int period = int.Parse(parts[0]);
                                    //int year = int.Parse(parts[1]);

                                    var (quarterStart, quarterEnd) = GetDateRange(filter.Value);

                                    predicate = predicate.And(m =>
                                        (m.StartDay <= quarterEnd && m.EndDay >= quarterStart));
                                    break;
                                }
                            case "name":
                                {
                                    predicate = predicate.And(x=>x.Name.Contains(filter.Value));
                                    break;
                                }
                            case "userName":
                                {
                                    predicate = await BuildFilterUserName(predicate, filter.Value);
                                    break;
                                }
                            case "departmentId":
                                {
                                    predicate = predicate.And(x=>x.DepartmentId == Guid.Parse(filter.Value));
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                if (Filters == null || !Filters.Any(f => f.FieldName == "period"))
                {
                    var currentQuarterRange = GetCurrentQuarterDateRange();
                    predicate = predicate.And(m => m.StartDay <= currentQuarterRange.Item2 && m.EndDay >= currentQuarterRange.Item1);
                }
                predicate = await AddDefaultConditions(predicate, Filters);
                return predicate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AppResponse<int>> CaculateOveralProgress(SearchRequest request)
        {
            var result = new AppResponse<int>();
            try
            {
                //var filter = request.Filters.Where(x=>x.FieldName == "targetType").First();
                //TargetType targetType = (TargetType)int.Parse(filter.Value);
                var query = await BuildFilterExpression(request.Filters);
                var numOfRecords = _objectiveRepository.CountRecordsByPredicate(query);
                var model = _objectiveRepository.FindByPredicate(query);
               
               
                //var test = model.ToList();
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
                var objectives = _objectiveRepository.FindBy(x=>x.Id == request.Id).First();
                var keyresults = _keyResultRepository.FindBy(x=>x.ObjectivesId == objectives.Id).ToList();
                //edit
                keyresults.ForEach(x =>
                {
                    var dto = request.KeyResults.Where(dto => dto.Id == x.Id).FirstOrDefault();
                    if(dto == null)
                    {
                        //delete
                        x.IsDeleted = true;
                    }
                    else
                    {
                        var change = AreKeyResultsEqual(dto, x);
                        if (!change)
                        {
                            //x.Deadline = dto.EndDay.Value;
                            x.IsDeleted = false;
                            x.MaximunPoint = (int)dto.MaximunPoint;
                            x.Percentage = (int)dto.Percentage;
                            x.Modifiedby = userName;
                            x.ModifiedOn = DateTime.UtcNow;
                            x.Status = dto.Status;
                            x.Description = dto.Description;
                        }
                    }
                    
                });
                //create 
                var listNewKeyresult = new List<KeyResults>();
                var newKeyresults = request.KeyResults.Where(x=>x.Id == null).ToList();
                newKeyresults.ForEach(x =>
                {
                    var newkeyresult = _mapper.Map<KeyResults>(x);
                    newkeyresult.Id = Guid.NewGuid();
                    newkeyresult.CreatedBy = userName;
                    newkeyresult.ObjectivesId = objectives.Id;
                    listNewKeyresult.Add(newkeyresult);
                });
                objectives.Name = request.Name;
                objectives.TargetType = request.TargetType;
                objectives.Period = request.Period;
                var Day = GetDateRange(request.Period + ":" + request.Year);
                objectives.StartDay = Day.StartDate;
                objectives.EndDay = Day.EndDate;
                objectives.status = request.status;
                _objectiveRepository.Edit(objectives, keyresults, listNewKeyresult);
                result.BuildResult(request);

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
        public async Task<AppResponse<StatusStatistics>> StatusStatistics(SearchRequest request)
        {
            var result = new AppResponse<StatusStatistics>();
            try
            {
                var expression = await BuildFilterExpression(request.Filters);
                var query = _objectiveRepository.FindByPredicate(expression);
                var data = new StatusStatistics
                {
                    atRisk = query.Where(x => x.status == StatusObjectives.atRisk).Count(),
                    closed = query.Where(x=>x.status == StatusObjectives.closed).Count(),
                    noStatus = query.Where(x=>x.status == StatusObjectives.noStatus).Count(),
                    offTrack = query.Where(x=>x.status == StatusObjectives.offTrack).Count(),
                    onTrack = query.Where(x=>x.status == StatusObjectives.onTrack).Count(),
                    total = query.Count(),
                };
                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
        private ExpressionStarter<Objectives> BuildFilterTargetType(ExpressionStarter<Objectives> predicate, List<Filter> Filters)
        {
            var filter = Filters.Where(x => x.FieldName == "targetType").First();
            var enumN = int.Parse(filter.Value);
            TargetType targetType = (TargetType)enumN;
            
            predicate = predicate.And(x => x.TargetType == targetType);
            if (targetType == TargetType.individual || targetType == TargetType.company)
            {
                return predicate;
            }
            else if(targetType == TargetType.team)
            {
                Department department = new Department();
                ApplicationUser user = new ApplicationUser();
                var filterCreateBy = Filters.Where(x => x.FieldName == "createBy").FirstOrDefault();
                if (filterCreateBy == null)
                {
                    user = _userManager.Users.Where(x => x.UserName == _contextAccessor.HttpContext.User.Identity.Name).FirstOrDefault();
                }
                else
                {
                    user = _userManager.Users.Where(x => x.UserName == filterCreateBy.Value).FirstOrDefault();
                }
                if (user.DepartmentId == null)
                {
                    throw new Exception("User does not have a department.");
                }
                predicate = predicate.And(x=>x.DepartmentId == user.DepartmentId);

            }

            return predicate;
        }
        public string GetUserRole()
        {
            var user = _contextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return null; 
            }
            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return role;
        }


        private (DateTime StartDate, DateTime EndDate) GetDateRange(string timePeriod)
        {
            var parts = timePeriod.Split(':');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid time period format. Expected format is {period}:{year}.");
            }

            string period = parts[0];
            if (!int.TryParse(parts[1], out int year))
            {
                throw new ArgumentException("Invalid year format.");
            }

            DateTime startDate, endDate;

            switch (period)
            {
                case "Q1":
                    startDate = new DateTime(year, 1, 1);
                    endDate = new DateTime(year, 3, 31);
                    break;
                case "Q2":
                    startDate = new DateTime(year, 4, 1);
                    endDate = new DateTime(year, 6, 30);
                    break;
                case "Q3":
                    startDate = new DateTime(year, 7, 1);
                    endDate = new DateTime(year, 9, 30);
                    break;
                case "Q4":
                    startDate = new DateTime(year, 10, 1);
                    endDate = new DateTime(year, 12, 31);
                    break;
                case "H1":
                    startDate = new DateTime(year, 1, 1);
                    endDate = new DateTime(year, 6, 30);
                    break;
                case "H2":
                    startDate = new DateTime(year, 7, 1);
                    endDate = new DateTime(year, 12, 31);
                    break;
                case "FY": // Full year
                    startDate = new DateTime(year, 1, 1);
                    endDate = new DateTime(year, 12, 31);
                    break;
                default:
                    throw new ArgumentException("Invalid period format. Expected Q1, Q2, Q3, Q4, H1, H2, or FY.");
            }

            return (startDate, endDate);
        }

      
        public async Task<ApplicationUser> CurrentUser()
        {
            var user = new ApplicationUser();
            user = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
            return user;
        }
        public AppResponse<List<string>> GetPeriods()
        {
            var result = new AppResponse<List<string>>();
            try
            {
                var list = _objectiveRepository.AsQueryable().Distinct().Select(x => x.Period + ":" + x.Year).ToList();
                result.BuildResult(list);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
        private bool AreKeyResultsEqual(KeyResultDto dto, KeyResults entity)
        {
            if (dto == null || entity == null)
            {
                return false;
            }

            return dto.Description == entity.Description &&
                   dto.Active == entity.Active &&
                   dto.EndDay == entity.Deadline &&
                   dto.Unit == entity.Unit &&
                   //dto.CurrentPoint == entity.CurrentPoint &&
                   dto.MaximunPoint == entity.MaximunPoint &&
                   dto.Percentage == entity.Percentage &&
                   dto.Status == entity.Status;
        }
        private (DateTime, DateTime) GetCurrentQuarterDateRange()
        {
            var currentDate = DateTime.Now;
            int currentQuarter = (currentDate.Month - 1) / 3 + 1;
            DateTime quarterStart = new DateTime(currentDate.Year, (currentQuarter - 1) * 3 + 1, 1);
            DateTime quarterEnd = quarterStart.AddMonths(3).AddDays(-1);
            return (quarterStart, quarterEnd);
        }

        private async Task<ExpressionStarter<Objectives>> BuildFilterUserName(ExpressionStarter<Objectives> predicate, string userName)
        {
            var User = await _userManager.FindByNameAsync(userName);
            predicate = predicate.And(x => x.ApplicationUserId == User.Id 
                || (x.DepartmentId == User.DepartmentId && User.DepartmentId != null));
            //var test = _objectiveRepository.AsQueryable().Where(x => (x.DepartmentId == User.DepartmentId )).ToList();
            //predicate = predicate.And(x=>x.IsPublic == true);
            return predicate;
        }
        private async Task<ExpressionStarter<Objectives>> AddDefaultConditions(ExpressionStarter<Objectives> predicate, List<Filter> filters)
        {
            predicate = predicate.And(x=>x.IsDeleted != true);
            var currentUser = await CurrentUser();
            //predicate = predicate.And(x => x.CreatedBy == currentUser.UserName || x.IsPublic == true);
            var filUseName = filters.Where(x=>x.FieldName == "userName").FirstOrDefault();
            if(filUseName == null || 
                (filUseName != null && filUseName.Value == currentUser.UserName)
                )
            {
                predicate = predicate.And(x => x.ApplicationUserId == currentUser.Id
               || (x.DepartmentId == currentUser.DepartmentId && currentUser.DepartmentId != null));
                //predicate = predicate.And(x => x.IsPublic == true || x.IsPublic == false);
            }
            else if(filUseName != null && filUseName.Value != currentUser.UserName)
            {
                var User = await _userManager.FindByNameAsync(filUseName.Value);
                predicate = predicate.And(x => x.ApplicationUserId == User.Id
              || (x.DepartmentId == User.DepartmentId && User.DepartmentId != null));
                predicate = predicate.And(x=>x.IsPublic == true);
            }
            return predicate;
        }
    }
}
