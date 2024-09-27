<template>
  <el-dialog
    v-model="props.dialogVisible"
    @close="handleClose"
    :title="props.isEdit ? 'Edit key result' : 'Create key result'"
    class="createDialog"
  >
    <div class="body">
      <div class="form-item">
        <p class="form-label">Description</p>
        <el-input v-model="keyResult.description" class="form-input" />
      </div>
      <div class="dl-flex">
        <div class="form-item">
          <p class="form-label">Deadline</p>
          <el-date-picker
            v-model="keyResult.deadline"
            type="date"
            class="form-input"
            format="DD/MM/YYYY"
          />
        </div>
        <div class="form-item">
          <p class="form-label">Unit</p>
          <el-select
            v-model="keyResult.unit"
            placeholder="Select"
            class="form-input"
          >
            <el-option
              v-for="item in [
                { key: 1, name: 'Value' },
                { key: 0, name: 'Percent' },
                { key: 2, name: 'Checked' },
              ]"
              :key="item.key"
              :label="item.name"
              :value="item.key"
            />
          </el-select>
        </div>
      </div>
      <div class="dl-flex">
        <div class="form-item">
          <p class="form-label">Current Point</p>
          <el-input-number
            :disabled="keyResult.unit === 2"
            v-model="keyResult.currentPoint"
            class="mx-4"
            :min="0"
            :max="keyResult.maximunPoint"
            controls-position="right"
          />
        </div>
        <div class="form-item">
          <p class="form-label">Maximum Point</p>
          <el-input-number
            :disabled="keyResult.unit === 2"
            v-model="keyResult.maximunPoint"
            class="mx-4"
            :min="1"
            controls-position="right"
          />
        </div>
        <div class="form-item">
          <p class="form-label">Sidequest</p>
          <div v-for="(item, index) in keyResult.sidequests">
            <el-checkbox
              :label="item.name"
              v-model="item.status"
              size="large"
            />
            <el-icon @click="handleDeleteSideQuest(index)"><Close /></el-icon>
          </div>

          <el-input
            v-model="sidequestsName"
            style="width: 240px"
            placeholder="Please input"
            clearable
            @keyup.enter="handleAddSideQuest()"
          />
        </div>
      </div>
    </div>
    <template #footer>
      <div class="dialog-footer">
        <el-button
          type="primary"
          @click="
            () => {
              if(validateKeyResult() == false) return;
              handleAddItem(keyResult);
              handleClose();
            }
          "
        >
          Confirm
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script lang="ts" setup>
import { onMounted, ref, watch } from "vue";
import { KeyResult } from "@/Models/KeyResult";
import { Sidequest } from "@/Models/Sidequests";
import { Close } from "@element-plus/icons-vue";
import { getUtcOffsetInHours } from "@/Service/formatDate";
import { deepCopy } from "@/Service/deepCopy";
import { ElMessage } from "element-plus";
const sidequestsName = ref("");
const keyResult = ref<KeyResult>({
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
const emit = defineEmits<{
  (e: "onAddItem", item: KeyResult): void;
  (e: "onTurnOffDialog"): void;
  (e: "onEditItem", item: KeyResult): void;
}>();
const props = defineProps<{
  keyresults: KeyResult;
  isEdit: boolean;
  dialogVisible: boolean;
}>();
const validateKeyResult = () => {
  // if (!keyResult.value.description) {
  //   ElMessage.error("Vui lòng nhập mô tả.");
  //   return false;
  // }

  if (!keyResult.value.deadline) {
    ElMessage.error("Vui lòng chọn ngày hết hạn.");
    return false;
  }

  if (keyResult.value.unit === undefined) {
    ElMessage.error("Vui lòng chọn đơn vị.");
    return false;
  }

  if (keyResult.value.unit !== 2 && keyResult.value.currentPoint! < 0) {
    ElMessage.error("Điểm hiện tại phải lớn hơn hoặc bằng 0.");
    return false;
  }

  if (keyResult.value.unit !== 2 && keyResult.value.maximunPoint! < 1) {
    ElMessage.error("Điểm tối đa phải lớn hơn hoặc bằng 1.");
    return false;
  }

  return true; // Tất cả điều kiện đều hợp lệ
};
const handleAddItem = (item: KeyResult) => {
  if (!validateKeyResult()) {
    return; 
  }
  if (!props.isEdit) {
    emit("onAddItem", deepCopy(item));
  }
  else{
    emit("onEditItem", item);
  }
  item = {
      id: undefined,
      description: "",
      active: true,
      deadline: undefined,
      unit: 0,
      currentPoint: 0,
      maximunPoint: 100,
      // objectId: "",
      sidequests: [],
      note: ""
    };
};

const handleAddSideQuest = () => {
  if (sidequestsName.value) {
    if (!keyResult.value.sidequests) {
      keyResult.value.sidequests = [] as Sidequest[];
    }
    if (keyResult.value.sidequests) {
      var newSideQuest: Sidequest = {
        name: sidequestsName.value,
        id: undefined,
        status: false,
        keyResultsId: undefined,
      };
      keyResult.value.sidequests.push(newSideQuest);
      sidequestsName.value = "";
    }
  }
};
const handleDeleteSideQuest = (index: number) => {
  keyResult.value.sidequests?.splice(index, 1);
};
onMounted(() => {
  if (props.isEdit) {
    keyResult.value = props.keyresults;
  } else {
    keyResult.value = {
      id: undefined,
      description: "",
      active: true,
      deadline: undefined,
      unit: 0,
      currentPoint: 0,
      maximunPoint: 100,
      // objectId: "",
      sidequests: [],
      note:""
    };
  }
});
watch(
  () => props.isEdit,
  () => {
    if (props.isEdit) {
      keyResult.value = props.keyresults;
    } else {
      keyResult.value = {
        id: undefined,
        description: "",
        active: true,
        deadline: undefined,
        unit: 0,
        currentPoint: 0,
        maximunPoint: 100,
        // objectId: "",
        sidequests: [],
        note: ""
      };
    }
  }
);

const handleClose = () => {
  emit("onTurnOffDialog");
};
</script>

<style scoped>
.body {
  display: flex;
  flex-direction: column;
  gap: 20px;
  padding: 20px;
}

.form-item {
  display: flex;
  flex-direction: column;
}

.form-label {
  margin-bottom: 8px;
  font-weight: bold;
}

.form-input {
  width: 100%;
  min-width: 150px;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  padding: 10px;
}

.el-button {
  margin-left: 10px;
}
.dl-flex {
  display: flex;
  gap: 20px;
  flex-wrap: wrap;
}
.el-dialog__header {
  background-color: white;
}
@media (max-width: 600px) {
  .body {
    padding: 10px;
  }

  .form-input {
    width: 100%;
  }

  .dialog-footer {
    flex-direction: column;
    align-items: stretch;
  }

  .el-button {
    margin-left: 0;
    margin-bottom: 10px;
  }
}
</style>
<style>
@media (max-width: 700px) {
  .createDialog {
    width: 100% !important;
    padding: 10px;
  }
}
</style>
