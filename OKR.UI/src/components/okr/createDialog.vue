<template>
  <el-button plain @click="dialogVisible = true"> Add </el-button>
  <el-dialog v-model="dialogVisible" title="Add" class="createDialog">
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
            type="datetime"
            placeholder="Select date and time"
            class="form-input"
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
                { key: '#', name: 'Value' },
                { key: '%', name: 'Percent' },
                { key: '01', name: 'Checked' },
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
            v-model="keyResult.currentPoint"
            class="mx-4"
            :min="1"
            :max="10"
            controls-position="right"
          />
        </div>
        <div class="form-item">
          <p class="form-label">Maximum Point</p>
          <el-input-number
            v-model="keyResult.maximumPoint"
            class="mx-4"
            :min="1"
            :max="10"
            controls-position="right"
          />
        </div>
        <div class="form-item">
          <p class="form-label">Sidequest</p>
          <el-checkbox v-for="item in keyResult.sidequests" :label="item.name" v-model="item.status" size="large" />
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
        <el-button @click="dialogVisible = false">Cancel</el-button>
        <el-button
          type="primary"
          @click="
            () => {
              handleAddItem(keyResult);
              dialogVisible = false;
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
import { ref } from "vue";
import { KeyResult } from "@/Models/KeyResult";
import { Sidequest } from "@/Models/Sidequests";
const dialogVisible = ref(false);
const sidequestsName = ref("");
const keyResult = ref<KeyResult>({
  id: "",
  description: "",
  active: true,
  deadline: undefined,
  unit: "",
  currentPoint: 0,
  maximumPoint: 0,
  objectId: "",
  sidequests: [],
});
const emit = defineEmits<{
  (e: "onAddItem", item: KeyResult): void;
}>();

const handleAddItem = (item: KeyResult) => {
  emit("onAddItem", item);
};

const handleAddSideQuest = () => {
  if (sidequestsName.value) {
    if(!keyResult.value.sidequests) {
      keyResult.value.sidequests = [] as Sidequest[]
    }
    if (keyResult.value.sidequests) {
      var newSideQuest: Sidequest = {
        name: sidequestsName.value,
        id: undefined,
        status: false,
      }
      keyResult.value.sidequests.push(newSideQuest);
      sidequestsName.value = "";
    }

  }
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
