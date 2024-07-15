
// @ts-ignore
import { AppResponse } from '../../../../Models/AppResponse.js'

// @ts-ignore
import { SearchRequest } from '../../BaseModels/SearchRequest.js'

// @ts-ignore
import { SearchResponse } from '../../BaseModels/SearchResponse.js'
// @ts-ignore
import { axiosInstance } from "../../../../Services/axiosConfig.js"
// @ts-ignore
import {Filter} from '../../BaseModels/Filter.js'
// @ts-ignore
import {SortByInfo} from '../../BaseModels/SortByInfo.js';


// @ts-ignore
import { TableColumn } from '../Models/TableColumn.ts'
// @ts-ignore
import { SearchDTOItem } from '../Models/SearchDTOItem.ts'
import { ApiActionType, CustomAction, CustomActionResponse } from '../Models/CustomAction.js'
import type { AxiosResponse } from 'axios'


export const handleAPISearch = async (model: SearchRequest, apiurl: string, searchUrl?: string): Promise<AppResponse<SearchResponse<SearchDTOItem[] | undefined>>> => {

    let resust: AppResponse<SearchResponse<SearchDTOItem[] | undefined>> = ({
        isSuccess: false,
        message: '',
        data: undefined
    });

    try {
        var url = searchUrl ? searchUrl : "/search";
        const postResult = await axiosInstance.post(apiurl + url, model);
        console.log(postResult.data);
        const responseObject = postResult.data
        resust = responseObject;
        if (resust.isSuccess) {
            return resust;
        }
        else {
            console.log(resust.message);
            return resust;
        }
    } catch (error) {
        console.error(error);

    }
    return resust;

}

export const handleAPICreate = async (model: SearchDTOItem, apiurl: string): Promise<AppResponse<SearchDTOItem | undefined>> => {

    let resust: AppResponse<SearchDTOItem | undefined> = ({
        isSuccess: false,
        message: '',
        data: undefined
    });

    try {
        const postResult = await axiosInstance.post(apiurl, model);
        console.log(postResult.data);
        const responseObject = postResult.data
        resust = responseObject;
        if (resust.isSuccess) {
            return resust;
        }
        else {
            console.log(resust.message);
            return resust;
        }
    } catch (error) {
        console.error(error);

    }
    return resust;

}

export const handleAPIUpdate = async (model: SearchDTOItem, apiurl: string): Promise<AppResponse<SearchDTOItem | undefined>> => {

    let resust: AppResponse<SearchDTOItem | undefined> = ({
        isSuccess: false,
        message: '',
        data: undefined
    });

    try {
        const postResult = await axiosInstance.put(apiurl, model);
        console.log(postResult.data);
        const responseObject = postResult.data
        resust = responseObject;
        if (resust.isSuccess) {
            return resust;
        }
        else {
            console.log(resust.message);
            return resust;
        }
    } catch (error) {
        console.error(error);

    }
    return resust;

}

export const handleAPIDelete = async (id: string, apiurl: string): Promise<AppResponse<undefined>> => {

    let resust: AppResponse<undefined> = ({
        isSuccess: false,
        message: '',
        data: undefined
    });

    try {
        const deleteUrl = apiurl + "/" + id;
        const postResult = await axiosInstance.delete(deleteUrl);
        console.log(postResult.data);
        const responseObject = postResult.data
        resust = responseObject;
        if (resust.isSuccess) {
            return resust;
        }
        else {
            console.log(resust.message);
            return resust;
        }
    } catch (error) {
        console.error(error);

    }
    return resust;

}


export const handleAPICustom = async (model: SearchDTOItem, action: CustomAction, ActionUrl: string): Promise<AppResponse<CustomActionResponse>> => {

    let resust: AppResponse<CustomActionResponse> = ({
        isSuccess: false,
        message: '',
        data: undefined
    });

    try {
        let postResult: any = "";
        if (action.ApiActiontype != undefined && action.ApiActiontype == ApiActionType.PUT) {
            postResult = await axiosInstance.put(ActionUrl, model);
        }
        if (action.ApiActiontype != undefined && action.ApiActiontype == ApiActionType.POST) {
            postResult = await axiosInstance.post(ActionUrl, model);
        }
        if (action.ApiActiontype != undefined && action.ApiActiontype == ApiActionType.DELETE) {
            postResult = await axiosInstance.delete(ActionUrl, model);
        }
        if (action.ApiActiontype != undefined && action.ApiActiontype == ApiActionType.GET) {
            postResult = await axiosInstance.get(ActionUrl + "/" + model.id);
        }

        console.log(postResult.data);
        const responseObject = postResult.data
        resust = responseObject;
        if (resust.isSuccess) {
            return resust;
        }
        else {
            console.log(resust.message);
            return resust;
        }
    } catch (error) {
        console.error(error);

    }
    return resust;

}
export const handleAPIGetDropdownList = async (apiurl: string): Promise<AppResponse<any[] | undefined>> => {

    let resust: AppResponse<SearchDTOItem[] | undefined> = ({
        isSuccess: false,
        message: '',
        data: undefined
    });

    try {
        const listResult = await axiosInstance.get(apiurl);
        
        const responseObject = listResult.data
        resust = responseObject;
        if (resust.isSuccess) {
            return resust;
        }
        else {
            console.log(resust.message);
            return resust;
        }
    } catch (error) {
        console.error(error);

    }
    return resust;

}

