<template>
  <div class="action-search">
    <!-- Dropdown để chọn tiêu chí tìm kiếm -->
    <el-select v-model="selectedFilterKey" placeholder="Chọn bộ lọc" @change="onFilterChange" class="select-filter">
      <el-option
        v-for="filter in filterOptions"
        :key="filter.FieldName"
        :label="filter.DisplayName"
        :value="filter.FieldName ?? ''"
      />
    </el-select>

    <!-- Hiển thị input tương ứng với bộ lọc đã chọn -->
    <div v-if="selectedFilter">
      <el-select
        v-model="selectedFilter.Value"
        clearable
        style="width: 190px"
        filterable
        remote
        :remote-method="seachUser"
        v-if="selectedFilter.FieldName === 'userName'"
        class="action-input"
        placeholder="owner name"
      >
        <el-option
          v-for="item in listUser"
          :key="item.userName"
          :label="item.userName"
          :value="item.userName ?? ''"
        />
      </el-select>

      <el-select
        v-model="selectedFilter.Value"
        clearable
        style="width: 190px"
        filterable
        remote
        :remote-method="searchDepartment"
        v-if="selectedFilter.FieldName === 'departmentId'"
        class="action-input"
        placeholder="team name"
      >
        <el-option
          v-for="item in listDepartment"
          :key="item.id"
          :label="item.name"
          :value="item.id ?? ''"
        />
      </el-select>
      <el-input
        v-model="filLabel.Value"
        class="action-input"
        placeholder="lable objectives"
      />
    </div>

    <!-- Nút tìm kiếm -->
    <el-button @click="search">
      <el-icon style="vertical-align: middle">
        <Search />
      </el-icon>
  </el-button>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import { ElSelect, ElOption, ElInput, ElButton } from "element-plus";
import type { UserModel } from "@/Models/UserModel";
import { axiosInstance } from "@/Service/axiosConfig";
import type { Filter } from "../maynghien/BaseModels/Filter";
import { deepCopy } from "@/Service/deepCopy";
import type { Department } from "@/Models/Department";
import Cookies from "js-cookie";
import { Search } from "@element-plus/icons-vue";

// Define filter options
const filOwner = ref<Filter>({
  FieldName: "userName",
  Value: Cookies.get("userName")?.toString() ?? "",
  DisplayName: "Owner",
  Operation: "",
  Type: "text",
  dropdownData: null,
});
const filLabel = ref<Filter>({
  FieldName: "name",
  Value: "",
  DisplayName: "Label",
  Operation: "",
  Type: "text",
  dropdownData: null,
});
const filTeam = ref<Filter>({
  FieldName: "departmentId",
  Value: "",
  DisplayName: "Team",
  Operation: "",
  Type: "text",
  dropdownData: null,
});
const emit = defineEmits<{
    (e: "onSearch" , filter: Filter[]): void;
}>();

// Track selected filter and filter options
const selectedFilterKey = ref<string>(filOwner.value.FieldName ?? ""); // Set default key to filLabel
const selectedFilter = ref<Filter>(deepCopy(filOwner.value)); // Set default filter to filLabel
const filterOptions = [filOwner.value, filTeam.value];
const listUser = ref<UserModel[]>([]);
const listDepartment = ref<Department[]>([]);

const seachUser = async (query: string) => {
  const url = "User/list-by-keyword/" + query;
  await axiosInstance.get(url).then((res) => {
    listUser.value = res.data.data;
  });
};
const searchDepartment = async (query: string) => {
  await axiosInstance.get("Department/list-by-keyword/" + query).then((res) => {
    listDepartment.value = res.data.data;
  });
};

// Update selected filter based on selected key
const onFilterChange = () => {
  const selected = filterOptions.find((filter) => filter.FieldName === selectedFilterKey.value);
  selectedFilter.value = selected ? deepCopy(selected) : null;
};

// Search function
const search = () => {
  const filters = [];
  if(selectedFilter.value.DisplayName == "Owner"){
    filters.push(deepCopy(selectedFilter.value));
  }
  if(filLabel.value.Value){
    filters.push(deepCopy(filLabel.value));
  }
  if(selectedFilter.value.DisplayName == "Team"){
    filters.push(deepCopy(selectedFilter.value));
  }
  emit("onSearch", deepCopy(filters));
};

// Watch the selected filter and log changes
watch(() => selectedFilter.value, () => {
  console.log(selectedFilter.value);  
});

</script>

<style scoped>
.action-search {
  display: flex;
}
.select-filter{
  max-width: 95px !important;
  min-width: 95px !important;
}
.action-input{
  width: 190px !important;
}
</style>
