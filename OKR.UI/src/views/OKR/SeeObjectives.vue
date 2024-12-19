<template>
  <div class="body">
    <DeatailObjectives :is-owner="false" :objectives="objectives" ></DeatailObjectives>
  </div>
  
</template>

<script setup lang="ts">
import DeatailObjectives from '@/components/okr/DetailObjectives.vue';
import { EntityObjectives } from '@/Models/EntityObjectives';
import type { Objectives } from '@/Models/Objective';
import { axiosInstance } from '@/Service/axiosConfig';
import { onBeforeMount, onMounted, ref } from 'vue';
import { useRoute } from "vue-router";
import DetailObjectives from '@/components/okr/DetailObjectives.vue';
import { ElLoading } from 'element-plus';
import { TargetType } from '@/Models/Enum/TargetType';

const route = useRoute();
const isLoading = ref(true); 
// const props = defineProps<{
//   objective: Objective
// }>();
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
  createdOn: new Date(),
  lastProgressUpdate: new Date(),
  numberOfPendingUpdates: 0,
  statusClose: 0
});
// const targetType = ref<string>("");
const search = async () => {
  const id = route.params.ObjectiveId.toString();
  objectives.value.id = id;
};

onBeforeMount(() => {
  search();
  // targetType.value = route.params.targetTpye.toString();
});
</script>

<style scoped>
.body{
  max-width: 1200px;
  margin-left: auto;
  margin-right: auto;
  background-color: #ffffffcc;
  border-radius: 1px;
  border-radius: 20px;
  padding: 20px;
  box-shadow: 0px 5px 5px 5px rgba(0, 0, 0, 0.3);
}
</style>