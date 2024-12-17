<template>
  <div class="common-layout">
    <el-container>
      <el-aside width="175px" style="background-color: #a7dcff;">
        <div class="layout2-logo" @click="handleAsideClick('')">
          <img src="@/assets/logo2.png" alt="" >
        </div>
        <div class="menu">
          <div class="menu-top">
            <el-menu 
              :default-active="activeIndex" 
              class="el-menu-vertical-demo" 
            >
              <el-menu-item index="/"  @click="handleAsideClick('')">
                <el-icon><Aim /></el-icon>
                Goal
              </el-menu-item>
            </el-menu>
          </div>
          <div class="menu-bottom">
            <el-menu 
              :default-active="activeIndex" 
              class="el-menu-vertical-demo" 
            >
              <el-menu-item index="/Department"   @click="handleAsideClick('Department')" v-if="hasPermission(userRoles as string[], ['superadmin','Admin'])">
                <el-icon>
                  <Flag />
                </el-icon>
                <span>Department</span>
              </el-menu-item>
              <el-menu-item index="/user"  @click="handleAsideClick('User')" v-if="hasPermission(userRoles as string[], ['Admin','superadmin'])">
                <el-icon>
                  <User />
                </el-icon>
                <span>User</span>
              </el-menu-item>
            </el-menu>
          </div>
        </div>
        
        
      </el-aside>
      <el-container>
        <el-header class="layout2_header"  style="text-align: right;">
         <div class="toolbar">
            <el-dropdown>
              <el-icon style="margin-right: 8px; margin-top: 1px">
                <Setting />
              </el-icon>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item @click="showChangePassword = true">Change Password</el-dropdown-item>
                  <el-dropdown-item divided @click="handleAsideClick('logout')">Logout</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
            <span> {{ Cookies.get("userName")?.toString() }}</span>
          </div>
        </el-header>
        <el-main class="layout2-main"><el-scrollbar><router-view /></el-scrollbar></el-main>
      </el-container>
    </el-container>
  </div>
  <el-dialog v-model="showChangePassword" title="Change Password" @close="showChangePassword = false" width="350px">
    <div class="input-changepassword">
      <p>Current password</p>
      <el-input v-model="user.password" placeholder="current password"></el-input>
    </div>
    <div class="input-changepassword">
      <p>New password</p>
      <el-input v-model="user.newPassword" placeholder="new password"></el-input>
    </div>
    <div style="text-align: center;">
      <el-button type="primary" @click="handleChangePassword">Save</el-button>
    </div>
    
  </el-dialog>
</template>

<script setup lang="ts">
import {
  Expand,
  User,
  OfficeBuilding,
  UserFilled,
  Search,
  Flag,
  Setting
} from "@element-plus/icons-vue";
import { ref, onMounted, onUnmounted, watch, reactive } from "vue";
import Cookies from "js-cookie";
import type { UserModel } from "@/Models/UserModel";
import { axiosInstance } from "@/Service/axiosConfig";
import { hasPermission } from "@/components/maynghien/Common/handleRole";
import { ElMessage } from "element-plus";
import { Aim } from "@element-plus/icons-vue";
import { useRoute, useRouter } from "vue-router";

const route = useRoute();
const router = useRouter();
const isAsideVisible = ref(true);
const isMobile = ref(false);
const drawerMenuMobile = ref(false);
const userRoles = ref<string[]>();
const activeIndex = ref(route.path);
const handleAsideClick = (action: string) => {
  switch (action) {
    case "logout":
      logout();
      break;
    case "User":
      router.push("/user");
      break;
    default:
      router.push("/" + action);
      break;
  }
};
const searchUsername = ref("");
const listUser = ref<UserModel[]>([]);
const user = reactive<UserModel>({
  userName: "",
  password: "",
  email: "",
  role: undefined,
  token: undefined,
  refreshToken: "",
  id: undefined,
  departmentName: "",
  departmentId: undefined,
  managerName: "",
  newPassword: "",
  isLocked: false
})
const showChangePassword = ref(false);
const getRole = () => {
  var jsonString = Cookies.get('Roles')?.toString() ?? '';
  var jsonObject = JSON.parse(jsonString);
  var arrayFromString = Object.values(jsonObject);
  userRoles.value = arrayFromString as string[];
}
getRole();
const toggleAside = () => {
  if (isMobile.value) {
    drawerMenuMobile.value = !drawerMenuMobile.value;
  } else {
    isAsideVisible.value = !isAsideVisible.value;
  }
};

const handleResize = () => {
  isMobile.value = window.innerWidth < 600;
  if (!isMobile.value) {
    isAsideVisible.value = true;
  }
};

onMounted(() => {
  window.addEventListener("resize", handleResize);
  handleResize();
});

onUnmounted(() => {
  window.removeEventListener("resize", handleResize);
});

function logout() {
  var cookies = document.cookie.split(";");

  for (var i = 0; i < cookies.length; i++) {
    var cookie = cookies[i];
    var eqPos = cookie.indexOf("=");
    var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
    document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;";
  }
  router.push("/login");
}

const handleChangePassword = () => {
  user.userName = Cookies.get("userName")?.toString() ?? '';
  user.email = Cookies.get("userName")?.toString() ?? '';
  axiosInstance.put("User/change-password", user).then((res) => {
    if(res.data.isSuccess){
      showChangePassword.value = false;
      ElMessage({ message: "Change password success", type: "success" })
    }
    else{
      ElMessage({ message: res.data.message, type: "error" })
    }
  })
};
const loadpage = () => {
  const url = "/UserName=" + searchUsername.value;
  window.open(url, "_blank"); // Mở cửa sổ mới với URL
}
watch(() => searchUsername.value, () => {
  loadpage();
})
watch(() => route.path, (newPath) => {
  activeIndex.value = newPath;
});
</script>

<style>
.item-menu {
  background-color: #ddfff6;
  margin: 0px 10px;
  min-width: 130px !important;
  max-width: 150px !important;
  height: 30px;
  display: flex !important;
  align-items: center;
  justify-content: center;
  border-radius: 5px;
}

.common-layout {
  background: linear-gradient(199deg, #e6673966, #9585f43b, #c4c1c1a3);
  background-size: cover; /* Đảm bảo nền phủ toàn bộ khung */
  background-position: center center;
  background-attachment: fixed; /* Cố định hình nền */
  min-height: 100vh; /* Chiều cao tối thiểu luôn bằng chiều cao của màn hình */
  width: 100%; /* Đảm bảo chiều rộng luôn bằng chiều rộng của màn hình */
}
.layout2-row-header {
  display: flex !important;
  align-items: center;
  align-content: center;
  justify-content: center;
  gap: 10px;
}
.w-100 {
  width: 100% !important;
}
.layout2_header .toolbar {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  right: 20px;
}
.el-col:hover {
  cursor: pointer;
}
.menu{
  height: calc(100vh - 114px);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}
.layout2-main {
  height: calc(100vh - 60px); 
  overflow: hidden;
}
.layout2-search > i {
  padding: 5px;
  font-size: 24px;
}
.layout2-logo{
  text-align: center;
  padding: 10px;
}
.layout2-logo :hover {
  cursor: pointer;
}
.layout2-logo > img{
  height: 90px;
}
.input-changepassword {
    width: 300px;
    margin: 15px;
}
.el-menu-item.is-active {
  background-color: #f0f8ff !important; /* Màu nền khi active */
  color: #409EFF !important; /* Màu chữ khi active */
}
</style>
