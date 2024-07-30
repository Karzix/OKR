<template>
  <el-card style="">
    <template #header>
      <div class="card-header">
        <el-progress type="circle" :percentage="overalProgress" />
        <el-button type="primary" @click="Create = true">new objective</el-button>
      </div>
    </template>
    <el-card style="" v-for="item in data.data">
      <div style="">
        <h3>objective name</h3>
        <el-progress :percentage="item.point" :color="customColorMethod" />
      </div>
      <el-tree :data="buildTree(item)" :props="defaultProps" />
    </el-card>
  </el-card>
  <el-dialog v-model="Create" class="createDialog">
    <CreateObjective></CreateObjective>
  </el-dialog>
</template>
<script lang="ts" setup>
import { ref } from "vue";
import { AppResponse } from '../../components/maynghien/adminTable/Models/AppRespone';
import { SearchResponse } from '../../components/maynghien/BaseModels/SearchResponse';
import { Objective } from '../../Models/Objective';
import { axiosInstance } from '../../Service/axiosConfig'
import { SearchRequest } from '../../components/maynghien/BaseModels/SearchRequest';
import CreateObjective from './CreateOKR.vue';
interface Tree {
  label: string;
  children?: Tree[];
}
const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
  SortBy: undefined
})
const defaultProps = {
  children: "children",
  label: "label",
};
const customColorMethod = (percentage: number) => {
  if (percentage < 30) {
    return "#909399";
  }
  if (percentage < 70) {
    return "#e6a23c";
  }
  return "#67c23a";
};
const data = ref<SearchResponse<Objective[]>>({
  data: undefined,
  totalRows: 0,
  totalPages: 0,
  currentPage: 1,
  rowsPerPage: 0,
})
const overalProgress = ref(0);
const Create = ref(false);
const buildTree = (objective: Objective) : Tree[] => {
  var dataTreeTemp = [] as Tree[] 
  for(let i  = 0; i < objective.listKeyResults?.length; i++) {
    var newTree = {
          label: "",
          children:[] as Tree[]
        } as Tree
    newTree.label= objective.listKeyResults[i].description ?? '';
    for(let j  = 0; j < (objective.listKeyResults[i]?.sidequests?.length ?? 0); j++) {
      newTree.children?.push({
        label: objective.listKeyResults[i].sidequests[j].name ?? '',
        children:[] as Tree[]
      })
    }
    dataTreeTemp.push(newTree);
  }
  return dataTreeTemp
}
const Search = async () => {
  var responeSeach = await axiosInstance.post("Objectives/search", searchRequest.value)
  data.value = responeSeach.data.data
  var responeOverallProgress = await axiosInstance.post("Objectives/overal-progress",searchRequest.value)
  overalProgress.value = responeOverallProgress.data.data
}
Search()
</script>
