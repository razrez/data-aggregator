<template>
  <div>
    <el-container>
      <el-header class="header-container">
        <el-row>
          <el-col :span="4">
            <NuxtLink class="link" to="/">
              <div class="logo">
                <el-image :src="'/vk-logo.svg'" fit="fill" style="width: 30px; height: 30px"></el-image>
                VK Analyzer
              </div>
            </NuxtLink>
          </el-col>
          <el-col :span="16">
            <div class="user-name">{{username}}</div>
          </el-col>
        </el-row>
      </el-header>
      <el-container class="main-container">
        <el-row>
          <el-col :span="4">
              <Menu />
          </el-col>
          <el-col :span="16">
              <slot/>
          </el-col>
        </el-row>
      </el-container>
    </el-container>
  </div>
</template>

<script setup lang="ts">

import {httpGet} from "~/utils/api";

const username = ref('');

onMounted(async () => {
  username.value = await httpGet("/auth/user/name", {});
})
</script>

<style scoped>
.logo {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: start;
  gap: 10px;
  font-size: 22px;
  font-weight: bold;
}

.el-header {
  background-color: #ffffff;
  color: #333;
  line-height: 60px;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(255, 255, 255, 0.2);
}

.user-name {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: end;
  gap: 10px;
  font-weight: bold;
  font-size: 18px;
}

.header-container {
  padding: 0 20%;
}

.main-container {
  padding: 20px 20%;
}
</style>