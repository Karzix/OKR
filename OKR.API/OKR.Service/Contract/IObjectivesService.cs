using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using OKR.DTO;

namespace OKR.Service.Contract
{
    public interface IObjectivesService
    {
        public Task<AppResponse<ObjectiveDto>> Create(ObjectiveDto request);
        public Task<AppResponse<SearchResponse<ObjectiveDto>>> Search(SearchRequest request);
        public AppResponse<ObjectiveDto> Get(Guid Id);
        public AppResponse<string> Delete(Guid Id);
        public Task<AppResponse<int>> CaculateOveralProgress(SearchRequest request);
        public AppResponse<ObjectiveDto> Edit(ObjectiveDto request);
        public AppResponse<List<string>> GetPeriods();
        public Task<AppResponse<StatusStatistics>> StatusStatistics(SearchRequest request);
    }
}
