<template>
  <el-dialog>
    <el-select
        v-model="idUser"
        multiple
        filterable
        remote
        reserve-keyword
        placeholder="Please enter a keyword"
        style="width: 240px"
        :remote-method="remoteMethod"
      >
        <el-option
          v-for="item in listUserDisplay"
          :key="item.id"
          :label="item.userName"
          :value="item.id"
        />
      </el-select>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { UserModel } from "../../Models/UserModel";
import { axiosInstance } from "@/Service/axiosConfig";

var listUser = ref<UserModel[]>([]);
var listUserDisplay = ref<UserModel[]>([]);
var idUser = ref<string>("");
const getAll = ()=>{
    axiosInstance.get("User").then((res)=>{
        listUser.value = res.data.data
    })
}

getAll()
const remoteMethod = (query: string) => {
  if (query) {
    setTimeout(() => {
        listUserDisplay.value = listUser.value.filter((item) => {
        return item.userName?.toLowerCase().includes(query.toLowerCase())
      }).slice(0, 10) ?? []
    }, 200)
  } else {
    listUserDisplay.value = []
  }
}
</script>
<style scoped>

</style>