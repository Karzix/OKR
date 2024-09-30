<template>
  <div class="Content">
    <el-form
      ref="ruleFormRef"
      :model="state"
      status-icon
      label-width="100px"
      class="demo-ruleForm"
      @keyup.enter="login"
    >
      <el-form-item label="Username" prop="username" class="login-formitem">
        <el-input v-model="state.email" placeholder="User name" autofocus class="login-input"/>
      </el-form-item>

      <el-form-item label="Password" prop="pass" class="login-formitem">
        <el-input
          v-model="state.password"
          type="password"
          placeholder="Password"
          autocomplete="off"
          show-password
          class="login-input"
        />
      </el-form-item>

      <el-form-item class="login-formitem">
        <el-button type="primary" @click="login()" class="login-btn">Sign In</el-button>
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
<style scoped>
.Content {
  width: 500px;
  height: 320px;
  background-color: white;
  display: flex;
  justify-content: center;
  align-items: center;
  margin: auto;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  padding: 20px;
  border-radius: 15px;
  box-shadow: -10px -10px 20px rgba(0, 0, 0, 0.3);
}
</style>
<style>
.login-formitem {
  width: 100%;
}

.login-btn {
  width: 100%;
}
.login-input{
  min-width: 300px !important;
}
</style>