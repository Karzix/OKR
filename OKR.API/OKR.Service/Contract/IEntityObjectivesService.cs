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
    public interface IEntityObjectivesService
    {
        AppResponse<SearchResponse<EntityObjectivesDto>> Search(SearchRequest request);
        AppResponse<EntityObjectivesDto> Get(Guid id);
    }
}
