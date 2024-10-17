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
    public interface IDepartmentProgressApprovalService
    {
        AppResponse<SearchResponse<DepartmentProgressApprovalDto>> Search(SearchRequest request);
    }
}
