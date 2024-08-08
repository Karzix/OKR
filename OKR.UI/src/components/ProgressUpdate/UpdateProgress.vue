<template>
  <div class="keyresult-container">
    <div class="input-group">
      <el-input-number
        v-model="props.keyresults.currentPoint"
        class="current-point"
        :min="0"
        :max="props.keyresults.maximunPoint"
        controls-position="right"
      />
      <p>/</p>
      <el-input
        v-model="props.keyresults.maximunPoint"
        class="max-point"
        :disabled="true"
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
import { ElMessage } from 'element-plus'

const props = defineProps<{
  keyresults: KeyResult;
}>();
const emit = defineEmits<{
  (e: "close"): void;
  (e: "onSaveUpdateProgress"): void;
  (e: "onUpdateProgress", point : number, keyresultId : string): void;
}>();
const Save = () =>{
    axiosInstance.put("KeyResults", props.keyresults).then((res) => {
        if(!res.data.isSuccess){
            ElMessage.error(res.data.message)
        }
        else{
            emit("onSaveUpdateProgress")
            emit("onUpdateProgress", props.keyresults.currentPoint ?? 0, props.keyresults.id ?? "")
        }
    })
}

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