// @ts-ignore
import {Filter} from '../BaseModels/Filter';
// @ts-ignore
import {SortByInfo} from "../BaseModels/SortByInfo";


export class SearchRequest {
    
    PageIndex: number | undefined;
    PageSize:number |undefined;
    filters:Filter[]|undefined;
    SortBy:SortByInfo|undefined;
}