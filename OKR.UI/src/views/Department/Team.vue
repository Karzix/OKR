<template>
  <Suspense>
    <BasicAdminFormVue
      :tableColumns="tableColumns"
      :apiName="'Department'"
      :allowAdd="true"
      :allowEdit="true"
      :allowDelete="true"
      title="Branch"
      :CustomActions="CustomActions"
      :changePageSize="false"
      :CustomFilters="CustomFilters"
    ></BasicAdminFormVue>
  </Suspense>
</template>
<script lang="ts" setup>
import type { LoginResult } from "@/Models/LoginResult";
import BasicAdminFormVue from "@/components/maynghien/adminTable/BasicAdminForm.vue";
import {
  ApiActionType,
  CustomAction,
  CustomActionDataType,
} from "@/components/maynghien/adminTable/Models/CustomAction";
import { TableColumn } from "@/components/maynghien/adminTable/Models/TableColumn";
import Cookies from "js-cookie";
import { ref } from "vue";
import { Filter } from "../../components/maynghien/BaseModels/Filter";

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
    key: "parentDepartmentId",
    label: "Branch",
    width: 1000,
    sortable: true,
    enableEdit: true,

    enableCreate: true,
    required: true,
    hidden: true,
    showSearch: true,
    inputType: "dropdown",
    dropdownData: {
      displayMember: "name",
      keyMember: "id",
      apiUrl: "Department/parent-department-by-level/2",
    },
  },
  {
    key: "parentDepartmentName",
    label: "Branch Name",
    width: 1000,
    sortable: true,
    enableEdit: false,

    enableCreate: false,
    required: false,
    hidden: false,
    showSearch: false,
    inputType: "text",
    dropdownData: null,
  },
];
const CustomActions: CustomAction[] = [];
const CustomFilters: Filter[] = [
  {
    FieldName: "level",
    DisplayName: "Level",
    Value: "2",
    Operation: "",
    Type: "",
    dropdownData: null,
  },
];
</script>
