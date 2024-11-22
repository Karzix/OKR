<template>
    <template v-if="props.targetType === TargetType.Individual">
        <el-tag class="ml-2" effect="plain" color="rgb(235, 180.6, 99)">
            <el-icon color="white"><User /></el-icon>
        </el-tag>
    </template>
    <template v-else-if="props.targetType === TargetType.Department">
        <el-tag class="ml-2" effect="plain" color="rgb(247, 137.4, 137.4)">
            <el-icon color="white"><Flag /></el-icon>
        </el-tag>
    </template>
    <template v-else-if="props.targetType === TargetType.Company">
        <el-tag class="ml-2" effect="plain" color="rgb(33.2, 61.4, 90.5)">
            <el-icon color="white"><OfficeBuilding /></el-icon>
        </el-tag>
    </template>
</template>

<script setup lang="ts">
import { TargetType } from '@/Models/Enum/TargetType';
import { Flag, OfficeBuilding, User } from '@element-plus/icons-vue';

const props = defineProps<{ 
    targetType: TargetType 
}>();
</script>

<style scoped>
.el-tag:hover {
    cursor: default; 
}
.el-icon:hover{
    cursor: default; 
}
</style>
