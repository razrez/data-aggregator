<template>
  <div>
    <el-form
        size="large"
        label-position="top"
        label-width="auto"
        class="login-form">
      <el-form-item label="Login">
        <el-input v-model="username"></el-input>
      </el-form-item>
      <el-form-item label="Password">
        <el-input type="password" v-model="password"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button color="#0077FF" @click="handleLogin">Sign in</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { defineComponent, ref } from 'vue';
import { setCookie} from "~/utils/cookies";

const username = ref('');
const password = ref('');

const handleLogin = async () => {
  const response = await httpPost('/auth/signin', { Username: username.value, Password: password.value });
  setCookie('authToken', response.token, response.experationDate);
  navigateTo("/");
};

definePageMeta({
  layout: "auth"
});
</script>

<style scoped>
.login-form {
  width: 300px;
  margin: 20px auto;
}
</style>