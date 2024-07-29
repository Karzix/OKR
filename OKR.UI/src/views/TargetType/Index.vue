<template>
  <Suspense>
    <BasicAdminFormVue
      :tableColumns="tableColumns"
      :apiName="'TargetType'"
      :allowAdd="true"
      :allowEdit="true"
      :allowDelete="true"
      title="Target type"
      :CustomActions="CustomActions"
      :changePageSize="false"
    ></BasicAdminFormVue>
  </Suspense>
</template>
<script lang="ts" setup>
import { TableColumn } from "@/components/maynghien/adminTable/Models/TableColumn";
import BasicAdminFormVue from "@/components/maynghien/adminTable/BasicAdminForm.vue";
import { CustomAction } from "@/components/maynghien/adminTable/Models/CustomAction";
import { onMounted, ref } from "vue";
import { axiosInstance } from "../../Service/axiosConfig";
import { getLevelName } from "../../Service/handleDisplayLevelDepartment";

const listLevelDepartment = ref<{levelApply: number; value: string}[]>([]);

const tableColumns: TableColumn[] = [
  {
    key: "id",
    label: "id",
    width: 1000,
    sortable: false,
    enableEdit: false,

    enableCreate: false,
    required: false,
    hidden: true,
    showSearch: false,
    inputType: "text",
    dropdownData: null,
  },
  {
    key: "name",
    label: "Name",
    width: 1000,
    sortable: true,
    enableEdit: true,

    enableCreate: true,
    required: false,
    hidden: false,
    showSearch: true,
    inputType: "text",
    dropdownData: null,
  },
  {
    key: "levelApply",
    label: "Level Apply",
    width: 1000,
    sortable: true,
    enableEdit: true,
    enableCreate: true,
    required: false,
    hidden: false,
    showSearch: true,
    inputType: "dropdown",
    dropdownData: {
      displayMember: "value",
      keyMember: "levelApply",
      data: listLevelDepartment.value,
    },
  },
];
const CustomActions: CustomAction[] = [];


onMounted(() => {
  axiosInstance.get("Department/department-level").then((res) => {
    var temp = res.data.data;
    for (let i = 0; i < temp.length; i++) {
      listLevelDepartment.value.push({
        levelApply: temp[i],
        value: getLevelName(temp[i]) ?? '',
      });
    }
  });
});
</script>
