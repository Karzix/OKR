<template>
    <el-dialog :model-value="props.openDialog" :title="(props.isEdit ? 'Edit ' : 'Create ')" class="form-dialog" @close="emit('onClose')">

        <div class="editform" >
            <div >
                <div >
                    <label>Email</label>
                    <el-input v-model="User.email" />
                </div>
                <div >
                    <label>Role</label>
                    <el-select v-model="User.role" placeholder="Select">
                        <el-option v-for="item in roles" :key="item.role" :label="item.roleName" :value="item.role" />
                    </el-select>
                </div>
                <div >
                    <label>Department</label>
                    <el-select v-model="User.departmentId" placeholder="Select">
                        <el-option v-for="item in department" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </div>
                <div>
                    <label>Manager</label>
                    <el-select
                        v-model="User.managerName"
                        clearable
                        placeholder="User Name"
                        filterable
                        remote
                        :remote-method="seachUser"
                        >
                        <el-option
                            v-for="item in listUser"
                            :key="item.userName"
                            :label="item.userName"
                            :value="item.userName"
                        />
                    </el-select>
                </div>
            </div>

        </div>
        <template #footer>
            <span class="dialog-footer">
                <el-button @click="() => emit('onClose')">Cancel</el-button>
                <el-button type="primary" @click="Save">
                    Confirm
                </el-button>
            </span>
        </template>
    </el-dialog>
</template>
<script setup lang="ts">
import type { Department } from '@/Models/Department';
import { UserModel } from '@/Models/UserModel';
import { axiosInstance } from '@/Service/axiosConfig';
import { ElMessage } from 'element-plus';
import { onMounted, ref } from 'vue';


const isEdit = ref(false);
const openDialog = ref(false);
const User = ref<UserModel>({
    userName: "",
    password: undefined,
    email: "",
    role: "",
    token: "",
    refreshToken: "",
    id: undefined,
    departmentName: "",
    departmentId: "",
    managerName: "",
})
const props = defineProps<{
    openDialog: Boolean,
    isEdit: Boolean,
    User: UserModel
}>();
const emit = defineEmits<{
    (e: 'onClose'): void;

}>()
var roles = ref([{
    role: "Admin",
    roleName: "Admin",
},{
    role: "Teamleader",
    roleName: "Teamleader",
},{
    role: "Employee",
    roleName: "Employee",
}]);
const department = ref<Department[]>([]);
const listUser = ref<UserModel[]>([]);


const loadDepartment = async () => {
    axiosInstance.get("Department").then((res) => {
        department.value = res.data.data
    })
}
const seachUser = async (query: string) => {
  const url = "User/list-by-keyword/" + query;
  await axiosInstance.get(url).then((res) => {
    listUser.value = res.data.data
  })
}

const Save = () => {
    if (props.isEdit) {
        axiosInstance.put("User", User.value).then((res) => {
            if (res.data.isSuccess) {
                // props.openDialog = false
                emit('onClose')
                console.log(res.data.message)
                ElMessage({ message: "Edit success", type: "success" })
            } else {
                console.log(res.data.message)
                ElMessage({ message: res.data.message, type: "error" })
            }
        })
    } else {
        axiosInstance.post("User", User.value).then((res) => {
            if (res.data.isSuccess) {
                // openDialog.value = false
                emit('onClose')
                console.log(res.data.message)
                ElMessage({ message: "Create success", type: "success" })
            } else {
                console.log(res.data.message)
                ElMessage({ message: res.data.message, type: "error" })
            }
        })
    }
}

onMounted(() => {
    loadDepartment()
    if(props.isEdit && props.User){
        User.value = props.User
    }
})

</script>

<style scoped>
/* Dialog styling */
.form-dialog {
    border-radius: 8px; /* Rounded corners for the dialog */
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); /* Soft shadow for depth */
}

/* Header styling */
.form-dialog .el-dialog__header {
    background-color: #f5f5f5; /* Light background for header */
    color: #333; /* Dark text color for contrast */
    padding: 15px; /* Padding for the header */
    border-bottom: 1px solid #e0e0e0; /* Subtle bottom border */
}

/* Input field styling */
.editform {
    padding: 15px; /* Padding inside the form */
}

/* Label styling */
.editform label {
    display: block; /* Make label block for better spacing */
    margin-bottom: 5px; /* Space between label and input */
    font-weight: bold; /* Bold text for emphasis */
    color: #555; /* Darker color for readability */
}

/* Input and select styling */
.el-input,
.el-select {
    width: 100%; /* Full width for inputs */
    margin-bottom: 15px; /* Space below each input */
    border-radius: 4px; /* Rounded corners for inputs */
    border: 1px solid #ccc; /* Light border color */
    transition: border-color 0.2s; /* Smooth transition for border color */
}

/* Focus effect for inputs */
.el-input:focus,
.el-select:focus {
    border-color: #3498db; /* Change border color on focus */
    box-shadow: 0 0 5px rgba(52, 152, 219, 0.5); /* Add shadow on focus */
}

/* Tree view styling */
.tree-view {
    margin-top: 10px; /* Space above the tree view */
}

/* Dialog footer styling */
.dialog-footer {
    display: flex;
    justify-content: flex-end; /* Align buttons to the right */
    padding: 10px; /* Padding for the footer */
    background-color: #f9f9f9; /* Light background for footer */
    border-top: 1px solid #e0e0e0; /* Subtle top border */
}

/* Button styling */
.el-button {
    margin-left: 10px; /* Space between buttons */
    border-radius: 4px; /* Rounded corners */
}

/* Responsive adjustments */
@media (max-width: 768px) {
    /* Adjust dialog width on small screens */
    .form-dialog {
        width: 90vw; /* Use viewport width for small screens */
    }

    /* Adjust padding and font size for inputs */
    .el-input,
    .el-select {
        font-size: 14px; /* Smaller font for mobile */
        padding: 10px; /* Less padding for inputs */
    }

    /* Adjust button size */
    .el-button {
        font-size: 14px; /* Smaller button text */
    }
}
</style>