<template>
  <div class="keyresult-form">
    <!-- <div class="header">New Key Result</div> -->
    <!-- <div class="left"> -->
      <div class="form-item">
        <p class="form-label">Title: </p>
        <el-input
          v-model="keyresults.description"
          placeholder="Enter your title, e.g 'increase sales by 10% in Q4'"
        />
      </div>
      <div class="form-item">
        <p>Status:</p>
        <el-radio-group v-model="keyresults.status" size="medium">
            <el-radio
            v-for="status in statusOptions"
            :key="status.value"
            :label="status.value"
            :border="true"
            >
            <el-tag :type="getTagType(status.value)" effect="dark">
                {{ status.text }}
            </el-tag>
            </el-radio>
        </el-radio-group>
      </div>
      <div class="form-item-2">
        <div class="form-item min-w-150px" >
            <p class="form-label">Progress type:</p> 
            <el-select
                v-model="keyresults.unit"
                placeholder="Select"
                class="form-input"
            >
                <el-option
                v-for="item in [
                    { key: 1, name: '# Value' },
                    { key: 0, name: '% Percent' },
                ]"
                :key="item.key"
                :label="item.name"
                :value="item.key"
                />
            </el-select>
        </div>
        <div class="form-item max-w-150px">
            <p class="form-label">Initial:</p>
            <el-input-number v-model="keyresults.currentPoint" class="form-input" :controls="false" :min="0"/>
        </div>
        <div class="form-item max-w-150px" >
            <p class="form-label">Target:</p>
            <el-input-number v-model="keyresults.maximunPoint" class="form-input" :controls="false" :max="keyresults.unit == 0 ?100 : Number.MAX_VALUE"/>
        </div>
      </div>
    <!-- </div> -->

  </div>
  <div class="btn-Save">
    <el-button type="primary" @click="AddKeyresult">Save</el-button>
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import { Objectives } from "@/Models/Objective";
import { TargetType } from "@/Models/Enum/TargetType";
import {
  Calendar,
  Flag,
  OfficeBuilding,
  User,
  View,
  Hide,
} from "@element-plus/icons-vue";
import type { KeyResult } from "@/Models/KeyResult";
import { getStatusText, StatusObjectives,getTagType } from "@/Models/EntityObjectives";
import { deepCopy } from "@/Service/deepCopy";

const keyresults = ref<KeyResult>({
  id: undefined,
  description: "",
  currentPoint: 0,
  status: StatusObjectives.noStatus,
  maximunPoint: 100,
  deadline: new Date(),
  unit: 0,
  active: true,
  note: "",
});
const props = defineProps<{ 
    keyresults: KeyResult,
    isEdit?: boolean
}>();

const emit = defineEmits<{
  (e: "onAddItem", item: KeyResult): void;
}>();
const statusOptions = Object.values(StatusObjectives)
  .filter(value => typeof value === 'number')
  .map(value => ({
    value: value as StatusObjectives,
    text: getStatusText(value as StatusObjectives),
  }));

onMounted(() => {
  if(!props.isEdit){
    keyresults.value = {
      id: undefined,
      description: "",
      currentPoint: 0,
      status: StatusObjectives.noStatus,
      maximunPoint: 100,
      deadline: new Date(),
      unit: 0,
      active: true,
      note: "",
    }
  }
  else{
    keyresults.value = props.keyresults
  }
});
const AddKeyresult = () => {
  emit("onAddItem", deepCopy(keyresults.value));
}

watch(() => keyresults.value.unit, () => {
//   console.log(keyresults.value);
    keyresults.value.maximunPoint = 100;
    keyresults.value.currentPoint = 0;
}, { immediate: true })
</script>
<style scope>
.form-item{
    font-weight: bold; 
}
.form-item-2{
    display: flex;
    gap: 10px;
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
</style>
<style>
.max-w-150px{
    max-width: 150px !important;
}
.min-w-150px{
    min-width: 150px !important;
}
</style>