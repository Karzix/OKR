<template>
  <div class="header">{{ props.isEdit ? 'Edit objective' : 'New objective' }}</div>
  <div class="objective-form">
    <div class="left">
      <div class="form-item">
        <p class="form-label">Title:</p>
        <el-input
          v-model="objectives.name"
          placeholder="Enter your title, e.g 'increase sales by 10% in Q4'"
        />
      </div>
      <div class="form-item">
        <p class="form-label">Target type:</p>
        <el-radio-group
          v-model="objectives.targetType"
          size="large"
        >
          <el-radio-button
            :value="TargetType.Individual"
            class="individual-button targetType-button"
          >
            <el-icon><User /></el-icon> Individual
          </el-radio-button>
          <el-radio-button
            :value="TargetType.Department"
            class="department-button targetType-button"
            :disabled="!(handleRole.IdentifyRoles([ 'Teamleader']))" 
          >
            <el-icon><Flag /></el-icon> Team
          </el-radio-button>
          <el-radio-button :value="TargetType.Company" class="company-button targetType-button"
          :disabled="!(handleRole.IdentifyRoles([ 'Admin']))" 
          >
            <el-icon><OfficeBuilding /></el-icon> Company
          </el-radio-button>
        </el-radio-group>
      </div>
      <div class="form-item">
        <p>Status:</p>
        <!-- <el-radio-group v-model="objectives.status" size="medium" :fill="customFillStatus">
            <el-radio
            v-for="status in statusOptions"
            :key="status.value"
            :label="status.value"
            :border="true"
            :style="getRadioStyle(status.value)"
            >
            <el-tag :type="getTagType(status.value)" effect="dark">
                {{ status.text }}
            </el-tag>
            </el-radio>
        </el-radio-group> -->
        <div class="radio-status">
          <el-check-tag v-for="status in statusOptions" :key="status.value" 
            :checked="objectives.status === status.value" 
            :type="getTagType(status.value)" 
            @click="selectStatus(status.value)">
            {{ status.text }}
          </el-check-tag>
        </div>
      </div>
      <!-- <Radio.Group block options={options} defaultValue="Pear" optionType="button" /> -->
      <div class="btn-add-keyresult form-item">
       <p>Keyresults</p>
        <el-button :icon="Plus" @click="onShowDialogCreateKeyResult">Add new</el-button> 
      </div>
      <div class="keyresults">
        <ListKeyresult :keyresults="objectives.keyResults" :is-create-or-edit="true" @on-select-key-result="onSelectKeyResult"></ListKeyresult>
      </div>
    </div>
    <div class="right">
      <div class="form-item">
        <p class="form-label">
          <el-icon><Calendar /></el-icon> Start/End date
        </p>
        <div class="input-group">
          <el-select v-model="objectives.period" style="min-width: 150px;">
            <el-option
              v-for="item in period"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
          <el-input-number
            v-if="objectives.period != 'custom'"
            v-model="objectives.year"
            :controls="false"
            placeholder="Enter year"
            :min="new Date().getFullYear()"
            controls-position="right"
          />
        </div>
        <div v-if="objectives.period == 'custom'" class="input-group">
          <el-date-picker 
            v-model="objectives.startDay"
            type="date"
            placeholder="Start date"
          />
          <el-date-picker
            v-model="objectives.endDay"
            type="date"
            placeholder="End date"
          />
        </div>
      </div>
      <div class="form-item">
        <p class="form-label">
          <el-icon><View /></el-icon>Visibility
        </p>
        <el-radio-group
          v-model="objectives.isPublic"
          size="large"
        >
          <el-radio-button :value="false" class="individual-button Visibility-button" :disabled="objectives.targetType == TargetType.Department || objectives.targetType == TargetType.Company  ">
            <el-icon><Hide /></el-icon> Private
          </el-radio-button>
          <el-radio-button :value="true" class="department-button Visibility-button" :style="{ width: '150px' }">
            <el-icon><View /></el-icon> Public
          </el-radio-button>
        </el-radio-group>
      </div>
      
    </div>
  </div>
  <div class="footer">
    <el-button type="primary" size="large"  @click="onSave" style="width: 120px;">Save</el-button>
  </div>
  <el-dialog
    v-model="dialogAddKeyResult"
    @close="dialogAddKeyResult = false"
    class="createDialog"
  >
    <AddKeyresult
      v-if="dialogAddKeyResult"
      :keyresults="keyresults"
      :isEdit="isEdit"
      @onAddItem="onAddKeyresult"
      @onEditItem="onEditKeyresult"
      @on-delete-item="onDeleteKeyResult"
      :index="indexSelected"
    />
  </el-dialog>
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
  Plus,
} from "@element-plus/icons-vue";
//@ts-ignore
import AddKeyresult from "./AddKeyresult.vue";
import type { KeyResult } from "@/Models/KeyResult";
import { getStatusText, StatusObjectives,getTagType, getStatusColor } from "@/Models/EntityObjectives";
import ListKeyresult from "./ListKeyresult.vue";
import { ElLoading, ElMessage } from "element-plus";
import { axiosInstance } from "@/Service/axiosConfig";
import { deepCopy } from "@/Service/deepCopy";
import * as handleRole from "@/components/maynghien/Common/handleRole";

const objectives = ref<Objectives>({
  id: undefined,
  name: "",
  startDay: undefined,
  endDay: undefined,
  keyResults: [],
  targetType: TargetType.Individual,
  targetTypeName: "",
  point: 0,
  status: StatusObjectives.noStatus,
  isPublic: true,
  isUserObjectives: true,
  year: new Date().getFullYear(),
  period: "Q1",
  lastProgressUpdate: new Date(),
  createdOn: new Date(),
  numberOfPendingUpdates: 0
});
const props = defineProps<{
  objectives: Objectives;
  isEdit?: boolean;
}>();
const emit = defineEmits<{
    (e: "updateData"): void;
    (e: "onClose"): void;
}>();
const statusOptions = Object.values(StatusObjectives)
  .filter(value => typeof value === 'number')
  .map(value => ({
    value: value as StatusObjectives,
    text: getStatusText(value as StatusObjectives),
  }));
const keyresults = ref<KeyResult>({
  id: undefined,
  description: "",
  currentPoint: 0,
  status: StatusObjectives.noStatus,
  maximunPoint: 100,
  endDay: new Date(),
  unit: 0,
  active: true,
  note: "",
  lastProgressUpdate: new Date(),
  createdOn: new Date(),
  progressUpdates: [],
  isCompleted: false
});
const dialogAddKeyResult = ref(false);
const isEdit = ref(false);
const period = [
  {
    value: "Q1",
    label: "Q1 (January - March)",
  },
  {
    value: "Q2",
    label: "Q2 (April - June)",
  },
  {
    value: "Q3",
    label: "Q3 (July - September)",
  },
  {
    value: "Q4",
    label: "Q4 (October - December)",
  },
  {
    value: "H1",
    label: "H1 (January - June)",
  },
  {
    value: "H2",
    label: "H2 (July - December)",
  },
  {
    value: "FY",
    label: "FY (January - December)",
  },
  {
    value: "custom",
    label: "Custom datetime",
  },
];
const indexSelected = ref(0);
const currentFill = ref("rgb(235, 180.6, 99)");
watch(
  () => objectives.value.targetType,
  (newValue: TargetType) => {
    switch (newValue) {
      case TargetType.Individual:
        currentFill.value = "rgb(235, 180.6, 99)";
        break;
      case TargetType.Department:
        currentFill.value = "rgb(247, 137.4, 137.4)";
        break;
      case TargetType.Company:
        currentFill.value = "rgb(33.2, 61.4, 90.5)";
        break;
      default:
    }
  }
);
const customFillStatus = ref(getStatusColor(objectives.value.status));
watch(
  () => objectives.value.status,
  (newValue: StatusObjectives) => {
    customFillStatus.value = getStatusColor(newValue);
  }
)

const onAddKeyresult = (keyresult : KeyResult) => {

    objectives.value.keyResults.push(keyresult);
    caculateWeightKeyresult();
    dialogAddKeyResult.value = false;
}
const onEditKeyresult = (keyresult : KeyResult , index : number = 0) => {
  if(keyresult.id != undefined && keyresult.id != null){
    const index2 = objectives.value.keyResults.findIndex(kr => kr.id == keyresult.id);
    objectives.value.keyResults[index2] = keyresult;
    caculateWeightKeyresult();
  }
  else{
    objectives.value.keyResults[index] = keyresult;
    caculateWeightKeyresult();
  }
    dialogAddKeyResult.value = false;
}
const caculateWeightKeyresult = () => {
  const keyResults = objectives.value.keyResults;
  const count = keyResults.length;

  if (count === 0) return;

  // Phân bổ trọng số cơ bản cho từng key result
  const baseWeight = Math.floor(100 / count);
  const remainder = 100 % count;

  keyResults.forEach((kr, index) => {
    kr.percentage = baseWeight + (index < remainder ? 1 : 0);
  });
};

const onShowDialogCreateKeyResult = () => {
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
        lastProgressUpdate: new Date(),
        progressUpdates: [],
        createdOn: new Date(),
        isCompleted: false
    };
    isEdit.value = false;
    dialogAddKeyResult.value = true;
}

const onSave = async () => {
    if (!validateForm()) return;

    const loadingInstance = ElLoading.service({
        lock: true,
        text: "Loading",
        background: "rgba(0, 0, 0, 0.7)"
    });

    try {
      if(props.isEdit){
        const res = await axiosInstance.put("Objectives", objectives.value);
        if (res.data.isSuccess) {
            ElMessage({
                message: "Update success",
                type: "success",
            });
            emit("updateData");
            emit("onClose");
            dialogAddKeyResult.value = false;
        } else {
            ElMessage({
                message: res.data.message,
                type: "error",
            });
        }
      }
      else{
        const res = await axiosInstance.post("Objectives", objectives.value);
        if (res.data.isSuccess) {
            ElMessage({
                message: "Create success",
                type: "success",
            });
            objectives.value = {
                id: undefined,
                name: "",
                startDay: undefined,
                endDay: undefined,
                keyResults: [],
                targetType: TargetType.Individual,
                targetTypeName: "",
                point: 0,
                status: 0,
                isPublic: true,
                isUserObjectives: true,
                year: new Date().getFullYear(),
                period: "Q1",
                lastProgressUpdate: new Date(),
                createdOn: new Date(),
                numberOfPendingUpdates: 0
            };
            dialogAddKeyResult.value = false;
        } else {
            ElMessage({
                message: res.data.message,
                type: "error",
            });
        }
      }
     
    } catch (error) {
        ElMessage({
            message: "An error occurred. Please try again.",
            type: "error",
        });
    } finally {
        loadingInstance.close();
        emit("updateData");
    }
};
watch(()=>objectives.value.targetType, () => {
    if(!(objectives.value.targetType == TargetType.Individual)){
        objectives.value.isPublic = true;
    }
})
const validateForm = () => {
    if(objectives.value.name == ""){
        ElMessage({
            message: "Title can not be empty",
            type: "error",
        })
        return false;
    }
    if(objectives.value.keyResults.length == 0){
        ElMessage({
            message: "requires at least 1 keyresult",
            type: "error",
        })
        return false;
    }
    return true;
}
onMounted(() => {
  if(props.isEdit){
    objectives.value = props.objectives;
  }
  else{
    objectives.value = {
        id: undefined,
        name: "",
        startDay: undefined,
        endDay: undefined,
        keyResults: [],
        targetType: TargetType.Individual,
        targetTypeName: "",
        point: 0,
        status: 0,
        isPublic: true,
        isUserObjectives: true,
        year: new Date().getFullYear(),
        period: "Q1",
        lastProgressUpdate: new Date(),
        createdOn: new Date(),
        numberOfPendingUpdates: 0
    }
  }
})
const onSelectKeyResult = (item: KeyResult, index: number) => {
  keyresults.value = deepCopy(item);
  isEdit.value = true;
  dialogAddKeyResult.value = true;
  indexSelected.value = index;
}
function getRadioStyle(status: StatusObjectives) {
  return {
    borderColor: objectives.value.status === status ? getStatusColor(status) : undefined
  };
}
const onDeleteKeyResult = (index : number) =>{
  objectives.value.keyResults.splice(index, 1);
  dialogAddKeyResult.value = false
  ElMessage({
    message: "Delete success",
    type: "success",
  })
}
const selectStatus = (index: StatusObjectives) => {
  objectives.value.status = index
}
</script>
<style scoped>
.header {
    text-align: center;
    font-weight: bold; 
    font-size: 1.5em;
}
.objective-form{
    display: flex;
    margin-top: 20px;
    gap: 20px;
}
.form-item{
  font-weight: bold;
  display: flex;
  /* gap: 5px; */
  flex-direction: column;
  gap: 4px;
}
.left {
    display: flex;
    flex-direction: column;
    gap: 10px;
    width: 60%;
}
.right{
    display: flex;
    flex-direction: column;
    gap: 10px;
    width: 40%;
    max-width: 327px;
    position: relative;
}
.input-group{
    display: flex;
}
.btn-add-keyresult{
    display: flex;
    justify-content: space-between;
}


.el-radio{
  margin-right: 0 !important;
}
.footer{
  display: flex;
  flex-direction: row-reverse;
  border-top: 1px solid #cccc;
  padding: 10px;
}
.radio-status{
  display: flex;
  justify-content: space-between;
}
</style>
<style>
.Visibility-button > span {
    width: 165px;
}
.targetType-button > span {
    width: 163px;
}
</style>