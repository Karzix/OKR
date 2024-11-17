using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using OKR.DTO;

namespace OKR.Service.Contract
{
    public interface IObjectivesService
    {
        public Task<AppResponse<ObjectivesRespone>> Create(ObjectivesRequest request);
        public Task<AppResponse<SearchResponse<ObjectivesRespone>>> Search(SearchRequest request);
        public AppResponse<ObjectivesRespone> Get(Guid Id);
        public AppResponse<string> Delete(Guid Id);
        public Task<AppResponse<int>> CaculateOveralProgress(SearchRequest request);
        public AppResponse<ObjectivesRespone> Edit(ObjectivesRequest request);
        public AppResponse<List<string>> GetPeriods();
        public Task<AppResponse<StatusStatistics>> StatusStatistics(SearchRequest request);
    }
}
