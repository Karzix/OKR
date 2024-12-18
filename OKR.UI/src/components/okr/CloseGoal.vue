<template>
  <el-radio-group v-model="selectedOutcome" class="display-column">
    <el-radio :label="ObjectivesStatusClose.Achieved" class="Achieved">
      <span>
        <i class="el-icon-circle-check" style="margin-right: 5px"></i>
        Achieved
      </span>
    </el-radio>
    <el-radio :label="ObjectivesStatusClose.Partial" class="Partial">
      <span>
        <i class="el-icon-warning" style="margin-right: 5px"></i> Partial
      </span>
    </el-radio>
    <el-radio :label="ObjectivesStatusClose.Missed" class="Missed">
      <span>
        <i class="el-icon-circle-close" style="margin-right: 5px"></i> Missed
      </span>
    </el-radio>
    <el-radio :label="ObjectivesStatusClose.Dropped" class="Dropped">
      <span>
        <i class="el-icon-remove-outline" style="margin-right: 5px"></i>
        Dropped
      </span>
    </el-radio>
  </el-radio-group>
  <div class="footer">
    <el-button type="primary" size="large" @click="handleSave" style="width: 120px;">Save</el-button>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { ObjectivesStatusClose } from "@/Models/Enum/ObjectivesStatusClose";
import { axiosInstance } from "@/Service/axiosConfig";
import { ElMessage } from "element-plus";

const props = defineProps<{
    objectivesId: string
}>();
const emit = defineEmits<{
    (e: "close"): void;
}>();
const selectedOutcome = ref<ObjectivesStatusClose | null>(null); // Giá trị kiểu Enum

const handleSave = () => {
  console.log("Outcome:", selectedOutcome.value);
  axiosInstance.put(`Objectives/CloseGoal/${props.objectivesId}/${selectedOutcome.value}`)
  .then((rs) => {
    if(rs.data.isSuccess){
      ElMessage({
        type: "success",
        message: "Success",
      })
      emit("close");
    }
    else{
      ElMessage({
        type: "error",
        message: rs.data.message,
      })
    }
  })
};
</script>
<style scoped>
.el-radio {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
}
.display-column {
  flex-direction: column;
}
.display-column .el-radio {
  width: 100px;
  margin: 0;
}
.footer{
  display: flex;
  flex-direction: row-reverse;
  border-top: 1px solid #cccc;
  padding: 10px;
}

</style>
