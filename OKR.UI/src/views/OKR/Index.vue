<template>
  <el-card style="">
    <template #header>
      <div class="card-header">
        <div class="dl-flex">
          <el-progress type="circle" :percentage="overalProgress" />
          <div class="buttons">
            <el-button-group>
              <el-button type="primary" @click="CreateObjectives" v-if="isLogin == true"
                >new objective</el-button
              >
              <el-button type="primary" @click="page = 0">Home</el-button>
              <el-button type="primary" @click="page = 1">progress</el-button>
            </el-button-group>
            <el-button-group>
              <el-button type="info" @click="() => {AddFilterTargetType('0'); searchRequest.PageIndex =1 ;Search()}">individual</el-button>
              <el-button type="warning" @click="() => {AddFilterTargetType('1');searchRequest.PageIndex =1 ; Search()}">branch</el-button>
              <el-button type="danger" @click="() => {AddFilterTargetType('2');searchRequest.PageIndex =1 ; Search()}">team</el-button>
            </el-button-group>
          </div>
        </div>
    
   
        <div class="filter-search">
          <MnActionPane
            :allowAdd="false"
            :tableColumns="tableColumns"
            :isEdit="false"
            @onBtnSearchClicked="AddFilterAndSearch"
            :CustomActions="[]"
            :openDialog="() => {}"
          >
          </MnActionPane>
        </div>
      </div>
    </template>
    <BodyIndex
      :data="data"
      @onEditObjective="editObjective"
      @onDeatail="handleDeatail"
      v-if="page == 0"
    ></BodyIndex>
    <ProgressUpdates v-if="page == 1" :search-request="searchRequest"></ProgressUpdates>
  </el-card>
  <el-dialog v-model="createDialog" class="dialogOKR">
    <CreateObjective
      :objective="editItem"
      :is-edit="EditDialog"
      @onSearchObjective="Search()"
      @onClose="
        () => {
          createDialog = false;
          EditDialog = false;
        }
      "
    ></CreateObjective>
  </el-dialog>
  <el-dialog v-model="DeatailDialog" class="dialogOKR">
    <Deatail :objective="editItem" @onSearchObjective="Search()"></Deatail>
  </el-dialog>
</template>
<script lang="ts" setup>
import { onMounted, ref } from "vue";
import { AppResponse } from "../../components/maynghien/adminTable/Models/AppRespone";
import { SearchResponse } from "../../components/maynghien/BaseModels/SearchResponse";
import { Objective } from "../../Models/Objective";
import { axiosInstance } from "../../Service/axiosConfig";
import { SearchRequest } from "../../components/maynghien/BaseModels/SearchRequest";
import CreateObjective from "./CreateOKR.vue";
import { Edit } from "@element-plus/icons-vue";
import { deepCopy } from "../../Service/deepCopy";
import { RecalculateTheDate } from "../../Service/formatDate";
import Deatail from "@/views/OKR/Deatail.vue";
import BodyIndex from "@/components/okr/BodyIndex.vue";
import ProgressUpdates from "@/components/okr/ProgressUpdate.vue";
import MnActionPane from "@/components/maynghien/adminTable/MnActionPane.vue";
import { CustomAction } from "@/components/maynghien/adminTable/Models/CustomAction";
import { TableColumn } from "@/components/maynghien/adminTable/Models/TableColumn";
import { Filter } from "@/components/maynghien/BaseModels/Filter";
import * as handleSearch from "@/components/maynghien/Common/handleSearchFilter";
import Cookies from "js-cookie";
import { useRoute, useRouter } from "vue-router";


const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
  SortBy: undefined,
});
const editItem = ref<Objective>({
  id: undefined,
  name: "",
  startDay: undefined,
  deadline: undefined,
  listKeyResults: [],
  targetType: undefined,
  targetTypeName: "",
  point: 0,
});
const data = ref<SearchResponse<Objective[]>>({
  data: undefined,
  totalRows: 0,
  totalPages: 0,
  currentPage: 1,
  rowsPerPage: 0,
});
const isLogin = ref<boolean>(false);
const tableColumns = ref<TableColumn[]>([
  {
    key: "createOn",
    label: "Date",
    width: 1000,
    sortable: true,
    enableEdit: true,
    enableCreate: true,
    required: false,
    hidden: false,
    showSearch: true,
    inputType: "date",
    dropdownData: null,
  },
  {
    key: "targetType",
    label: "Target Type",
    enableEdit: true,
    enableCreate: true,
    hidden: false,
    width: 300,
    required: true,
    sortable: false,
    showSearch: true,
    inputType: "dropdown",
    dropdownData: {
      displayMember: "name",
      keyMember: "id",
      data: [
        {
          id: "0",
          name: "Percent",
        },
        {
          id: "1",
          name: "Value",
        },
        {
          id: "2",
          name: "Checked",
        },
      ],
    },
  },
]);
const page = ref<number>(0);
const overalProgress = ref(0);
const createDialog = ref(false);
const EditDialog = ref(false);
const DeatailDialog = ref(false);
const route = useRoute()
const Search = async () => {
  handleSearch.handleFilterBeforSearch(searchRequest.value.filters);
  var responeSeach = await axiosInstance.post(
    "Objectives/search",
    searchRequest.value
  );
  data.value = responeSeach.data.data;
  data.value.data?.forEach((item) => {
    item.deadline = RecalculateTheDate(item.deadline);
    item.startDay = RecalculateTheDate(item.startDay);
    item.listKeyResults?.forEach((keyResult) => {
      keyResult.deadline = RecalculateTheDate(keyResult.deadline);
    });
  });
  var responeOverallProgress = await axiosInstance.post(
    "Objectives/overal-progress",
    searchRequest.value
  );
  overalProgress.value = responeOverallProgress.data.data;
};

const editObjective = (objective: Objective) => {
  console.log(objective);
  editItem.value = deepCopy(objective);
  EditDialog.value = true;
  createDialog.value = true;
};
const CreateObjectives = () => {
  EditDialog.value = false;
  createDialog.value = true;
};
const handleDeatail = (objective: Objective) => {
  console.log(objective);
  editItem.value = deepCopy(objective);
  DeatailDialog.value = true;
};

const AddFilterAndSearch = (filters: Filter[])=>{
  searchRequest.value.filters = filters;
  Search();
}
const AddFilterTargetType = (emunTarget: string)=>{
  var filterTargetType = new Filter();
  filterTargetType.FieldName = "targetType";
  filterTargetType.Value = emunTarget;
  handleSearch.addFilter(searchRequest.value.filters ?? [],filterTargetType);
  // Search();
}

onMounted(() => {
  if(!Cookies.get("accessToken")){
    var fil = new Filter();
    fil.FieldName = 'createBy'
    fil.Value = route.params.UserName.toString();
    handleSearch.addFilter(searchRequest.value.filters as [],fil);
  }
  else{
    isLogin.value = true
  }
  Search();
})
</script>
<style>
@media screen and (max-width: 600px) {
  .dialogOKR {
    width: 100% !important;
  }
}
</style>
<style scoped>
.sidequests {
  display: flex;
  flex-direction: column;
}
.buttons{
  display: flex;
  flex-wrap: wrap;
  align-content: center;
  flex-direction: column;
  justify-content: center;
}

.buttons .el-button{
  width: 100px;
}
.dl-flex{
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
}
</style>
