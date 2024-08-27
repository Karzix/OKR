<template>
  <div id="info-user">
    <el-card style="width: 20%">
      <p>Name: karzix</p>
      <p>Department: cc</p>
    </el-card>
    <div style="width: 80%" class="list-objcetive">
      <el-card>
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
          </el-card>
          more.....
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
          </el-card>
          more.....
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
          </el-card>
          more.....
        </el-card>
      </el-card>
    </div>
  </div>
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

const percentage = ref(50);

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
  Branch.value =await Search(searchRequest);
  searchRequest.filters = [];
  filter.Value = "2";
  addFilter(searchRequest.filters, filter);
  Team.value =await Search(searchRequest);
});
</script>
<style>
#info-user {
  display: flex;
}

.list-objcetive .el-card .el-card {
  margin: 5px;
}
</style>async 
