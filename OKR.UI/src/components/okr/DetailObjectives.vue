<template>
    <div class="detail-objectives">
        <div class="left">
            <div class="left-header">
                <el-button-group class="ml-4">
                    <el-button type="primary" :icon="Edit" v-if="isOwner" @click="onEdit"/>
                    <el-button type="primary" :icon="Share" @click="copyLinkShare" />
                    <el-button type="primary" :icon="Delete" v-if="isOwner" @click="onDelete"/>
                </el-button-group>
            </div>
            <div>
            <p class="title">{{ objectives.name }}</p>
            </div>
            <div class="objective-progress">
                <el-progress :percentage="objectives.point" color="#6366F1" />
                <p class="progress-caption">The objectives progress is calculated from the key results</p>
            </div>
            <div class="objective-status">
                <p class="label">Status: </p>
                <el-tag :type="getTagType(objectives.status)">{{ getStatusText(objectives.status) }}</el-tag>
            </div>
            <div class="keyresults">
                <ListKeyresult :keyresults="objectives.keyResults" :is-create-or-edit="false" @on-select-key-result="onSelectKeyResult"/>
            </div>
        </div>
        <div class="right">
            <div class="right-item">
                <p class="label">Created: </p>
                <p class="value">{{ formatDate(objectives.createdOn) }}</p>
            </div>
            <div class="right-item">
                <p class="label">Start date: </p>
                <p class="value">{{ formatDate(objectives.startDay) }}</p>
            </div>
            <div class="right-item">
                <p class="label">End date: </p>
                <p class="value">{{ formatDate(objectives.endDay) }}</p>
            </div>
            <div class="right-item">
                <p class="label">Last progress update: </p>
                <p class="value">{{ objectives.lastProgressUpdate ? formatDate_dd_mm_yyyy_hh_mm(objectives.lastProgressUpdate) : '' }}</p>
            </div>
            <div class="right-item">
                <p class="label">Visibility: </p>
                <p class="value">{{  objectives.isPublic ? 'Public' : 'Private' }}</p>
            </div>
            <div class="right-item">
                <p class="label">Created by: </p>
                <p class="value">{{ objectives.createdBy }}</p>
            </div>
        </div>
    </div>
    <div class="comment-progress">
        <el-tabs v-model="tabs" class="demo-tabs">
            <el-tab-pane label="Comment" name="comment"><EvaluateTarget :searchRequest="searchRequest"></EvaluateTarget></el-tab-pane>
            <el-tab-pane label="Progress" name="progress"><ProgressUpdate :searchRequest="searchRequest" :key="keyProgressUpdate"></ProgressUpdate></el-tab-pane>
        </el-tabs>
    </div>
    <el-dialog v-model="showDialogDetailKeyresult">
        <DetailKeyresult :keyresult-id="keyresultIdSelect" :key="keyresultIdSelect" @update-data="refreshObjectives" 
        :allow-update-weight="allowUpdateWeight" :objectives="objectives"></DetailKeyresult>
    </el-dialog>
    <el-dialog v-model="showDialogEdit" class="dialog-Create-Objective">
        <CreateEditObjectives :objectives="objectives" :is-edit="true" @update-data="refreshObjectives"></CreateEditObjectives>
    </el-dialog>   
</template>
<script setup lang="ts">
import { Edit, Share, Delete } from "@element-plus/icons-vue";
import { onBeforeMount, onMounted, ref } from "vue";
import { KeyResult } from "@/Models/KeyResult";
import ListKeyresult from "./Create-Edit/ListKeyresult.vue";
import type { SearchRequest } from "../maynghien/BaseModels/SearchRequest";
import ProgressUpdate from "./ProgressUpdate.vue";
import EvaluateTarget from "./EvaluateTarget/EvaluateTarget.vue";
import type { Objectives } from "@/Models/Objective";
import { TargetType } from "@/Models/Enum/TargetType";
import { axiosInstance, urlUI } from "@/Service/axiosConfig";
import { getTagType,getStatusText } from "@/Models/EntityObjectives";
import { formatDate, formatDate_dd_mm_yyyy_hh_mm, RecalculateTheDate } from "@/Service/formatDate";
import { Filter } from "../maynghien/BaseModels/Filter";
import { addFilter } from "../maynghien/Common/handleSearchFilter";
import { deepCopy } from "@/Service/deepCopy";
import DetailKeyresult from "./DetailKeyresult.vue";
import CreateEditObjectives from "./Create-Edit/Create.vue";
import { ElMessage, ElMessageBox } from "element-plus";
import Cookies from "js-cookie";


const props = defineProps<{
    isOwner?: boolean;
    objectives: Objectives;
    // allowUpdateWeight?: boolean
}>()
const objectives = ref<Objectives>({
  id: undefined,
  name: "",
  startDay: undefined,
  endDay: undefined,
  keyResults: [],
  targetType: TargetType.Individual,
  targetTypeName: "",
  point: 0,
  status: 0,
  isPublic: true,
  isUserObjectives: true,
  year: new Date().getFullYear(),
  period: "Q1",
  createdOn: new Date(),
  lastProgressUpdate: new Date(),
  numberOfPendingUpdates: 0
});
const keyresults = ref<KeyResult[]>([]);
const tabs = ref("comment");
const showDialogDetailKeyresult = ref(false);
const keyresultIdSelect = ref("");
const showDialogEdit = ref(false);
const allowUpdateWeight = ref(false);
const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
  SortBy: undefined,
});
const keyProgressUpdate = ref(1);
const emit = defineEmits<{
    (e: "update:objectives" , objectives: Objectives): void;
    (e: "delete:objectives" , objectives: Objectives): void;
}>();
const getObjectives = async () => {
    
    var url = `Objectives/${props.objectives.id}`
    await axiosInstance.get(url).then((res) => {
        objectives.value = res.data.data
        objectives.value.createdOn = RecalculateTheDate(objectives.value.createdOn);
        objectives.value.lastProgressUpdate = objectives.value.lastProgressUpdate? RecalculateTheDate(objectives.value.lastProgressUpdate) :undefined;
        objectives.value.startDay = RecalculateTheDate(objectives.value.startDay);
        objectives.value.endDay = RecalculateTheDate(objectives.value.endDay);
    })
}
const onSelectKeyResult = (item: KeyResult) => {
    keyresultIdSelect.value = item.id ?? "";
    showDialogDetailKeyresult.value = true
}
const refreshObjectives = async () => {
    await getObjectives(); // Làm mới dữ liệu objectives
    keyProgressUpdate.value++;
    emit('update:objectives', objectives.value)
};

// const copyLinkShare = () =>{
//     navigator.clipboard.writeText(urlUI + "Objectives=" + props.objectives.id);
//     ElMessage({
//     message: 'Copy url success ',
//     type: 'success',
//   })
// }
async function copyLinkShare() {
    var text = urlUI + "Objectives=" + props.objectives.id
    if (navigator.clipboard && navigator.clipboard.writeText) {
        try {
            await navigator.clipboard.writeText(text);
            console.log("Text copied to clipboard");
                ElMessage({
                    message: 'Copy url success ',
                    type: 'success',
                })
        } catch (err) {
            console.error("Error copying text: ", err);
        }
    } else {
        // Sử dụng execCommand làm cách thay thế
        copyTextToClipboard(text);
    }
}
function copyTextToClipboard(text : string) {
    const textArea = document.createElement("textarea");
    textArea.value = text;
    
    // Đảm bảo phần tử không ảnh hưởng đến giao diện
    textArea.style.position = "fixed";
    textArea.style.left = "-9999px";
    
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();
    
    try {
        const successful = document.execCommand("copy");
        if (successful) {
            console.log("Text copied to clipboard");
               ElMessage({
                    message: 'Copy url success ',
                    type: 'success',
                })
        } else {
            console.log("Failed to copy text");
        }

    } catch (err) {
        console.error("Error copying text: ", err);
    }
    
    document.body.removeChild(textArea);
}
const onDelete = () => {
    
    axiosInstance.delete(`Objectives/${props.objectives.id}`).then(() => {
        emit('delete:objectives', objectives.value)
    })
    ElMessageBox.confirm(
        'Are you sure to delete this objective?',
        'Warning',
        {
            confirmButtonText: 'OK',
            cancelButtonText: 'Cancel',
            type: 'warning',
        }
    ).then(() => {
        axiosInstance.delete(`Objectives/${props.objectives.id}`).then(() => {
            emit('delete:objectives', objectives.value)
        })
    })
   .catch(() => {
        ElMessage({
            type: 'info',
            message: 'Delete canceled',
        })
    })
}
const onEdit = () => {
    showDialogEdit.value = true
}

const CheckPermissions = () =>{
    var departmentId = Cookies.get("DepartmentId")?.toString() ?? "";
    var userid = Cookies.get("UserId")?.toString() ?? "";
    if(objectives.value.departmentId == departmentId || objectives.value.applicationUserId == userid
     || objectives.value.targetType == TargetType.Company){
        allowUpdateWeight.value = true
    }
    else{
        allowUpdateWeight.value = false
    }
    
}
onMounted(() => {
    getObjectives().then(() => {
        CheckPermissions()
    })
})
onBeforeMount(() => {
    var filter = new Filter();
    filter.FieldName = "objectivesId";
    filter.Value = props.objectives.id;
    addFilter(searchRequest.value.filters as [],deepCopy(filter));
})
</script>

<style scoped>
.objective-progress .progress-caption {
    font-size: 12px;
    color: #6b7280; 
    margin-top: 8px;
}
.detail-objectives{
    display: flex;
    gap: 20px;
}
.left{
    width: 65%;
    display: flex;
    flex-direction: column;
    gap: 15px;
}
.left-header{
    display: flex;
    align-items: center;
    justify-content:end;
}
.title{
    font-size: 24px;
    font-weight: bold;
}
.objective-status >.label{
    font-weight: bold;
}
.objective-status{
    display: flex; 
    align-items: center;
    gap: 10px;
}
.value{
    font-weight: bold;
    font-size: 18px;
    padding-left: 10px;
}
.right {
    display: flex;
    flex-direction: column;
    gap: 10px;
    border-left: 2px solid #ccc;
    padding-left: 15px;
}
</style>
