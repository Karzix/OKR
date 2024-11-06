<template>
  <div class="okr-table">
    <el-table :data="listObjectivesDisplay" style="width: 100%" v-loading="loadingTable">
      <!-- Expandable Row -->
      <el-table-column type="expand">
        <template #default="props">
          <!-- Nested Table without Headers -->
           <div class="expand-table">
            <el-table
              :data="props.row.keyResults"
              style="width: 100%; padding-left: 20px"
              :show-header="false"
            >
              <!-- Title Column -->
              <el-table-column >
                <template #default="{ row }">
                  <div class="title-cell">
                    <CustomIconTargetType :targetType="row.targetType"></CustomIconTargetType>
                    <span>{{ row.description }}</span>
                  </div>
                </template>
              </el-table-column>

              <!-- Progress Column -->
              <el-table-column :with="customCSS.withProgress">
                <template #default="{ row }">
                  <el-progress :percentage="(row.currentPoint/row.maximunPoint * 100).toFixed(2)" :status="getTagType(row.status)" />
                  <span class="progress-percentage">{{ (row.currentPoint/row.maximunPoint * 100).toFixed(2)}}%</span>
                </template>
              </el-table-column>

              <!-- Status Column -->
              <el-table-column label="Status" :with="customCSS.withStatus">
                <template #default="{ row }">
                  <el-tag :status="getTagType(row.status)">{{ getStatusText(row.status) }}</el-tag>
                </template>
              </el-table-column>

              <!-- End Date Column -->
              <el-table-column :with="customCSS.withEndDate">
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
      <el-table-column label="Title" >
        <template #default="{ row }">
          <div class="title-cell">
            <CustomIconTargetType :targetType="row.targetType"></CustomIconTargetType>
            <span>{{ row.name }}</span>
          </div>
        </template>
      </el-table-column>

      <el-table-column label="Progress"  :with="customCSS.withProgress">
        <template #default="{ row }">
          <el-progress :percentage="row.point" :status="getTagType(row.status)" />
          <span class="progress-percentage">{{ row.point }}%</span>
        </template>
      </el-table-column>

      <el-table-column label="Status" :with="customCSS.withStatus">
        <template #default="{ row }">
          <el-tag :status="getTagType(row.status)">{{ getStatusText(row.status) }}</el-tag>
        </template>
      </el-table-column>

      <el-table-column label="End Date" :with="customCSS.withEndDate">
        <template #default="{ row }">
          <span :class="{ 'date-warning': row.status === 'At risk' }">{{
            getDisplayString(row.period + ":" + row.year) 
          }}</span>
        </template>
      </el-table-column>
      <el-table-column width="50">
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
  <el-dialog v-model="dialogDetail" width="900px">
    <DetailObjectives  :objectives="ObjectivesSelect" v-if="dialogDetail" :is-owner="allowEdit" @update:objectives="refreshObjectives"></DetailObjectives>
  </el-dialog>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { EntityObjectives, StatusObjectives } from "@/Models/EntityObjectives";
import { SearchResponse } from "../../components/maynghien/BaseModels/SearchResponse";
import { formatDate } from "../../Service/formatDate";
import { getStatusText, getTagType} from "@/Models/EntityObjectives"
import CustomIconTargetType from "../icons/CustomIconTargetType.vue";
import { SearchRequest } from "../maynghien/BaseModels/SearchRequest";
import { axiosInstance } from "@/Service/axiosConfig";
import type { Objectives } from "@/Models/Objective";
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
  withProgress: 200,
  withStatus: 150,
  withEndDate: 200,

}
const props = defineProps<{
  searchRequest: SearchRequest;
}>();


const noMore = ref(false);
const loadingTable = ref(false);
const dialogDetail = ref(false);
const ObjectivesSelect = ref<Objectives>({} as Objectives);
const allowEdit = ref(false);
const search = async () => {
  try{
    loadingTable.value = true;
    await axiosInstance
    .post("Objectives/search", searchRequest.value)
    .then((res) => {
      var result = res.data as AppResponse<SearchResponse<Objectives[]>>;
      searchResponseObjectives.value = result.data as SearchResponse<Objectives[]>;
      if(res)
      if (searchResponseObjectives.value.data && searchResponseObjectives.value.data?.length == 0) {
        noMore.value = true;
      }else{
        if (Array.isArray(searchResponseObjectives.value.data)) {
          listObjectivesDisplay.value.push(...searchResponseObjectives.value.data);
        }
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
    listObjectivesDisplay.value.splice(index, 1, objective); // Replace the item at the found index
  }
};
onMounted(() => {
  searchRequest.value = props.searchRequest;
  search();
});
defineExpose({ onAddFilterAndSearch });
</script>

<style scoped>
.okr-table {
  border-radius: 10px;
  overflow: hidden;
  background: #f9fafb;
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
 margin-left: 40px;
}
</style>
