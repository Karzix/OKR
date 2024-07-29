<template>
  <div class="Content">
    <el-form
      ref="ruleFormRef"
      :model="state"
      status-icon
      label-width="px"
      class="demo-ruleForm"
      @keyup.enter="login"
    >
      <el-form-item label="" prop="username">
        <el-input v-model="state.email" placeholder="User name" />
      </el-form-item>

      <el-form-item label="" prop="pass">
        <el-input
          v-model="state.password"
          type="password"
          placeholder="PassWord"
          autocomplete="off"
        />
      </el-form-item>

      <el-form-item>
        <el-button type="primary" @click="login()">Sign In</el-button>
      </el-form-item>
    </el-form>

    <el-dialog v-model="dialogVisible" title="Error">
      <p>Invalid login information</p>
      <el-button type="primary" @click="dialogVisible = false">OK</el-button>
    </el-dialog>
  </div>
</template>
<script setup lang="ts">
import { Calendar, Search, User, Key } from "@element-plus/icons-vue";
import { reactive, ref } from "vue";

import { UserModel } from "../../Models/UserModel";
import { handleLogin } from "../../Service/Auth/handleLogin";
import { LoginModel } from "../../Models/LoginModel";

//const _toast = useToast();
const state = reactive<LoginModel>({
  email: "",
  password: "",
  twoFactorCode: "",
  twoFactorRecoveryCode: "",
});
import { ElMessageBox } from "element-plus";
import router from "@/router";

const dialogVisible = ref(false);

const login = async () => {
  const loginResult = await handleLogin(state);
  if (!loginResult) {
    //delete cookie
    var cookies = document.cookie.split(";");
    for (var i = 0; i < cookies.length; i++) {
      var cookie = cookies[i];
      var eqPos = cookie.indexOf("=");
      var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
      document.cookie =
        name + "=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;";
    }
    dialogVisible.value = true;
  } else {
    router.push("/");
  }
};
</script>
<style>
/* .el-input{
        height:30px;
      } */
.el-form-item {
  width: 500px;
}

/* .el-form{
        margin: 145px;
      } */
.Content {
  width: 100%;
  height: 550px;
  background-color: white;
  display: flex;
  justify-content: center;
  align-items: center;
}

main.el-main {
  background-color: white;
}
body {
  margin: 0px;
}
</style>
