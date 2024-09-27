<template>
  <div>
    <div>
      <el-radio-group v-model="objective.targetType" size="large">
        <el-radio-button :label="TargetType[0]" :value="0" />
        <el-radio-button :label="TargetType[1]" :value="1" :disabled="!( handleRole.IdentifyRoles(['Admin','superadmin']))"/>
        <el-radio-button :label="TargetType[2]" :value="2" :disabled="!( handleRole.IdentifyRoles(['Teamleader']))"/>
      </el-radio-group>
    </div>
    <div class="form-item">
      <p class="form-label">Objective name</p>
      <el-input v-model="objective.name" />
    </div>
    <div class="form-item">
      <p class="form-label">start Day</p>
      <el-date-picker
        format="DD/MM/YYYY"
        v-model="objective.startDay"
        type="date"
      />
    </div>
    <div class="form-item">
      <p class="form-label">end Day</p>
      <el-date-picker
        format="DD/MM/YYYY"
        v-model="objective.deadline"
        type="date"
      />
    </div>
  </div>
  <div id="add-keyResult">
    <div
      v-for="(o, index) in objective.listKeyResults"
      :key="o.id"
      class="key-result-item"
    >
      <div class="key-result-header">
        <p class="key-result-description">
          <strong>Description:</strong> {{ o.description }}
        </p>
        <el-button @click="deleteKeyResult(index)"
          ><el-icon><CloseBold /></el-icon
        ></el-button>
        <el-button @click="editKeyresult(index)"
          ><el-icon>Edit</el-icon></el-button
        >
      </div>
      <p class="key-result-info">
        <strong>Deadline:</strong>
        {{ o.deadline ? formatDate(o.deadline) : "No deadline" }}
      </p>
      <p class="key-result-info">
        <strong>Current Point:</strong> {{ o.currentPoint }}
      </p>
      <p class="key-result-info">
        <strong>Maximum Point:</strong> {{ o.maximunPoint }}
      </p>
      <p class="key-result-info"><strong>Unit:</strong> {{ o.unit }}</p>
      <div v-if="o.sidequests.length > 0" class="sidequests">
        <p><strong>Sidequests:</strong></p>
        <ul>
          <li v-for="sq in o.sidequests" :key="sq.id">
            {{ sq.name }} - Status: {{ sq.status ? "Completed" : "Incomplete" }}
          </li>
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
    <el-button plain @click="addKeyresult">
      Add key result
    </el-button>
  </div>
  <el-button type="primary" @click="Save()">Confirm</el-button>
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
.key-result-item {
  border: 1px solid #dcdcdc;
  border-radius: 5px;
  padding: 10px;
  margin-bottom: 10px;
}
.key-result-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.key-result-description {
  font-size: 16px;
  font-weight: bold;
}
.key-result-info {
  margin: 5px 0;
}
.sidequests {
  margin-top: 10px;
}
.sidequests ul {
  padding-left: 20px;
}
</style>
