<script setup>
import { ref, onMounted } from "vue";

// Dữ liệu tin tức giả
const newsArticles = ref([]);
const isLoading = ref(true);

// Bài viết đang được chọn để xem chi tiết
const selectedArticle = ref(null);

// Hàm xem chi tiết bài viết
function viewDetail(article) {
  selectedArticle.value = article;
  window.scrollTo({ top: 0, behavior: "smooth" });
}

// Giả lập dữ liệu
const mockNews = [
  {
    id: 1,
    title: "POLY ra mắt bộ sưu tập Hè 2025",
    summary: "Đón chào mùa hè với BST mới đầy năng động và trẻ trung từ POLY",
    content:
      "Hè 2025 đánh dấu sự trở lại mạnh mẽ của POLY với bộ sưu tập mới đầy sáng tạo. Lấy cảm hứng từ những chuyến phiêu lưu mùa hè, BST mang đến những thiết kế tối giản nhưng không kém phần ấn tượng với gam màu tươi sáng, họa tiết độc đáo và chất liệu thoáng mát.",
    image: "https://cdn.brvn.vn/editor/2023/07/A42_333084-fashion-show-6_1688631273.jpg",
    date: "2024-05-15",
    author: "Admin POLY",
  },
  {
    id: 2,
    title: "Phong cách Y2K đang trở lại",
    summary: "Xu hướng Y2K đang làm mưa làm gió trong làng thời trang",
    content:
      "Phong cách Y2K đình đám những năm 2000 đang quay trở lại mạnh mẽ trong làng thời trang hiện đại. POLY đã nhanh chóng nắm bắt xu hướng này với loạt sản phẩm mới kết hợp giữa hoài niệm Y2K và phong cách đường phố hiện đại. Bạn có thể tìm thấy những chiếc áo crop top, quần baggy, và phụ kiện đậm chất Y2K trong BST mới nhất của chúng tôi.",
    image: "https://thieuhoa.com.vn/wp-content/uploads/2023/03/BfFzk1NZAuPK0bZMRFLFgS0vIfxHWls0Wne7C4PS.webp",
    date: "2024-05-10",
    author: "StylePOLY",
  },
  {
    id: 3,
    title: "POLY hợp tác với nghệ sĩ local",
    summary: "Dự án hợp tác độc quyền với các nghệ sĩ đường phố Việt Nam",
    content:
      "Với mong muốn quảng bá văn hóa đường phố Việt Nam, POLY vừa công bố dự án hợp tác với 5 nghệ sĩ graffiti nổi tiếng trong nước. Các thiết kế độc quyền này sẽ được in lên những sản phẩm giới hạn của POLY, hứa hẹn mang đến làn gió mới cho thời trang đường phố Việt Nam.",
    image: "https://img.vietcetera.com/uploads/images/09-oct-2020/untitled-1.jpg",
    date: "2024-04-28",
    author: "Creative Team",
  },
  {
    id: 4,
    title: "POLY cam kết thời trang bền vững",
    summary:
      "Chuyển đổi quy trình sản xuất sang hướng thân thiện với môi trường",
    content:
      "Hưởng ứng làn sóng thời trang bền vững toàn cầu, POLY đã cam kết chuyển đổi 50% quy trình sản xuất sang sử dụng vật liệu thân thiện với môi trường trước năm 2025.",
    image: "https://btnmt.1cdn.vn/2020/06/28/tui-vai-bao-ve-moi-truong.jpg",
    date: "2024-04-15",
    author: "Green Team",
  },
  {
    id: 5,
    title: "Mở rộng cửa hàng POLY tại Cầu Giấy",
    summary: "Cửa hàng thứ 3 của POLY sẽ khai trương vào tháng 7/2024",
    content:
      "Tiếp nối thành công của hai cửa hàng tại Đống Đa và Hà Đông, POLY chính thức công bố kế hoạch mở rộng hệ thống với cửa hàng thứ 3 tại Đà Nẵng.",
    image:
      "https://images.squarespace-cdn.com/content/v1/591fd77d29687fd09cca478b/1555546030336-YXVPG30KTCM92JW89UTL/ke17ZwdGBToddI8pDm48kDrQ9tfdcvPUv7NgXGP4R2R7gQa3H78H3Y0txjaiv_0fDoOvxcdMmMKkDsyUqMSsMWxHk725yiiHCCLfrh8O1z4YTzHvnKhyp6Da-NYroOW3ZGjoBKy3azqku80C789l0gmXcXvEVFTLbYX9CdVcGe4zwrosjp5YtnrvbmlM1LFKb7wNXE8lRZ0Z8l5PIsW3Vw/AdobeStock_139559217.jpeg",
    date: "2024-03-30",
    author: "Marketing POLY",
  },
];

// Tải dữ liệu khi component được mount
onMounted(async () => {
  await new Promise((resolve) => setTimeout(resolve, 1000));
  newsArticles.value = mockNews;
  isLoading.value = false;
});

// Format ngày
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

    <!-- CHỖ HIỂN THỊ CHI TIẾT BÀI VIẾT -->
    <div v-if="selectedArticle" class="article-detail-box">
      <button class="back-btn" @click="selectedArticle = null">
        ⬅ Quay lại danh sách
      </button>

      <h1 class="detail-title">{{ selectedArticle.title }}</h1>

      <div class="detail-info">
        <span class="date">{{ formatDate(selectedArticle.date) }}</span>
        <span class="author">• {{ selectedArticle.author }}</span>
      </div>

      <div class="detail-image">
        <img :src="selectedArticle.image" style="width: 1200px;"/>
      </div>

      <p class="detail-summary">{{ selectedArticle.summary }}</p>

      <div class="detail-content">
        {{ selectedArticle.content }}
      </div>
    </div>

    <!-- NẾU KHÔNG ĐANG XEM CHI TIẾT -->
    <div class="news-content" v-else>
      <!-- FEATURED ARTICLE -->
      <div
        class="featured-article"
        v-if="newsArticles.length > 0"
        @click="viewDetail(newsArticles[0])"
        style="cursor:pointer"
      >
        <div class="featured-image">
          <img :src="newsArticles[0].image" :alt="newsArticles[0].title" />
        </div>
        <div class="featured-content">
          <span class="article-date">{{
            formatDate(newsArticles[0].date)
          }}</span>
          <h2 class="article-title">{{ newsArticles[0].title }}</h2>
          <p class="article-summary">{{ newsArticles[0].summary }}</p>
          <p class="article-text">{{ newsArticles[0].content }}</p>
          <div class="article-author">
            <i class="bi bi-person-circle"></i>
            <span>{{ newsArticles[0].author }}</span>
          </div>
        </div>
      </div>

      <!-- RECENT ARTICLES -->
      <div class="recent-articles">
        <h3 class="section-title">Bài viết gần đây</h3>

        <div class="articles-grid">
          <div
            v-for="article in newsArticles.slice(1)"
            :key="article.id"
            class="article-card"
            @click="viewDetail(article)"
            style="cursor: pointer"
          >
            <div class="article-image">
              <img :src="article.image" :alt="article.title" />
            </div>
            <div class="article-card-content">
              <span class="article-date">{{ formatDate(article.date) }}</span>
              <h3 class="article-card-title">{{ article.title }}</h3>
              <p class="article-card-summary">{{ article.summary }}</p>
              <div class="article-author">
                <i class="bi bi-person-circle"></i>
                <span>{{ article.author }}</span>
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

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 40px 0;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #000;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

.news-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

/* Featured article */
.featured-article {
  display: flex;
  flex-direction: column;
  gap: 30px;
  margin-bottom: 60px;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
  overflow: hidden;
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
}

.article-date {
  display: block;
  font-size: 14px;
  color: #666;
  margin-bottom: 10px;
}

.article-title {
  font-size: 24px;
  margin-bottom: 15px;
  font-weight: 600;
}

.article-summary {
  font-size: 16px;
  margin-bottom: 20px;
  font-weight: 500;
  color: #333;
}

.article-text {
  font-size: 16px;
  line-height: 1.6;
  color: #444;
  margin-bottom: 20px;
}

.article-author {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #666;
  font-size: 14px;
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

