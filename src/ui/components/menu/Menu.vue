<script setup lang="ts">
import {MenuItems} from "~/navigation/MenuItems";

const onClick = (route: string) => {
  navigateTo(route);
}
</script>

<template>
<div class="menu">
  <MenuItem
      v-for="menuItem of MenuItems"
      :key="menuItem.id"
      :name="menuItem.name"
      :icon-name="menuItem.iconName"
      @click="() => onClick(menuItem.route)"
  />
</div>
</template>

<style scoped>
.menu {
  display: flex;
  flex-direction: column;
  align-items: start;
}
</style>