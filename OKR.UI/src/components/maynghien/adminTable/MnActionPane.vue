<template>
    <el-row class="action-pane">
        <el-col :span="24">
            <el-row class="ep-bg-purple-dark el-col-24">
                <el-col :span="24">
                    <el-row>
                        <div v-for="filter in filters">

                            <el-input v-model="filter.Value" :placeholder="filter.DisplayName"
                                v-if="filter.Type == undefined || filter.Type == 'text'" class="action-input">
                            </el-input>
                            <el-select v-model="filter.Value" :placeholder="filter.DisplayName" class="action-input"
                                v-if="filter.Type == 'dropdown'"  filterable>
                                <el-option label="" value="" />

                                <el-option v-for="item in filter.dropdownData.data"
                                    :key="item[filter.dropdownData.keyMember]"
                                    :label="item[filter.dropdownData.displayMember]"
                                    :value="item[filter.dropdownData.keyMember]" />
                            </el-select>
                            <div class="block"  v-if="filter.Type == 'date'" style="display: flex; align-items: center;justify-content: center; gap:5px">
                                <span class="demonstration">{{filter.DisplayName}} <el-icon style="font-size: 30px;"><Calendar /></el-icon></span>
                                <el-date-picker
                                    v-model="filter.Value"
                                    type="daterange"
                                    range-separator="To"
                                    start-placeholder="Start date"
                                    end-placeholder="End date"
                                    format="DD/MM/YYYY"        
                                    value-format="DD/MM/YYYY"  
                                    style="width: 290px;"
                                />
                              </div>
                        </div>
                        <el-button v-if="filters != undefined && filters.length > 0" :icon="Search"
                            @click="handlebtnSearchClicked"> </el-button>

                    </el-row>

                </el-col>



            </el-row>
            <el-row>
                <el-col :span="12" class="buttons p-1">
                    <el-button v-for="customAction in CustomActions" :icon="customAction.Icon"
                        @click="handlebtnCustomActionClicked(customAction)">
                        {{ customAction.ActionLabel }}
                    </el-button>
                    <el-button :icon="Plus" @click="handlebtnAddClicked" v-if="allowAdd"> Create</el-button>
                </el-col>

            </el-row>
        </el-col>
    </el-row>
</template>

<script setup lang="ts">
import {
    Check,
    Delete,
    Edit,
    Message,
    Search,
    Star,
    Plus,
    Calendar
} from '@element-plus/icons-vue';

// @ts-ignore
import { TableColumn } from './Models/TableColumn.ts'

// @ts-ignore
import { Filter } from '../BaseModels/Filter';

import { ref, watch } from 'vue';
import { CustomActionResponse, CustomAction } from './Models/CustomAction';
import { handleAPIGetDropdownList } from './Service/BasicAdminService';
const props = defineProps<{
    tableColumns: TableColumn[];
    allowAdd: boolean;
    CustomActions: CustomAction[];
}>();

const emit = defineEmits<{
    (e: 'onBtnAddClicked'): void;
    (e: 'onBtnSearchClicked', filters: Filter[]): void;
    (e: 'onCustomAction', item: CustomActionResponse): void;
}>();
const filters = ref<Filter[]>([]);

const dropdownData = ref<any[]>([]);
props.tableColumns.forEach(colum => {
    if (colum.showSearch) {
        const newFilter: Filter = {
            FieldName: colum.key,
            DisplayName: colum.label,
            Value: "",
            Operation: "",
            Type: colum.inputType,
            dropdownData: colum.dropdownData,
        };
        filters.value?.push(newFilter);
    }
});
const handlebtnAddClicked = () => {
    //console.log("btn add click");
    emit("onBtnAddClicked");
    //console.log("not err");
}
const handlebtnSearchClicked = () => {
    const filtersRequest = filters.value.slice().filter(filter => filter.Value !== null && filter.Value !== undefined && filter.Value != "");
    for(let i = 0; i < filtersRequest.length; i++){
        if(filtersRequest[i].Type == "date"){
            var value = filtersRequest[i].Value?.toString();
            var filename = filtersRequest[i].FieldName?.toString()
            filtersRequest.splice(i,1);
            filtersRequest.push({FieldName: filename, DisplayName: filename, Value: value, Operation: "", Type: "text", dropdownData: undefined});
            i--;
        }
    }
    emit("onBtnSearchClicked", filtersRequest);
}

const handlebtnCustomActionClicked = async (action: CustomAction) => {
    let response: CustomActionResponse = new CustomActionResponse(action, filters);
    emit("onCustomAction", response);
}
watch(() => props.tableColumns, async () => {
    props.tableColumns.forEach(async tableCol => {
        if ((tableCol.inputType == "dropdown" ||  tableCol.inputType == "tree")&& tableCol.dropdownData.apiUrl != undefined) {
            var data = await handleAPIGetDropdownList(tableCol.dropdownData.apiUrl);
            console.log(data);

            if (data != undefined && data.data) {
                tableCol.dropdownData.data = data.data;
            }

        }


    });

    console.log(props.tableColumns);
    filters.value = [];
    props.tableColumns.forEach(colum => {
        if (colum.showSearch) {
            const newFilter: Filter = {
                FieldName: colum.key,
                DisplayName: colum.label,
                Value: "",
                Operation: "",
                Type: colum.inputType,
                dropdownData: colum.dropdownData,
            };
            filters.value?.push(newFilter);
        }
    });

}, { immediate: true })

</script>
<style scoped>
.action-pane {
    width: "100%";
    padding: 10px;
}

.action-pane .buttons {
    padding: 5px 0px;
}

.action-pane .action-input {
    padding-right: 5px;
}
</style>