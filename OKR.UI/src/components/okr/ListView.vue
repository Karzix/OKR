<template>
  <div class="okr-table">
    <el-table :data="listObjectivesDisplay" v-loading="loadingTable">
      <!-- Expandable Row -->
      <el-table-column type="expand">
        <template #default="props">
          <!-- Nested Table without Headers -->
           <div class="expand-table">
            <el-table
              :data="props.row.keyResults"
              style="width: 100%;"
              :show-header="false"
            >
              <!-- Title Column -->
              <el-table-column  width="350">
                <template #default="{ row }">
                  <div class="title-cell">
                    <CustomIconTargetType :targetType="row.targetType"></CustomIconTargetType>
                    <span>{{ row.description }}</span>
                  </div>
                </template>
              </el-table-column>

              <!-- Progress Column -->
              <el-table-column :width="customCSS.withProgress">
                <template #default="{ row }">
                  <el-progress :percentage="Math.round(row.currentPoint / row.maximunPoint * 100)" :color="getStatusColor(row.status)" />
                  <!-- <span class="progress-percentage">{{ (row.currentPoint/row.maximunPoint * 100).toFixed(2)}}%</span> -->
                </template>
              </el-table-column>

              <!-- Status Column -->
              <el-table-column label="Status" :width="customCSS.withStatus">
                <template #default="{ row }">
                  <el-tag :type="getTagType(row.status)">{{ getStatusText(row.status) }}</el-tag>
                </template>
              </el-table-column>

              <!-- End Date Column -->
              <el-table-column :width="customCSS.withEndDate">
                <template #default="{ row }">
                  <span :class="{ 'date-warning': row.status === 'At risk' }">{{
                    formatDate(row.deadline)
                  }}</span>
                </template>
              </el-table-column>
            </el-table>
           </div>
        
        </template>
      </el-table-column>

      <!-- Main Table Columns -->
      <el-table-column label="Title" width="350" >
        <template #default="{ row }">
          <div class="title-cell">
            <CustomIconTargetType :targetType="row.targetType"></CustomIconTargetType>
            <span>{{ row.name }}</span>
            <el-icon v-if="!row.isPublic"><Hide /></el-icon>
            <el-popover 
                placement="top"
                trigger="click"
                width="100%"
                
              >
                <template #reference>
                  <el-badge class="item" 
                    :value="row.numberOfPendingUpdates" 
                    v-if="row.numberOfPendingUpdates != 0 && isTeamleadOrOwner(row)" 
                  >
                    <el-icon class="icon-Notification"><BellFilled /></el-icon>
                  </el-badge>
                </template>
                <DepartmentProgressQueue :objectivesId="row.id ?? ''" 
                @close="DialogDepartmentProgressQueueVisible = false"
                @onSuccess="(request) => {DepartmentProgressQueue_onSuccess(row, request)}"
                />
            </el-popover>
              
          </div>
        </template>
      </el-table-column>

      <el-table-column label="Progress"  :width="customCSS.withProgress">
        <template #default="{ row }">
          <el-progress :percentage="row.point" :color="getStatusColor(row.status)" />
          <!-- <span class="progress-percentage">{{ row.point }}%</span> -->
        </template>
      </el-table-column>

      <el-table-column label="Status" :width="customCSS.withStatus">
        <template #default="{ row }">
          <el-tag :type="getTagType(row.status)">{{ getStatusText(row.status) }}</el-tag>
        </template>
      </el-table-column>

      <el-table-column label="End Date" :width="customCSS.withEndDate">
        <template #default="{ row }">
          <span :class="{ 'date-warning': row.status === 'At risk' }">{{
            getDisplayString(row.period + ":" + row.year) 
          }}</span>
        </template>
      </el-table-column>
      <el-table-column width="50" fixed="right">
        <template #default="scope">
          <el-button type="info" link @click="onShowDialogDetail(scope.row)"><el-icon><MoreFilled /></el-icon></el-button>
        </template>
      </el-table-column>
      <template #append>
        <div style="text-align: center; padding: 10px;" @click="handleAddRow" class="btn-add-item" v-if="!noMore">
          <el-text class="mx-1" type="primary">see more...</el-text>
        </div>
        <div style="text-align: center; padding: 10px;" class="btn-add-item" v-else>
          <el-text class="mx-1" type="primary">no more...</el-text>
        </div>
      </template>
    </el-table>
  </div>
  <el-dialog v-model="dialogDetail" style="width: 900px !important;">
    <DetailObjectives  :objectives="ObjectivesSelect" v-if="dialogDetail" :is-owner="allowEdit"
     @update:objectives="refreshObjectives" @delete:objectives="onDelete" ></DetailObjectives>
  </el-dialog>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { EntityObjectives, StatusObjectives } from "@/Models/EntityObjectives";
import { SearchResponse } from "../../components/maynghien/BaseModels/SearchResponse";
import { formatDate } from "../../Service/formatDate";
import { getStatusText, getTagType, getStatusColor} from "@/Models/EntityObjectives"
import CustomIconTargetType from "../icons/CustomIconTargetType.vue";
import { SearchRequest } from "../maynghien/BaseModels/SearchRequest";
import { axiosInstance } from "@/Service/axiosConfig";
import { Objectives, recaculateObjectivesAfterProgressApproval } from "@/Models/Objective";
import type { AppResponse } from "../maynghien/BaseModels/AppResponse";
import { getDisplayString } from "@/Service/OKR/DisplayPeriod";
import { Filter } from "../maynghien/BaseModels/Filter";
import * as handleSearch from "@/components/maynghien/Common/handleSearchFilter";
import { deepCopy } from "@/Service/deepCopy";
import { defineExpose } from 'vue';
import {MoreFilled} from '@element-plus/icons-vue'
import DetailObjectives from "./DetailObjectives.vue";
import { id } from "element-plus/es/locales.mjs";
import Cookies from "js-cookie";
import type { KeyResult } from "@/Models/KeyResult";
import {Hide} from '@element-plus/icons-vue'
import DepartmentProgressQueue from "@/components/DepartmentProgressApproval/DepartmentProgressQueue.vue";
import { hasPermission } from "../maynghien/Common/handleRole";
import type { DepartmentProgressApprovalDto } from "@/Models/DepartmentProgressApprovalDto";
import { ElMessage } from "element-plus";
import { toQueryParams } from "../maynghien/Common/toQueryParams";
import { TargetType } from "@/Models/Enum/TargetType";


const searchResponseObjectives = ref<SearchResponse<Objectives[]>>({
  data: undefined,
  currentPage: 1,
  totalRows: 0,
  totalPages: 0,
  rowsPerPage: 0,
});
var listObjectivesDisplay = ref<Objectives[]>([]);
const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
  SortBy: undefined,
})
const customCSS = {
  withProgress: 350,
  withStatus: 200,
  withEndDate: 300,
}
const props = defineProps<{
  searchRequest: SearchRequest;
}>();

const emit = defineEmits<{
    (e: "update:objectives" , objectives: Objectives): void;
    (e: "delete:objectives" , objectives: Objectives): void;
}>();
const noMore = ref(false);
const loadingTable = ref(false);
const dialogDetail = ref(false);
const ObjectivesSelect = ref<Objectives>({} as Objectives);
const allowEdit = ref(false);
const DialogDepartmentProgressQueueVisible = ref(false);

const search = async () => {
  try{
    loadingTable.value = true;
    var url = "Objectives/search";
    var parramsQuery = toQueryParams(searchRequest.value);
    var urlFull = url + "?" + parramsQuery;
    await axiosInstance
    .get(urlFull )
    .then((res) => {
      var result = res.data as AppResponse<SearchResponse<Objectives[]>>;
      if(result.isSuccess == false){
        ElMessage.error(result.message);
      }
      searchResponseObjectives.value = result.data as SearchResponse<Objectives[]>;
      
      if(res)
      if (searchResponseObjectives.value.data && searchResponseObjectives.value.data?.length == 0) {
        noMore.value = true;
      }else{
        if (Array.isArray(searchResponseObjectives.value.data)) {
          listObjectivesDisplay.value.push(...searchResponseObjectives.value.data);
        }
      }

      if(searchResponseObjectives.value.currentPage == searchResponseObjectives.value.totalPages){
        noMore.value = true;
      }

    });
  }
  catch(e){
    console.log(e)
  }
  finally{
    loadingTable.value = false;
  }
}
const handleAddRow = async () => {
  searchRequest.value.PageIndex = searchRequest.value.PageIndex!  + 1;
  await search();
}
const onAddFilterAndSearch = (filter : Filter[]) => {
  console.log(filter);
  // filter.forEach((item) => {
  //   handleSearch.addFilter(searchRequest.value.filters as [], deepCopy(item));
  // })
  searchRequest.value.filters= filter;
  listObjectivesDisplay.value = [];
  searchRequest.value.PageIndex = 1;
  search();
}
const onShowDialogDetail = (objective : Objectives) =>{
  allowEdit.value = false;
  ObjectivesSelect.value = objective;
  if(ObjectivesSelect.value.createdBy && ObjectivesSelect.value.createdBy == (Cookies.get("userName")?.toString() ?? "")){
    allowEdit.value = true;
  }
  // idObjectivesSelect.value = idObjectives;
  dialogDetail.value = true;
}
const refreshObjectives = (objective: Objectives) => {
  const index = listObjectivesDisplay.value.findIndex((item) => item.id === objective.id);
  if (index !== -1) {
    listObjectivesDisplay.value.splice(index, 1, objective); 
  }
  emit('update:objectives', objective);
};
const onDelete = (Objectives : Objectives) => {
  const index = listObjectivesDisplay.value.findIndex((item) => item.id === Objectives.id);
  if (index !== -1) {
    listObjectivesDisplay.value.splice(index, 1); 
  }
  emit('delete:objectives', Objectives);
}
const showDialogDepartmentProgressQueue = () => {
  DialogDepartmentProgressQueueVisible.value = true;
  console.log("showDialogDepartmentProgressQueue");
}
onMounted(() => {
  searchRequest.value = props.searchRequest;
  search();
});
const ReLoad = () => {
  listObjectivesDisplay.value = [];
  searchRequest.value.PageIndex = 1;
  search();
}
const isTeamleadOrOwner = (objectives : Objectives) : boolean => {
  var userLogin = Cookies.get("userName")?.toString();
  var userIdOfCurrentUser = Cookies.get("UserId")?.toString();
  var departmentIdOfCurrentUser = Cookies.get("DepartmentId")?.toString();
  var jsonString = Cookies.get('Roles')?.toString() ?? '';
  var jsonObject = JSON.parse(jsonString);
  var arrayFromString = Object.values(jsonObject);
  var userRoles = arrayFromString as string[];
  if(objectives.departmentId == departmentIdOfCurrentUser && hasPermission(userRoles as string[], ['Teamleader'])
    && objectives.targetType == TargetType.Department
  ){
    return true;
  }
  else if(objectives.targetType == TargetType.Company && hasPermission(userRoles as string[], ['Admin'])){
    return true;
  }
  else if(objectives.applicationUserId == userIdOfCurrentUser){
    return true;
  }
  return false
}
const DepartmentProgressQueue_onSuccess = (objectives: Objectives, DepartmentProgressApproval : DepartmentProgressApprovalDto) => {
  var o = listObjectivesDisplay.value.filter((item) => item.id === objectives.id)[0];
  var keyresult = o.keyResults.filter((item) => item.id === DepartmentProgressApproval.keyresultID)[0];
  keyresult.isCompleted = DepartmentProgressApproval.isCompleted ?? false;
  o.point = recaculateObjectivesAfterProgressApproval(objectives, DepartmentProgressApproval);
  o.numberOfPendingUpdates -=1;
}
defineExpose({ onAddFilterAndSearch, ReLoad });
</script>

<style scoped>
.okr-table {
  border-radius: 10px;
  overflow: hidden;
  max-width: 1300px;
  margin-left: auto;
  margin-right: auto;
}

.title-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

.title-cell .icon {
  width: 24px;
  height: 24px;
  border-radius: 50%;
}

.progress-percentage {
  font-size: 12px;
  color: #6b7280;
  margin-left: 8px;
}

.date-warning {
  color: #ef4444;
}
.expand-table{
 margin-left: 48px;
}
.okr-table {

}

.el-table .el-table__row,
.el-table .el-table__header th {
  height: 50px; /* Tăng chiều cao hàng */
}

.el-table .title-cell {
  font-size: 16px; /* Tăng kích thước font của ô tiêu đề */
  padding: 10px; /* Thêm khoảng cách padding */
}

.el-progress {
  height: 12px; /* Tăng chiều cao thanh tiến độ */
}

.el-tag {

  padding: 8px 12px; /* Thêm padding cho tag */
}

.date-warning {
  font-weight: bold;
  font-size: 14px; /* Tăng kích thước font của ngày cảnh báo */
  color: red;
}

.expand-table .el-table .el-table__row,
.expand-table .el-table .el-table__header th {
  height: 50px; /* Tăng chiều cao hàng trong bảng mở rộng */
}

.el-dialog__body {
  font-size: 18px; /* Tăng kích thước font trong dialog */
}

.el-button {
  font-size: 14px; /* Tăng kích thước font của button */
}

.btn-add-item {
  font-size: 14px; /* Tăng kích thước font cho nút "see more..." */
}
</style>
