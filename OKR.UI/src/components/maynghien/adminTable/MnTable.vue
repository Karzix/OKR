<template>
    <div  >
        <el-table :class="scroll == false ? 'admin-table' :'scroll'" :data="datas" @sort-change="handleSortChange" border row-key="id" table-layout="auto" 
            @row-click="handleRowClick" v-loading="loadding == true" :row-class-name="rowClassName">
            <!-- <el-table-column v-for="column in shownCol" :key="column.key" :prop="column.key" :label="column.label"
                :sortable="column.sortable ? 'custom' : 'false'" :visible="column.hidden == false" /> -->
                <el-table-column label="STT" width="50" fixed>
                    <template #default="scope">
                    <span>{{ scope.$index + 1 }}</span>
                    </template>
                </el-table-column>

                <el-table-column type="expand" fixed v-if="isMobile">
                    <template #default="props" class="a">
                        <div class = "element-lv2">
                            <div v-for="element in shownCol.filter(x=>x.hiddenElement == true)" :key="element.key" >
                               <!-- <span> {{ element.label }}: {{ element.key? props.row[element.key]:"" }}</span> -->
                               <el-link v-if="element.inputType == 'link' && element.key && props.row[element.key]"  :href="props.row[element.key]" target="_blank" type="primary">{{ element.label}}</el-link>
                               <div v-else-if="element.key">{{ element.label}}:     {{ props.row[element.key] }}</div>
                            </div>
                        </div>
                        <!-- Thêm các thông tin khác ở đây -->
                    </template>
                </el-table-column>

            <el-table-column v-for="column in isMobile ? shownCol.filter(x=>x.hiddenElement != true) : shownCol" :key="column.key" :label="column.label" :visible="column.hidden == false" :width="scroll ? column.width : undefined"
            :sortable="column.sortable == true ? 'custom' : false" :prop="column.key" :span="1" :fixed="column.fixed && isMobile == false"
            >
                <template #default="scope">
                    <el-link v-if="column.inputType == 'link' && column.key && scope.row[column.key]"  :href="scope.row[column.key]" target="_blank" type="primary">Xem</el-link>
                    <el-link v-else-if="column.inputType == 'phoneNumber' && column.key && scope.row[column.key]" :href="'tel:' + scope.row[column.key]" target="_blank" type="primary">{{hideMiddleNumbers(scope.row[column.key]) }}</el-link>
                    <!-- <span v-else-if="column.key">{{ scope.row[column.key] }}</span> -->
                    
                    <div v-else-if="column.key" v-html="formatHTML(scope.row[column.key])"></div>
                </template>
            </el-table-column>
            <el-table-column label="Operations" v-if="enableDelete || enableEdit || CustomActions.length > 0" fixed="right" :span="2">
                <template #default="scope">
                    <el-button v-if="enableEdit" :icon="Edit" size="small"
                        @click="handleEdit(scope.$index, scope.row)">Edit</el-button>
                    <el-button v-if="enableDelete" :icon="Delete" size="small" type="danger"
                        @click="handleDelete(scope.$index, scope.row)">Delete</el-button>
                    <el-button v-for="action in CustomActions" :icon="action.Icon" size="small" 
                        @click="handleCustomAction(scope.$index, scope.row, action)">{{ action.ActionLabel }}</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
</template>
  
<script setup lang="ts">
import { TableColumn } from './Models/TableColumn'
import { SearchDTOItem } from './Models/SearchDTOItem'
import { ref, watch } from 'vue';
import {
    Check,
    Delete,
    Edit,
    Message,
    Search,
    Star,
    Plus,
} from '@element-plus/icons-vue';
import { CustomActionResponse, type CustomAction } from './Models/CustomAction';
import { handleAPICustom } from './Service/BasicAdminService';

const isMobile = ref(false);

const props = defineProps<{
    columns: TableColumn[];
    datas: SearchDTOItem[];
    enableEdit: boolean;
    enableDelete: boolean;
    CustomActions: CustomAction[];

    scroll?: boolean
    loadding?: boolean
}>();
const emit = defineEmits<{
    (e: 'onEdit', item: SearchDTOItem): void;
    (e: 'onDelete', item: SearchDTOItem): void;
    (e: 'onCustomAction', item: CustomActionResponse): void;
    (e: 'onSortChange', event: any): void;
}>()
const selectedId = ref("");

const shownCol = ref<TableColumn[]>([]);

// column: The column component
// prop: The property associated with the column
// order: 'ascending' or 'descending'

// Perform sorting logic here based on the prop and order
// You can update the tableData array with sorted data
const handleSortChange = (event: any) => {
    emit('onSortChange', event);
};
const getWidth = (column: TableColumn): string => {
    let result: string = "";
    if (column.width != undefined) {
        result += column.width + "%";
    }

    return result;

}
const getValue = (item: SearchDTOItem, key: string): string | number => {
    return item[key];
};

const handleRowClick = (row: any, column: any, event: any) => {

    selectedId.value = row.id;

}

const handleEdit = (index: number, row: SearchDTOItem) => {

    emit("onEdit", row)
}
const handleDelete = (index: number, row: SearchDTOItem) => {
    var result = confirm("Bạn có chắc chắn muốn xóa không?");
    if(result)
    {
        emit("onDelete", row["id"])
    }
}

const handleCustomAction = async (index: number, row: SearchDTOItem, action: CustomAction) => {
    let response: CustomActionResponse = new CustomActionResponse(action, row);



    emit("onCustomAction", response);

}
watch(() => props.columns, () => {
    shownCol.value = props.columns.filter(m => m.hidden == false);

}, { immediate: true })



function hideMiddleNumbers(phoneNumber : string) {
    try{
        const hiddenNumber = phoneNumber.substring(0, 3) + "xxx" + phoneNumber.substring(6);
        return hiddenNumber;
    }
    catch{
        return phoneNumber
    }
}
const rowClassName = ({ row }: { row: SearchDTOItem }) => {
    var className = "";
//   return row.watched === false ? 'row-not-watched' : '';
    if(row.watched === false){
        className = "row-not-watched"
    }
    else if(row.customerCancel == false){
        className = "row-sale-cancel"
    }
    else{
        className = row.statusText;
    }
    return className;
};
function formatHTML(htmlContent : string) {
        // Sử dụng v-pre để bảo vệ khỏi việc escape HTML
        return `<div v-pre style="white-space: normal;">${htmlContent}</div>`;
    }

const checkMobile = () => {
  var chieuRongManHinh = window.innerWidth;
  if(chieuRongManHinh <= 600){
    isMobile.value = true;
  }
}
checkMobile();
</script>
  
<style>
/* Add your table styling here */
.admin-table {
    width: 100%;
}

.scroll{
    overflow-x: scroll;
    height: 80vh !important;
}
.row-not-watched {
  background-color: blue !important; /* Chọn màu xanh tùy ý */
}
.Pending{
    background-color: #e2e73f !important;
}
.Approved{
    background-color: lightgreen !important;
}
.Rejected{
    background-color: #e56363e8 !important;
}
.Cancel{
    background-color: #b166c4 !important;
}
.row-sale-cancel{
    background-color: orange !important;
}
.element-lv2 {
    padding-left: 15px;
    background-color: #f1f0f785;
}
.element-lv2 > div {
    border-bottom: 1px solid rgb(183, 179, 179);
}
.el-scrollbar__bar.is-horizontal {
    height: 8px !important;
}
.el-scrollbar__thumb {
    background-color: blue !important;
}
</style>
  