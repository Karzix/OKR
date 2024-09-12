<template>
  <div class="objective_form">
    <div class="objective_el-radio-group">
      <el-radio-group v-model="objective.targetType" size="large">
        <el-radio-button
          v-for="item in targetTypeValues"
          :label="TargetType[item]"
          :value="item"
        />
      </el-radio-group>
    </div>

    <div class="objective_form-item">
      <p class="objective_form-label">Objective Name</p>
      <div class="objective_input-container">
        <el-input v-model="objective.name" />
      </div>
    </div>

    <div class="objective_form-item">
      <p class="objective_form-label">Start Day</p>
      <div class="objective_input-container">
        <el-date-picker format="DD/MM/YYYY" v-model="objective.startDay" type="date" />
      </div>
    </div>

    <div class="objective_form-item">
      <p class="objective_form-label">End Day</p>
      <div class="objective_input-container">
        <el-date-picker format="DD/MM/YYYY" v-model="objective.deadline" type="date" />
      </div>
    </div>

    <createKeyResultDialog
      @on-add-item="handleAddKeyResult"
      :is-edit="isEditKeyresults"
      :keyresults="editKeyresultItme"
      :dialog-visible="createKeyResultDialogVisible"
      v-if="createKeyResultDialogVisible"
      @on-turn-off-dialog="handleClose"
      @on-edit-item="handleEditKeyResult"
    />
    <el-button plain @click="createKeyResultDialogVisible = true">
      Add key result
    </el-button>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import createKeyResultDialog from "@/components/okr/createKeyResultDialog.vue";
import { KeyResult } from "@/Models/KeyResult";
import { Objective } from "@/Models/Objective";
import { axiosInstance } from "../../Service/axiosConfig";
// import { TargetType } from "@/Models/TargetType";
import { CloseBold } from "@element-plus/icons-vue";
import { formatDate } from "../../Service/formatDate";
import { deepCopy } from "../../Service/deepCopy";
import { TargetType } from "@/Models/Enum/TargetType";

const objective = ref<Objective>({
  id: undefined,
  name: "",
  startDay: undefined,
  deadline: undefined,
  listKeyResults: [],
  targetType: 0,
  targetTypeName: "",
  point: 0,
});
const props = defineProps<{
  objective: Objective;
  isEdit: boolean;
}>();
const emit = defineEmits<{
  (e: "onSearchObjective"): void;
  (e: "onClose"): void;
}>();
// const TargetTypes = ref([
//   {
//     id: 0,
//     name: "Individual",
//   },
//   {
//     id: 1,
//     name: "Branch",
//   },
//   {
//     id: 2,
//     name: "Team",
//   },
// ]);
const targetTypeValues = Object.keys(TargetType)
    .map(key => Number(key))
    .filter(value => !isNaN(value));

const editKeyresultItme = ref<KeyResult>({
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
const isEditKeyresults = ref<boolean>(false);
const createKeyResultDialogVisible = ref<boolean>(false);
const handleAddKeyResult = (item: KeyResult) => {
  objective.value.listKeyResults.push(item);
};
const handleEditKeyResult = (item: KeyResult) => {
  // const index = objective.value.listKeyResults.findIndex(
  //   (x) => x.id === item.id
  // );
  // if (index !== -1) {
  //   objective.value.listKeyResults.splice(index, 1, item);
  // }
};
// const GetGeneralData = () => {
//   axiosInstance.get("TargetType").then((res) => {
//     TargetTypes.value = res.data.data;
//   });
// };
onMounted(() => {
  // GetGeneralData();
  if (props.isEdit) {
    objective.value = props.objective;
  }
});
watch(
  () => props.isEdit,
  () => {
    if (props.isEdit) {
      objective.value = props.objective;
    } else {
      objective.value = {
        id: undefined,
        name: "",
        startDay: undefined,
        deadline: undefined,
        listKeyResults: [],
        targetType: undefined,
        targetTypeName: "",
        point: 0,
      };
    }
  }
);
const Save = () => {
  if (props.isEdit) {
    axiosInstance
      .put("Objectives", objective.value)
      .then((res) => {
        console.log(res);
        if (res.data.isSuccess) {
          emit("onSearchObjective");
          emit("onClose");
        }
      })
      .catch((error) => {
        console.log(error);
      });
  } else {
    axiosInstance
      .post("Objectives", objective.value)
      .then((res) => {
        if (res.data.isSuccess) {
          emit("onSearchObjective");
        }
      })
      .catch((error) => {
        console.log(error);
      });
  }
};
function deleteKeyResult(index: number) {
  objective.value.listKeyResults.splice(index, 1);
}
function editKeyresult(index: number) {
  editKeyresultItme.value = objective.value.listKeyResults[index]
  isEditKeyresults.value = true;
  createKeyResultDialogVisible.value = true;
}
function addKeyresult() {
  isEditKeyresults.value = false;
  editKeyresultItme.value = new KeyResult();
  createKeyResultDialogVisible.value = true;
}
function handleClose() {
  createKeyResultDialogVisible.value = false;
}
</script>
<style scoped>
.objective_form {
  display: flex;
  flex-direction: column;
  width: 400px;
  margin: auto;
  padding: 10px;
  border-radius: 10px;
}

.objective_el-radio-group {
  display: flex;
  justify-content: center;
  margin-bottom: 15px;
}

.objective_form-item {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
}

.objective_form-label {
  font-weight: bold;
  width: 200px;
  font-size: 16px;
}

.objective_input-container {
  width: 60%;
}

.el-input__inner,
.el-date-editor.el-input__inner {
  padding: 8px;
  font-size: 14px;
  border-radius: 6px;
}

.objective_buttons {
  display: flex;
  justify-content: center;
  gap: 10px;
  margin-top: 10px;
}

.objective_buttons .el-button {
  margin: 0 5px;
}

.el-button.primary {
  background-color: #67c23a;
  border-color: #67c23a;
  color: white;
}

.el-button.primary:hover {
  background-color: #85e3a1;
  border-color: #85e3a1;
}

.el-button.plain {
  border: 1px solid #409eff;
  color: #409eff;
}

.el-button.plain:hover {
  background-color: #ecf5ff;
}
</style>
