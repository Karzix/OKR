export class SearchDTOItem {
    [key: string]: any;
    constructor(columns:TableColumn[]){
        columns.forEach(column => {
            if(column.key!=undefined)
                this[column.key]= null;
        });
    }
}
import {TableColumn} from "./TableColumn"