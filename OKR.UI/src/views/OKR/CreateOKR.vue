<template>
  <el-card style="max-width: 480px">
    <template #header>
      <div class="card-header">
        <span>Objective</span>
      </div>
    </template>
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

    <CreateDialog @on-add-item="handleAddKeyResult"/>
  </el-card>
</template>
<script setup lang="ts">
import { ref } from "vue";
import CreateDialog from "@/components/okr/createDialog.vue";
import { KeyResult } from "@/Models/KeyResult";
const listKeyResult = ref<KeyResult[]>([]);

const handleAddKeyResult = (item: KeyResult) => {
  listKeyResult.value.push(item);
};

</script>
<style scoped>

</style>