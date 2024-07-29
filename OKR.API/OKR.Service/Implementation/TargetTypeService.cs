using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Helpers;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace OKR.Service.Implementation
{
    public class TargetTypeService : ITargetTypeService
    {
        private readonly IMapper _mapper;
        private ITargetTypeRepository _targetTypeRepository;
        private IHttpContextAccessor _httpContextAccessor;

        public TargetTypeService(IMapper mapper, IHttpContextAccessor httpContextAccessor, ITargetTypeRepository targetTypeRepository)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _targetTypeRepository = targetTypeRepository;
        }

        public AppResponse<TargetTypeDto> Create(TargetTypeDto request)
        {
            var result = new AppResponse<TargetTypeDto>();
            try
            {
                //var UserName = ClaimHelper.GetClainByName(_httpContextAccessor, "UserName");
                //if (UserName == null)
                //{
                //    return result.BuildError("Cannot find Account by this user");
                //}
                var targetType = _mapper.Map<TargetType>(request);
                targetType.Id = Guid.NewGuid();
                //targetType.CreatedBy = UserName;
                _targetTypeRepository.Add(targetType);
                request.Id = targetType.Id;
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> Delete(Guid Id)
        {
            var result = new AppResponse<string>();
            try
            {
                var targetType = _targetTypeRepository.Get(Id);
                targetType.IsDeleted = true;
                _targetTypeRepository.Edit(targetType);
                result.BuildResult("OK");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<TargetTypeDto> Edit(TargetTypeDto request)
        {
            var result = new AppResponse<TargetTypeDto>();
            try
            {
               
                var targetType = _targetTypeRepository.Get(request.Id.Value);
                targetType.Name = request.Name;
                targetType.LevelApply = request.LevelApply;
                _targetTypeRepository.Edit(targetType);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<TargetTypeDto> Get(Guid id)
        {
            var result = new AppResponse<TargetTypeDto>();
            try
            {
                var targetType = _targetTypeRepository.Get(id);
                var data = _mapper.Map<TargetTypeDto>(targetType);
                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<List<TargetTypeDto>> GetAll()
        {
            var result = new AppResponse<List<TargetTypeDto>>();
            try
            {
                var list = _targetTypeRepository.GetAll().Where(x => x.IsDeleted == false).Select(x => new TargetTypeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                result.BuildResult(list);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<SearchResponse<TargetTypeDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<TargetTypeDto>>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _targetTypeRepository.CountRecordsByPredicate(query);
                var model = _targetTypeRepository.FindByPredicate(query);
                if (request.SortBy != null)
                {
                    model = _targetTypeRepository.addSort(model, request.SortBy);
                }
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var List = model.Skip(startIndex).Take(pageSize)
                    .Select(x => new TargetTypeDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        LevelApply = x.LevelApply,

                    })
                    .ToList();


                var searchUserResult = new SearchResponse<TargetTypeDto>
                {
                    TotalRows = numOfRecords,
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    CurrentPage = pageIndex,
                    Data = List,
                };
                result.BuildResult(searchUserResult);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }


        private ExpressionStarter<TargetType> BuildFilterExpression(IList<Filter> Filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<TargetType>(true);
                if (Filters != null)
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "name":
                                predicate = predicate.And(m => m.Name.Contains(filter.Value));
                                break;
                            default:
                                break;
                        }
                    }
                predicate = predicate.And(m => m.IsDeleted == false);
                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
