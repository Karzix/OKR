<template>
  <div class="keyresult-container">
    <div class="input-group">
      <el-input
        v-model="caculateCrrentPoint"
        class="current-point"
        controls-position="right"
        :disabled="true"
      />
      <p>/</p>
      <el-input
        v-model="props.keyresults.maximunPoint"
        class="max-point"
        :disabled="true"
      />
    </div>
    <div>
      <el-input-number
        v-model="props.keyresults.addedPoints"
        class="current-point"
        :min="(-1 * (props.keyresults.currentPoint ?? 0))"
        :max="props.keyresults.maximunPoint"
        controls-position="right"
      />
    </div>
    <div class="note-container">
      <el-input
        v-model="props.keyresults.note"
        :rows="2"
        type="textarea"
        class="note-input"
      />
    </div>
    <el-button type="primary" @click="Save" class="save-button">Save</el-button>
  </div>
</template>
<script setup lang="ts">
import { KeyResult } from "@/Models/KeyResult";
import { axiosInstance } from "../../Service/axiosConfig";
import { ElMessage, ElLoading } from "element-plus";
import { ref, watch } from "vue";

const props = defineProps<{
  keyresults: KeyResult;
}>();
const emit = defineEmits<{
  (e: "close"): void;
  (e: "onSaveUpdateProgress"): void;
  (e: "onUpdateProgress", point: number, keyresultId: string): void;
}>();
const caculateCrrentPoint = ref(0);
const Save = async () => {
  const loading = ElLoading.service({
    lock: true,
    text: "The request is being processed",
    background: "rgba(0, 0, 0, 0.7)",
  });
  await axiosInstance.put("KeyResults", props.keyresults).then((res) => {
    if (!res.data.isSuccess) {
      ElMessage.error(res.data.message);
    } else {
      emit("onSaveUpdateProgress");
      var crpoint = props.keyresults.currentPoint ?? 0;
      var addpoint = props.keyresults.addedPoints ?? 0;
      emit(
        "onUpdateProgress",
        (crpoint + addpoint) ?? 0,
        props.keyresults.id ?? ""
      );
    }
  });
  loading.close();
};
watch(() => props.keyresults.addedPoints , () => {
  var cur = props.keyresults.currentPoint ?? 0 ;
  var add = props.keyresults.addedPoints ?? 0;
  caculateCrrentPoint.value = cur + add;
},{immediate: true})
</script>
<style scoped>
.keyresult-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 20px;
  background-color: #f9f9f9;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.input-group {
  display: flex;
  align-items: center;
  margin-bottom: 20px;
}

.input-group p {
  margin: 0 10px;
  font-size: 18px;
}

.current-point,
.max-point {
  width: 100px;
}

.note-container {
  width: 100%;
  margin-bottom: 20px;
}

.note-input {
  width: 100%;
}

.save-button {
  width: 100%;
}
</style>
