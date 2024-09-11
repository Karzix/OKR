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
    public interface IEvaluateTargetService
    {
        AppResponse<SearchResponse<EvaluateTargetDto>> Search(SearchRequest request);
        AppResponse<EvaluateTargetDto> Create(EvaluateTargetDto request);
        AppResponse<EvaluateTargetDto> Edit(EvaluateTargetDto request);
        AppResponse<string> Delete(Guid id);
    }
}
