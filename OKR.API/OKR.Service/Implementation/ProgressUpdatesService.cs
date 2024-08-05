﻿using AutoMapper;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using OKR.Repository.Implementation;
using OKR.Service.Contract;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace OKR.Service.Implementation
{
    public class ProgressUpdatesService : IProgressUpdatesService
    {
        private IMapper _mapper;
        private IProgressUpdatesRepository _progressUpdatesRepository;

        public ProgressUpdatesService(IMapper mapper, IProgressUpdatesRepository progressUpdatesRepository)
        {
            _mapper = mapper;
            _progressUpdatesRepository = progressUpdatesRepository;
        }


        public AppResponse<SearchResponse<ProgressUpdatesDto>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ProgressUpdatesDto>>();
            try
            {
                var query = BuildFilterExpression(request.Filters);
                var numOfRecords = _progressUpdatesRepository.CountRecordsByPredicate(query);
                var users = _progressUpdatesRepository.FindByPredicate(query);
                if (request.SortBy != null)
                {
                    users = _progressUpdatesRepository.addSort(users, request.SortBy);
                }
                else
                {
                    users = users.OrderByDescending(x => x.CreatedOn);
                }
                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * (int)pageSize;
                var UserList = users.Skip(startIndex).Take(pageSize);
                var dtoList = UserList.Select(x => new ProgressUpdatesDto
                {
                    Id = x.Id,
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
                predicate = predicate.And(x => x.IsDeleted == false);
                return predicate;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
