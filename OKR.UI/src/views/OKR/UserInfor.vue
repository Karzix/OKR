<template>
  <div id="info-user">
    <el-card style="width: 20%">
      <p>Name: karzix</p>
      <p>Department: cc</p>
    </el-card>
    <div style="width: 80%" class="list-objcetive">
      <el-card>
        <el-card>
          <template #header>
            <div class="card-header">
              <span>Cá nhân</span>
              <el-progress :percentage="percentage" :color="customColors" />
            </div>
          </template>

          <el-card>
            <el-progress :percentage="90" :color="customColors" />
            
          </el-card>
          <el-card>
            <el-progress :percentage="70" :color="customColors" />
            
          </el-card>
          more.....
        </el-card>


      </el-card>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { Minus, Plus } from "@element-plus/icons-vue";

const percentage = ref(50);

const customColors = [
  { color: "#909399", percentage: 40 },
  { color: "#e6a23c", percentage: 70 },
  { color: "#67c23a", percentage: 100 },
];
const defaultProps = {
  children: "children",
  label: "label",
};



</script>
<style>
#info-user {
  display: flex;
}

.list-objcetive .el-card .el-card {
  margin: 5px;
}
</style>
