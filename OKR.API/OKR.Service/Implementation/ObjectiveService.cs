using AutoMapper;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Service.Implementation
{
    public class ObjectiveService : IObjectiveService
    {
        private IHttpContextAccessor _contextAccessor;
        private IObjectiveRepository _objectiveRepository;
        private IMapper _mapper;
        private ITargetTypeRepository _targetTypeRepository;
        private IKeyResultRepository _keyResultRepository;

        public ObjectiveService(IHttpContextAccessor contextAccessor, IObjectiveRepository objectiveRepository, IMapper mapper,
            ITargetTypeRepository targetTypeRepository, IKeyResultRepository keyResultRepository)
        {
            _contextAccessor = contextAccessor;
            _objectiveRepository = objectiveRepository;
            _mapper = mapper;
            _targetTypeRepository = targetTypeRepository;
            _keyResultRepository = keyResultRepository;
        }

        public AppResponse<ObjectiveDto> Create(ObjectiveDto request)
        {
            var result = new AppResponse<ObjectiveDto>();
            try
            {
                var userName = _contextAccessor.HttpContext.User.Identity.Name;
                var targetType = _targetTypeRepository.FindBy(x=>x.Id == request.TargetTypeId);
                if(targetType.Count() == 0)
                {
                    return result.BuildError("Need to select target type");
                }
                var objective =_mapper.Map<Objective>(request);
                objective.Id = Guid.NewGuid();
                objective.CreatedBy = userName; 
                var keyResults = _mapper.Map<List<KeyResults>>(request.ListKeyResults);
                foreach (var item in keyResults)
                {
                    item.Id = Guid.NewGuid();
                    item.CreatedBy = userName;
                }
                _objectiveRepository.Add(objective,keyResults);


                request.Id = objective.Id;
                request.ListKeyResults = _mapper.Map<List<KeyResultDto>>(keyResults);

                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<string> Delete(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<ObjectiveDto> Get(Guid Id)
        {
            var result = new AppResponse<ObjectiveDto>();
            try
            {
                var ojective = _objectiveRepository.Get(Id);
                var listKeyResult = _keyResultRepository.FindBy(x=>x.ObjectiveId == Id).ToList();
                var data = _mapper.Map<ObjectiveDto>(ojective);
                data.ListKeyResults = _mapper.Map<List<KeyResultDto>>(listKeyResult);

                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<SearchResponse<ObjectiveDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ObjectiveDto>>();
            try
            {

            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
