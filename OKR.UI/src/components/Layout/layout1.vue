<template>
  <div class="common-layout">
    <el-container>
      <div id="aside">
        <el-aside v-if="!isMobile" v-show="isAsideVisible" class="aside-fixed">
          <el-row class="tac">
            <el-col>
              <el-menu default-active="1" class="el-menu-vertical-demo">
                <el-menu-item index="1" @click="handleAsideClick('logout')">
                  <el-icon>
                    <Close />
                  </el-icon>
                  <span>{{ Cookies.get("userName")?.toString() }}</span>
                </el-menu-item>
                <el-menu-item index="2" @click="handleAsideClick('User')">
                  <el-icon>
                    <User />
                  </el-icon>
                  <span>User</span>
                </el-menu-item>
                <el-menu-item index="3" @click="handleAsideClick('Team')">
                  <!-- <el-icon>
                    <User />
                  </el-icon> -->
                  <span>Team</span>
                </el-menu-item>
                <el-menu-item index="4" @click="handleAsideClick('Branch')">
                  <!-- <el-icon>
                    <User />
                  </el-icon> -->
                  <span>Branch</span>
                </el-menu-item>
              </el-menu>
            </el-col>
          </el-row>
        </el-aside>

        <el-drawer v-model="drawerMenuMobile" width="100%" direction="ltr">
          <el-row class="tac">
            <el-col>
              <el-menu default-active="1" class="el-menu-vertical-demo">
                <el-menu-item index="1" @click="handleAsideClick('logout')">
                  <el-icon>
                    <Close />
                  </el-icon>
                  <span>{{ Cookies.get("userName")?.toString() }}</span>
                </el-menu-item>
                <el-menu-item index="2" @click="handleAsideClick('User')">
                  <el-icon>
                    <User />
                  </el-icon>
                  <span>User</span>
                </el-menu-item>
              </el-menu>
            </el-col>
          </el-row>
        </el-drawer>
      </div>

      <el-container>
        <el-header v-if="isMobile">
          Header
          <el-button
            v-if="isMobile"
            class="toggle-button"
            type="primary"
            :icon="Expand"
            circle
            @click="toggleAside"
          />
        </el-header>
        <el-main class="scrollable-main">
          <el-scrollbar>
            <router-view />
          </el-scrollbar>
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from "vue";
import router from "@/router";
import Cookies from "js-cookie";
import { Close, Expand, User } from "@element-plus/icons-vue";

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
      router.push("/" + action)
      break;
  }
};

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
.el-aside {
  background-color: rgb(5, 51, 111);
  height: 100vh;
}

.toggle-button {
  display: none;
}
@media (min-width: 600px) {
  .toggle-button {
    display: none;
  }
}
@media (max-width: 600px) {
  .toggle-button {
    display: block;
  }
  .el-drawer.ltr {
    width: 80% !important;
  }
  .el-aside {
    width: 100%;
  }
  .el-drawer__body {
    padding: 0 !important;
  }
}

</style>
<style scoped>
.common-layout {
  height: 100vh;
}

.el-container {
  height: 100%;
}

#aside {
  height: 100%;
}

.el-aside {
  height: 100%;
}



.el-main {
  height: 100%;
  overflow: hidden;
}

.scrollable-main {
  overflow-y: auto;
}
</style>