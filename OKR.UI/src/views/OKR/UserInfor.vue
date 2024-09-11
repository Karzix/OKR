<template>
  <div id="info-user" class="info-user">
    <div class="list-objective">
      <!-- Thẻ thông tin người dùng -->
      <div class="user-card">
        <el-card class="card">
          <p><strong>Name:</strong> Karzix</p>
          <p><strong>Department:</strong> CC</p>
        </el-card>
      </div>

      <!-- Thẻ mục tiêu cá nhân -->
      <el-card class="objective-card">
        <template #header>
          <div class="card-header">
            <span>Individual</span>
            <el-progress :percentage="percentageIndividual" :color="customColors" />
          </div>
        </template>
        <el-card v-for="item in Individual" :key="item.name" class="sub-card">
          <div class="objective-content">
            <h4>{{ item.name }}</h4>
            <el-progress :percentage="item.point" :color="customColors" />
            <el-tree :data="buildTree(item)" :props="defaultProps" />
          </div>
        </el-card>
        <p class="read-more" @click="handleShowDialog('0')">Read More</p>
      </el-card>

      <!-- Thẻ mục tiêu chi nhánh -->
      <el-card class="objective-card">
        <template #header>
          <div class="card-header">
            <span>Branch</span>
            <el-progress :percentage="percentageBranch" :color="customColors" />
          </div>
        </template>
        <el-card v-for="item in Branch" :key="item.name" class="sub-card">
          <div class="objective-content">
            <h4>{{ item.name }}</h4>
            <el-progress :percentage="item.point" :color="customColors" />
            <el-tree :data="buildTree(item)" :props="defaultProps" />
          </div>
        </el-card>
        <p class="read-more" @click="handleShowDialog('2')">Read More</p>
      </el-card>

      <!-- Thẻ mục tiêu nhóm -->
      <el-card class="objective-card">
        <template #header>
          <div class="card-header">
            <span>Team</span>
            <el-progress :percentage="percentageTeam" :color="customColors" />
          </div>
        </template>
        <el-card v-for="item in Team" :key="item.name" class="sub-card">
          <div class="objective-content">
            <h4>{{ item.name }}</h4>
            <el-progress :percentage="item.point" :color="customColors" />
            <el-tree :data="buildTree(item)" :props="defaultProps" />
          </div>
        </el-card>
        <p class="read-more" @click="handleShowDialog('1')">Read More</p>
      </el-card>
    </div>
  </div>

  <!-- Hộp thoại hiển thị chi tiết -->
  <el-dialog v-model="showDialog" v-if="showDialog" class="dialog-custom">
    <DialogListObjectives
      :filters="DialogListObjectives_SearchRequest.filters"
      :title="TargetType.Individual"
      :showDialog="showDialog"
    />
  </el-dialog>
</template>


<script setup lang="ts">
import { onMounted, ref } from "vue";
import { Minus, Plus } from "@element-plus/icons-vue";
import { SearchRequest } from "@/components/maynghien/BaseModels/SearchRequest";
import { axiosInstance } from "@/Service/axiosConfig";
import { SearchResponse } from "@/Models/SearchResponse";
import type { Objective } from "@/Models/Objective";
import {
  addFilter,
  removeFilter,
} from "@/components/maynghien/Common/handleSearchFilter";
import { Filter } from "@/components/maynghien/BaseModels/Filter";
import { buildTree } from "@/Service/OKR/buildTree";
import DialogListObjectives from "@/components/okr/DialogListObjectives.vue";
import { TargetType } from "@/Models/Enum/TargetType";
import * as handleSearch  from "@/components/maynghien/Common/handleSearchFilter";

const percentageIndividual = ref(50);
const percentageTeam = ref(0);
const percentageBranch = ref(0);
const showDialog = ref(false);
const customColors = [
  { color: "#909399", percentage: 40 },
  { color: "#e6a23c", percentage: 70 },
  { color: "#67c23a", percentage: 100 },
];
const defaultProps = {
  children: "children",
  label: "label",
};
const Individual = ref<Objective[]>([]);
const Branch = ref<Objective[]>([]);
const Team = ref<Objective[]>([]);
const DialogListObjectives_SearchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
  SortBy: undefined,
});

const Search = async (searchRequest: SearchRequest): Promise<Objective[]> => {
  let data = new SearchResponse<Objective>();
  data.data = [];
  try {
    await axiosInstance.post("Objectives/search", searchRequest).then((res) => {
      data = res.data.data;
      console.log(data);
    });
    return data.data;
  } catch (e) {
    console.error(e);
    return data.data;
  }
};

onMounted(async () => {
  var searchRequest = new SearchRequest();
  searchRequest.PageIndex = 1;
  searchRequest.PageSize = 5;
  searchRequest.filters = [];
  var filter = new Filter();
  filter.FieldName = "targetType";
  filter.Value = "0";

  addFilter(searchRequest.filters, filter);
  Individual.value = await Search(searchRequest);
  searchRequest.filters = [];
  filter.Value = "1";
  addFilter(searchRequest.filters, filter);
  Branch.value = await Search(searchRequest);
  searchRequest.filters = [];
  filter.Value = "2";
  addFilter(searchRequest.filters, filter);
  Team.value = await Search(searchRequest);
});

const handleShowDialog = (TargetType: string) => {
  var filter = new Filter();
  filter.FieldName = "targetType";
  filter.Value = TargetType;
  // DialogListObjectives_SearchRequest.value.filters?.push(filter);
  handleSearch.addFilter(DialogListObjectives_SearchRequest.value.filters as [], filter);
  showDialog.value = true;
};
</script>
<style scoped>
.info-user {
  padding: 20px;
  background-color: #f5f5f5;
}

.list-objective {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.user-card .card {
  margin-bottom: 20px;
  background-color: #e0f7fa;
  border: 1px solid #00838f;
}

.objective-card {
  padding: 20px;
  background-color: #ffffff;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.objective-content {
  margin-top: 10px;
}

.sub-card {
  margin-bottom: 10px;
  padding: 15px;
  border: 1px solid #ececec;
  border-radius: 8px;
  background-color: #fafafa;
}

.read-more {
  margin-top: 10px;
  color: #007bff;
  cursor: pointer;
  text-align: right;
  transition: color 0.3s;
}

.read-more:hover {
  color: #0056b3;
}

.dialog-custom .el-dialog__body {
  padding: 20px;
}

h4 {
  margin: 0 0 10px 0;
  color: #424242;
}

p {
  margin: 0;
  color: #757575;
}
</style>