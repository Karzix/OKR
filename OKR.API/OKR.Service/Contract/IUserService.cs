using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using OKR.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Service.Contract
{
    public interface IUserService
    {
        Task<AppResponse<UserDto>> Create(UserDto request);
        Task<AppResponse<UserDto>> Update(UserDto request);
        Task<AppResponse<string>> LockAsync(UserDto request, int day = 30);
        Task<AppResponse<SearchResponse<UserDto>>> Search(SearchRequest request);
        AppResponse<List<UserDto>> GetAll();
        Task<AppResponse<UserDto>> Get(string iduserName);
    }
}
