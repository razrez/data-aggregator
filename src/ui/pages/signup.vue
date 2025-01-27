<template>
  <div>
    <el-form
        size="large"
        label-position="top"
        label-width="auto"
        class="register-form">
      <<el-form-item label="Name">
      <el-input v-model="name"></el-input>
    </el-form-item>
      <el-form-item label="Login">
        <el-input v-model="username"></el-input>
      </el-form-item>
      <el-form-item label="Password">
        <el-input type="password" v-model="password"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleRegister">Sign up</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from 'vue';

export default defineComponent({
  name: 'SignUpPage',
  setup() {
    const name = ref('');
    const username = ref('');
    const password = ref('');

    const handleRegister = () => {
      //todo: регистрация
    };

    return {
      name,
      username,
      password,
      handleRegister,
    };
  },
});
</script>

<style scoped>
.register-form {
  width: 300px;
  margin: 20px auto;
}
</style>