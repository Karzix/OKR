// @strict: false
export class CustomAction {
    ActionName?: string = "";
    ActionLabel?: string = "";
    Icon?: any;
    IsRowAction?: boolean = false;
    ApiAction?: string;
    ApiActiontype?: ApiActionType =ApiActionType.POST ;
    DataType?: CustomActionDataType = CustomActionDataType.RowId;

}

export class CustomActionResponse {
    Action: CustomAction;
    Data: any;
    constructor(action: CustomAction, data: any) {
        this.Action = action;
        this.Data = data;
    }
}

export enum CustomActionDataType {
    MultiRowId, FullRow, RowId,Filters,null
}
export enum ApiActionType {
    POST, GET, PUT, DELETE
}