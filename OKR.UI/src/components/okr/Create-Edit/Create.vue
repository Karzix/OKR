<template>
  <div class="header">New Objectives</div>
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
          :fill="currentFill"
        >
          <el-radio-button
            :value="TargetType.Individual"
            class="individual-button"
          >
            <el-icon><User /></el-icon> Individual
          </el-radio-button>
          <el-radio-button
            :value="TargetType.Department"
            class="department-button"
          >
            <el-icon><Flag /></el-icon> Department
          </el-radio-button>
          <el-radio-button :value="TargetType.Company" class="company-button">
            <el-icon><OfficeBuilding /></el-icon> Company
          </el-radio-button>
        </el-radio-group>
      </div>
      <div class="btn-add-keyresult form-item">
       <p>Keyresults</p>
        <el-button :icon="Plus" @click="onShowDialogCreateKeyResult">Add new</el-button> 
      </div>
      <div class="keyresults">
        <ListKeyresult :keyresults="objectives.keyResults" :is-create-or-edit="true"></ListKeyresult>
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
            v-model="objectives.year"
            :controls="false"
            placeholder="Enter year"
            :min="new Date().getFullYear()"
            controls-position="right"
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
          <el-radio-button :value="false" class="individual-button">
            <el-icon><Hide /></el-icon> Private
          </el-radio-button>
          <el-radio-button :value="true" class="department-button">
            <el-icon><View /></el-icon> Public
          </el-radio-button>
        </el-radio-group>
      </div>
      <div class="btn-Save">
        <el-button type="primary" size="large" round @click="onSave">Save</el-button>
      </div>
    </div>
  </div>
  <el-dialog
    v-model="dialogAddKeyResult"
    @close="dialogAddKeyResult = false"
    :title="isEdit ? 'Edit key result' : 'new key result'"
    class="createDialog"
  >
    <AddKeyresult
      :keyresults="keyresults"
      :isEdit="isEdit"
      @onAddItem="onAddKeyresult"
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
import { StatusObjectives } from "@/Models/EntityObjectives";
import ListKeyresult from "./ListKeyresult.vue";
import { ElLoading, ElMessage } from "element-plus";
import { axiosInstance } from "@/Service/axiosConfig";

const objectives = ref<Objectives>({
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
});
const props = defineProps<{
  objectives: Objectives;
  isEdit?: boolean;
}>();

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
  lastProgressUpdate: new Date(),
  createdOn: new Date(),
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
];
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

const onAddKeyresult = (keyresult : KeyResult) => {

    objectives.value.keyResults.push(keyresult);
    caculateWeightKeyresult();
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
        deadline: new Date(),
        unit: 0,
        active: true,
        note: "",
        lastProgressUpdate: new Date(),
  createdOn: new Date(),
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
            };
            dialogAddKeyResult.value = false;
        } else {
            ElMessage({
                message: res.data.message,
                type: "error",
            });
        }
    } catch (error) {
        ElMessage({
            message: "An error occurred. Please try again.",
            type: "error",
        });
    } finally {
        loadingInstance.close();
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
}
onMounted(() => {
  if(isEdit.value){
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
    }
  }
})
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
}
.input-group{
    display: flex;
}
.btn-add-keyresult{
    display: flex;
    justify-content: space-between;
}
</style>
