<template>
  <div ref="scrollContainer" class="scroll-container">
    <el-row :gutter="20">
      <el-col :span="24" v-for="post in posts" :key="post.id" style="margin-bottom: 20px;">
        <el-card v-for="post in posts" :key="post.id" class="post-card">
          <h2>{{ post.groupName }}</h2>
          <p>{{ post.text }}</p>
          <div class="post-footer">
            <div class="post-actions">
              <span><el-icon><ElementPlus /></el-icon> {{ post.likes }}</span>
              <span><el-icon><ChatDotSquare /></el-icon> {{ post.views }}</span>
            </div>
            <div class="post-date">{{ formatDate(post.date) }}</div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { ElCard } from 'element-plus';
import {httpGet} from "~/utils/api";

interface Post {
  id: number;
  groupName: string;
  text: string;
  likes: number;
  views: number;
  date: Date;
}

const scrollContainer = ref<HTMLElement | null>(null);
const posts = ref<Post[]>([]);
const loading = ref(false);
let page = 1;
const pageSize = 10;

// Метод для загрузки данных
const loadPosts = async () => {
  if (loading.value) return;

  loading.value = true;

  // Имитация запроса к серверу
  const newPosts = await fetchPosts(page, pageSize);

  posts.value = [...posts.value, ...newPosts];
  page += 1;
  loading.value = false;
};

// Имитация асинхронного запроса к серверу
const fetchPosts = async (page: number, size: number) => {
  const data = await httpGet(`/posts?limit=${size}&offset=${page*size}`, {});
  posts.value = data.map((post) => ({
    id: post.id,
    groupName: post.groupName,
    text: post.text,
    likes: post.likes,
    comments: 0,
    shares: 0,
    date: post.date,
  }));
};

// Обработчик события прокрутки
const handleScroll = () => {
  const container = scrollContainer.value;
  if (
      container &&
      container.scrollTop + container.clientHeight >= container.scrollHeight - 10 &&
      !loading.value
  ) {
    loadPosts();
  }
};

// Форматирование даты
const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('ru-RU', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  }).format(date);
};

// Установка и удаление обработчиков событий
onMounted(() => {
  loadPosts(); // Загрузка первоначальных данных
  if (scrollContainer.value) {
    scrollContainer.value.addEventListener('scroll', handleScroll);
  }
});

onUnmounted(() => {
  if (scrollContainer.value) {
    scrollContainer.value.removeEventListener('scroll', handleScroll);
  }
});
</script>

<style scoped>
.scroll-container {
  height: 85vh;
  width: 100%;
  overflow-y: auto;
  display: flex;
  padding-right: 10px;
  flex-direction: column;
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

.scroll-container::-webkit-scrollbar {
  width: 8px; /* Ширина скроллбара */
  height: 8px; /* Высота скроллбара */
}

.scroll-container::-webkit-scrollbar-thumb {
  background-color: #a0a0a0; /* Цвет полосы прокрутки */
  border-radius: 10px; /* Закругление углов */
}

.scroll-container::-webkit-scrollbar-track {
  background-color: transparent; /* Прозрачный фон */
}
</style>