<template>
  <DeatailObjectives :is-owner="false" :objectives="objectives"></DeatailObjectives>
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

const route = useRoute();
const isLoading = ref(true); 
// const props = defineProps<{
//   objective: Objective
// }>();
const objectives = ref<Objectives>({} as Objectives);
// const targetType = ref<string>("");
const search = async () => {
  ElLoading.service({
    lock: true,
    text: 'Loading',
    background: 'rgba(0, 0, 0, 0.7)',
  })
  const id = route.params.ObjectiveId.toString();
  await axiosInstance
    .get(`Objectives/${id}`)
    .then((res) => {
      objectives.value = res.data;
      isLoading.value = false;
    })
    .catch((err) => {
      console.log(err);
    });
  
};

onBeforeMount(() => {
  search();
  // targetType.value = route.params.targetTpye.toString();
});
</script>

<style scoped>
</style>