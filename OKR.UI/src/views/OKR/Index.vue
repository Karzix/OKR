<template>
  <div class="progress-overview">
    <div class="search">
      <ActionSearch @onSearch="AddFilterAndSearch" />
    </div>
    <div class="overall-progress">
      <div class="progress-circle">
        <el-progress :percentage="OverallProgress" type="circle" color="#6366F1" />
      </div>
      <div class="status-info">
        <p class="title">Overall progress</p>
        <p class="subtitle">Overall progress is based on the progress of the target objectives.</p>
        <div class="status-tags">
          <el-tag>{{ statusStatistics.noStatus }} No status</el-tag>
          <el-tag type="success">{{ statusStatistics.onTrack }} On track</el-tag>
          <el-tag type="warning">{{ statusStatistics.atRisk }} At risk</el-tag>
          <el-tag type="danger">{{ statusStatistics.offTrack }} Off track</el-tag>
          <el-tag type="info">{{ statusStatistics.closed }} Closed</el-tag>
        </div>
        <div class="actions">
          
        </div>
      </div>
    </div>
    <div class="btn-objectives">

      <el-button type="primary" @click="dialogCreate = true" :icon="Plus">Add new objective</el-button>
    </div>
    
  </div>
  <div class="ListView">
    <ListView ref="listViewRef" :searchRequest="searchRequest" :allow-update-weight="true"></ListView>
  </div>
  <el-dialog v-model="dialogCreate">
    <CreateEdit :objectives="objectives" @updateData="onUpdateData"></CreateEdit>
  </el-dialog>


  
</template>

<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import ListView from "@/components/okr/ListView.vue";
import CreateEdit from "@/components/okr/Create-Edit/Create.vue";
import type { Objectives } from "@/Models/Objective";
import { TargetType } from "@/Models/Enum/TargetType";
import { Plus } from "@element-plus/icons-vue";
import MnActionPane from "@/components/maynghien/adminTable/MnActionPane.vue";
import { ApiActionType, CustomAction } from "@/components/maynghien/adminTable/Models/CustomAction";
import type { TableColumn } from "@/components/maynghien/adminTable/Models/TableColumn";
import { Filter } from "@/components/maynghien/BaseModels/Filter";
import { axiosInstance } from "@/Service/axiosConfig";
import { getDisplayString } from "@/Service/OKR/DisplayPeriod";
import type { AppResponse } from "@/components/maynghien/BaseModels/AppResponse";
import * as handleSearch from "@/components/maynghien/Common/handleSearchFilter";
import type { SearchRequest } from "@/components/maynghien/BaseModels/SearchRequest";
import type { StatusStatistics } from "@/Models/StatusStatistics";
import ActionSearch from "@/components/okr/ActionSearch.vue";

const progressPercentage = ref(63);
const dialogCreate = ref(false);

const setView = (view: string) => {
  console.log(`Switching to ${view} view`);
};
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
  lastProgressUpdate: new Date(),
  createdOn: new Date(),
  numberOfPendingUpdates: 0
});
const listViewRef = ref<InstanceType<typeof ListView> | null>(null);
const periods = ref<{value: string; label: string}[]>([]);
const statusStatistics = ref<StatusStatistics>({
  onTrack: 0,
  atRisk: 0,
  offTrack: 0,
  closed: 0,
  total: 0,
  noStatus: 0
});
const OverallProgress = ref(0);
const AddFilterAndSearch = (filters: Filter[]) => {
  listViewRef.value?.onAddFilterAndSearch(filters);
  searchRequest.value.filters = filters;
  getStatusStatistics()
  getOverallProgress()
}
const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
  SortBy: undefined,
})
const getStatusStatistics = async () => {
  await axiosInstance.post("Objectives/statusStatistics",searchRequest.value).then((res) => {
    var result = res.data as AppResponse<StatusStatistics>;
      if (result.data) {
        statusStatistics.value = result.data;
      }
  })
}
const getOverallProgress = async () => {
  var responeOverallProgress = await axiosInstance.post(
    "Objectives/overal-progress",
    searchRequest.value
  );
  OverallProgress.value = responeOverallProgress.data.data;
}
onBeforeMount(async () => {
  await axiosInstance.get("Objectives/periods").then((res) => {
    var result = res.data as AppResponse<string[]>;
    result.data?.forEach((element) => {
      periods.value.push({value: element, label: getDisplayString(element)})
    })
  })
  getStatusStatistics()
  getOverallProgress()
})

const onUpdateData = async () => {
  listViewRef.value?.ReLoad();
}

const onClickButtonSearch = (filters: Filter[]) => {
  searchRequest.value.filters = filters;
  onUpdateData();
}
</script>

<style scoped>
.progress-overview {
  border-radius: 10px;
  padding: 20px;
  position: relative;
}

.overall-progress {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
}

.progress-circle {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-shrink: 0;
  width: 100px; /* Adjust as needed */
  text-align: center;
}

.status-info {
  display: flex;
  flex-direction: column;
  margin-left: 20px;
  flex: 1;
}

.title {
  font-size: 18px;
  font-weight: bold;
}

.subtitle {
  font-size: 14px;
  color: #6b7280;
}

.status-tags {
  display: flex;
  gap: 10px;
  margin-top: 10px;
}

.actions {
  margin-top: 15px;
  display: flex;
  align-items: center;
  gap: 20px;
}

.view-options .el-button {
  background-color: #6366f1;
  color: #ffffff;
}

.view-options .el-button[disabled] {
  background-color: #e0e0e0;
}
.ListView > div{
  max-width: 1300px;
  text-align: center;
  margin-left: auto;
    margin-right: auto;
    box-shadow: 5px 5px 5px 5px #00000080;
    border-radius: 6px;
}
.btn-objectives{
  position: absolute;
  top: 0;
  right: 0;
}
.search{
  margin-bottom: 20px;
}
</style>
<style>
@media screen and (max-width: 1200px) {
  .el-dialog {
    width: 80% !important;
  }
}
/* @media screen and (max-width: 1000px) {
  .el-dialog {
    width: 80% !important;
  }
} */
</style>