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
    public interface IProgressUpdatesService
    {
        AppResponse<SearchResponse<ProgressUpdatesDto>> Search(SearchRequest request);
        AppResponse<List<DataChart>> GetDataChart(SearchRequest request);
    }
}
