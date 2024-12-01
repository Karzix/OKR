<template>
  <div
    v-for="(item, index ) in props.keyresults"
    :key="item.description"
    class="keyresult-item"
    @click="emit('onSelectKeyResult', item, index)"
  >
    <div class="keyresult">
      <el-input-number
        @click.stop
        v-model="item.percentage"
        :controls="false"
        style="width: 55px; min-width: 55px !important"
        :disabled="!props.isCreateOrEdit"
      />
      <p class="keyresult-title">{{ item.description }}</p>
    </div>

    <el-progress v-if="item.unit != 2"
      :percentage="
        (((item.currentPoint ?? 0) / (item.maximunPoint ?? 1)) * 100).toFixed(2)
      "
      :color="getStatusColor(item.status)"
      style="width: 150px; min-width: 150px"
      class="keyresult-progress"
    />
    <el-button v-else @click.stop="() =>{item.isCompleted = !item.isCompleted}"
      :disabled="true" > 
      {{ item.isCompleted ? 'Completed' : 'Not Completed' }}
    </el-button>
  </div>
</template>
<script setup lang="ts">
import type { KeyResult } from "@/Models/KeyResult";
import { Objectives } from "@/Models/Objective";
import { getTagType, StatusObjectives, getStatusColor } from "@/Models/EntityObjectives";

const emit = defineEmits<{
  (e: "onAddItem", item: KeyResult): void;
  (e: "onTurnOffDialog"): void;
  (e: "onSelectKeyResult", item: KeyResult, index: number): void;
}>();
const props = defineProps<{
  keyresults: KeyResult[];
  isCreateOrEdit: boolean;
}>();


</script>

<style scoped>
.keyresult {
  display: flex;
  align-items: center;
  gap: 5px;
  /* width: calc(100% - 150px); */
}

.keyresult-item {
  display: flex;
  justify-content: space-between;
}
</style>
