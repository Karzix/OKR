using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using OKR.DTO;

namespace OKR.Service.Contract
{
    public interface IObjectivesService
    {
        public AppResponse<ObjectiveDto> Create(ObjectiveDto request);
        public AppResponse<SearchResponse<ObjectiveDto>> Search(SearchRequest request);
        public AppResponse<ObjectiveDto> Get(Guid Id);
        public AppResponse<string> Delete(Guid Id);
        public AppResponse<int> CaculateOveralProgress(SearchRequest request);
        public AppResponse<ObjectiveDto> Edit(ObjectiveDto request);
    }
}
