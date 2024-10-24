<template>
  <div class="objective-form">
    <div class="radio-group">
      <el-radio-group v-model="objective.targetType" size="large">
        <el-radio-button :label="TargetType[0]" :value="0" />
        <el-radio-button :label="TargetType[1]" :value="1" :disabled="!(handleRole.IdentifyRoles(['Admin', 'superadmin', 'BranchManagement']))" />
        <el-radio-button :label="TargetType[2]" :value="2" :disabled="!(handleRole.IdentifyRoles(['Teamleader']))" />
      </el-radio-group>
    </div>
    
    <div class="form-item">
      <p class="form-label">Objective Name</p>
      <el-input v-model="objective.name" placeholder="Enter objective name" />
    </div>
    
    <div class="form-item">
      <p class="form-label">Start Day</p>
      <el-date-picker
        format="DD/MM/YYYY"
        v-model="objective.startDay"
        type="date"
        placeholder="Select start date"
      />
    </div>
    
    <div class="form-item">
      <p class="form-label">End Day</p>
      <el-date-picker
        format="DD/MM/YYYY"
        v-model="objective.deadline"
        type="date"
        placeholder="Select end date"
      />
    </div>
    
    <div id="add-keyResult" class="key-results">
      <div v-for="(o, index) in objective.listKeyResults" :key="o.id" class="key-result-item">
        <div class="key-result-header">
          <el-button-group class="ml-4">
            <el-button @click="deleteKeyResult(index)" :icon="CloseBold" type="danger" plain/>
            <el-button @click="editKeyresult(index)" :icon="EditPen" type="primary" plain></el-button>
          </el-button-group>
          
        </div>
        <p class="key-result-description"><strong>Description:</strong> {{ o.description }}</p>
        <p class="key-result-info"><strong>Deadline:</strong> {{ o.deadline ? formatDate(o.deadline) : "No deadline" }}</p>
        <p class="key-result-info"><strong>Current Point:</strong> {{ o.currentPoint }}</p>
        <p class="key-result-info"><strong>Maximum Point:</strong> {{ o.maximunPoint }}</p>
        <p class="key-result-info"><strong>Unit:</strong> {{ o.unit }}</p>
        <div v-if="o.sidequests.length > 0" class="sidequests">
          <p><strong>Sidequests:</strong></p>
          <ul>
            <li v-for="sq in o.sidequests" :key="sq.id">{{ sq.name }} - Status: {{ sq.status ? "Completed" : "Incomplete" }}</li>
          </ul>
        </div>
      </div>
      
      <createKeyResultDialog
        @on-add-item="handleAddKeyResult"
        :is-edit="isEditKeyresults"
        :keyresults="editKeyresultItme"
        :dialog-visible="createKeyResultDialogVisible"
        v-if="createKeyResultDialogVisible"
        @on-turn-off-dialog="handleClose"
        @onEditItem="handleEditKeyResult"
      />
      <el-button plain @click="addKeyresult">Add Key Result</el-button>
    </div>
    
    <el-button type="primary" @click="Save()">Confirm</el-button>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import createKeyResultDialog from "@/components/okr/createKeyResultDialog.vue";
import { KeyResult } from "@/Models/KeyResult";
import { Objective } from "@/Models/Objective";
import { axiosInstance } from "../../Service/axiosConfig";
// import { TargetType } from "@/Models/TargetType";
import { CloseBold, EditPen } from "@element-plus/icons-vue";
import { formatDate } from "../../Service/formatDate";
import { deepCopy } from "../../Service/deepCopy";
import { TargetType } from "@/Models/Enum/TargetType";
import * as handleRole from "@/components/maynghien/Common/handleRole";
import { ElMessage } from "element-plus";

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
function validateObjective() {
  if (!objective.value.name) {
    ElMessage.error("Vui lòng nhập tên mục tiêu.");
    return false;
  }

  if (!objective.value.startDay) {
    ElMessage.error("Vui lòng chọn ngày bắt đầu.");
    return false;
  }

  if (!objective.value.deadline) {
    ElMessage.error("Vui lòng chọn ngày kết thúc.");
    return false;
  }

  if (objective.value.startDay >= objective.value.deadline) {
    ElMessage.error("Ngày bắt đầu phải trước ngày kết thúc.");
    return false;
  }

  if (objective.value.listKeyResults.length === 0) {
    ElMessage.error("Vui lòng thêm ít nhất một kết quả then chốt.");
    return false;
  }

  return true; // Tất cả điều kiện đều hợp lệ
}
const Save = () => {
  if (!validateObjective()) {
    return; 
  }
  if (props.isEdit) {
    axiosInstance
      .put("Objectives", objective.value)
      .then((res) => {
        console.log(res);
        if (res.data.isSuccess) {
          ElMessage.success("Update OK");
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
          ElMessage.success("Create OK");
          emit("onSearchObjective");
          emit("onClose");
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
.objective-form {
  max-width: 600px;
  margin: auto;
  padding: 20px;
  /* background-color: #f9f9f9; */
  border-radius: 8px;
  /* box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); */
}

.radio-group {
  margin-bottom: 20px;
}

.form-item {
  margin-bottom: 15px;
}

.form-label {
  margin-bottom: 5px;
  font-weight: bold;
}

.key-results {
  margin-top: 20px;
  border-top: 1px solid #eaeaea;
  padding-top: 10px;
  position: relative;
}

.key-result-item {
  background-color: #ffffff; /* Màu nền trắng cho mỗi item */
  border: 1px solid #eaeaea; /* Viền xung quanh item */
  border-radius: 4px; /* Bo góc cho item */
  padding: 15px; /* Khoảng cách bên trong item */
  margin-bottom: 15px; /* Khoảng cách giữa các item */
  transition: box-shadow 0.2s ease; /* Hiệu ứng chuyển tiếp khi hover */
  position: relative; /* Cho phép định vị các phần tử con */
}

.key-result-item:hover {
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); /* Hiệu ứng bóng khi hover */
}

.key-result-header {
  display: block;
  position: absolute;
  right: 0;
  top: 0;
}

.key-result-description {
  font-weight: bold; /* Đậm mô tả của key result */
  color: #333; /* Màu chữ tối cho mô tả */
}

.key-result-info {
  margin: 5px 0; /* Khoảng cách giữa các thông tin */
  color: #666; /* Màu chữ nhạt hơn cho thông tin */
}

.sidequests {
  margin-top: 10px; /* Khoảng cách trên cho sidequests */
  padding-left: 10px; /* Khoảng cách bên trái cho sidequests */
  border-left: 2px solid #007bff; /* Viền bên trái cho sidequests */
}
</style>
<style>

</style>