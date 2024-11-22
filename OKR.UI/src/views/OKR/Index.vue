<template>
  <div class="action">
    <div class="left">
      <el-radio-group v-model="targetType">
        <el-radio value="MyObjectives" size="large" class="custom-radio" >My objectives</el-radio>
        <el-radio value="Team" size="large" class="custom-radio" >Team</el-radio>
        <el-radio value="Comany" size="large" class="custom-radio" >Comany</el-radio>
      </el-radio-group>
      <div class="period">
        <el-select v-model="period" placeholder="Filter by period" @change="onChangePeriod" filterable>
          <el-option v-for="period in periods" :key="period.value" :label="period.label" :value="period.value" />
        </el-select>
      </div>
     
    </div>
    <div class="right">
      <div class="btn-objectives">
        <el-button type="primary" @click="dialogCreate = true" :icon="Plus">New objective</el-button>
      </div>
      <div class="search">
        <ActionSearch @onSearch="AddFilterAndSearch" />
      </div>
    </div>
    
    
  </div>
  <div class="progress-overview">
    
    <div class="overall-progress">
      <div class="progress-circle">
        <el-progress :percentage="OverallProgress" type="circle" color="#6366F1" />
        <p class="title">Overall progress</p>
      </div>
      <div class="status-info">
        
        <div class="status-tags">
          <el-tag>{{ statusStatistics.noStatus }} No status</el-tag>
          <el-tag type="success">{{ statusStatistics.onTrack }} On track</el-tag>
          <el-tag type="warning">{{ statusStatistics.atRisk }} At risk</el-tag>
          <el-tag type="danger">{{ statusStatistics.offTrack }} Off track</el-tag>
          <el-tag type="info">{{ statusStatistics.closed }} Closed</el-tag>
        </div>
        
      </div>
    </div>
    
    
  </div>
  <div class="ListView">
    <ListView ref="listViewRef" :searchRequest="searchRequest" :allow-update-weight="true"></ListView>
  </div>
  <el-dialog v-model="dialogCreate" class="dialog-Create-Objective">
    <CreateEdit :objectives="objectives" @updateData="onUpdateData" @close="dialogCreate = false"></CreateEdit>
  </el-dialog>


  
</template>

<script setup lang="ts">
import { onBeforeMount, ref, watch } from "vue";
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
import { deepCopy } from "@/Service/deepCopy";
import { toQueryParams } from "@/components/maynghien/Common/toQueryParams";

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
const period = ref("");
const statusStatistics = ref<StatusStatistics>({
  onTrack: 0,
  atRisk: 0,
  offTrack: 0,
  closed: 0,
  total: 0,
  noStatus: 0
});
const OverallProgress = ref(0);
const targetType = ref<string>("MyObjectives");
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
  var url = "Objectives/statusStatistics";
    var parramsQuery = toQueryParams(searchRequest.value);
    var urlFull = url + "?" + parramsQuery;
  await axiosInstance.get(urlFull).then((res) => {
    var result = res.data as AppResponse<StatusStatistics>;
      if (result.data) {
        statusStatistics.value = result.data;
      }
  })
}
const getOverallProgress = async () => {
  var url = "Objectives/overal-progress";
  var parramsQuery = toQueryParams(searchRequest.value);
  var urlFull = url + "?" + parramsQuery;
  var responeOverallProgress = await axiosInstance.get(urlFull);
  OverallProgress.value = responeOverallProgress.data.data;
}
onBeforeMount(async () => {
  var filTargetType = new Filter();
  filTargetType.FieldName = "targetType";
  filTargetType.Value = TargetType.Individual.toString();
  handleSearch.addFilter(searchRequest.value.filters as [], deepCopy(filTargetType));
  await axiosInstance.get("Objectives/periods").then((res) => {
    var result = res.data as AppResponse<string[]>;
    periods.value.push({value: "default", label: "Default"})
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

// const onClickButtonSearch = (filters: Filter[]) => {
//   searchRequest.value.filters = filters;
//   onUpdateData();
// }
const onChangePeriod = (filters: string) => {
  var filPeriod = new Filter();
  filPeriod.FieldName = "period";
  filPeriod.Value = filters;

  handleSearch.addFilter(searchRequest.value.filters as [], deepCopy(filPeriod));
  searchRequest.value.PageIndex = 1;
  AddFilterAndSearch(searchRequest.value.filters as [])
}
watch(() => targetType.value, () => {
  var filTargetType = new Filter();
  filTargetType.FieldName = "targetType";
  switch (targetType.value) {
    case "MyObjectives":
      filTargetType.Value = TargetType.Individual.toString();
      break;
    case "Team":
      filTargetType.Value = TargetType.Department.toString();
      break;
    case "Comany":
      filTargetType.Value = TargetType.Company.toString();
      break;
    default:
      break;
  }
  handleSearch.addFilter(searchRequest.value.filters as [], deepCopy(filTargetType));
  searchRequest.value.PageIndex = 1;
  AddFilterAndSearch(searchRequest.value.filters as [])
})
</script>

<style scoped>
.progress-overview {
  border-radius: 10px;
  padding: 20px;
  position: relative;
  background-color: #ffffff;
  margin-bottom: 20px;
  max-width: 1300px;
  margin-left: auto;
  margin-right: auto;
  box-shadow: -1px -1px 10px -1px #00000030;
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
  width: 160px;
  text-align: center;
  flex-direction: column;
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
  justify-content: center;
}

.period {
  margin-top: 15px;
  display: flex;
  align-items: center;
  gap: 20px;
}
.period > .el-select {
  width: 230px;
}
.view-options .el-button {
  background-color: #6366f1;
  color: #ffffff;
}

.view-options .el-button[disabled] {
  background-color: #e0e0e0;
}
.ListView > div{
  max-width: 100px;
  text-align: center;
  margin-left: auto;
  margin-right: auto;
  border-radius: 6px;
}
.ListView{
  box-shadow: -1px -1px 10px -1px #00000030;
  width: 1300px;
  margin-left: auto;
  margin-right: auto;
}
.action{
  display: flex;
  gap: 10px;
  justify-content: space-between;
  max-width: 1300px;
  margin-left: auto;
  margin-right: auto;
}
.action > .right{
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 10px;

}
.action > .left{
  display: flex;
  flex-direction: column;
}
.action > .left .el-radio-group {
  gap: 20px;
}
.action > .left .el-radio-group .el-radio .el-radio__label{
  padding: 0px !important;
}
.search{
  margin-bottom: 20px;
}
.custom-radio {
  border: none; /* Ẩn border mặc định */
  position: relative; /* Để áp dụng border dưới */
}

.custom-radio.is-checked::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 2px;
  background-color: #409eff; /* Màu border */
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
.el-dialog.dialog-Create-Objective {
    width: 871px !important;
}
.status-tags .el-tag{
  width: 110px;
  height: 44px;
}
</style>