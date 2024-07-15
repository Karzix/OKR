export class SearchResponse<T>{
    data:T|undefined;
    currentPage:number|undefined;
    totalPages:number|undefined;
    rowsPerPage:number|undefined;
    totalRows:number|undefined;
}