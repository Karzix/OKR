<template>
  <div class="deatail-objectives">
    <div class="objective-header">
      <h1>{{ props.objective.name }}</h1>
      <div>
        <el-icon @click="copyLink()"><Share /></el-icon>
      </div>
    </div>

    <div class="objective-container">
      <el-progress
        type="circle"
        :percentage="caculateObjective(props.objective)"
      />
      <div class="objective-details">
        <p>
          <strong>Create by:</strong>
          {{ props.objective.createBy ?? "Unknow" }}
        </p>
        <p>
          <strong>Start Day:</strong> {{ formatDate(props.objective.startDay) }}
        </p>
        <p>
          <strong>Deadline:</strong> {{ formatDate(props.objective.deadline) }}
        </p>
      </div>
    </div>
    <el-tabs v-model="page">
      <el-tab-pane label="Objectives" name="Objectives">
        <keyresultsOfObjectives
          :objective="props.objective"
          @on-search-objective="
            () => {
              keyChart++;
              emit('onSearchObjective');
            }
          "
          :is-guest="props.isGuest"
        ></keyresultsOfObjectives>
        <LineChart :searchRequest="searchRequest" :key="keyChart" />
      </el-tab-pane>
      <el-tab-pane label="Progress Update" name="ProgressUpdate">
        <ProgressUpdate
          :search-request="searchRequest"
          :key="page"
          v-if="page === 'ProgressUpdate'"
        ></ProgressUpdate>
      </el-tab-pane>
      <el-tab-pane label="Comment" name="Comment">
        <EvaluateTarget
          :search-request="searchRequest"
          :target-type="props.targetType"
          v-if="page === 'Comment'"
        ></EvaluateTarget>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup lang="ts">
import { Objective } from "@/Models/Objective";
import { formatDate } from "../../Service/formatDate";
import { KeyResult } from "@/Models/KeyResult";
import { onMounted, ref } from "vue";
import UpdateProgress from "@/components/ProgressUpdate/UpdateProgress.vue";
import {
  caculateKeyResult,
  caculateObjective,
} from "../../Service/OKR/caculateKeyResult";
import Cookies from "js-cookie";
import type { SearchRequest } from "@/components/maynghien/BaseModels/SearchRequest";
import LineChart from "@/components/okr/LineChart.vue";
import keyresultsOfObjectives from "./Objectives/keyresults.vue";
import ProgressUpdate from "./ProgressUpdate.vue";
import { Filter } from "../maynghien/BaseModels/Filter";
import {
  handleFilterBeforSearch,
  addFilter,
} from "../maynghien/Common/handleSearchFilter";
import SeeObjectives from "@/views/OKR/SeeObjectives.vue";
import EvaluateTarget from "./EvaluateTarget/EvaluateTarget.vue";
import { Share } from "@element-plus/icons-vue";
import { axiosInstance, urlUI } from "@/Service/axiosConfig";
import { ElMessage } from "element-plus";

const props = defineProps<{
  objective: Objective;
  isGuest?: boolean;
  targetType: string;
  entityObjectivesId: string;
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
  filters: [
    {
      FieldName: "objectivesId",
      Value: props.objective.id,
      Operation: "eq",
      dropdownData: undefined,
      DisplayName: undefined,
      Type: undefined,
    },
  ],
  SortBy: {
    FieldName: "createOn",
    Ascending: true,
  },
});

const page = ref("Objectives");
const keyChart = ref(0);
const changePoint = (point: number, keyresultId: string) => {
  props.objective.listKeyResults.filter(
    (x) => x.id === keyresultId
  )[0].currentPoint = point;
};

onMounted(() => {
  if (Cookies.get("accessToken")) {
    isLogin.value = true;
  }
  var filter = new Filter();
  filter.FieldName = "objectivesId";
  filter.Value = props.objective.id;
  addFilter(searchRequest.value.filters as [], filter);
  filter.FieldName = "entityObjectivesId";
  filter.Value = props.entityObjectivesId;
  addFilter(searchRequest.value.filters as [], filter);
});

const copyLink = () => {
  try{
    const text = `${urlUI}Objectives=${props.entityObjectivesId}&${props.targetType}`;
 
    var textField = document.createElement('textarea');
    textField.innerText = text;
    document.body.appendChild(textField);
    textField.select();
    document.execCommand('copy');
    document.body.removeChild(textField);
    ElMessage.success("Đã sao chép liên kết");
  }
  catch(err){
    ElMessage.error("Không thể sao chép liên kết: " + err);
  }

};
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
.objective-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.objective-header > div > i {
  font-size: 25px;
}
.deatail-objectives {
  max-width: 70vw;
  margin: 0 auto;
  background-color: #f8f9faa6;
  padding: 20px;
  border-radius: 15px;
}
</style>
