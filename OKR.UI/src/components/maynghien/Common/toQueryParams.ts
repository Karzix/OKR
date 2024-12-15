import type { SearchRequest } from "../BaseModels/SearchRequest";

export function toQueryParams(searchRequest: SearchRequest): string {
  const { filters, SortBy, PageIndex, PageSize } = searchRequest;
  const filterParams: Record<string, string> = filters
    ? filters.reduce((acc, filter, index) => {
        acc[`Filters[${index}].FieldName`] = filter.FieldName!;
        acc[`Filters[${index}].Value`] = filter.Value!;
        if (filter.Operation) {
          acc[`Filters[${index}].Operation`] = filter.Operation;
        }
        return acc;
      }, {} as Record<string, string>)
    : {};
  const sortByParams: Record<string, string> = SortBy
    ? {
        "SortBy.FieldName": SortBy.FieldName ?? "",
        "SortBy.Ascending": SortBy.Ascending.toString(),
      }
    : {};
  const queryParams: Record<string, string> = {
    ...filterParams,
    ...sortByParams,
    PageIndex: (PageIndex ?? 1).toString(),
    PageSize: (PageSize ?? 0).toString(),
  };
  return new URLSearchParams(queryParams).toString();
}
