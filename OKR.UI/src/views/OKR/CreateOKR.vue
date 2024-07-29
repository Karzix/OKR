<template>
  <div>
    <div>
      <el-radio-group v-model="objective.targetTypeId" size="large">
        <el-radio-button v-for="item in TargetTypes" :label="item.name" :value="item.id" />
      </el-radio-group>
    </div>
    <div class="form-item">
      <p class="form-label">Objective name</p>
      <el-input v-model="objective.name" style="width: 240px" />
    </div>
  </div>
  <div id="add-keyResult">
    <p v-for="o in listKeyResult" :key="o.id" class="text item">
      {{
        o.description +
        " " +
        (o.deadline ? o.deadline.toLocaleDateString("en-GB") : "") +
        " " +
        o.currentPoint +
        " " +
        o.maximumPoint
      }}
    </p>
    <createKeyResultDialog @on-add-item="handleAddKeyResult" />
  </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from "vue";
import createKeyResultDialog from "@/components/okr/createKeyResultDialog.vue";
import { KeyResult } from "@/Models/KeyResult";
import { Objective } from "@/Models/Objective";
import { axiosInstance } from '../../Service/axiosConfig'
import { TargetType } from "@/Models/TargetType";

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
  })
}
onMounted(() => {
  GetGeneralData()
})
</script>
<style scoped></style>
