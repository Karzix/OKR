import {Filter} from "@/components/maynghien/BaseModels/Filter";
function isString(value: any) {
    return typeof value === 'string';
}
export function addFilter(listFilter : Filter[], add : Filter){
    var findFilName = listFilter.find(x => x.FieldName == add.FieldName);
    if(findFilName == null){
        listFilter.push(add);
    }
    else{
        findFilName.Value = add.Value;
    }
}

export function removeFilter(listFilter : Filter[], remove : string){
    var findFilName = listFilter.find(x => x.FieldName == remove);
    if(findFilName != null){
        listFilter = listFilter.filter(x => x.FieldName != remove);
    }
    return listFilter;
}

export function handleFilterBeforSearch(listFilter? : Filter[]) : Filter[]{
    var result = [] as Filter[];
    if(!listFilter){
        listFilter = [] as Filter[];
    }
    listFilter.forEach(x => {
        var newf = new Filter();
        newf.FieldName == x.FieldName;
        // newf.Value == x.Value;
        newf.Operation == x.Operation;
        newf.Type == x.Type;
        newf.dropdownData == x.dropdownData;
        newf.DisplayName == x.DisplayName;

        if(x.Value != null && x.Value != undefined && x.Value != ""){
            if(!isString(x.Value)){
                newf.Value = (x.Value as string).toString();
            }
            else{
                newf.Value = x.Value;
            }
            result.push(newf);
        }
    });
    return result;
}
