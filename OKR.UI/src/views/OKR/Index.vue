<template>
  <div class="progress-overview">
    <div class="overall-progress">
      <div class="progress-circle">
        <el-progress :percentage="63" type="circle" color="#6366F1" />
      </div>
      <div class="status-info">
        <p class="title">Overall progress</p>
        <p class="subtitle">Set/edit goal weights</p>
        <div class="status-tags">
          <el-tag>0 No status</el-tag>
          <el-tag type="success">1 On track</el-tag>
          <el-tag type="warning">1 At risk</el-tag>
          <el-tag type="danger">0 Off track</el-tag>
          <el-tag type="info">8 Closed</el-tag>
        </div>
        <div class="actions">
          <el-button-group class="view-options">
            <el-button type="primary" size="small" @click="setView('list')"
              >List view</el-button
            >
            <el-button size="small" @click="setView('feed')"
              >Feed view</el-button
            >
            
          </el-button-group>
        </div>
      </div>
    </div>
    <div class="btn-add-objectives">
      <el-button type="primary" @click="dialogCreate = true" :icon="Plus">Add new objective</el-button>
    </div>
    <div class="search">
      <MnActionPane
          :allowAdd="false"
          :tableColumns="tableColumns"
          :isEdit="false"
          @onBtnSearchClicked="AddFilterAndSearch"
          :CustomActions="[]"
          :openDialog="() => {}"
        />
    </div>
  </div>
  <div class="ListView">
    <ListView ref="listViewRef"></ListView>
  </div>
  <el-dialog v-model="dialogCreate">
    <CreateEdit :objectives="objectives"></CreateEdit>
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
});
const listViewRef = ref<InstanceType<typeof ListView> | null>(null);
const periods = ref<{value: string; label: string}[]>([]);
const tableColumns = ref<TableColumn[]>([
  {
    key: "period",
    label: "Period",
    width: 1000,
    sortable: true,
    enableEdit: true,
    enableCreate: true,
    required: false,
    hidden: false,
    showSearch: true,
    inputType: "dropdown",
    dropdownData: {
      displayMember: "label",
      keyMember: "value",
      data: periods.value
    },
  },
  {
    key: "status",
    label: "Status",
    width: 1000,
    sortable: true,
    enableEdit: true,
    enableCreate: true,
    required: false,
    hidden: false,
    showSearch: true,
    inputType: "dropdown",
    dropdownData: {
      displayMember: "name",
      keyMember: "value",
      data: [{
        value: "0",
        name: "No Status",
      },{
        value: "1",
        name: "On Track",
      },{
        value: "2",
        name: "At Risk",
      },{
        value: "3",
        name: "Off Track",
      },{
        value: "2",
        name: "Closed",
      }],
    },
  },{
    key: "targetType",
    label: "Type",
    width: 1000,
    sortable: true,
    enableEdit: true,
    enableCreate: true,
    required: false,
    hidden: false,
    showSearch: true,
    inputType: "dropdown",
    dropdownData: {
      displayMember: "label",
      keyMember: "value",
      data: [{
        value: "0",
        label: "Individual",
      },{
        value: "1",
        label: "Department",
      },{
        value: "2",
        label: "Company",
      }

      ],
    },
  },
]);
const AddFilterAndSearch = (filters: Filter[]) => {
  listViewRef.value?.onAddFilterAndSearch(filters);
}


onBeforeMount(async () => {
  await axiosInstance.get("Objectives/periods").then((res) => {
    var result = res.data as AppResponse<string[]>;
    result.data?.forEach((element) => {
      periods.value.push({value: element, label: getDisplayString(element)})
    })
  })
})
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
.btn-add-objectives{
  position: absolute;
  top: 0;
  right: 0;
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