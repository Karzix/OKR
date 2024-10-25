<template>
  <!-- <el-dialog v-model="showDialog"> -->
    <el-tabs v-model="taps" class="demo-tabs" v-if="!load">
      <el-tab-pane label="objectives" name="objectives">
        <Objectives :searchRequest="searchRequest" @onDeatail="handleDetail"></Objectives>
      </el-tab-pane>
      <el-tab-pane label="progress" name="progress">
        <ProgressUpdate :searchRequest="searchRequest"></ProgressUpdate>
      </el-tab-pane>
    </el-tabs>
  <!-- </el-dialog> -->
</template>
<script setup lang="ts">
import { onMounted, ref } from "vue";
import { Filter } from "../maynghien/BaseModels/Filter";
import type { SearchRequest } from "../maynghien/BaseModels/SearchRequest";
import ProgressUpdate from "./ProgressUpdate.vue";
import Objectives from './BodyIndex.vue'
import { SearchResponse } from "../maynghien/BaseModels/SearchResponse";
import type { Objective } from "@/Models/Objective";
import * as handleSearch from '@/components/maynghien/Common/handleSearchFilter'
import { axiosInstance } from "@/Service/axiosConfig";
import { TargetType } from "@/Models/Enum/TargetType";
import type { EntityObjectives } from "@/Models/EntityObjectives";
import router from "@/router";
import { useRoute } from "vue-router";

const props = defineProps<{
  filters?: Filter[];
  showDialog: boolean;
  title: string;
}>();
const taps = ref<string>("objectives");
const route = useRoute();
const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
  SortBy: undefined,
});
const load = ref<boolean>(true);
// const searchResponseObjectives = ref<SearchResponse<Objective[]>>({
//   data: [] as Objective[],
//   totalRows: 0,
//   totalPages: 0,
//   currentPage: 1,
//   rowsPerPage: 0,
// });
// const search = async () => {
//   await axiosInstance.post("Objectives/search", searchRequest.value).then(
//     (res) => {
//       searchResponseObjectives.value = res.data;
//     }
//   )
// }
onMounted( () => {
  var targetType = new Filter();
  targetType.FieldName = "targetType";
  targetType.Value = props.title.toString();
  if(searchRequest.value.filters){
    searchRequest.value.filters = [];
  }
  handleSearch.addFilter(searchRequest.value.filters as [], targetType);
  var filtercreateBy = new Filter();
  // if(targetType.Value == "0"){
    filtercreateBy.FieldName = "createBy";
  // }
  // else{
  //   filtercreateBy.FieldName = "userName";
  // }
  filtercreateBy.Value = route.params.UserName.toString();
  handleSearch.addFilter(searchRequest.value.filters as [], filtercreateBy);
//  search()
  load.value = false
});
const handleDetail = (entityObjectives: EntityObjectives)=> {
  router.push('objectives=' + entityObjectives.id+ '&'+entityObjectives.targetType);
}
</script>
<style scoped>
</style>
