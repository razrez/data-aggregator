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

<script lang="ts">
import { defineComponent, ref } from 'vue';

export default defineComponent({
  name: 'SignInPage',
  setup() {
    const username = ref('');
    const password = ref('');

    const handleLogin = () => {
      //todo: авторизация
    };

    return {
      username,
      password,
      handleLogin,
    };
  },
});
</script>

<style scoped>
.login-form {
  width: 300px;
  margin: 20px auto;
}
</style>