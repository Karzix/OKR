<template>
    <div  v-for="item in props.keyresults" :key="item.description" class="keyresult-item">
        <div class="keyresult">
            <el-input-number
                v-model="item.percentage"
                :controls="false"
                style="width: 55px; min-width: 55px !important;"
            />
            <p class="keyresult-title">{{ item.description }} </p>
        </div>
        
        <el-progress :percentage="(item.currentPoint ?? 0) / (item.maximunPoint ?? 1)" style="width: 150px;min-width: 150px;" class="keyresult-progress"/>
    </div>
</template>
<script setup lang="ts">
import type { KeyResult } from '@/Models/KeyResult';
import { Objectives } from '@/Models/Objective';
import { getTagType} from '@/Models/EntityObjectives';

const emit = defineEmits<{
  (e: "onAddItem", item: KeyResult): void;
  (e: "onTurnOffDialog"): void;
}>();
const props = defineProps<{
  keyresults: KeyResult[];
}>();
</script>

<style scoped>
.keyresult {
  display: flex;
  align-items: center;
  gap: 5px;
  /* width: calc(100% - 150px); */
}

.keyresult-item{
    display: flex;
    justify-content: space-between;
}

</style>