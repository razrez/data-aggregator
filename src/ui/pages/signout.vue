<script setup lang="ts">
const handleSignout = () => {
  deleteCookie("authToken")
  navigateTo('signin')
};

definePageMeta({
  layout: "auth"
});
</script>

<template>
  <div class="container">
    <div class="title">Are you sure you want to sign out?</div>
    <div class="buttons">
      <el-button color="#0077FF" @click="handleSignout">Sign out</el-button>
      <el-button color="#0077FF" @click="() => navigateTo('/')">Back to main</el-button>
    </div>
  </div>
</template>

<style scoped>
.container {
  width: 20%;
  margin: auto;
}

.title {
  color: black;
  font-weight: bold;
  font-size: 18px;
  padding: 15px;
}

.buttons {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
}
</style>
