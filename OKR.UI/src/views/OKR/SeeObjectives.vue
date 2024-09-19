<template>
  <DeatailObjectives v-if="!isLoading" :objective="objectives" :is-guest="true" :target-type="targetType"></DeatailObjectives>
</template>

<script setup lang="ts">
import DeatailObjectives from '@/components/okr/DeatailObjectives.vue';
import { EntityObjectives } from '@/Models/EntityObjectives';
import type { Objective } from '@/Models/Objective';
import { axiosInstance } from '@/Service/axiosConfig';
import { onMounted, ref } from 'vue';
import { useRoute } from "vue-router";

const route = useRoute();
const isLoading = ref(true); 
// const props = defineProps<{
//   objective: Objective
// }>();
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
const targetType = ref<string>("");
const search = async () => {
  const id = route.params.EntityObjectiveId.toString();
  var entityObjectives = new EntityObjectives();
  await axiosInstance.get(`EntityObjectives/${id}`).then((res) => {
    if (res.data.isSuccess) {
      entityObjectives = res.data.data;
      objectives.value.id = entityObjectives.objectivesId;
      objectives.value.name = entityObjectives.name;
      objectives.value.point = entityObjectives.point;
      objectives.value.startDay = entityObjectives.startDay;
      objectives.value.deadline = entityObjectives.deadline;
      objectives.value.listKeyResults = entityObjectives.listKeyResults;
      objectives.value.targetType = entityObjectives.targetType;
      objectives.value.targetTypeName = entityObjectives.targetTypeName;
      objectives.value.createBy = entityObjectives.createBy;
      objectives.value.createOn = entityObjectives.createOn;
    } else {
      console.log(res.data.message);
    }
    isLoading.value = false; // Đặt cờ thành false khi dữ liệu đã được tải xong
  });
};

onMounted(() => {
  search();
  targetType.value = route.params.targetTpye.toString();
});
</script>

<style scoped>
</style>
