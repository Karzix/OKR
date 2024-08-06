using AutoMapper;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;

namespace OKR.Service.Implementation
{
    public class KeyResultsService : IKeyResultsService
    {
        private IKeyResultRepository _keyResultRepository;
        private IHttpContextAccessor _contextAccessor;
        private IMapper _mapper;
        private IProgressUpdatesRepository _progressUpdatesRepository;

        public KeyResultsService(IKeyResultRepository keyResultRepository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IProgressUpdatesRepository progressUpdatesRepository)
        {
            _keyResultRepository = keyResultRepository;
            _contextAccessor = httpContextAccessor;
            _mapper = mapper;
            _progressUpdatesRepository = progressUpdatesRepository;
        }

        public AppResponse<KeyResultDto> Update(KeyResultDto request)
        {
            var result = new AppResponse<KeyResultDto>();
            try
            {
                
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                var keyresult = _keyResultRepository.Get(request.Id.Value);
                if (request.CurrentPoint == null || request.CurrentPoint > keyresult.MaximunPoint)
                {
                    return result.BuildError("current point is invalid");
                }
                var progressUpdates = new ProgressUpdates();
                var updateString = request.Note.IsNullOrEmpty() ? GetUpdateString(request, keyresult) : request.Note;
                progressUpdates.CreatedBy = userName;
                progressUpdates.CreatedOn = DateTime.UtcNow;
                progressUpdates.Note = updateString;
                progressUpdates.KeyResultId = keyresult.Id;
                progressUpdates.OldPoint = keyresult.CurrentPoint;
                progressUpdates.NewPoint = request.CurrentPoint.Value;

                keyresult.MaximunPoint = (int)request.MaximunPoint;
                keyresult.CurrentPoint = (int)request.CurrentPoint;
                keyresult.Description = request.Description;
                _keyResultRepository.Edit(keyresult);
                

                
                _progressUpdatesRepository.Add(progressUpdates);

                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private string GetUpdateString(KeyResultDto NewKeyResult, KeyResults CurKeyResults)
        {
            string content = _contextAccessor.HttpContext.User.Identity.Name + " ";
            if(NewKeyResult.Description != CurKeyResults.Description)
            {
                content += "update keyresults name from " + CurKeyResults.Description + " to " + NewKeyResult.Description + "; ";
            }
            if (NewKeyResult.CurrentPoint != CurKeyResults.CurrentPoint)
            {
                content += "update weights from " + CurKeyResults.Description + " to " + NewKeyResult.Description + "; ";
            }
            if(NewKeyResult.Deadline != CurKeyResults.Deadline)
            {
                content += "update deadline from " + CurKeyResults.Deadline.ToString("dd/MM/yyyy") + " to " + NewKeyResult.Deadline.Value.ToString("dd/MM/yyyy") + "; ";
            }
            return content;
        }
    }
}
