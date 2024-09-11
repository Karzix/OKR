<template>
  <div id="info-user" style="display: flex; justify-content: space-between;">
    <div style="width: 100%" class="list-objcetive">
      <div style="width: 100%; display: flex; justify-content: flex-end;">
        <el-card style="width: 100%; text-align: center;">
            <p style="text-align: left">Name: karzix</p>
            <p style="text-align: left;">Department: cc</p>
        </el-card>
    </div>
      <el-card>
        <template #header>
          <div class="card-header">
            <span>Individual</span>
            <el-progress :percentage="percentage" :color="customColors" />
          </div>
        </template>
        <el-card v-for="item in Individual">
          {{ item.name }}
          <el-progress :percentage="item.point" :color="customColors" />
          <el-tree :data="buildTree(item)" :props="defaultProps" />
        </el-card>
        <p @click="showDialog = true">Read More</p>
      </el-card>
      <el-card>
        <template #header>
          <div class="card-header">
            <span>Branch</span>
            <el-progress :percentage="percentage" :color="customColors" />
          </div>
        </template>
        <el-card v-for="item in Branch">
          {{ item.name }}
          <el-progress :percentage="item.point" :color="customColors" />
          <el-tree :data="buildTree(item)" :props="defaultProps" />
        </el-card>
        <p>Read More</p>
      </el-card>
      <el-card>
        <template #header>
          <div class="card-header">
            <span>Team</span>
            <el-progress :percentage="percentage" :color="customColors" />
          </div>
        </template>
        <el-card v-for="item in Team">
          {{ item.name }}
          <el-progress :percentage="item.point" :color="customColors" />
          <el-tree :data="buildTree(item)" :props="defaultProps" />
        </el-card>
        <p>Read More</p>
      </el-card>
    </div>
  </div>
  <el-dialog v-model="showDialog">
    <DialogListObjectives
      :filters="DialogListObjectives_SearchRequest.filters"
      :title="TargetType.Individual"
      :showDialog="showDialog"
    ></DialogListObjectives>
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

const percentage = ref(50);
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

const handleShowDialog = (TargetType: string) => {};
</script>
<style>
#info-user {
  display: flex;
  gap: 20px;
  padding: 20px;
  background-color: #f9f9f9;
}

.el-card {
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  background-color: #fff;
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: bold;
  font-size: 18px;
  color: #333;
}

.el-progress {
  width: 100px;
}

.el-card .el-progress {
  margin-top: 10px;
}

.el-tree {
  margin-top: 10px;
}

.list-objcetive {
  width: 80%;
}

.list-objcetive .el-card {
  margin-bottom: 20px;
}

.list-objcetive .el-card .el-card {
  margin-bottom: 15px;
}
</style>