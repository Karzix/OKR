
export class TableColumn {
    key: string = "";
    label?: string = "";
    width?: number = 0;
    sortable?: boolean = false;
    enableEdit?: boolean = true;

    enableCreate?: boolean = true;
    required?: boolean = false;
    hidden?: boolean = false;
    showSearch?: boolean = false;
    inputType?: string = "text";
    dropdownData?: any | null;
    fixed?: boolean = false;
    hiddenElement?: boolean = false;
    constructor() {

    };

}
