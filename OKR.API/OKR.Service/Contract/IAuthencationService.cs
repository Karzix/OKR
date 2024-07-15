using MayNghien.Models.Response.Base;
using OKR.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Service.Contract
{
    public interface IAuthencationService
    {
        public Task<AppResponse<LoginResult>> AuthencationUser(UserDto user);
    }
}
