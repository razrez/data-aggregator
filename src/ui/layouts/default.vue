<template>
  <div>
    <el-header>
      <el-row>
        <el-col :span="12">
          <NuxtLink class="link" to="/">
            <div class="logo">
              <el-image :src="'/vk-logo.svg'" fit="fill" style="width: 30px; height: 30px"></el-image>
              VK Analyzer
            </div>
          </NuxtLink>
        </el-col>
        <el-col :span="12">
          <div v-if="true" class="user">
            <NuxtLink class="link" to="signin">Sign in</NuxtLink>/
            <NuxtLink class="link" to="signup">Sign up</NuxtLink>
          </div>
          <div v-if="false" class="user">
            <NuxtLink class="link" to="settings">Settings</NuxtLink>
          </div>
        </el-col>
      </el-row>
    </el-header>
    <main>
      <slot />
    </main>
  </div>
</template>

<script setup lang="ts">


</script>

<style scoped>
.logo {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  gap: 10px;
  font-size: 22px;
  font-weight: bold;
}


.el-header {
  background-color: #ffffff;
  color: #333;
  line-height: 60px;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
}

.user {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  gap: 10px;
  font-weight: bold;
  font-size: 18px;
}
</style>