<template>
    <div class="detail-keyresult">
        <div class="left">
            <div class="left-header">
                <p class="user-name">{{ keyresult.createdBy }}</p>
                <el-button-group class="ml-4">
                    <el-button type="primary" :icon="Edit" />
                    <el-button type="primary" :icon="Share" />
                    <el-button type="primary" :icon="Delete" />
                </el-button-group>
            </div>
            <div>
            <p class="title">{{ keyresult.description }}</p>
            </div>
            <div class="keyresult-progress">
                <el-progress :percentage="(keyresult.currentPoint ?? 0) / (keyresult.maximunPoint ?? 1) * 100" color="#6366F1" />
                <!-- <p class="progress-caption">The objectives progress is calculated from the key results</p> -->
                 {{ keyresult.currentPoint }} / {{ keyresult.maximunPoint }}
            </div>
            <div class="keyresult-status">
                <p class="label">Status: </p>
                <el-tag :type="getTagType(keyresult.status)">{{ getStatusText(keyresult.status) }}</el-tag>
            </div>
        </div>
        <div class="right">
            <div class="right-item">
                <p class="label">Created: </p>
                <p class="value">{{ formatDate(keyresult.createdOn) }}</p>
            </div>
            <div class="right-item">
                <p class="label">Start date: </p>
                <p class="value">{{ formatDate(keyresult.startDay) }}</p>
            </div>
            <div class="right-item">
                <p class="label">End date: </p>
                <p class="value">{{ formatDate(keyresult.endDay) }}</p>
            </div>
            <div class="right-item">
                <p class="label">Last progress update: </p>
                <p class="value">{{ keyresult.lastProgressUpdate ? formatDate_dd_mm_yyyy_hh_mm(keyresult.lastProgressUpdate) : '' }}</p>
            </div>
        </div>
    </div>
    <div class="comment-progress">
        <el-tabs v-model="tabs" class="demo-tabs">
            <!-- <el-tab-pane label="Comment" name="comment"><EvaluateTarget :searchRequest="searchRequest" :targetType="'0'"></EvaluateTarget></el-tab-pane> -->
            <el-tab-pane label="Progress" name="progress"><ProgressUpdate :searchRequest="searchRequest"></ProgressUpdate></el-tab-pane>
       </el-tabs>
    </div>  
</template>
<script setup lang="ts">
import { Edit, Share, Delete } from "@element-plus/icons-vue";
import { onBeforeMount, onMounted, ref } from "vue";
import { KeyResult } from "@/Models/KeyResult";
import ListKeyresult from "../Create-Edit/ListKeyresult.vue";
import ProgressUpdate from "../ProgressUpdate.vue";
import EvaluateTarget from "../EvaluateTarget/EvaluateTarget.vue";
import type { Objectives } from "@/Models/Objective";
import { TargetType } from "@/Models/Enum/TargetType";
import { axiosInstance } from "@/Service/axiosConfig";
import { getTagType,getStatusText } from "@/Models/EntityObjectives";
import { formatDate, formatDate_dd_mm_yyyy_hh_mm, RecalculateTheDate } from "@/Service/formatDate";
import type { SearchRequest } from "@/components/maynghien/BaseModels/SearchRequest";
import { Filter } from "@/components/maynghien/BaseModels/Filter";
import { addFilter } from "@/components/maynghien/Common/handleSearchFilter";
import { deepCopy } from "@/Service/deepCopy";


const keyresult =  ref<KeyResult>({
    id: undefined,
    description: "test ets ets etr stgasd sdhjasd sdahjk",
    currentPoint: 0,
    status: 1,
    maximunPoint: 100,
    // deadline: new Date(),
    unit: 0,
    active: true,
    note: "",
    percentage: 50,
    createdOn: new Date(),
    lastProgressUpdate: new Date(),
});
const tabs = ref('progress');
const props = defineProps<{
    keyresultId : string;
}>()
const searchRequest = ref<SearchRequest>({
    PageIndex: 1,
    PageSize: 10,
    filters: [],
    SortBy: undefined,
})

const getKeyresult = async () => {
    await axiosInstance.get(`KeyResults/${props.keyresultId}`).then((res) => {
        keyresult.value = res.data.data
        keyresult.value.endDay = RecalculateTheDate(keyresult.value.endDay);
        keyresult.value.createdOn = RecalculateTheDate(keyresult.value.createdOn);
        keyresult.value.lastProgressUpdate =keyresult.value.lastProgressUpdate? RecalculateTheDate(keyresult.value.lastProgressUpdate) : undefined;
        keyresult.value.startDay = RecalculateTheDate(keyresult.value.startDay);
        console.log(keyresult.value);
    })
}
onBeforeMount(async () => {
    var filter = new Filter();
    filter.FieldName = "keyresultId";
    filter.Value = props.keyresultId;
    addFilter(searchRequest.value.filters as [],deepCopy(filter));
    await getKeyresult();
})
</script>
<style scoped>
.keyresult-progress .progress-caption {
    font-size: 12px;
    color: #6b7280; 
    margin-top: 8px;
}
.detail-keyresult{
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
    justify-content: space-between;
}
.title{
    font-size: 24px;
    font-weight: bold;
}
.keyresult-status >.label{
    font-weight: bold;
}
.keyresult-status{
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
