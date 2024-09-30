<template>
  <div class="common-layout">
    <div class="userName" @click="handleAsideClick('logout')">
      {{ Cookies.get("userName")?.toString() }}
    </div>
    <el-container>
      <el-header class="layout2_header" v-if="!isMobile">
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
          <el-menu-item index="2" @click="handleAsideClick('User')">
            <el-icon>
              <User />
            </el-icon>
            <span>User</span>
          </el-menu-item>
          <el-menu-item index="3" @click="handleAsideClick('Team')">
            <el-icon>
              <UserFilled />
              <UserFilled />
            </el-icon>
            <span>Team</span>
          </el-menu-item>
          <el-menu-item index="4" @click="handleAsideClick('Branch')">
            <el-icon><OfficeBuilding /></el-icon>
            <span>Branch</span>
          </el-menu-item>
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
</style>
