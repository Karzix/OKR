<template>
  <Suspense>
    <BasicAdminFormVue
      :tableColumns="tableColumns"
      :apiName="'User'"
      :allowAdd="true"
      :allowEdit="true"
      :allowDelete="false"
      title="User"
      :CustomActions="CustomActions"
      :changePageSize="false"
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
    hiddenElement: true
  },
  {
    key: "userName",
    label: "Tên Người dùng",
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
    key: "password",
    label: "Mật Khẩu",
    width: 1000,
    sortable: true,
    enableEdit: false,

    enableCreate: true,
    required: false,
    hidden: true,
    showSearch: false,
    inputType: "text",
    dropdownData: null,
  },
  {
    key: "email",
    label: "Email",
    width: 1000,
    sortable: false,
    enableEdit: true,

    enableCreate: true,
    required: false,
    hidden: false,
    showSearch: false,
    inputType: "text",
    dropdownData: null,
    hiddenElement: true
  },
  {
    key: "lockoutEnabled",
    label: "Khóa/Mở",
    width: 500,
    sortable: false,
    enableEdit: false,

    enableCreate: false,
    required: false,
    hidden: false,
    showSearch: false,
    inputType: "text",
    dropdownData: null,
    hiddenElement: true
  },
  {
    key: "role",
    label: "Role",
    enableEdit: true,
    enableCreate: true,
    hidden: false,
    width: 300,
    required: true,
    sortable: false,
    showSearch: false,
    inputType: "dropdown",
    dropdownData: {
      displayMember: "roleName",
      keyMember: "role",
      data: [
        {
          role: "Admin",
          roleName: "Admin",
        },
        {
          role: "Teamleader",
          roleName: "Teamleader",
        },
      ],
    },
  },
  {
    key: "departmentId",
    label: "Department",
    width: 1000,
    sortable: true,
    enableEdit: true,

    enableCreate: true,
    required: true,
    hidden: true,
    showSearch: true,
    inputType: "tree",
    dropdownData: {
      displayMember: "name",
      keyMember: "id",
      apiUrl: "Department",
    },
  },
  {
    key: "departmentName",
    label: "depaetment",
    width: 1000,
    sortable: false,
    enableEdit: false,

    enableCreate: false,
    required: false,
    hidden: false,
    showSearch: false,
    inputType: "text",
    dropdownData: null,
    hiddenElement: true
  },
];
const CustomActions: CustomAction[] = [

];
</script>
