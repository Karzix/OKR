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
                    <div v-else-if="column.inputType == 'dropdown' && column.key && scope.row[column.key]" style="white-space: normal;">
                        {{ getDropdownDisplayValue(column.dropdownData, scope.row[column.key])  }}
                    </div>
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
import { onMounted, onUnmounted, ref, watch } from 'vue';
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

const handleResize = () => {
  isMobile.value = window.innerWidth < 600;

};
onMounted(() => {
  window.addEventListener("resize", handleResize);
  handleResize(); // Gọi hàm này để kiểm tra kích thước ban đầu
});

onUnmounted(() => {
  window.removeEventListener("resize", handleResize);
});

function getDropdownDisplayValue(dropdownData: { data: any[], keyMember: string, displayMember: string }, key: any): string {
    const item = dropdownData.data.find(x => x[dropdownData.keyMember] === key);
    return item ? item[dropdownData.displayMember] : '';
}

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
    padding: 10px;
    background-color: #f9f9f9;
    border: 1px solid #ddd;
    border-radius: 4px;
    margin-bottom: 10px;
}
.element-lv2 > div {
    padding: 5px 0;
    border-bottom: 1px solid #eee;
}
.element-lv2 > div:last-child {
    border-bottom: none;
}
.el-scrollbar__bar.is-horizontal {
    height: 8px !important;
}


.expand-transition {
    transition: all 0.3s ease;
    overflow: hidden;
}
.expand-transition-enter-active, .expand-transition-leave-active {
    max-height: 500px;
}
.expand-transition-enter, .expand-transition-leave-to /* .expand-transition-leave-active in <2.1.8 */ {
    max-height: 0;
}
</style>
<style scoped>
/* Basic reset for the table */
.admin-table,
.scroll {
    width: 100%;
    border-collapse: collapse;
    margin: 20px 0;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

/* Styling for table headers */
.admin-table th,
.scroll th {
    background-color: #f4f4f4;
    color: #333;
    padding: 12px 15px;
    text-align: left;
    font-weight: bold;
}

/* Styling for table cells */
.admin-table td,
.scroll td {
    padding: 12px 15px;
    border-bottom: 1px solid #ddd;
}

/* Alternate row color for better readability */
.admin-table tr:nth-child(even),
.scroll tr:nth-child(even) {
    background-color: #f9f9f9;
}

/* Hover effect for rows */
.admin-table tr:hover,
.scroll tr:hover {
    background-color: #f1f1f1;
}

/* Fixed column styling */
.admin-table th.fixed,
.scroll th.fixed,
.admin-table td.fixed,
.scroll td.fixed {
    position: sticky;
    left: 0;
    background-color: white;
    z-index: 2;
}

/* Responsive styles for mobile devices */
@media (max-width: 768px) {
    .admin-table,
    .scroll {
        font-size: 14px; /* Smaller text on mobile */
    }

    .admin-table th,
    .scroll th,
    .admin-table td,
    .scroll td {
        padding: 10px; /* Less padding on mobile */
    }

    .admin-table {
        display: block; /* Make the table block to allow for scrolling */
        overflow-x: auto; /* Allow horizontal scrolling */
    }

    .element-lv2 {
        display: flex;
        flex-direction: column; /* Stack elements vertically */
        padding: 10px;
    }
}

/* Button styling */
.el-button {
    margin: 0 5px; /* Spacing between buttons */
    border-radius: 4px; /* Rounded corners */
}

/* Link styling */
.el-link {
    color: #3498db; /* Link color */
    text-decoration: none; /* Remove underline */
}

.el-link:hover {
    text-decoration: underline; /* Underline on hover */
}
</style>