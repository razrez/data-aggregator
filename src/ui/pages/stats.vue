<script setup lang="ts">
import {ElCard} from "element-plus";
import type {Post} from "~/models/Post";
import {formatDate} from "../utils/date";


const mostLiked = ref<Post>({
  id: 0,
  public_name: "",
  text: "",
  likes: 0,
  views: 0,
  date: 1,
});
const mostViewed = ref<Post>({
  id: 0,
  public_name: "",
  text: "",
  likes: 0,
  views: 0,
  date: 1,
});

const mostLikedFrom = ref<Date>(new Date(2025, 0, 1));
const mostLikedTo = ref<Date>(new Date(2025, 11, 31));
const mostViewedFrom = ref<Date>(new Date(2025, 0, 1));
const mostViewedTo = ref<Date>(new Date(2025, 11, 31));


const setMostLiked = async () => {
  const data =
      await httpGet(`analytics/topLiked?startDate=${mostLikedFrom.value.toISOString()}&endDate=${mostLikedTo.value.toISOString()}&count=1&sortBy=likes`, {});
  mostLiked.value = data[0];
}

const setMostViewedFrom = async () => {
  const data =
      await httpGet(`analytics/topLiked/?startDate=${mostViewedFrom.value.toISOString()}&endDate=${mostViewedTo.value.toISOString()}&count=1&sortBy=views`, {});

  mostViewed.value = data[0];
}

watch(() => ({ mostLikedFrom, mostLikedTo}), async () => await setMostLiked(), { deep: true });

watch(() => ({ mostViewedFrom, mostViewedTo}), async () => await setMostViewedFrom(), { deep: true })

onMounted( async () => {
  await setMostLiked();
  await setMostViewedFrom();
})
</script>

<template>
  <div class="container">
    <div class="stat likes">
      <div class="title">
        Most liked
        <div class="period">
          <el-date-picker
              v-model="mostLikedFrom"
              type="date"
              placeholder="From"
              size="large"
          />
          -
          <el-date-picker
              v-model="mostLikedTo"
              type="date"
              placeholder="To"
              size="large"
          />
        </div>
      </div>
      <el-card class="post-card">
        <h2>{{ mostLiked?.public_name }}</h2>
        <p>{{ mostLiked?.text }}</p>
        <div class="post-footer">
          <div class="post-actions">
            <span><el-icon><ElementPlus /></el-icon> {{ mostLiked?.likes }}</span>
            <span><el-icon><ChatDotSquare /></el-icon> {{ mostLiked?.views }}</span>
          </div>
          <div class="post-date">{{ formatDate(new Date(mostLiked.date * 1000)) }}</div>
        </div>
      </el-card>
    </div>
    <div class="stat views">
      <div class="title">
        Most viewed
        <div class="period">
          <el-date-picker
              v-model="mostViewedFrom"
              type="date"
              placeholder="From"
              size="large"
          />
          -
          <el-date-picker
              v-model="mostViewedTo"
              type="date"
              placeholder="To"
              size="large"
          />
        </div>
      </div>
      <el-card class="post-card">
        <h2>{{ mostViewed?.public_name }}</h2>
        <p>{{ mostViewed?.text }}</p>
        <div class="post-footer">
          <div class="post-actions">
            <span><el-icon><ElementPlus /></el-icon> {{ mostViewed?.likes }}</span>
            <span><el-icon><ChatDotSquare /></el-icon> {{ mostViewed?.views }}</span>
          </div>
          <div class="post-date"> {{ formatDate(new Date(mostViewed.date * 1000))}}</div>
        </div>
      </el-card>
    </div>
  </div>
</template>

<style scoped>
.container {
  width: 100%;

}

.stat {
  color: black;
}

.post-card {
  margin-bottom: 20px;
  border-radius: 12px;
}

.post-card h2 {
  font-weight: bold;
  margin: 0 0 10px;
}

.post-card p {
  margin: 0 0 10px;
  text-align: start;
}

.post-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.post-actions span {
  margin-right: 15px;
}

.title {
  font-weight: bold;
  font-size: 24px;
  margin-bottom: 12px;
  text-align: left;
}
</style>