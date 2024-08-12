using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
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

        public DepartmentService(IDepartmentRepository departmentRepository, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        public AppResponse<DepartmentDto> Create(DepartmentDto request)
        {
            var result = new AppResponse<DepartmentDto>();
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
                newDeparment.Level = request.ParentDepartmentId == null ? 1 : parentDepartment.First().Level + 1; 
                _departmentRepository.Add(newDeparment);

                request.Id = newDeparment.Id;
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

        public AppResponse<DepartmentDto> Get(Guid id)
        {
            var result = new AppResponse<DepartmentDto>();
            try
            {
                var department =_departmentRepository.Get(id);
                var dto = _mapper.Map<DepartmentDto>(department);
                result.BuildResult(dto);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<List<DepartmentDto>> GetAll()
        {
            var result = new AppResponse<List<DepartmentDto>>();
            try
            {
                var data = _departmentRepository.GetAll().Select(x => new DepartmentDto
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

        public AppResponse<List<DepartmentDto>> GetByParentDepartment(Guid parentId)
        {
            var result = new AppResponse<List<DepartmentDto>>();
            try
            {
                var data = _departmentRepository.FindBy(x=>x.ParentDepartmentId == parentId)
                    .Select(x => new DepartmentDto
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

        public AppResponse<DepartmentDto> Update(DepartmentDto request)
        {
            var result = new AppResponse<DepartmentDto>();
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

                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<SearchResponse<DepartmentDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<DepartmentDto>>();
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
                var dtoList = UserList.Select(x => new DepartmentDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentDepartmentId = x.ParentDepartmentId,
                    ParentDepartmentName = x.ParentDepartmentId != null ? x.ParentDepartment.Name : "",
                    level = x.Level,
                }).ToList();
                var searchResult = new SearchResponse<DepartmentDto>
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

        public AppResponse<List<DepartmentDto>> GetParentDepartmentByLevel(int level)
        {
            var result = new AppResponse<List<DepartmentDto>>();
            try
            {
                var data = _departmentRepository.GetAll().Where(x=>x.Level == (level - 1))
                    .Select(x=> new DepartmentDto
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

        private List<DepartmentDto> BuildTree(List<DepartmentDto> departments)
        {
            var departmentDict = departments.ToDictionary(d => d.Id);
            var rootDepartments = new List<DepartmentDto>();

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
                        parentDepartment.Zones = new List<DepartmentDto>();
                    }
                    parentDepartment.Zones.Add(department);
                }
            }

            return rootDepartments;
        }

        public AppResponse<List<int>> DepartmentLevelNumber()
        {
            var result = new AppResponse<List<int>>();
            try
            {
                var max = _departmentRepository.GetAll().Max(x=>x.Level);
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

    }
}
