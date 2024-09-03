<template>
  <el-card style="" v-for="item in searchResponseObjectives.data" @click=" handleDeatail(item)">
    <div style="">
      <h3>
        {{ item.name }} <el-icon @click="() =>{if(isLogin)editObjective(item)}"><Edit /></el-icon>
      </h3>
      <el-progress :percentage="item.point" :color="customColorMethod" />
    </div>
    <el-tree :data="buildTree(item)" :props="defaultProps" />
  </el-card>
</template>
<script setup lang="ts">
import type { Objective } from '@/Models/Objective';
import { SearchResponse } from '../../components/maynghien/BaseModels/SearchResponse';
import Cookies from 'js-cookie';
import { onMounted, ref } from 'vue';
import { axiosInstance } from '@/Service/axiosConfig';
import type { SearchRequest } from '../maynghien/BaseModels/SearchRequest';
import { RecalculateTheDate } from '@/Service/formatDate';
import * as handleSearch from '@/components/maynghien/Common/handleSearchFilter';

const props = defineProps<{
  searchRequest: SearchRequest;
}>();
const emit = defineEmits<{
  (e: "onEditObjective", objective: Objective): void;
  (e: "onDeatail", objective: Objective): void;
}>();
const defaultProps = {
  children: "children",
  label: "label",
};
interface Tree {
  label: string;
  children?: Tree[];
}
const isLogin = ref(false)
const searchResponseObjectives = ref<SearchResponse<Objective[]>>({
  data: [] as Objective[],
  totalRows: 0,
  totalPages: 0,
  currentPage: 1,
  rowsPerPage: 0,
});
const customColorMethod = (percentage: number) => {
  if (percentage < 30) {
    return "#909399";
  }
  if (percentage < 70) {
    return "#e6a23c";
  }
  return "#67c23a";
};
const editObjective = (objective: Objective) =>{
    emit("onEditObjective",objective)
}
const handleDeatail = (objective: Objective) => {
    emit("onDeatail",objective)
}
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
onMounted(() => {
  if(!Cookies.get("accessToken")){

  }
  else{
    isLogin.value = true
  }
})
const Search = async () => {
  var responeSeach = await axiosInstance.post(
    "Objectives/search",
    props.searchRequest
  );
  searchResponseObjectives.value = responeSeach.data.data;
  if(!searchResponseObjectives.value.data){
    searchResponseObjectives.value.data = []
  }
  searchResponseObjectives.value.data?.forEach((item) => {
    item.deadline = RecalculateTheDate(item.deadline);
    item.startDay = RecalculateTheDate(item.startDay);
    item.listKeyResults?.forEach((keyResult) => {
      keyResult.deadline = RecalculateTheDate(keyResult.deadline);
    });
  });
};
onMounted(() => {
  Search();
});
</script>