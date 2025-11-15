<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();
const newsArticles = ref([]);
const isLoading = ref(true);
const error = ref(null);

const fetchNews = async () => {
  try {
    console.log('🔄 Đang gọi API News...');
    const response = await fetch('https://localhost:7055/api/News');
    
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    
    const data = await response.json();
    console.log('✅ Data nhận được:', data);
    
    // Xử lý response object/array
    let articles = [];
    if (Array.isArray(data)) {
      articles = data;
    } else if (typeof data === 'object' && data !== null) {
      if (data.$values && Array.isArray(data.$values)) {
        articles = data.$values;
      } else {
        articles = [data];
      }
    }
    
    newsArticles.value = articles;
    error.value = null;
    
  } catch (err) {
    console.error('❌ Lỗi:', err);
    error.value = err.message;
    newsArticles.value = [];
  } finally {
    isLoading.value = false;
  }
};

//Dùng router để điều hướng sang trang chi tiết
function viewDetail(article) {
  router.push(`/news/${article.id}`);
}

onMounted(() => {
  fetchNews();
});

function formatDate(dateString) {
  if (!dateString) return '';
  try {
    return new Date(dateString).toLocaleDateString("vi-VN");
  } catch {
    return 'Invalid Date';
  }
}
</script>

<template>
  <div class="news-page">
    <div class="news-header">
      <h1>TIN TỨC & BÀI VIẾT</h1>
      <p class="subtitle">Cập nhật những tin tức mới nhất từ POLY</p>
    </div>

    <!-- Loading -->
    <div class="loading-container" v-if="isLoading">
      <div class="spinner"></div>
      <p>Đang tải tin tức...</p>
    </div>

    <!-- Error -->
    <div v-if="error && !isLoading" class="error-container">
      <p>⚠️ {{ error }}</p>
    </div>

    <!-- No Data -->
    <div v-if="!isLoading && newsArticles.length === 0" class="no-data">
      <p>📭 Không có bài viết nào</p>
    </div>

    <div class="news-content" v-if="!isLoading && newsArticles.length > 0">
      <!-- Featured Article -->
      <div 
        class="featured-article"
        @click="viewDetail(newsArticles[0])"
      >
        <div class="featured-image">
          <img 
            :src="newsArticles[0].image || 'https://via.placeholder.com/600x400'" 
            :alt="newsArticles[0].title || 'No title'"
          />
        </div>
        <div class="featured-content">
          <span class="article-date">
            {{ formatDate(newsArticles[0].date) }}
          </span>
          <h2>{{ newsArticles[0].title || 'Không có tiêu đề' }}</h2>
          <p class="summary">{{ newsArticles[0].summary || 'Không có mô tả' }}</p>
          <div class="article-author">
            <i class="bi bi-person-circle"></i>
            <span>{{ newsArticles[0].author || 'Không rõ tác giả' }}</span>
          </div>
          <button class="read-more-btn">Đọc tiếp →</button>
        </div>
      </div>

      <!-- Recent Articles -->
      <div class="recent-articles" v-if="newsArticles.length > 1">
        <h3 class="section-title">Bài viết gần đây</h3>

        <div class="articles-grid">
          <div
            v-for="article in newsArticles.slice(1)"
            :key="article.id"
            class="article-card"
            @click="viewDetail(article)"
          >
            <div class="article-image">
              <img 
                :src="article.image || 'https://via.placeholder.com/350x200'" 
                :alt="article.title || 'No title'"
              />
            </div>
            <div class="article-card-content">
              <span class="article-date">{{ formatDate(article.date) }}</span>
              <h3 class="article-card-title">{{ article.title || 'Không có tiêu đề' }}</h3>
              <p class="article-card-summary">{{ article.summary || 'Không có mô tả' }}</p>
              <div class="article-author">
                <i class="bi bi-person-circle"></i>
                <span>{{ article.author || 'Không rõ tác giả' }}</span>
              </div>
              <button class="read-more-btn">Đọc tiếp →</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- NEWSLETTER -->
    <div class="newsletter-section">
      <div class="newsletter-content">
        <h3>Đăng ký nhận tin</h3>
        <p>Nhận thông tin về bộ sưu tập mới và khuyến mãi từ POLY</p>
        <form class="newsletter-form">
          <input type="email" placeholder="Email của bạn" />
          <button type="submit" class="subscribe-btn">Đăng ký</button>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.news-page {
  padding: 40px 0;
}

.news-header {
  text-align: center;
  margin-bottom: 40px;
}

.news-header h1 {
  font-size: 28px;
  margin-bottom: 10px;
  font-weight: 600;
}

.subtitle {
  color: #666;
  font-size: 16px;
}

.loading-container, .error-container, .no-data {
  text-align: center;
  padding: 40px 0;
  color: #666;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #000;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 15px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.news-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

/* Featured article */
.featured-article {
  display: flex;
  gap: 30px;
  margin-bottom: 60px;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
  overflow: hidden;
  cursor: pointer;
  transition: transform 0.2s ease;
}

.featured-article:hover {
  transform: translateY(-5px);
}

.featured-image {
  width: 100%;
  height: 400px;
}

.featured-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.featured-content {
  padding: 30px;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.article-date {
  display: block;
  font-size: 14px;
  color: #666;
  margin-bottom: 10px;
}

.featured-content h2 {
  font-size: 24px;
  margin-bottom: 15px;
  font-weight: 600;
}

.summary {
  font-size: 16px;
  margin-bottom: 20px;
  font-weight: 500;
  color: #333;
}

.article-author {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #666;
  font-size: 14px;
  margin-bottom: 15px;
}

.read-more-btn {
  background: #000;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  align-self: flex-start;
  transition: background 0.2s;
}

.read-more-btn:hover {
  background: #333;
}

/* Recent articles */
.section-title {
  font-size: 22px;
  margin-bottom: 30px;
  font-weight: 600;
}

.articles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 30px;
  margin-bottom: 60px;
}

.article-card {
  background-color: #fff;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
  transition: transform 0.2s ease;
  cursor: pointer;
}

.article-card:hover {
  transform: translateY(-5px);
}

.article-image {
  height: 200px;
}

.article-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.article-card-content {
  padding: 20px;
}

.article-card-title {
  font-size: 18px;
  margin-bottom: 10px;
  font-weight: 600;
}

.article-card-summary {
  font-size: 14px;
  margin-bottom: 15px;
  color: #444;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Newsletter section */
.newsletter-section {
  background-color: #f7f7f7;
  padding: 50px 0;
  margin: 60px -20px 0;
}

.newsletter-content {
  max-width: 600px;
  margin: 0 auto;
  text-align: center;
  padding: 0 20px;
}

.newsletter-content h3 {
  font-size: 22px;
  margin-bottom: 15px;
  font-weight: 600;
}

.newsletter-content p {
  color: #666;
  margin-bottom: 25px;
}

.newsletter-form {
  display: flex;
  max-width: 450px;
  margin: 0 auto;
}

.newsletter-form input {
  flex: 1;
  padding: 12px 15px;
  border: 1px solid #ddd;
  border-radius: 4px 0 0 4px;
  font-size: 16px;
}

.subscribe-btn {
  background-color: #000;
  color: white;
  border: none;
  padding: 0 25px;
  border-radius: 0 4px 4px 0;
  cursor: pointer;
  font-size: 16px;
}

@media (min-width: 768px) {
  .featured-article {
    flex-direction: row;
  }

  .featured-image {
    width: 50%;
  }
  
  .featured-content {
    width: 50%;
  }
}

@media (max-width: 767px) {
  .featured-image {
    height: 250px;
  }

  .articles-grid {
    grid-template-columns: 1fr;
  }

  .newsletter-form {
    flex-direction: column;
    gap: 10px;
  }

  .newsletter-form input,
  .subscribe-btn {
    width: 100%;
    border-radius: 4px;
  }
}
</style>