<template>
  <Suspense>
    <BasicAdminFormVue
      :tableColumns="tableColumns"
      :apiName="'User'"
      :allowAdd="false"
      :allowEdit="false"
      :allowDelete="false"
      title="User"
      :CustomActions="CustomActions"
      :changePageSize="false"
       @onCustomAction="onCustomAction"
        ref="basicAdminFormRef"
    ></BasicAdminFormVue>
  </Suspense>
  <Create v-if="showDialogCustomCreate" :openDialog="showDialogCustomCreate" @onClose="onClose()" :isEdit="isEdit" :User="userSelect"></Create>
</template>
<script lang="ts" setup>
import type { LoginResult } from "@/Models/LoginResult";
import { UserModel } from "@/Models/UserModel";
import BasicAdminFormVue from "@/components/maynghien/adminTable/BasicAdminForm.vue";
import {
  ApiActionType,
  CustomAction,
  CustomActionDataType,
  CustomActionResponse,
} from "@/components/maynghien/adminTable/Models/CustomAction";
import { TableColumn } from "@/components/maynghien/adminTable/Models/TableColumn";
import Cookies from "js-cookie";
import { ref } from "vue";
import Create from "./Create.vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { axiosInstance } from "@/Service/axiosConfig";
import { deepCopy } from "@/Service/deepCopy";

const CustomActions: CustomAction[] = [
  {
    ActionName: "Create",
    ActionLabel: "Create",
    ApiActiontype: ApiActionType.PUT,
    IsRowAction: false,
    DataType: CustomActionDataType.Filters,
  },
  {
    ActionName: "Edit",
    ActionLabel: "Edit",
    ApiActiontype: ApiActionType.PUT,
    IsRowAction: true,
    DataType: CustomActionDataType.Filters,
  },
  {
    ActionName: "Lock/Unlock",
    ActionLabel: "Lock/Unlock",
    ApiActiontype: ApiActionType.PUT,
    IsRowAction: true,
    DataType: CustomActionDataType.Filters,
  }
];
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
    key: "isLocked",
    label: "Locked",
    enableEdit: false,
    enableCreate: false,
    hidden: false,
    width: 300,
    required: false,
    sortable: false,
    showSearch: false,
    inputType: "text",
    // dropdownData: {
    //   displayMember: "label",
    //   keyMember: "value",
    //   data: [
    //     {
    //       label: "Unlock",
    //       value: false,
    //     },
    //     {
    //       label: "Lock",
    //       value: true,
    //     },
    //   ],
    // },
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
        // {
        //   role: "BranchManagement",
        //   roleName: "Branch Management",
        // },
        {
          role: "Employee",
          roleName: "Employee",
        }
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
    inputType: "dropdown",
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
  {
    key: "managerName",
    label: "Manager",
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
const showDialogCustomCreate = ref(false);
const isEdit = ref(false);
const userSelect = ref<UserModel>({
  userName: "",
  password: "",
  email: "",
  role: "",
  token: "",
  refreshToken: "",
  id: undefined,
  departmentName: "",
  departmentId: "",
  managerName: "",
  newPassword: "",
  isLocked: false
});
const basicAdminFormRef = ref<InstanceType<typeof BasicAdminFormVue> | null>(null);
const onCustomAction = (item: CustomActionResponse) => {
  if(item.Action.ActionName == "Create"){
    isEdit.value = false;
    showDialogCustomCreate.value = true;
  }
  if(item.Action.ActionName == "Edit"){
    isEdit.value = true;
    showDialogCustomCreate.value = true;
    userSelect.value = deepCopy(item.Data);
  }
  if(item.Action.ActionName == "Lock/Unlock"){
    ElMessageBox.confirm(
      "Are you sure you want to ' " + (item.Data.isLocked ? "Unlock" : "Lock") + " ' this user?",
      "Warning",
      {
        confirmButtonText: "OK",
        cancelButtonText: "Cancel",
        type: "warning",
      }
    )
      .then(() => {
        axiosInstance.put(`User/lock?userId=${item.Data.id}&isLock=${!item.Data.isLocked}`).then((rs) => {
          if(rs.data.isSuccess){
            ElMessage({
              type: "success",
              message: "Success",
            })
            // basicAdminFormRef.value?.Search;
            item.Data.isLocked = !item.Data.isLocked;
          }
          else{
            ElMessage({
              type: "error",
              message: rs.data.message,
            })
          }
        })
      })
      .catch(() => {
        ElMessage({
          type: "info",
          message: "Delete canceled",
        });
      });
  }
};
const onClose = () => {
  basicAdminFormRef.value?.Search();
  showDialogCustomCreate.value = false;
}
</script>
