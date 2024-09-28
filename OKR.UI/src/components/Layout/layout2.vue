<template>
  <div class="common-layout">
    <div class="userName" @click="handleAsideClick('logout')">
      {{ Cookies.get("userName")?.toString() }}
    </div>
    <el-container>
      <el-header class="layout2_header">
        <el-row :gutter="20" class="w-100 layout2-row-header">
          <el-col :span="6" class="item-menu" @click="handleAsideClick('User')">
            <el-icon>
              <User />
            </el-icon>
            <span>User</span>
          </el-col>
          <el-col :span="6" class="item-menu" @click="handleAsideClick('Team')">
            <el-icon>
              <UserFilled />
              <UserFilled />
            </el-icon>
            <span>Team</span>
          </el-col>
          <el-col
            :span="6"
            class="item-menu"
            @click="handleAsideClick('Branch')"
          >
            <el-icon>
              <OfficeBuilding />
            </el-icon>
            <span>Branch</span>
          </el-col>
          <el-col :span="6" class="layout2-search">
            <el-select
              v-model="searchUsername"
              clearable
              placeholder="User Name"
              style="width: 190px"
              filterable
              remote
            >
              <el-option
                v-for="item in options"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
            <el-icon><Search /></el-icon>
          </el-col>
        </el-row>
      </el-header>
      <el-main class="layout2-main"><router-view /></el-main>
    </el-container>
  </div>
</template>
<script setup lang="ts">
import router from "@/router";
import {
  Close,
  Expand,
  User,
  OfficeBuilding,
  UserFilled,
  Search,
} from "@element-plus/icons-vue";
import { ref, onMounted, onUnmounted } from "vue";
import Cookies from "js-cookie";

const isAsideVisible = ref(true);
const isMobile = ref(false);
const drawerMenuMobile = ref(false);

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
const options = [
  {
    value: "Option1",
    label: "Option1",
  },
  {
    value: "Option2",
    label: "Option2",
  },
  {
    value: "Option3",
    label: "Option3",
  },
];
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
  console.log(isAsideVisible.value);
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
  background-size: cover;
  background-position: center center;
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
  align-content: center;
  justify-content: center;
  margin-top: 20px;
  margin-left: auto;
  margin-right: auto;
  width: 80%;
  border-radius: 15px;
  height: 100% !important;
  min-height: 60px !important;
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
}
.layout2-main {
  background-color: #ffffff00 !important;
}
.layout2-search {
  display: flex !important;
  align-items: center;
  justify-content: center;
  min-width: 190px !important;
  max-width: 200px !important;
}
.layout2-search > i {
    padding: 5px;
    font-size: 24px;
}
</style>
