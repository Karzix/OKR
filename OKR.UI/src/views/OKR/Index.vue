<template>
  <el-card style="">
    <template #header>
      <div class="card-header">
        <el-progress type="circle" :percentage="overalProgress" />
        <el-button type="primary" @click="CreateObjectives">new objective</el-button>
      </div>
      <BodyIndex :data="data" @onEditObjective="editObjective" @onDeatail="handleDeatail"></BodyIndex>
    </template>
 
  </el-card>
  <el-dialog v-model="createDialog" class="dialogOKR">
    <CreateObjective :objective="editItem" :is-edit="EditDialog" @onSearchObjective="Search()"></CreateObjective>
  </el-dialog>
  <el-dialog v-model="DeatailDialog" class="dialogOKR">
    <Deatail :objective="editItem" @onSearchObjective="Search()" ></Deatail>
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
import { Edit } from '@element-plus/icons-vue'
import {deepCopy} from '../../Service/deepCopy'
import { RecalculateTheDate } from '../../Service/formatDate';
import Deatail from '@/views/OKR/Deatail.vue'
import BodyIndex from '@/components/okr/BodyIndex.vue'

const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
  SortBy: undefined
})
const editItem = ref<Objective>({
  id: undefined,
  name: "",
  startDay: undefined,
  deadline: undefined,
  listKeyResults: [],
  targetTypeId: undefined,
  targetTypeName: "",
  point: 0,
});
const data = ref<SearchResponse<Objective[]>>({
  data: undefined,
  totalRows: 0,
  totalPages: 0,
  currentPage: 1,
  rowsPerPage: 0,
})
const overalProgress = ref(0);
const createDialog = ref(false);
const EditDialog = ref(false);
const DeatailDialog = ref(false);
const Search = async () => {
  var responeSeach = await axiosInstance.post("Objectives/search", searchRequest.value)
  data.value = responeSeach.data.data
  data.value.data?.forEach((item) => {
    item.deadline = RecalculateTheDate(item.deadline)
    item.startDay = RecalculateTheDate(item.startDay)
    item.listKeyResults?.forEach((keyResult) => {
      keyResult.deadline = RecalculateTheDate(keyResult.deadline)
    })
  })
  var responeOverallProgress = await axiosInstance.post("Objectives/overal-progress",searchRequest.value)
  overalProgress.value = responeOverallProgress.data.data
}
Search()
const editObjective = (objective: Objective) => {
  console.log(objective);
  editItem.value = deepCopy(objective);
  EditDialog.value = true
  createDialog.value = true
}
const CreateObjectives = () => {
  EditDialog.value = false
  createDialog.value = true
}
const handleDeatail = (objective: Objective) => {
  console.log(objective);
  editItem.value = deepCopy(objective);
  DeatailDialog.value = true
}
</script>
<style >
@media screen and (max-width: 600px) {
  .dialogOKR{
    width: 100% !important;
  }
}
</style>
<style scoped>
.sidequests {
    display: flex;
    flex-direction: column;
}
</style>