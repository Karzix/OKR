<template>
  <div class="common-layout">
    <div class="userName" @click="handleAsideClick('logout')">
      {{ Cookies.get("userName")?.toString() }}
    </div>
    <el-container>
      <el-header class="layout2_header" v-if="!isMobile">
        <el-row :gutter="20" class="w-100 layout2-row-header">
          <el-col :span="6" class="item-menu" @click="handleAsideClick('User')" v-if="hasPermission(userRoles as string[], ['Admin','superadmin'])">
            <el-icon>
              <User />
            </el-icon>
            <span>User</span>
          </el-col>
          <el-col :span="6" class="item-menu" @click="handleAsideClick('Department')" v-if="hasPermission(userRoles as string[], ['superadmin','Admin'])">
            <el-icon>
              <Flag />
            </el-icon>
            <span>Department</span>
          </el-col>
          <!-- <el-col
            v-if="hasPermission(userRoles as string[], ['Admin','superadmin'])"
            :span="6"
            class="item-menu"
            @click="handleAsideClick('Branch')"
          >
            <el-icon>
              <OfficeBuilding />
            </el-icon>
            <span>Branch</span>
          </el-col> -->
          <!-- <el-col :span="6" class="layout2-search">
            <el-select
              v-model="searchUsername"
              clearable
              placeholder="User Name"
              style="width: 190px"
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
            <el-icon><Search /></el-icon>
          </el-col> -->
        </el-row>
      </el-header>
      <el-header class="mobile-header" v-if="isMobile">
        <div class="mobile-header-content">
          <el-button
            class="toggle-button"
            type="primary"
            :icon="Expand"
            circle
            @click="toggleAside"
          />
          <span class="mobile-header-title">Menu</span>
        </div>
      </el-header>
      <el-main class="layout2-main"><router-view /></el-main>
    </el-container>
  </div>

  <el-drawer v-model="drawerMenuMobile" width="100%" direction="ltr">
    <el-row class="tac">
      <el-col>
        <el-menu default-active="1" class="el-menu-vertical-demo">
          <el-menu-item index="2" @click="handleAsideClick('User')" v-if="hasPermission(userRoles as string[], ['Admin','superadmin'])">
            <el-icon>
              <User />
            </el-icon>
            <span>User</span>
          </el-menu-item>
          <el-menu-item index="3" @click="handleAsideClick('Department')" v-if="hasPermission(userRoles as string[], ['Admin','superadmin'])">
            <el-icon>
              <Flag />
            </el-icon>
            <span>Team</span>
          </el-menu-item>
          <!-- <el-menu-item index="4" @click="handleAsideClick('Branch')" v-if="hasPermission(userRoles as string[], ['Admin','superadmin'])">
            <el-icon><OfficeBuilding /></el-icon>
            <span>Branch</span>
          </el-menu-item> -->
          <!-- <el-menu-item index="5" >
            <el-select
              v-model="searchUsername"
              clearable
              placeholder="User Name"
              style="width: 190px"
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
            <el-icon><Search /></el-icon>
          </el-menu-item> -->
        </el-menu>
      </el-col>

    </el-row>
  </el-drawer>
</template>

<script setup lang="ts">
import router from "@/router";
import {
  Expand,
  User,
  OfficeBuilding,
  UserFilled,
  Search,
  Flag,
} from "@element-plus/icons-vue";
import { ref, onMounted, onUnmounted, watch } from "vue";
import Cookies from "js-cookie";
import type { UserModel } from "@/Models/UserModel";
import { axiosInstance } from "@/Service/axiosConfig";
import { hasPermission } from "@/components/maynghien/Common/handleRole";

const isAsideVisible = ref(true);
const isMobile = ref(false);
const drawerMenuMobile = ref(false);
const userRoles = ref<string[]>();
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
  window.location.reload();
}

const seachUser = async (query: string) => {
  const url = "User/list-by-keyworld/" + query;
  await axiosInstance.get(url).then((res) => {
    listUser.value = res.data.data
  })
}
const loadpage = () => {
  const url = "/UserName=" + searchUsername.value;
  window.open(url, "_blank"); // Mở cửa sổ mới với URL
}
watch(() => searchUsername.value, () => {
  loadpage();
})
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
  border-radius: 10px;
}
.common-layout {
  background-image: url("../../assets/web.png");
  background-repeat: no-repeat;
  background-size: cover; /* Đảm bảo hình nền phủ toàn bộ khung */
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
.layout2_header {
  background-color: #00ffff99;
  display: flex !important;
  align-items: center;
  justify-content: center;
  margin-top: 20px;
  width: 80%;
  border-radius: 15px;
  min-height: 60px !important;
}
.mobile-header {
  background-color: #00ffff99;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 20px;
  border-radius: 0;
}
.mobile-header-content {
  display: flex;
  align-items: center;
}
.mobile-header-title {
  font-weight: bold;
  font-size: 18px;
  margin-left: 10px;
}
.el-col:hover {
  cursor: pointer;
}
.userName {
  position: fixed;
  background-color: #0000ffad;
  right: 0;
  color: white;
  padding: 12px;
  border-radius: 0 0 0 10px;
  z-index: 999;
}
.layout2-main {
  background-color: #ffffff00 !important;

}
.layout2-search > i {
  padding: 5px;
  font-size: 24px;
}
.layout2-search{
    display: flex !important;
    align-items: center;
    justify-content: center;
}
.layout2-search-mobile{

}
</style>
