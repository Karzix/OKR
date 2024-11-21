using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;
using System.Data.Entity;
using System.Linq.Expressions;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace OKR.Service.Implementation
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IHttpContextAccessor _httpContextAccessor;
        private IDepartmentRepository _departmentRepository;
        private IMapper _mappper;
        private readonly IMemoryCache _memoryCache;
        private readonly string cacheUserKey = "user_list";

        public UserService(IUserRepository userRepository, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, IDepartmentRepository departmentRepository
            , IMapper mapper, IMemoryCache memoryCache)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _departmentRepository = departmentRepository;
            _mappper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<AppResponse<UserRespone>> Create(UserRequest request)
        {
            var result = new AppResponse<UserRespone>();
            try
            {
                var departmant = request.DepartmentId != null ? _departmentRepository.FindBy(x=> x.Id == request.DepartmentId).FirstOrDefault() : null;
                if (request.DepartmentId != null && departmant == null)
                {
                    return result.BuildError("can't find department!");
                }
                var newUser = new ApplicationUser
                {
                    DepartmentId = request.DepartmentId,
                    Email = request.Email,
                    UserName = request.Email,
                    EmailConfirmed = true,
                    ManagerName = request.ManagerName
                };
                await _userManager.CreateAsync(newUser);
                await _userManager.AddPasswordAsync(newUser, request.Password ?? "Abc@123456");
                if (!(await _roleManager.RoleExistsAsync(request.Role )))
                {
                    IdentityRole role = new IdentityRole { Name = request.Role };
                    await _roleManager.CreateAsync(role);
                }
                await _userManager.AddToRoleAsync(newUser, request.Role);
                result.BuildResult(_mappper.Map<UserRespone>(newUser));
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<string>> LockAsync(UserRequest request, int day = 30)
        {
            var result = new AppResponse<string>();
            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(request.Id.ToString());
                if (user == null)
                {
                    return result.BuildError("can't find user");
                }
                if (user.LockoutEnabled == false)
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    user.LockoutEnd = null;
                    await _userManager.UpdateAsync(user);
                   
                }
                else
                {
                    DateTimeOffset LockoutEndnable = DateTimeOffset.UtcNow.AddDays(day);
                    await _userManager.SetLockoutEnabledAsync(user, false);
                    await _userManager.UpdateAsync(user);
                }
                return result.BuildResult("OK");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<SearchResponse<UserRespone>>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<UserRespone>>();
            try
            {
                var query =  BuildFilterExpression(request.Filters);
                var numOfRecords = _userRepository.CountRecordsByPredicate(query);
                var users = _userRepository.FindByPredicate(query);
                if (request.SortBy != null)
                {
                    users = addSort(users, request.SortBy);
                }
                else
                {
                    users = users.OrderBy(x => x.Email);
                }
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var UserQueryable = users.Skip(startIndex).Take(pageSize).Include(x=>x.Department);
                var UserList = UserQueryable.ToList();
                var dtoList = UserQueryable.Select(x => new UserRespone
                    {
                        Email = x.Email,
                        UserName = x.UserName,
                        Id = (x.Id),
                        DepartmentName = x.DepartmentId != null ? x.Department.Name : "",
                        DepartmentId = x.DepartmentId,
                        ManagerName = x.ManagerName
                    }).ToList();
                if (dtoList != null && dtoList.Count > 0)
                {
                    for (int i = 0; i < dtoList.Count; i++)
                    {
                        var userDto = dtoList[i];
                        var identityUser = UserList[i];
                        userDto.Role = (await _userManager.GetRolesAsync(identityUser)).FirstOrDefault();
                    }
                }
                var searchUserResult = new SearchResponse<UserRespone>
                {
                    TotalRows = numOfRecords,
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    CurrentPage = pageIndex,
                    Data = dtoList,
                };

                result.Data = searchUserResult;
                result.IsSuccess = true;

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<UserRespone>> Update(UserRequest request)
        {
            var result = new AppResponse<UserRespone>();
            try
            {
                var user = await _userManager.FindByIdAsync(request.Id.ToString());
                user.DepartmentId = request.DepartmentId;
                if (request.Role != null)
                {
                    var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    if (role != null)
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                    if (!(await _roleManager.RoleExistsAsync(request.Role)))
                    {
                        IdentityRole findrole = new IdentityRole { Name = request.Role };
                        await _roleManager.CreateAsync(findrole);
                    }
                    await _userManager.AddToRoleAsync(user, request.Role);
                }

                await _userManager.UpdateAsync(user);
                result.BuildResult(_mappper.Map<UserRespone>(user));
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + ex.StackTrace);
            }
            return result;
        }

        private IQueryable<ApplicationUser> addSort(IQueryable<ApplicationUser> input, SortByInfo sortByInfo)
        {
            var result = input.AsQueryable();
            var type = sortByInfo.FieldName;
            type = char.ToUpper(type[0]) + type.Substring(1);

            var param = Expression.Parameter(typeof(ApplicationUser), "m");
            var property = Expression.Property(param, type);
            var lambda = Expression.Lambda<Func<ApplicationUser, object>>(Expression.Convert(property, typeof(object)), param);


            if (sortByInfo.Ascending != null && sortByInfo.Ascending.Value)
            {
                result = result.OrderBy(lambda);
            }
            else
            {
                result = result.OrderByDescending(lambda);
            }

            return (IQueryable<ApplicationUser>)result;
        }

        private ExpressionStarter<ApplicationUser> BuildFilterExpression(List<Filter> Filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<ApplicationUser>(true);


                if (Filters != null)
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "userName":
                                predicate = predicate.And(m => m.UserName.Contains(filter.Value));
                                break;
                            case "departmentId":
                                predicate = predicate.And(x => x.DepartmentId == Guid.Parse(filter.Value));
                                break;
                            default:
                                break;
                        }
                    }

                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public AppResponse<List<UserRespone>> GetAll()
        {
            var result = new AppResponse<List<UserRespone>>();
            try
            {
                var data = _userRepository.FindByPredicate(x => true).Select(x => new UserRespone
                {
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    Email = x.Email,
                    Id = (x.Id),
                    UserName = x.UserName,
                }).ToList();
                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + ex.StackTrace);
            }
            return result;
        }
        public async Task<AppResponse<UserRespone>> Get(string userName)
        {
            var result = new AppResponse<UserRespone>();
            try
            {
                var applicationUser = await _userManager.FindByNameAsync(userName);
                var userDto = _mappper.Map<UserRespone>(applicationUser);
                var department = _departmentRepository.FindBy(x=>x.Id == applicationUser.DepartmentId).FirstOrDefault();
                if(department != null)
                {
                    userDto.DepartmentName = department.Name;
                }
                result.BuildResult(userDto);
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<List<UserRespone>> GetListByKeyworld(string userName)
        {
            var result = new AppResponse<List<UserRespone>>();
            try
            {
                if (!_memoryCache.TryGetValue(cacheUserKey, out List<UserRespone> users))
                {
                    int page = 0;
                    users = new List<UserRespone>();
                    while (true)
                    {
                        var list = _userRepository.AsQueryable().Skip(page*500).Take(500).Select(x=> new UserRespone
                        {
                            DepartmentId = x.DepartmentId,
                            Email = x.Email,
                            Id = (x.Id),
                            UserName = x.UserName,
                        }).ToList();
                        if(list.Count() == 0)
                        {
                            break;
                        }
                        users.AddRange(list);
                        list.Clear();
                        page++;
                    }

                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        Priority = CacheItemPriority.NeverRemove
                    };

                    // Lưu vào bộ nhớ cache
                    _memoryCache.Set(cacheUserKey, users, cacheEntryOptions);
                    result.BuildResult(users.Where(x=>x.UserName.Contains(userName)).Take(10).ToList());
                }
                else
                {
                    result.BuildResult(users.Where(x => x.UserName.Contains(userName)).Take(10).ToList());
                }
            }
            catch(Exception ex)
            {
                result.BuildError(ex.Message + " -> " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<string>> ChangePassword(UserRequest request)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = await _userManager.FindByNameAsync(request.Email);
                if (user == null)
                {
                    return result.BuildError("cannot find user");
                }
                var checkCurPassword = await _userManager.CheckPasswordAsync(user,request.Password);
                if(checkCurPassword == false)
                {
                    return result.BuildError("Current passowrd wrong!");
                }
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                    return result.BuildError($"Failed to change password: {errors}");
                }
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
