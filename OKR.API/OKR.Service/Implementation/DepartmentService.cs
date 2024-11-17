using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Repository.Implementation;
using OKR.Service.Contract;
using System.Linq;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace OKR.Service.Implementation
{
    public class DepartmentService : IDepartmentService
    {
        private IDepartmentRepository _departmentRepository;
        private IHttpContextAccessor _contextAccessor;
        private IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly string cacheDepartmentKey = "department_list";

        public DepartmentService(IDepartmentRepository departmentRepository, IHttpContextAccessor contextAccessor,
            IMapper mapper, IMemoryCache memoryCache)
        {
            _departmentRepository = departmentRepository;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public AppResponse<DepartmentRespone> Create(DepartmentRequest request)
        {
            var result = new AppResponse<DepartmentRespone>();
            try
            {
                var parentDepartment =  _departmentRepository.FindBy(x=>x.Id == request.ParentDepartmentId);
                if (parentDepartment.Count() == 0 && request.ParentDepartmentId != null)
                {
                    return result.BuildError("can't find Paren epartment");
                }
                var newDeparment = _mapper.Map<Department>(request);
                newDeparment.Id = Guid.NewGuid();
                newDeparment.CreatedBy = _contextAccessor.HttpContext.User.Identity.Name;
                newDeparment.Level = 1; 
                _departmentRepository.Add(newDeparment);

                request.Id = newDeparment.Id;
                result.BuildResult(_mapper.Map<DepartmentRespone>(newDeparment));

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
                var deparment =_departmentRepository.Get(id);
                deparment.IsDeleted = true;
                _departmentRepository.Edit(deparment);
                result.BuildResult("OK");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<DepartmentRespone> Get(Guid id)
        {
            var result = new AppResponse<DepartmentRespone>();
            try
            {
                var department =_departmentRepository.Get(id);
                var dto = _mapper.Map<DepartmentRespone>(department);
                result.BuildResult(dto);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<List<DepartmentRespone>> GetAll()
        {
            var result = new AppResponse<List<DepartmentRespone>>();
            try
            {
                var data = _departmentRepository.AsQueryable().Select(x => new DepartmentRespone
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentDepartmentId = x.ParentDepartmentId,
                    ParentDepartmentName = x.ParentDepartmentId != null ? x.ParentDepartment.Name : "",
                }).ToList();
                data = BuildTree(data);
                result.BuildResult(data);


                var test = _departmentRepository.GetParentOfChildDepartment(3, Guid.Parse("f0adc7e4-0f54-49a2-9e22-91efcf18f069"));

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<List<DepartmentRespone>> GetByParentDepartment(Guid parentId)
        {
            var result = new AppResponse<List<DepartmentRespone>>();
            try
            {
                var data = _departmentRepository.FindBy(x=>x.ParentDepartmentId == parentId)
                    .Select(x => new DepartmentRespone
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ParentDepartmentId = x.ParentDepartmentId,
                    }).ToList();
                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<DepartmentRespone> Update(DepartmentRequest request)
        {
            var result = new AppResponse<DepartmentRespone>();
            try
            {
                var parentDepartment = _departmentRepository.FindBy(x => x.Id == request.ParentDepartmentId);
                if (parentDepartment.Count() == 0 && request.ParentDepartmentId == null)
                {
                    return result.BuildError("can't find Paren epartment");
                }
                var department = _departmentRepository.Get(request.Id.Value);
                var currentParentDepaetment = department.ParentDepartmentId;
                department.Name = request.Name;
                department.ParentDepartmentId = request.ParentDepartmentId;
                department.Modifiedby = _contextAccessor.HttpContext.User.Identity.Name;
                department.ModifiedOn = DateTime.UtcNow;
                if(currentParentDepaetment != request.ParentDepartmentId && currentParentDepaetment != null)
                {
                    department.Level = _departmentRepository.Get(currentParentDepaetment.Value).Level + 1;
                }
                _departmentRepository.Edit(department);

                var allChild = _departmentRepository.GetAllChildDepartments(department.Id);
                allChild.ForEach(x => x.Level = x.Level + 1);

                _departmentRepository.EditRange(allChild);

                result.BuildResult(_mapper.Map<DepartmentRespone>(department));
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<SearchResponse<DepartmentRespone>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<DepartmentRespone>>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _departmentRepository.CountRecordsByPredicate(query);
                var users = _departmentRepository.FindByPredicate(query);
                if (request.SortBy != null)
                {
                    users = _departmentRepository.addSort(users, request.SortBy);
                }
                else
                {
                    users = users.OrderBy(x => x.Name);
                }
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var UserList = users.Skip(startIndex).Take(pageSize);
                var dtoList = UserList.Select(x => new DepartmentRespone
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentDepartmentId = x.ParentDepartmentId,
                    ParentDepartmentName = x.ParentDepartmentId != null ? x.ParentDepartment.Name : "",
                    level = x.Level,
                }).ToList();
                var searchResult = new SearchResponse<DepartmentRespone>
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

        private ExpressionStarter<Department> BuildFilterExpression(List<Filter> Filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<Department>(true);


                if (Filters != null)
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "name":
                                predicate = predicate.And(m => m.Name.Contains(filter.Value));
                                break;
                            case "level":
                                predicate = predicate.And(m=>m.Level == int.Parse(filter.Value)); 
                                break;
                            case "parentDepartmentId":
                                predicate = predicate.And(m => m.ParentDepartmentId == Guid.Parse(filter.Value));
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

        public AppResponse<List<DepartmentRespone>> GetParentDepartmentByLevel(int level)
        {
            var result = new AppResponse<List<DepartmentRespone>>();
            try
            {
                var data = _departmentRepository.AsQueryable().Where(x=>x.Level == (level - 1))
                    .Select(x=> new DepartmentRespone
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ParentDepartmentId = x.ParentDepartmentId,
                        ParentDepartmentName = x.ParentDepartmentId != null ? x.ParentDepartment.Name : ""
                    }).ToList();
                result.BuildResult(data);
            }
            catch (Exception ex) 
            { 
                result.BuildError(ex.Message + " " + ex.StackTrace);    
            }
            return result;
        }

        private List<DepartmentRespone> BuildTree(List<DepartmentRespone> departments)
        {
            var departmentDict = departments.ToDictionary(d => d.Id);
            var rootDepartments = new List<DepartmentRespone>();

            foreach (var department in departments)
            {
                if (department.ParentDepartmentId == null)
                {
                    rootDepartments.Add(department);
                }
                else if (departmentDict.TryGetValue(department.ParentDepartmentId, out var parentDepartment))
                {
                    if (parentDepartment.Zones == null)
                    {
                        parentDepartment.Zones = new List<DepartmentRespone>();
                    }
                    parentDepartment.Zones.Add(department);
                }
            }

            return rootDepartments;
        }

        public AppResponse<List<int>> GetDepartmentLevelNumber()
        {
            var result = new AppResponse<List<int>>();
            try
            {
                var max = _departmentRepository.AsQueryable().Max(x=>x.Level);
                var listINT = new List<int>();
                for (int i = 1; i <= max; i++)
                {
                    listINT.Add(i);
                }
                result.BuildResult(listINT);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<List<DepartmentRespone>> GetDepartByKeyword(string keyword)
        {
            var result = new AppResponse<List<DepartmentRespone>>();
            try
            {
                if (!_memoryCache.TryGetValue(cacheDepartmentKey, out List<DepartmentRespone> departments))
                {
                    int page = 0;
                    departments = new List<DepartmentRespone>();
                    while (true)
                    {
                        var list = _departmentRepository.AsQueryable().Skip(page * 500).Take(500).Select(x => new DepartmentRespone
                        {
                            Id = x.Id,
                            Name = x.Name,
                        }).ToList();
                        if (list.Count() == 0)
                        {
                            break;
                        }
                        departments.AddRange(list);
                        list.Clear();
                        page++;
                    }

                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        Priority = CacheItemPriority.NeverRemove
                    };

                    // Lưu vào bộ nhớ cache
                    _memoryCache.Set(cacheDepartmentKey, departments, cacheEntryOptions);
                    result.BuildResult(departments.Where(x => x.Name.Contains(keyword)).Take(10).ToList());
                }
                else
                {
                    result.BuildResult(departments.Where(x => x.Name.Contains(keyword)).Take(10).ToList());
                }
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

    }
}
