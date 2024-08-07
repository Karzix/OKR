using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;

namespace OKR.Service.Implementation
{
    public class SidequestsService : ISidequestsService
    {
        private ISidequestsRepository _sidequestsRepository;
        private IHttpContextAccessor _contextAccessor;
        private IProgressUpdatesRepository _progressUpdatesRepository;

        public SidequestsService(ISidequestsRepository sidequestsRepository, IHttpContextAccessor contextAccessor, IProgressUpdatesRepository progressUpdatesRepository)
        {
            _sidequestsRepository = sidequestsRepository;
            _contextAccessor = contextAccessor;
            _progressUpdatesRepository = progressUpdatesRepository;
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
