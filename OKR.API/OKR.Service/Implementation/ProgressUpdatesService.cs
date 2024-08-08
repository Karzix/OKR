using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Service.Contract;
using System;
using System.Data.Entity;
using System.Diagnostics;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace OKR.Service.Implementation
{
    public class ProgressUpdatesService : IProgressUpdatesService
    {
        private IMapper _mapper;
        private IProgressUpdatesRepository _progressUpdatesRepository;
        private IHttpContextAccessor _httpContextAccessor;

        public ProgressUpdatesService(IMapper mapper, IProgressUpdatesRepository progressUpdatesRepository, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _progressUpdatesRepository = progressUpdatesRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        public AppResponse<SearchResponse<ProgressUpdatesDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ProgressUpdatesDto>>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _progressUpdatesRepository.CountRecordsByPredicate(query);
                var progress = _progressUpdatesRepository.FindByPredicate(query);
                if (request.SortBy != null)
                {
                    progress = _progressUpdatesRepository.addSort(progress, request.SortBy);
                }
                else
                {
                    progress = progress.OrderByDescending(x => x.CreatedOn);
                }
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var progressList = progress.Skip(startIndex).Take(pageSize).Include(x=>x.KeyResults);
                var dtoList = progressList.Select(x => new ProgressUpdatesDto
                {
                    Id = x.Id,
                    CreateBy = x.CreatedBy,
                    CreateOn = x.CreatedOn,
                    KeyResultId = x.KeyResultId,
                    NewPoint = x.NewPoint,
                    Note = x.Note,
                    OldPoint = x.OldPoint,
                    Unit = x.KeyResults.Unit,
                }).ToList();
                var searchResult = new SearchResponse<ProgressUpdatesDto>
                {
                    TotalRows = numOfRecords,
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    CurrentPage = pageIndex,
                    Data = dtoList,
                };

                result.Data = searchResult;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private ExpressionStarter<ProgressUpdates> BuildFilterExpression(List<Filter> Filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<ProgressUpdates>(true);


                if (Filters != null)
                    foreach (var filter in Filters)
                    {
                        switch (filter.FieldName)
                        {
                            
                            default:
                                break;
                        }
                    }
                var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (Filters.Where(x => x.FieldName == "createBy").Count() == 0)
                {
                    predicate = predicate.And(x => x.CreatedBy.Equals(userName));
                }
                predicate = predicate.And(x => x.IsDeleted != true);
                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
