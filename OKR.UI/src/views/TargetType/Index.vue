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
];
const CustomActions: CustomAction[] = [];
</script>
