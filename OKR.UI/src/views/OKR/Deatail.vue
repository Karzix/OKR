<template>
  <el-page-header :icon="null">
    <template #title>
      <span class="text-large font-600 mr-3"> {{ props.objective.name }} </span>
    </template>
  </el-page-header>
  <div>
    <el-progress type="circle" :percentage="caculateObjective(props.objective)" />
    <div>
      <p>Start Day: {{ formatDate(props.objective.startDay) }}</p>
      <p>Deadline: {{ formatDate(props.objective.deadline) }}</p>
    </div>
  </div>
  <div>
    <div v-for="item in props.objective.listKeyResults" class="key-result">
      <el-progress :percentage="caculateKeyResult(item)" />
      <div class="custom-header">
        <div class="header-content flex items-center">
          <span class="text-large font-600 mr-3">{{ item.description }}</span>
          <el-tag>{{
            item.unit == 0 ? "Percent" : item.unit == 1 ? "Value" : "Checked"
          }}</el-tag>
        </div>
        <div class="header-extra flex items-center">
          <el-button
            type="primary"
            class="ml-2"
            @click="handleProgressUpdate(item)"
            ><el-icon><EditIcon /></el-icon
          ></el-button>
        </div>
      </div>
      <el-descriptions :column="3" class="mt-4">
        <el-descriptions-item label="Description">
          {{ item.description }}
        </el-descriptions-item>
        <el-descriptions-item label="progress">
          {{ item.currentPoint }}/{{ item.maximunPoint }}
        </el-descriptions-item>
      </el-descriptions>
      <div class="sidequests">
        <el-checkbox
          size="large"
          v-for="sidequest in item.sidequests"
          :label="sidequest.name"
          v-model="sidequest.status"
          @change="handleChangeSidequest(sidequest)"
        />
      </div>
    </div>
  </div>
  <el-dialog v-model="visibleDialogProgressUpdate" class="">
    <UpdateProgress
      :keyresults="tempKeyResults"
      @onSaveUpdateProgress="
        () => {
          emit('onSearchObjective');
          visibleDialogProgressUpdate = false;
        }
      "
      @onUpdateProgress="changePoint"
    ></UpdateProgress>
  </el-dialog>
</template>
<script setup lang="ts">
import { Objective } from "@/Models/Objective";
import { formatDate } from "../../Service/formatDate";
import { Edit as EditIcon } from "@element-plus/icons-vue";
import { KeyResult } from "@/Models/KeyResult";
import { ref } from "vue";
import UpdateProgress from "@/components/ProgressUpdate/UpdateProgress.vue";
import { deepCopy } from "../../Service/deepCopy";
import { caculateKeyResult , caculateObjective} from "../../Service/OKR/caculateKeyResult";
import { Sidequest } from "@/Models/Sidequests";
import { axiosInstance } from "../../Service/axiosConfig";
import { ElMessage } from "element-plus";

const props = defineProps<{
  objective: Objective;
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
  // objectId: "",
  sidequests: [],
  note: "",
});
const visibleDialogProgressUpdate = ref(false);
const handleProgressUpdate = (keyresults: KeyResult) => {
  tempKeyResults.value = deepCopy(keyresults);
  console.log(tempKeyResults.value);
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
const changePoint = (point : number, keyresultId : string) =>{
  props.objective.listKeyResults.filter(x=>x.id == keyresultId)[0].currentPoint = point
}
</script>
<style scoped>
.custom-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  border-bottom: 1px solid #ebeef5;
}

.header-content {
  display: flex;
  align-items: center;
}

.header-extra {
  display: flex;
  align-items: center;
}
.key-result {
  border: 1px solid black;
}
</style>
