<template>
  <el-page-header :icon="null" class="header">
    <template #title>
      <span class="text-large font-600 mr-3">{{ props.objective.name }}</span>
    </template>
  </el-page-header>

  <div class="objective-container">
    <el-progress type="circle" :percentage="caculateObjective(props.objective)" />
    <div class="objective-details">
      <p><strong>Start Day:</strong> {{ formatDate(props.objective.startDay) }}</p>
      <p><strong>Deadline:</strong> {{ formatDate(props.objective.deadline) }}</p>
    </div>
  </div>

  <div class="key-results">
    <div v-for="item in props.objective.listKeyResults" :key="item.id" class="key-result">
      <el-progress :percentage="caculateKeyResult(item)" />
      <div class="custom-header">
        <div class="header-content">
          <span class="text-large font-600 mr-3">{{ item.description }}</span>
          <el-tag>{{ item.unit == 0 ? "Percent" : item.unit == 1 ? "Value" : "Checked" }}</el-tag>
        </div>
        <div class="header-extra">
          <el-button v-if="isLogin" type="primary" class="ml-2" @click="handleProgressUpdate(item)">
            <el-icon><EditIcon /></el-icon>
          </el-button>
        </div>
      </div>
      <el-descriptions :column="3" class="mt-4">
        <el-descriptions-item label="Progress">{{ item.currentPoint }}/{{ item.maximunPoint }}</el-descriptions-item>
      </el-descriptions>
      <div class="sidequests">
        <el-checkbox
          size="large"
          v-for="sidequest in item.sidequests"
          :key="sidequest.id"
          :label="sidequest.name"
          v-model="sidequest.status"
          @change="handleChangeSidequest(sidequest)"
        />
      </div>
    </div>
  </div>

  <LineChart :searchRequest="searchRequest" :key="keyChart"/>

  <el-dialog v-model="visibleDialogProgressUpdate">
    <UpdateProgress
      :keyresults="tempKeyResults"
      @onSaveUpdateProgress="() => { emit('onSearchObjective'); visibleDialogProgressUpdate = false; }"
      @onUpdateProgress="changePoint"
    />
  </el-dialog>
</template>

<script setup lang="ts">
import { Objective } from "@/Models/Objective";
import { formatDate } from "../../Service/formatDate";
import { Edit as EditIcon } from "@element-plus/icons-vue";
import { KeyResult } from "@/Models/KeyResult";
import { onMounted, ref } from "vue";
import UpdateProgress from "@/components/ProgressUpdate/UpdateProgress.vue";
import { deepCopy } from "../../Service/deepCopy";
import { caculateKeyResult, caculateObjective } from "../../Service/OKR/caculateKeyResult";
import { Sidequest } from "@/Models/Sidequests";
import { axiosInstance } from "../../Service/axiosConfig";
import { ElMessage } from "element-plus";
import Cookies from "js-cookie";
import type { SearchRequest } from "@/components/maynghien/BaseModels/SearchRequest";
import LineChart from "@/components/okr/LineChart.vue";

const props = defineProps<{ objective: Objective }>();
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
const keyChart = ref(0);
const handleProgressUpdate = (keyresults: KeyResult) => {
  tempKeyResults.value = deepCopy(keyresults);
  visibleDialogProgressUpdate.value = true;
};

const handleChangeSidequest = (item: Sidequest) => {
  axiosInstance.put("Sidequests", item).then((res) => {
    if (res.data.isSuccess) {
      emit("onSearchObjective");
    } else {
      ElMessage.error(res.data.message);
    }
  });
};

const changePoint = (point: number, keyresultId: string) => {
  keyChart.value += 1;
  props.objective.listKeyResults.filter(x => x.id === keyresultId)[0].currentPoint = point;
};

onMounted(() => {
  if (Cookies.get("accessToken")) {
    isLogin.value = true;
  }
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
