<template>
  <div class="common-layout">
    <el-container>
      <el-aside width="200px">
        <el-row class="tac">
          <el-col :span="30">
            <el-menu default-active="1" class="el-menu-vertical-demo">
              <el-menu-item index="8" @click="handleAsideClick('logout')">
                <el-icon>
                  <Close />
                </el-icon>
                <span>Logout</span>
              </el-menu-item>
            </el-menu>
          </el-col>
        </el-row>
      </el-aside>
      <el-container>
        <el-header>Header</el-header>
        <el-main> <router-view /></el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script setup lang="ts">
import router from "@/router";

const handleAsideClick = (action: string) => {
  switch (action) {
    case "logout":
        logout()
      break;

    default:
      break;
  }
};
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
