<template>
  <div>
    <el-container>
      <el-header class="header-container">
        <div class="row">
          <div class="left-column">
            <NuxtLink class="link" to="/">
              <div class="logo">
                <el-image :src="'/vk-logo.svg'" fit="fill" style="width: 30px; height: 30px"></el-image>
                VK Analyzer
              </div>
            </NuxtLink>
          </div>
          <div class="right-column">
            <div class="user-name">{{username}}</div>
          </div>
        </div>
      </el-header>
      <div class="main-container">
          <div class="left-column">
              <Menu />
          </div>
          <div class="right-column">
              <slot/>
          </div>
      </div>
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

.row {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
}

.left-column {
  flex: 1
}

.right-column {
  flex: 4
}
</style>