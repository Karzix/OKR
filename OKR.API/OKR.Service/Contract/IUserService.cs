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
        Task<AppResponse<UserRespone>> Create(UserRequest request);
        Task<AppResponse<UserRespone>> Update(UserRequest request);
        Task<AppResponse<string>> LockAsync(UserRequest request, int day = 30);
        Task<AppResponse<SearchResponse<UserRespone>>> Search(SearchRequest request);
        AppResponse<List<UserRespone>> GetAll();
        Task<AppResponse<UserRespone>> Get(string iduserName);
        AppResponse<List<UserRespone>> GetListByKeyworld(string userName);
        Task<AppResponse<string>> ChangePassword(UserRequest request);
    }
}
