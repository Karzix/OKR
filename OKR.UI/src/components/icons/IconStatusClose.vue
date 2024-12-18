<template>
    <el-icon v-if="props.status != null && props.status != undefined"  :style="{ color: iconColor }"><Lock /></el-icon>
</template>
<script setup lang="ts">
import type { ObjectivesStatusClose } from "@/Models/Enum/ObjectivesStatusClose";
import {Lock} from "@element-plus/icons-vue"
import { computed, ref } from "vue";

const props = defineProps<{ 
    status?: ObjectivesStatusClose
}>();
// const iconColor = computed(() => {
//   switch (props.status) {
//     case 0: // Achieved
//       return "#67c23a";
//     case 1: // Partial
//       return "#e6a23c";
//     case 2: // Missed
//       return "#f56c6c";
//     case 3: // Dropped
//       return "#909399";
//     default:
//       return "black";
//   }
// });
const iconColor = ref("black");
const getColor = () => {
  switch (props.status) {
    case 0: // Achieved
      iconColor.value = "#67c23a";
      break;
    case 1: // Partial
      iconColor.value = "#e6a23c";
      break;
    case 2: // Missed
      iconColor.value = "#f56c6c";
      break;
    case 3: // Dropped
      iconColor.value = "#909399";
      break;
    default:
      iconColor.value = "black";
  }
}
getColor()
</script>