<template>
  <el-page-header :icon="null" class="header">
    <template #title>
      <span class="text-large font-600 mr-3">{{ props.objective.name }}</span>
    </template>
  </el-page-header>

  <div class="objective-container">
    <el-progress type="circle" :percentage="caculateObjective(props.objective)" />
    <div class="objective-details">
      <p><strong>Create by:</strong> {{ props.objective.createBy ?? "karzix demo" }} </p>
      <p><strong>Start Day:</strong> {{ formatDate(props.objective.startDay) }}</p>
      <p><strong>Deadline:</strong> {{ formatDate(props.objective.deadline) }}</p>
    </div>
  </div>
  <el-tabs v-model="page">
    <el-tab-pane label="Objectives" name="Objectives">
      <keyresultsOfObjectives :objective="props.objective" @on-search-objective="() => {keyChart++ ;emit('onSearchObjective')}" :is-guest="props.isGuest"></keyresultsOfObjectives>
      <LineChart :searchRequest="searchRequest" :key="keyChart"/>
    </el-tab-pane>
    <el-tab-pane label="Progress Update" name="ProgressUpdate">
      <ProgressUpdate :search-request="searchRequest" :key="page"></ProgressUpdate>
    </el-tab-pane>
    <el-tab-pane label="Comment" name="Comment">
      Comment
    </el-tab-pane>
  </el-tabs>
  

</template>

<script setup lang="ts">
import { Objective } from "@/Models/Objective";
import { formatDate } from "../../Service/formatDate";
import { KeyResult } from "@/Models/KeyResult";
import { onMounted, ref } from "vue";
import UpdateProgress from "@/components/ProgressUpdate/UpdateProgress.vue";
import { caculateKeyResult, caculateObjective } from "../../Service/OKR/caculateKeyResult";
import Cookies from "js-cookie";
import type { SearchRequest } from "@/components/maynghien/BaseModels/SearchRequest";
import LineChart from "@/components/okr/LineChart.vue";
import keyresultsOfObjectives from "./Objectives/keyresults.vue";
import ProgressUpdate from "./ProgressUpdate.vue";
import { Filter } from "../maynghien/BaseModels/Filter";
import { handleFilterBeforSearch, addFilter } from "../maynghien/Common/handleSearchFilter";

const props = defineProps<{
   objective: Objective,
   isGuest? : boolean,
  }>();
const emit = defineEmits<{
  (e: "close"): void;
  (e: "onSearchObjective"): void;
}>();
const tempKeyResults = ref<KeyResult>({
  id: undefined,
  description: "",
  active: true,
  deadline: undefined,
  unit: 0,
  currentPoint: 0,
  maximunPoint: 100,
  sidequests: [],
  note: "",
});
const isLogin = ref(false);
const visibleDialogProgressUpdate = ref(false);
const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [{
    FieldName: "objectivesId",
    Value: props.objective.id,
    Operation: "eq",
    dropdownData: undefined,
    DisplayName: undefined,
    Type: undefined,
  }],
  SortBy: {
    FieldName: "createOn",
    Ascending: true,
  },
});

const page = ref("Objectives");
const keyChart = ref(0);
const changePoint = (point: number, keyresultId: string) => {
  props.objective.listKeyResults.filter(x => x.id === keyresultId)[0].currentPoint = point;
};

onMounted(() => {
  if (Cookies.get("accessToken")) {
    isLogin.value = true;
  }
  var filter = new Filter();
  filter.FieldName = "objectivesId";
  filter.Value = props.objective.id;
  addFilter(searchRequest.value.filters as [], filter);
});
</script>

<style scoped>
.header {
  padding: 20px 0;
  background-color: #f5f7fa;
  border-bottom: 1px solid #ebeef5;
  margin-bottom: 20px;
}

.objective-container {
  display: flex;
  align-items: center;
  margin-bottom: 30px;
}

.objective-details {
  margin-left: 20px;
}

.key-results {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.key-result {
  background: #fff;
  border: 1px solid #ebeef5;
  border-radius: 8px;
  padding: 16px;
}

.custom-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 8px;
  border-bottom: 1px solid #ebeef5;
  margin-bottom: 16px;
}

.header-content {
  display: flex;
  align-items: center;
}

.header-extra {
  display: flex;
  align-items: center;
}

.sidequests {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-top: 20px;
}
</style>
