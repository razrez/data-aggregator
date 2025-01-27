<template>
  <div id="app">
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'App',
});
</script>

<style>
/* Глобальные стили */
#app {
  font-family: Tahoma, Arial, Verdana, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #edeef0;
}
body {
  background-color: #e8e8e8 !important;
}

.link {
  text-decoration: none;
  color: #000;
}
</style>