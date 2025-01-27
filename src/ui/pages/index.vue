<template>
  <div>
    <div class="title">Posts</div>
    <el-row :gutter="20">
      <el-col :span="24" v-for="post in posts" :key="post.id" style="margin-bottom: 20px;">
        <el-card class="post-card">
          <div slot="header" class="clearfix">
            <div class="group-name">{{ post.authorName }}</div>
          </div>
          <p>{{ post.text.slice(0, 100) }}...</p>
          <div class="post-stats">
            <el-tooltip content="Likes" placement="top">
              <div>
                <el-icon><ElementPlus /></el-icon> {{ post.likes }}
              </div>
            </el-tooltip>
            <el-tooltip content="Commnets" placement="top">
              <div>
                <el-icon><ChatDotSquare /></el-icon> {{ post.comments }}
              </div>
            </el-tooltip>
            <el-tooltip content="Share" placement="top">
              <div>
                <el-icon><Promotion /></el-icon> {{ post.reposts }}
              </div>
            </el-tooltip>
          </div>
        </el-card>
      </el-col>
    </el-row>
    <el-pagination
        layout="prev, pager, next"
        :total="totalPosts"
        :current-page="currentPage"
        :page-size="pageSize"
        @current-change="handlePageChange"
    ></el-pagination>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from 'vue';
import axios from 'axios';
import {ChatDotSquare, Comment, ElementPlus, Promotion} from "@element-plus/icons-vue";

interface Post {
  id: number;
  authorName: string;
  text: string;
  likes: number;
  comments: number;
  reposts: number;
}

export default defineComponent({
  name: 'PostsPage',
  components: {ElementPlus, Promotion, ChatDotSquare, Comment},
  setup() {
    const posts = ref<Post[]>([]);
    const totalPosts = ref(0);
    const currentPage = ref(1);
    const pageSize = ref(5);
    const mockPosts = [
      {
        id: 1,
        authorName: 'Группы 1',
        text: 'Это текст поста номер 1. Он содержит некоторую информацию...',
        likes: 5,
        comments: 3,
        reposts: 2,
      },
      {
        id: 2,
        authorName: 'Группы 2',
        text: 'Это текст поста номер 2. Он также содержит некоторую информацию...',
        likes: 8,
        comments: 4,
        reposts: 1,
      },
      {
        id: 1,
        authorName: 'Группы 1',
        text: 'Это текст поста номер 1. Он содержит некоторую информацию...',
        likes: 5,
        comments: 3,
        reposts: 2,
      },
      {
        id: 2,
        authorName: 'Группы 2',
        text: 'Это текст поста номер 2. Он также содержит некоторую информацию...',
        likes: 8,
        comments: 4,
        reposts: 1,
      },
      {
        id: 1,
        authorName: 'Группы 1',
        text: 'Это текст поста номер 1. Он содержит некоторую информацию...',
        likes: 5,
        comments: 3,
        reposts: 2,
      },
      {
        id: 2,
        authorName: 'Группы 2',
        text: 'Это текст поста номер 2. Он также содержит некоторую информацию...',
        likes: 8,
        comments: 4,
        reposts: 1,
      },
      {
        id: 1,
        authorName: 'Группы 1',
        text: 'Это текст поста номер 1. Он содержит некоторую информацию...',
        likes: 5,
        comments: 3,
        reposts: 2,
      },
      {
        id: 2,
        authorName: 'Группы 2',
        text: 'Это текст поста номер 2. Он также содержит некоторую информацию...',
        likes: 8,
        comments: 4,
        reposts: 1,
      },
    ];

    const mockFetchPosts = async (page: number, size: number) => {
      try {
        const start = (page - 1) * size;
        const end = start + size;
        const paginatedPosts = mockPosts.slice(start, end);
        posts.value = paginatedPosts;

        totalPosts.value = mockPosts.length;
      } catch (error) {
        console.error('Ошибка при получении постов:', error);
      }
    };

    const fetchPosts = async (page: number, size: number) => {
      try {
        const response = await axios.get('https://localhost:5000/api/posts', {
          params: {
            page,
            size,
          },
        });
        posts.value = response.data.posts;
        totalPosts.value = response.data.total;
      } catch (error) {
        console.error('Ошибка при получении постов:', error);
      }
    };

    const handlePageChange = (newPage: number) => {
      currentPage.value = newPage;
      mockFetchPosts(newPage, pageSize.value);
    };

    onMounted(() => {
      mockFetchPosts(currentPage.value, pageSize.value);
    });

    return {
      posts,
      totalPosts,
      currentPage,
      pageSize,
      handlePageChange,
    };
  },
});
</script>

<style scoped>
.post-card {
  width: 100%;
  border: 14px;
  border-radius: 10px !important;
}

.post-stats {
  display: flex;
  justify-content: space-around;
  margin-top: 10px;
}

.post-stats .el-button {
  padding: 0;
}

.group-name {
  font-size: 18px;
  font-weight: bold
}

.title {
  color: black;
  font-weight: bold !important;
  font-size: 24px !important;
  margin: 15px;
  text-align: start
}
</style>