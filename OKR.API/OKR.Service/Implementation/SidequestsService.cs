using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Repository.Implementation;
using OKR.Service.Contract;

namespace OKR.Service.Implementation
{
    public class SidequestsService : ISidequestsService
    {
        private ISidequestsRepository _sidequestsRepository;
        private IHttpContextAccessor _contextAccessor;
        private IProgressUpdatesRepository _progressUpdatesRepository;
        private IKeyResultRepository _keyResultRepository;
        private IObjectivesRepository _objectivesRepository;

        public SidequestsService(ISidequestsRepository sidequestsRepository, IHttpContextAccessor contextAccessor,
            IProgressUpdatesRepository progressUpdatesRepository, IKeyResultRepository keyResultRepository, IObjectivesRepository objectivesRepository)
        {
            _sidequestsRepository = sidequestsRepository;
            _contextAccessor = contextAccessor;
            _progressUpdatesRepository = progressUpdatesRepository;
            _keyResultRepository = keyResultRepository;
            _objectivesRepository = objectivesRepository;
        }

        public AppResponse<SidequestsDto> ChangeStatus(SidequestsDto request)
        {
            var result = new AppResponse<SidequestsDto>();
            try
            {
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                var sidequests = _sidequestsRepository.Get(request.Id.Value);
                sidequests.Status = request.Status.Value;
                _sidequestsRepository.Edit(sidequests);

                var progressUpdate = new ProgressUpdates
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = userName,
                    CreatedOn = DateTime.UtcNow,
                    Note = userName + " change " + request.Name + " to " + (request.Status.Value == true ? "true": "false"),
                    KeyResultId = sidequests.KeyResultsId,
                };
                var keyresult = _keyResultRepository.Get(sidequests.KeyResultsId);
                progressUpdate.KeyresultCompletionRate = _keyResultRepository.caculatePercentKeyResults(keyresult);
                Dictionary<Guid, int> op = _objectivesRepository.caculatePercentObjectives(_objectivesRepository.AsQueryable().Where(x => x.Id == keyresult.ObjectivesId));
                progressUpdate.ObjectivesCompletionRate = op.ContainsKey(keyresult.ObjectivesId) ? op[keyresult.ObjectivesId] : 0;
                _progressUpdatesRepository.Add(progressUpdate);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message+ " " + ex.StackTrace);
            }
            return result;
        }
    }
}
