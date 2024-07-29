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
        AppResponse<List<DepartmentDto>> GetByParentDepartment(Guid parentId);
        AppResponse<List<DepartmentDto>> GetAll();
        AppResponse<DepartmentDto> Get(Guid id);
        AppResponse<DepartmentDto> Update(DepartmentDto request);
        AppResponse<DepartmentDto> Create(DepartmentDto request);
        AppResponse<string> Delete(Guid id);
        AppResponse<SearchResponse<DepartmentDto>> Search(SearchRequest request);
        AppResponse<List<DepartmentDto>> GetParentDepartmentByLevel(int level);
        AppResponse<List<int>> DepartmentLevelNumber();
    }
}
