<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";

const route = useRoute();
const router = useRouter();

// ID bài viết lấy từ URL
const postId = route.params.id;

// State
const article = ref(null);
const isLoading = ref(true);

// Gọi API lấy chi tiết bài viết
onMounted(async () => {
  try {
    const res = await fetch(`https://localhost:7070/api/posts/${postId}`);
    article.value = await res.json();
  } catch (err) {
    console.error("Lỗi tải bài viết:", err);
  }
  isLoading.value = false;
});

// Format ngày giống trang list
function formatDate(dateString) {
  const date = new Date(dateString);
  return date.toLocaleDateString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}
</script>

<template>
  <div class="detail-page">
    <!-- Loading -->
    <div class="loading" v-if="isLoading">
      <div class="spinner"></div>
      <p>Đang tải bài viết...</p>
    </div>

    <!-- Nội dung bài viết -->
    <div class="article-detail" v-else-if="article">
      <button class="back-btn" @click="router.push('/')">⬅ Quay lại</button>

      <h1 class="title">{{ article.title }}</h1>

      <div class="meta">
        <span class="date">{{ formatDate(article.date) }}</span>
        <span class="author">• {{ article.author }}</span>
      </div>

      <div class="image">
        <img :src="article.image" :alt="article.title" />
      </div>

      <p class="summary">{{ article.summary }}</p>

      <div class="content">
        {{ article.content }}
      </div>
    </div>

    <!-- Khi không tìm thấy bài viết -->
    <div v-else class="not-found">
      <p>Không tìm thấy bài viết.</p>
      <button @click="router.push('/')">Quay lại trang chủ</button>
    </div>
  </div>
</template>

<style scoped>
.detail-page {
  max-width: 900px;
  margin: 0 auto;
  padding: 40px 20px;
}

.back-btn {
  background-color: transparent;
  border: none;
  font-size: 16px;
  margin-bottom: 20px;
  cursor: pointer;
}

.title {
  font-size: 28px;
  font-weight: bold;
  margin-bottom: 15px;
}

.meta {
  font-size: 14px;
  color: #666;
  margin-bottom: 20px;
}

.image {
  width: 70%;
  height: auto;
  margin: 0 auto 30px;
}

.image img {
  width: 100%;
  height: auto;
  object-fit: cover;
}

.summary {
  font-weight: 600;
  font-size: 18px;
  margin-bottom: 20px;
}

.content {
  font-size: 16px;
  line-height: 1.7;
  color: #444;
}

.not-found {
  text-align: center;
  margin-top: 50px;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #ddd;
  border-top: 4px solid #000;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 15px;
}

@keyframes spin {
  0% { transform: rotate(0); }
  100% { transform: rotate(360deg); }
}
</style>
