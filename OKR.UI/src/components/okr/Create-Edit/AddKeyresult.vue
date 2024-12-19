<template>
  <div class="keyresult-form">
    <!-- <div class="header">New Key Result</div> -->
     <div class="header">
      <div class="header">{{ props.isEdit ? 'Edit key result' : 'New key result' }}</div>
      <el-button type="danger" :icon="Delete" circle v-if="props.isEdit" @click="onDeleteKeyResult" />
     </div>
     
      <div class="form-item">
        <p class="form-label">Title: </p>
        <el-input
          v-model="keyresults.description"
          placeholder="Enter your title, e.g 'increase sales by 10% in Q4'"
        />
      </div>
      <div class="form-item">
        <p>Status:</p>
        <div class="radio-status">
          <el-check-tag v-for="status in statusOptions" :key="status.value" 
            :checked="keyresults.status === status.value" 
            :type="getTagType(status.value)" 
            @click="selectStatus(status.value)">
            {{ status.text }}
          </el-check-tag>
        </div>
      </div>
      <div class="form-item-2">
        <div class="form-item min-w-200px" >
            <p class="form-label">Progress type:</p> 
            <el-select
                v-model="keyresults.unit"
                placeholder="Select"
                class="form-input"
                 @change="handleUnitChange"
            >
                <el-option
                v-for="item in [
                    { key: 1, name: '# Value' },
                    { key: 0, name: '% Percent' },
                    { key: 2, name: ' Completed/Not Completed' },
                ]"
                :key="item.key"
                :label="item.name"
                :value="item.key"
                />
            </el-select>
        </div>
        <div class="form-item max-w-150px">
            <p class="form-label">Initial:</p>
            <el-input-number v-model="keyresults.currentPoint" class="form-input" :controls="false" :min="0" :disabled="keyresults.unit == 2"/>
        </div>
        <div class="form-item max-w-150px" >
            <p class="form-label">Target:</p>
            <el-input-number v-model="keyresults.maximunPoint" class="form-input" :controls="false" :max="keyresults.unit == 0 ?100 : Number.MAX_VALUE"  :disabled="keyresults.unit == 2"/>
        </div>
      </div>
    <!-- </div> -->

  </div>
  <div class="footer">
    <el-button type="primary" size="large"  @click="AddSave" style="width: 120px;">Save</el-button>
  </div>
</template>
<script setup lang="ts">
import { onBeforeMount, onMounted, ref, watch } from "vue";
import { Objectives } from "@/Models/Objective";
import { TargetType } from "@/Models/Enum/TargetType";
import {
  Calendar,
  Flag,
  OfficeBuilding,
  User,
  View,
  Hide,
  Delete
} from "@element-plus/icons-vue";
import type { KeyResult } from "@/Models/KeyResult";
import { getStatusText, StatusObjectives,getTagType, getStatusColor } from "@/Models/EntityObjectives";
import { deepCopy } from "@/Service/deepCopy";
import { ElMessage, ElMessageBox } from "element-plus";


const keyresults = ref<KeyResult>({
  id: undefined,
  description: "",
  currentPoint: 0,
  status: StatusObjectives.noStatus,
  maximunPoint: 100,
  endDay: new Date(),
  unit: 1,
  active: true,
  note: "",
  createdOn: new Date(),
  lastProgressUpdate: new Date(),
  progressUpdates:[],
  isCompleted: false
});
const props = defineProps<{ 
    keyresults: KeyResult,
    isEdit?: boolean,
    index : number
}>();
const currentUnit = ref(0);
const emit = defineEmits<{
  (e: "onAddItem", item: KeyResult): void;
  (e: "onEditItem", item: KeyResult, index: number): void;
  (e: "onDeleteItem", index: number): void;
}>();
const statusOptions = Object.values(StatusObjectives)
  .filter(value => typeof value === 'number')
  .map(value => ({
    value: value as StatusObjectives,
    text: getStatusText(value as StatusObjectives),
  }));

onBeforeMount(() => {
  if(!props.isEdit){
    keyresults.value = {
      id: undefined,
      description: "",
      currentPoint: 0,
      status: StatusObjectives.noStatus,
      maximunPoint: 100,
      endDay: new Date(),
      unit: 0,
      active: true,
      note: "",
      createdOn: new Date(),
      lastProgressUpdate: new Date(),
      progressUpdates:[],
      isCompleted: false
    }
  }
  else{
    keyresults.value = props.keyresults
  }
});
const AddSave = () => {
  if(props.isEdit){
    emit("onEditItem", deepCopy(keyresults.value), props.index ?? 0);
  }
  else{
    emit("onAddItem", deepCopy(keyresults.value));
  }
  
}

watch(() => keyresults.value.unit, () => {
//   console.log(keyresults.value);
  if(keyresults.value.unit == 0 && !props.isEdit){
    keyresults.value.maximunPoint = 100;
    keyresults.value.currentPoint = 0;
  }
})
const handleUnitChange = () => {
    keyresults.value.maximunPoint = 100;
    keyresults.value.currentPoint = 0;
}
const customFillStatus = ref(getStatusColor(keyresults.value.status));
watch(
  () => keyresults.value.status,
  (newValue: StatusObjectives) => {
    customFillStatus.value = getStatusColor(keyresults.value.status);
  }
)
function getRadioStyle(status: StatusObjectives) {
  return {
    borderColor: keyresults.value.status === status ? getStatusColor(status) : undefined
  };
}
const onDeleteKeyResult = () => {
  ElMessageBox.confirm(
    'Are you sure delete this keyresult?',
    'Warning',
    {
      confirmButtonText: 'OK',
      cancelButtonText: 'Cancel',
      type: 'warning',
    }
  )
  .then(() => {
    emit("onDeleteItem", props.index ?? 0);
  })
  .catch(() => {

  });
  
}
onMounted(() => {
  if(props.isEdit){
    currentUnit.value = keyresults.value.unit ?? 0
  }
})
watch(() => keyresults.value.unit, () => {
  if(props.isEdit && keyresults.value.unit != currentUnit.value){
    keyresults.value.unit = currentUnit.value
    ElMessage({
      type: 'warning',
      message: 'If you want to change the unit, delete and create a new keyresults',
    })
  }
})
const selectStatus = (index: StatusObjectives) => {
  keyresults.value.status = index
}
</script>
<style scope>
.header {
    text-align: center;
    font-weight: bold; 
    font-size: 22px;
}
.form-item{
  font-weight: bold;
  display: flex;
  /* gap: 5px; */
  flex-direction: column;
  gap: 4px;
}
.form-item-2{
    display: flex;
    gap: 10px;
    justify-content: space-between;
}
.keyresult-form{
    padding: 0 40px;
    display: flex;
    flex-direction: column;
    gap: 20px;
}
.btn-Save{
    text-align: center;
    margin-top: 30px ;
}
.radio-status-addkeyresult{
  width: 100%;
  justify-content: space-between;
}
.header{
  display: flex;
    gap: 20px;
    align-content: center;
    align-items: center;
}
.footer{
  display: flex;
  flex-direction: row-reverse;
  border-top: 1px solid #cccc;
  padding: 10px;
  margin-top: 10px;
}
.radio-status{
  display: flex;
  justify-content: space-between;
}
</style>
<style>
.max-w-150px{
    max-width: 150px !important;
}
.min-w-200px{
    min-width: 200px !important;
}
</style>