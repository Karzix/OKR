<template>
    <el-dialog :model-value="openDialog" :title="(isEdit ? 'Edit ' : 'Create ') + title" class="form-dialog"
        :width="dialogWidth" @close="emit('onCloseClicked')">

        <div class="editform" v-if="model != undefined">
            <div v-for="column in columns" :key="column.key">
                <div v-if="(isEdit && column.enableEdit == true) || (!isEdit && column.enableCreate == true)">
                    <!-- Use double curly braces to bind variable values in templates -->
                    <label>{{ column.label }}</label>

                    <el-input v-model="model[column.key]" :placeholder="column.label"
                        v-if="column.key != undefined && (column.inputType == undefined || column.inputType == 'text' || column.inputType == 'number' || column.inputType == 'link' || column.inputType == 'textarea')"
                        :type="column.inputType" />

                    <el-select v-if="column.key != undefined && (column.inputType == 'dropdown')"
                        v-model="model[column.key]" :placeholder="column.label" class="action-input" filterable>
                        <el-option label="" value="" />
                        <el-option v-for="item in column.dropdownData.data" :key="item[column.dropdownData.keyMember]"
                            :label="item[column.dropdownData.displayMember]" :value="item[column.dropdownData.keyMember]" />
                    </el-select>

                    <div v-if="column.key != undefined && (column.inputType == 'tree')" class="tree-view">
                        <el-tree 
                            style="max-width: 600px"
                            :props="treeProps"
                            :data="column.dropdownData.data"
                            @node-click="(data:any)=>{ if(column.key) model[column.key] = data.id; console.log(data.id) }"
                            :highlight-current="true"
                        />
                        <!-- {{ model[column.key] }} -->
                    </div>

                </div>

            </div>

        </div>
        <template #footer>
            <span class="dialog-footer">
                <el-button @click="emit('onCloseClicked')">Cancel</el-button>
                <el-button type="primary" @click="Save">
                    Confirm
                </el-button>
            </span>
        </template>
    </el-dialog>
</template>
  
<script setup lang="ts">
import { ref, toRefs, computed, watch, inject } from 'vue';
// @ts-ignore
import { ElMessage, ElInput } from 'element-plus';
// @ts-ignore
import { handleAPICreate, handleAPIUpdate } from './Service/BasicAdminService.ts'
import type { TableColumn } from './Models/TableColumn';
import MnDropdown from './Input/MnDropdown.vue';
// @ts-ignore
import { SearchDTOItem } from './Models/SearchDTOItem.ts';
const emit = defineEmits<{
    (e: 'onSaved'): void;
    (e: 'onCloseClicked'): void;

}>()
const props = defineProps<{
    columns: TableColumn[];
    editItem: SearchDTOItem;
    apiName: string;
    createUrl?: string;
    editUrl?: string;
    isEdit: boolean;
    openDialog: boolean;
    title: string;
}>();
interface Tree {
  id: number
  name: string
  leaf?: boolean
  zones?: Tree[]
}
const treeProps = {
    label: 'name',
    children: 'zones'
};
const dialogWidth = ref('35%');
if (window.innerWidth < 600) {
    dialogWidth.value = '100%';
}
// Use computed to create a filtered model
const model = ref<SearchDTOItem>(props.editItem);
const Validate = (): boolean => {
    if (model != undefined)
        props.columns.forEach(column => {
            if (column.enableEdit && column.key != undefined) {
                const value = model.value[column.key];
                if (column.key == "id" && props.isEdit) {
                    if (value == undefined)
                        return false;
                }
                if (column.required && (value == undefined || value == "")) {
                    return false;
                }
            }

        });
    else return false;
    return true;
}
const Save = async () => {
    const valid = Validate();
    if (valid) {
        if (props.isEdit == true && props.editItem != undefined) {
            const editUrl = props.apiName + (props.editUrl != undefined ? "/" + props.editUrl : "");
            var editresult = await handleAPIUpdate(props.editItem, editUrl);
            if (editresult.isSuccess) {
                ElMessage({
                    message: 'data Updated.',
                    type: 'success',
                });
            }
            else {
                ElMessage.error('Update failed.');
                return;
            }
        }
        else if (props.editItem != undefined) {
            const createUrl = props.apiName + (props.createUrl != undefined ? "/" + props.createUrl : "");
            var createresult = await handleAPICreate(props.editItem, createUrl);
            if (createresult.isSuccess) {
                ElMessage({
                    message: 'data Created.',
                    type: 'success',
                });
            }
            else {
                ElMessage.error('Create failed.');
                return;
            }
        }
        emit("onSaved");
    }
    else {
        ElMessage.error('valid failed.');
    }
}
const handleUpdateValue = (key: string | undefined, value: string): void => {
    if (key != undefined) {
        model.value[key] = value;
        console.log(model.value);
    }

}

watch(() => props.editItem, () => {
    model.value = props.editItem;
}, { immediate: true })
</script>

<style> 
.form-dialog {
    margin-top: 0 !important;
    margin-right: 0 !important;
    height: 100%;
}

.editform .el-select {
    width: 100%;
}
.tree-view{
    border: 2px solid #ccc;
    border-radius: 5px;
}
@media screen and (max-width: 650px) {
    .form-dialog{
        width: 100% !important;
    }
}
</style>
<style scoped>
/* Dialog styling */
.form-dialog {
    border-radius: 8px; /* Rounded corners for the dialog */
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); /* Soft shadow for depth */
}

/* Header styling */
.form-dialog .el-dialog__header {
    background-color: #f5f5f5; /* Light background for header */
    color: #333; /* Dark text color for contrast */
    padding: 15px; /* Padding for the header */
    border-bottom: 1px solid #e0e0e0; /* Subtle bottom border */
}

/* Input field styling */
.editform {
    padding: 15px; /* Padding inside the form */
}

/* Label styling */
.editform label {
    display: block; /* Make label block for better spacing */
    margin-bottom: 5px; /* Space between label and input */
    font-weight: bold; /* Bold text for emphasis */
    color: #555; /* Darker color for readability */
}

/* Input and select styling */
.el-input,
.el-select {
    width: 100%; /* Full width for inputs */
    margin-bottom: 15px; /* Space below each input */
    border-radius: 4px; /* Rounded corners for inputs */
    border: 1px solid #ccc; /* Light border color */
    transition: border-color 0.2s; /* Smooth transition for border color */
}

/* Focus effect for inputs */
.el-input:focus,
.el-select:focus {
    border-color: #3498db; /* Change border color on focus */
    box-shadow: 0 0 5px rgba(52, 152, 219, 0.5); /* Add shadow on focus */
}

/* Tree view styling */
.tree-view {
    margin-top: 10px; /* Space above the tree view */
}

/* Dialog footer styling */
.dialog-footer {
    display: flex;
    justify-content: flex-end; /* Align buttons to the right */
    padding: 10px; /* Padding for the footer */
    background-color: #f9f9f9; /* Light background for footer */
    border-top: 1px solid #e0e0e0; /* Subtle top border */
}

/* Button styling */
.el-button {
    margin-left: 10px; /* Space between buttons */
    border-radius: 4px; /* Rounded corners */
}

/* Responsive adjustments */
@media (max-width: 768px) {
    /* Adjust dialog width on small screens */
    .form-dialog {
        width: 90vw; /* Use viewport width for small screens */
    }

    /* Adjust padding and font size for inputs */
    .el-input,
    .el-select {
        font-size: 14px; /* Smaller font for mobile */
        padding: 10px; /* Less padding for inputs */
    }

    /* Adjust button size */
    .el-button {
        font-size: 14px; /* Smaller button text */
    }
}
</style>