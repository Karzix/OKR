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
    public interface ITargetTypeService
    {
        AppResponse<List<TargetTypeDto>> GetAll();
        AppResponse<TargetTypeDto> Get(Guid id);
        AppResponse<TargetTypeDto> Edit(TargetTypeDto request);
        AppResponse<TargetTypeDto> Create(TargetTypeDto request);
        AppResponse<string> Delete(Guid Id);
        AppResponse<SearchResponse<TargetTypeDto>> Search(SearchRequest request);
    }
}
