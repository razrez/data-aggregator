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
  flex-direction: row;
  gap: 5px;
  padding: 5px 5px;
  align-items: start;
  background-color: white;
  border-radius: 12px;
  margin-bottom: 15px;
}
</style>