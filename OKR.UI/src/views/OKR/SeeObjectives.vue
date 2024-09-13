<template>
  <DeatailObjectives v-if="!isLoading" :objective="objectives" :is-guest="true"></DeatailObjectives>
</template>

<script setup lang="ts">
import DeatailObjectives from '@/components/okr/DeatailObjectives.vue';
import type { Objective } from '@/Models/Objective';
import { axiosInstance } from '@/Service/axiosConfig';
import { onMounted, ref } from 'vue';
import { useRoute } from "vue-router";

const route = useRoute();
const isLoading = ref(true); 

const objectives = ref<Objective>({
  id: undefined,
  name: "",
  startDay: undefined,
  deadline: undefined,
  listKeyResults: [],
  targetType: undefined,
  targetTypeName: "",
  point: 0,
});

const search = async () => {
  const id = route.params.ObjectiveId.toString();
  await axiosInstance.get(`Objectives/${id}`).then((res) => {
    if (res.data.isSuccess) {
      objectives.value = res.data.data;
    } else {
      console.log(res.data.message);
    }
    isLoading.value = false; // Đặt cờ thành false khi dữ liệu đã được tải xong
  });
};

onMounted(() => {
  search();
});
</script>

<style scoped>
</style>
