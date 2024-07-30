<template>
  <div>
    <div>
      <el-radio-group v-model="objective.targetTypeId" size="large">
        <el-radio-button
          v-for="item in TargetTypes"
          :label="item.name"
          :value="item.id"
        />
      </el-radio-group>
    </div>
    <div class="form-item">
      <p class="form-label">Objective name</p>
      <el-input v-model="objective.name"  />
    </div>
    <div class="form-item">
      <p class="form-label">start Day</p>
      <el-date-picker
        v-model="objective.startDay"
        type="date"
      />
    </div>
    <div class="form-item">
      <p class="form-label">start Day</p>
      <el-date-picker
        v-model="objective.deadline"
        type="date"
      />
    </div>
  </div>
  <div id="add-keyResult">
    <div v-for="(o, index) in listKeyResult" :key="o.id" class="key-result-item">
      <div class="key-result-header">
        <p class="key-result-description">
          <strong>Description:</strong> {{ o.description }}
        </p>
        <el-button @click="deleteKeyResult(index)"><el-icon><CloseBold /></el-icon></el-button
        >
      </div>
      <p class="key-result-info">
        <strong>Active:</strong> {{ o.active ? "Yes" : "No" }}
      </p>
      <p class="key-result-info">
        <strong>Deadline:</strong>
        {{
          o.deadline ? o.deadline.toLocaleDateString("en-GB") : "No deadline"
        }}
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
    <createKeyResultDialog @on-add-item="handleAddKeyResult" />
  </div>
  <el-button type="primary" @click="Create()">Confirm</el-button>
</template>
<script setup lang="ts">
import { ref, onMounted } from "vue";
import createKeyResultDialog from "@/components/okr/createKeyResultDialog.vue";
import { KeyResult } from "@/Models/KeyResult";
import { Objective } from "@/Models/Objective";
import { axiosInstance } from "../../Service/axiosConfig";
import { TargetType } from "@/Models/TargetType";
import {CloseBold} from '@element-plus/icons-vue'

const listKeyResult = ref<KeyResult[]>([]);
const objective = ref<Objective>({
  id: undefined,
  name: "",
  startDay: undefined,
  deadline: undefined,
  listKeyResults: listKeyResult.value,
  targetTypeId: undefined,
  targetTypeName: "",
  point: 0,
});

const TargetTypes = ref<TargetType[]>([]);
const handleAddKeyResult = (item: KeyResult) => {
  listKeyResult.value.push(item);
};
const GetGeneralData = () => {
  axiosInstance.get("TargetType").then((res) => {
    TargetTypes.value = res.data.data;
  });
};
onMounted(() => {
  GetGeneralData();
});
const Create = () => {
  axiosInstance
    .post("Objectives", objective.value)
    .then((res) => {
      console.log(res);
    })
    .catch((error) => {
      console.log(error);
    });
};
function deleteKeyResult(index : number) {
    listKeyResult.value.splice(index, 1);
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
