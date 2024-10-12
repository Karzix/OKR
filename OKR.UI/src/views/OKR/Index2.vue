<template>
  <el-card class="profile-card">
    <template #header>
      <div class="filter-search">
        <MnActionPane
          :allowAdd="false"
          :tableColumns="tableColumns"
          :isEdit="false"
          @onBtnSearchClicked="AddFilterAndSearch"
          :CustomActions="[]"
          :openDialog="() => {}"
        />
      </div>
    </template>

    <div class="sidebar"></div>
    <div class="content-container">

      <div class="card-header">
        <div class="progress-container">
          <el-progress
            type="dashboard"
            :percentage="overalProgress"
            stroke-width="10"
          >
            <template #default="{ percentage }">
              <span class="percentage-value">{{ percentage }}%</span>
              <span class="percentage-label">Progressing</span>
            </template>
          </el-progress>
          <el-button
            type="primary"
            @click="CreateObjectives"
            class="new-objective-btn"
            >New Objective</el-button
          >
          <el-button-group class="mb-2 button-group">
            <el-button
              type="primary"
              plain
              @click="page = 0"
              :class="page == 0 ? 'page-select' : ''"
              >Objectives</el-button
            >
            <el-button
              type="primary"
              plain
              @click="page = 1"
              :class="page == 1 ? 'page-select' : ''"
              >Progress</el-button
            >
          </el-button-group>
        </div>
      </div>
      <div class="tabs-container">
        <el-tabs v-model="targetType" class="custom-tabs">
          <el-tab-pane
            v-for="item in targetTypeValues"
            :key="item"
            :label="TargetType[item]"
            :name="item.toString()"
          >
            <div class="tab-content" v-if="targetType == item.toString()">
              <div v-if="page == 0">
                <BodyIndex
                  :search-request="searchRequest"
                  @onEditObjective="editObjective"
                  @onDeatail="handleDeatail"
                  :key="bodyIndexKey"
                />
              </div>
              <div v-if="page == 1">
                <ProgressUpdates
                  :search-request="searchRequest"
                  :test="TargetType[item]"
                />
              </div>
            </div>
          </el-tab-pane>
        </el-tabs>
      </div>
    </div>
  </el-card>

  <el-dialog v-model="createDialog" class="OKR-Index2-dialogOKR">
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
    />
  </el-dialog>
  <el-dialog v-model="DeatailDialog" class="OKR-Index2-dialogOKR">
    <Deatail
      :objective="editItem"
      @onSearchObjective="Search()"
      :target-type="targetType"
      :entity-objectives-id="entityObjectivesId"
      v-if="DeatailDialog"
    />
  </el-dialog>
</template>

<script lang="ts" setup>
import { onMounted, ref, watch } from "vue";
import { SearchResponse } from "../../components/maynghien/BaseModels/SearchResponse";
import { Objective } from "../../Models/Objective";
import { axiosInstance } from "../../Service/axiosConfig";
import { SearchRequest } from "../../components/maynghien/BaseModels/SearchRequest";
import CreateObjective from "./CreateOKR.vue";
import { Edit } from "@element-plus/icons-vue";
import { deepCopy } from "../../Service/deepCopy";
import { RecalculateTheDate } from "../../Service/formatDate";
import Deatail from "@/components/okr/DeatailObjectives.vue";
import BodyIndex from "@/components/okr/BodyIndex.vue";
import ProgressUpdates from "@/components/okr/ProgressUpdate.vue";
import MnActionPane from "@/components/maynghien/adminTable/MnActionPane.vue";
import { TableColumn } from "@/components/maynghien/adminTable/Models/TableColumn";
import { Filter } from "@/components/maynghien/BaseModels/Filter";
import * as handleSearch from "@/components/maynghien/Common/handleSearchFilter";
import Cookies from "js-cookie";
import { useRoute, useRouter } from "vue-router";
import { TargetType } from "@/Models/Enum/TargetType";
import type { EntityObjectives } from "@/Models/EntityObjectives";

const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
  SortBy: undefined,
});

const editItem = ref<EntityObjectives>({
  id: undefined,
  name: "",
  startDay: undefined,
  deadline: undefined,
  listKeyResults: [],
  targetType: undefined,
  targetTypeName: "",
  point: 0,
  objectivesId: undefined,
  status: 0,
});

const tableColumns = ref<TableColumn[]>([
  {
    key: "createOn",
    label: "",
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
]);
const page = ref<number>(0);
const overalProgress = ref(0);
const createDialog = ref(false);
const EditDialog = ref(false);
const DeatailDialog = ref(false);
const route = useRoute();
const targetType = ref<string>("0");
const searchKey = ref<string>("");
const bodyIndexKey = ref(0);
const progressUpdatesKey = ref(0);
const entityObjectivesId = ref("");
const targetTypeValues = Object.keys(TargetType)
  .map((key) => Number(key))
  .filter((value) => !isNaN(value));
const Search = async () => {
  createDialog.value = false;
  EditDialog.value = false;
  handleSearch.handleFilterBeforSearch(searchRequest.value.filters);
  var responeOverallProgress = await axiosInstance.post(
    "Objectives/overal-progress",
    searchRequest.value
  );
  overalProgress.value = responeOverallProgress.data.data;

  if (page.value === 0) {
    bodyIndexKey.value++;
  } else if (page.value === 1) {
    progressUpdatesKey.value++;
  }
  searchKey.value = `${targetType.value}-${page.value}-${bodyIndexKey.value}-${progressUpdatesKey.value}`;
};

const editObjective = (entityObjectives: EntityObjectives) => {
  entityObjectivesId.value = entityObjectives.id ?? "";
  editItem.value = entityObjectives;
  EditDialog.value = true;
  createDialog.value = true;
};

const CreateObjectives = () => {
  EditDialog.value = false;
  createDialog.value = true;
};

const handleDeatail = (entityObjectives: EntityObjectives) => {
  entityObjectivesId.value = entityObjectives.id ?? "";
  editItem.value = entityObjectives;
  DeatailDialog.value = true;
};

const AddFilterAndSearch = (filters: Filter[]) => {
  filters.forEach((filter) => {
    handleSearch.addFilter(searchRequest.value.filters as [], filter);
  });
  // searchRequest.value.filters?.push(...filters);
  Search();
};

const AddFilterTargetType = (emunTarget: string) => {
  var filterTargetType = new Filter();
  filterTargetType.FieldName = "targetType";
  filterTargetType.Value = emunTarget;
  handleSearch.addFilter(searchRequest.value.filters ?? [], filterTargetType);
};

onMounted(async () => {
  if (route.params.UserName != undefined) {
    var fil = new Filter();
    fil.FieldName = "createBy";
    fil.Value = route.params.UserName.toString();
    handleSearch.addFilter(searchRequest.value.filters as [], fil);
  } else {
  }
  AddFilterTargetType(targetType.value);
  // Search();
  var responeOverallProgress = await axiosInstance.post(
    "Objectives/overal-progress",
    searchRequest.value
  );
  overalProgress.value = responeOverallProgress.data.data;
});
watch(
  () => targetType.value,
  () => {
    searchRequest.value.PageIndex = 1;
    AddFilterTargetType(targetType.value);
    Search();
  }
);
</script>
<style>
@media screen and (max-width: 600px) {
  .OKR-Index2-dialogOKR {
    width: 100% !important;
  }
}
</style>

<style scoped>
.profile-card {
  /* max-width: 9000px; */
  margin: auto;
  background-color: #f8f9faa6;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  min-height: 80vh;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 20px;
  flex-wrap: wrap;
}

.filter-search {
  width: 100%;
  margin-bottom: 20px;
}

.progress-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
  position: relative;
  width: 200px;
  height: 200px;
}

.new-objective-btn {
  margin-top: 20px;
  background-color: #007bff;
  border-color: #007bff;
  color: #fff;
}

.content-container {
  display: flex;
  margin-top: 20px;
}

.sidebar {
  display: flex;
  flex-direction: column;
  margin-right: 20px;
}

.tabs-container {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
}

.custom-tabs {
  flex-grow: 1;
}

.tab-content {
  margin-top: 10px;
  padding-left: 20px;
}

.button-group {
  display: flex;
  flex-direction: row;
}

.button-group .el-button {
  border-radius: 20px;
  padding: 10px 20px;
  font-weight: bold;
}

.percentage-value {
  display: block;
  font-size: 27px;
  font-weight: bold;
  color: #409eff;
}

.percentage-label {
  display: block;
  margin-top: 5px;
  font-size: 14px;
  color: #909399;
}

@media screen and (max-width: 768px) {
  .card-header {
    flex-direction: column;
    align-items: center;
  }
  .filter-search {
    width: 100%;
  }
  .content-container {
    flex-direction: column;
  }
  .sidebar {
    flex-direction: row;
    margin-bottom: 20px;
  }
  .tabs-container {
    width: 100%;
  }
}
.page-select {
  background-color: #007bff;
  color: #fff;
}
</style>
<style>
.OKR-Index2-dialogOKR {
  margin-top: 0 !important;
  margin-bottom: 0 !important;
}
/* .profile-card {
} */
</style>
