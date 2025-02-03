<script setup lang="ts">
import {getIcon} from "~/utils/icons";

const props = defineProps({
  name: String,
  iconName: String,
})
</script>

<template>
  <div class="menu-item">
    <el-icon class="menu-item__icon"><component :is="getIcon(props.iconName)" /></el-icon>
    {{props.name}}
  </div>
</template>

<style scoped>
.menu-item {
  display: flex;
  flex-direction: row;
  gap: 5px;
  padding: 8px 0 8px 8px;
  align-items: center;
  border-radius: 5px;
  height: 100%;
  width: calc(100% - 16px);
  background-color: #ffffff;
  color: #000;

  &:hover {
    background-color: #d5d5d5;
    cursor: pointer;
  }
}

.menu-item__icon {
  color: #0077FF;
}
</style>