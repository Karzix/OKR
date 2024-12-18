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
        private IDepartmentProgressApprovalRepository _progressApprovalRepository;

        public ObjectiveService(IHttpContextAccessor contextAccessor, IObjectivesRepository objectiveRepository, IMapper mapper,
            IKeyResultRepository keyResultRepository, UserManager<ApplicationUser> userManager,
            IDepartmentRepository departmentRepository, IProgressUpdatesRepository progressUpdatesRepository, 
            IDepartmentProgressApprovalRepository departmentProgressApprovalRepository)
        {
            _contextAccessor = contextAccessor;
            _objectiveRepository = objectiveRepository;
            _mapper = mapper;
            _keyResultRepository = keyResultRepository;
            _userManager = userManager;
            _departmentRepository = departmentRepository;
            _progressUpdatesRepository = progressUpdatesRepository;
            _progressApprovalRepository = departmentProgressApprovalRepository;
        }

        public async Task<AppResponse<ObjectivesRespone>> Create(ObjectivesRequest request)
        {
            var result = new AppResponse<ObjectivesRespone>();
            try
            {
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                var now = DateTime.UtcNow;
                var user = await _userManager.FindByNameAsync(userName);
                var objectives = _mapper.Map<Objectives>(request);
                objectives.Id = Guid.NewGuid();
                objectives.KeyResults.Clear();
                objectives.CreatedBy = userName;
                var Day = GetDateRange(request);
                if(Day.EndDate < now)
                {
                    return result.BuildError("You need to select the cycle in the current or future time");
                }
                objectives.StartDay = Day.StartDate;
                objectives.EndDay = Day.EndDate;
                if (request.TargetType == TargetType.individual)
                {
                    objectives.ApplicationUserId = user.Id;
                }
                else
                {
                    var role = GetUserRole();
                    if((role == "Teamleader" && request.TargetType == TargetType.company) || (role == "Admin" && request.TargetType == TargetType.department))
                    {
                        return result.BuildError("wrong range!");
                    }
                    if(request.TargetType == TargetType.department) 
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
                result.BuildResult(_mapper.Map<ObjectivesRespone>(objectives));
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

        public AppResponse<ObjectivesRespone> Get(Guid Id)
        {
            var result = new AppResponse<ObjectivesRespone>();
            try
            {
                var ojective = _objectiveRepository.Get(Id);
                var listKeyResult = _keyResultRepository.FindBy(x=>x.ObjectivesId == Id).ToList();
                var data = _mapper.Map<ObjectivesRespone>(ojective);
                data.KeyResults = _mapper.Map<List<KeyResultRespone>>(listKeyResult);
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

        public async Task<AppResponse<SearchResponse<ObjectivesRespone>>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ObjectivesRespone>>();
            try
            {
                var query = await  BuildFilterExpression(request.Filters);
                    var numOfRecords = _objectiveRepository.CountRecordsByPredicate(query);
                var model = _objectiveRepository.FindByPredicate(query);
                if (request.SortBy != null)
                {
                    model = AddSort(model, request.SortBy);
                }
                else
                {
                    model = model.OrderByDescending(x => x.CreatedOn);
                }
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                
                //model = model;
                //var objectId_point = _objectiveRepository.caculatePercentObjectives(model);
                var List = model.Include(x=>x.TargetType).Skip(startIndex).Take(pageSize)
                    .Select(x => new ObjectivesRespone
                    {
                        Id = x.Id,
                        Name = x.Name,
                        TargetType = x.TargetType,
                        TargetTypeName = getTargetTypeName(x.TargetType),
                        KeyResults = _keyResultRepository.AsQueryable().Where(k=>k.ObjectivesId == x.Id)
                        .Select(k=> new KeyResultRespone
                        {
                            IsCompleted = k.IsCompleted,
                            CurrentPoint = k.CurrentPoint,
                            //Deadline = k.Deadline,
                            Description = k.Description,
                            Id = k.Id,
                            MaximunPoint = k.MaximunPoint,
                            Unit = k.Unit,
                            Status = k.Status,
                            Percentage = k.Percentage
                        }).ToList(),
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
                        CreatedOn = x.CreatedOn,
                        NumberOfPendingUpdates = _progressApprovalRepository.AsQueryable().Where(da=>da.KeyResults.ObjectivesId == x.Id 
                            && da.IsDeleted != true).Count(),
                        StatusClose = x.StatusClose,
                    })
                    .ToList();
                foreach (var item in List)
                {
                    item.Point = _objectiveRepository.caculatePercentObjectivesById((Guid)item.Id);
                }
                var searchUserResult = new SearchResponse<ObjectivesRespone>
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
                result.BuildError(ex.Message + " " );
            }
            return result;
        }

        private IQueryable<Objectives> AddSort(IQueryable<Objectives> input, SortByInfo sortByInfo)
        {
            var result = input.AsQueryable();
            switch (sortByInfo.FieldName)
            {

                case "Status":
                    {
                        if (sortByInfo.Ascending != null && sortByInfo.Ascending.Value)
                        {
                            result = result.OrderBy(m => m.status);

                        }
                        else
                        {
                            result = result.OrderByDescending(m => m.status);
                        }
                    }
                    break;
                case "End Date":
                    {
                        if (sortByInfo.Ascending != null && sortByInfo.Ascending.Value)
                        {
                            result = result.OrderBy(m => m.EndDay).ThenBy(x=>x.Period);

                        }
                        else
                        {
                            result = result.OrderByDescending(m => m.EndDay).ThenBy(x => x.Period);
                        }
                    }
                    break;
                default:
                    break;
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
                                    Status statusObjectives = (Status)enumN;
                                    predicate = predicate.And(x => x.status == statusObjectives);
                                    break;
                                }
                            case "period":
                                {
                                    if(filter.Value == "default")
                                    {
                                        var currentQuarterRange = GetCurrentQuarterDateRange();
                                        predicate = predicate.And(m => m.StartDay <= currentQuarterRange.Item2 && m.EndDay >= currentQuarterRange.Item1);
                                        var test = _objectiveRepository.AsQueryable().Where(predicate).ToList();
                                        break;
                                    }
                                    var Day = GetDateRange(filter.Value);
                                    predicate = predicate.And(x=>x.StartDay.Date < Day.EndDate && x.EndDay.Date > x.StartDay);
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


        public AppResponse<ObjectivesRespone> Edit(ObjectivesRequest request)
        {
            var result = new AppResponse<ObjectivesRespone>();
            try
            {
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                var objectives = _objectiveRepository.FindBy(x=>x.Id == request.Id).First();
                if (objectives.StatusClose != null)
                {
                    return result.BuildError("This objectives is close");
                }
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
                var Day = GetDateRange(request);
                objectives.StartDay = Day.StartDate;
                objectives.EndDay = Day.EndDate;
                objectives.status = request.status;
                _objectiveRepository.Edit(objectives, keyresults, listNewKeyresult);
                result.BuildResult(_mapper.Map<ObjectivesRespone>(objectives));

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
                    atRisk = query.Where(x => x.status == Status.atRisk).Count(),
                    closed = query.Where(x=>x.status == Status.closed).Count(),
                    noStatus = query.Where(x=>x.status == Status.noStatus).Count(),
                    offTrack = query.Where(x=>x.status == Status.offTrack).Count(),
                    onTrack = query.Where(x=>x.status == Status.onTrack).Count(),
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
            
            
            if (targetType == TargetType.company)
            {
                predicate = predicate.And(x => x.TargetType == targetType);
                return predicate;
            }
            if(targetType == TargetType.individual)
            {
                string user = "";
                var filterCreeateBy = Filters.Where(x => x.FieldName == "createBy").FirstOrDefault();
                if (filterCreeateBy == null)
                {
                    user =  _contextAccessor.HttpContext.User.Identity.Name;
                }
                else
                {
                    user = filterCreeateBy.Value;
                }
                predicate = predicate.And(x => x.CreatedBy == user);
            }
            else if(targetType == TargetType.department)
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
                predicate = predicate.And(x=>x.DepartmentId == user.DepartmentId && x.TargetType == TargetType.department);

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


        private (DateTime StartDate, DateTime EndDate) GetDateRange(ObjectivesRequest objectivesRequest)
        {
            if (objectivesRequest.Period == "custom")
            {
                return ((DateTime)objectivesRequest.StartDay!, (DateTime)objectivesRequest.EndDay!);
            }
            var parts = objectivesRequest.Period.Split(':');
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
        private (DateTime StartDate, DateTime EndDate) GetDateRange(string Period)
        {
            var parts = Period.Split(':');
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

        public AppResponse<List<string>> GetPeriods()
        {
            var result = new AppResponse<List<string>>();
            try
            {
                var list = _objectiveRepository.AsQueryable().Select(x => x.Period + ":" + x.Year).Distinct().ToList();
                result.BuildResult(list);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
        private bool AreKeyResultsEqual(KeyResultRequest dto, KeyResults entity)
        {
            if (dto == null || entity == null)
            {
                return false;
            }

            return dto.Description == entity.Description &&
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
            var currentUserName = _contextAccessor.HttpContext.User.Identity.Name;
            if(User.ManagerName != currentUserName && User.UserName != currentUserName)
            {
                throw new Exception("You are not the manager of this user");
            }
            else if (User.UserName == currentUserName)
            {
                var currentUser = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
                predicate = predicate.And(x => x.ApplicationUserId == currentUser.Id
              || (x.DepartmentId == currentUser.DepartmentId && currentUser.DepartmentId != null) || x.TargetType == TargetType.company);
                //predicate = predicate.And(x=>x.TargetType == TargetType.company);
            }
            else if(User.UserName != currentUserName)
            {
                predicate = predicate.And(x => x.CreatedBy == User.UserName);
                predicate = predicate.And(x => x.IsPublic == true);
            }
            //predicate = predicate.And(x => x.ApplicationUserId == User.Id
            //   /* || (x.DepartmentId == User.DepartmentId && User.DepartmentId != null)*/);
            //var test = _objectiveRepository.AsQueryable().Where(x => (x.DepartmentId == User.DepartmentId )).ToList();
            //predicate = predicate.And(x=>x.IsPublic == true);
            return predicate;
        }
        private async Task<ExpressionStarter<Objectives>> AddDefaultConditions(ExpressionStarter<Objectives> predicate, List<Filter> filters)
        {
            predicate = predicate.And(x => x.IsDeleted != true);
            if (filters == null || !filters.Any(f => f.FieldName == "period"))
            {
                var currentQuarterRange = GetCurrentQuarterDateRange();
                predicate = predicate.And(m => m.StartDay <= currentQuarterRange.Item2 && m.EndDay >= currentQuarterRange.Item1);
                //var test = _objectiveRepository.AsQueryable().Where(predicate).ToList();
            }
            if (filters == null || !filters.Any(f => f.FieldName == "userName"))
            {
                var currentUser = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
                predicate = predicate.And(x => x.ApplicationUserId == currentUser.Id
              || (x.DepartmentId == currentUser.DepartmentId && currentUser.DepartmentId != null) || x.TargetType == TargetType.company);
                //var test = _objectiveRepository.AsQueryable().Where(predicate).ToList();
            }
            return predicate;
        }

        public AppResponse<string> Close(Guid ObjecivesId, ObjectivesStatusClose status)
        {
            var result  = new AppResponse<string>();
            try
            {
                var objectives  = _objectiveRepository.Get(ObjecivesId);
                objectives.StatusClose = status;
                objectives.Modifiedby = _contextAccessor.HttpContext.User.Identity.Name;
                objectives.ModifiedOn = DateTime.UtcNow;
                _objectiveRepository.Edit(objectives);
                result.BuildResult("OK");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

    }
}
