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
    public interface IDepartmentService
    {
        AppResponse<List<DepartmentRespone>> GetByParentDepartment(Guid parentId);
        AppResponse<List<DepartmentRespone>> GetAll();
        AppResponse<DepartmentRespone> Get(Guid id);
        AppResponse<DepartmentRespone> Update(DepartmentRequest request);
        AppResponse<DepartmentRespone> Create(DepartmentRequest request);
        AppResponse<string> Delete(Guid id);
        AppResponse<SearchResponse<DepartmentRespone>> Search(SearchRequest request);
        AppResponse<List<DepartmentRespone>> GetParentDepartmentByLevel(int level);
        AppResponse<List<int>> GetDepartmentLevelNumber();
        AppResponse<List<DepartmentRespone>> GetDepartByKeyword(string keyword);
    }
}
