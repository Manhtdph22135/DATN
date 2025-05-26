<script setup>
import { ref, onMounted } from "vue";

// State
const isLoading = ref(true);
const featuredProducts = ref([]);
const trending = ref([]);
const bestsellers = ref([]);

// Mock data
const mockFeaturedProducts = [
  {
    id: 1,
    name: "Áo thun Signature Tee",
    price: 320000,
    image: "https://media1.thehungryjpeg.com/thumbs/800_4262428_zzultqdefwrzpkyb25nsmpzexo4h46xkmpbv2rmb.png",
    views: 1520,
    sold: 389,
  },
  {
    id: 2,
    name: "Quần cargo Two-tone",
    price: 590000,
    image: "https://bizweb.dktcdn.net/thumb/1024x1024/100/451/385/products/hinh00955-copy-1684995399598.jpg?v=1684996025080",
    views: 1280,
    sold: 256,
  },
  {
    id: 3,
    name: "Áo khoác Varsity",
    price: 1200000,
    image: "https://dosi-in.com/images/detailed/526/dosiin-icon-denim-ao-khoac-varsity-icondenim-526901526901.jpg",
    views: 2100,
    sold: 187,
  },
  {
    id: 4,
    name: "Áo sơ mi Minimalist",
    price: 450000,
    image: "https://product.hstatic.net/1000360022/product/ao-so-mi-linen-nam-tay-ngan-minimal-collection-form-regular__7__96ae3e35f57049438841a8a8459c336a.jpg",
    views: 980,
    sold: 205,
  },
];

const mockTrending = [
  {
    id: 5,
    name: "Áo thun Graphic Tee",
    price: 350000,
    image: "https://down-vn.img.susercontent.com/file/vn-11134207-7qukw-lgdg80aizilz45",
    badgeText: "NEW",
  },
  {
    id: 6,
    name: "Quần jean Straight fit",
    price: 480000,
    image: "https://vn-test-11.slatic.net/p/9bb2a97169e7673623ade19ccafeaff3.jpg",
    badgeText: "HOT",
  },
  {
    id: 7,
    name: "Áo hoodie Basic",
    price: 550000,
    image: "https://saigonsneaker.com/wp-content/uploads/2021/10/Ao-khoac-hoodie-flexible-toi-gian-xanh-reu-9.jpg",
    badgeText: "NEW",
  },
  {
    id: 8,
    name: "Áo polo Stripe",
    price: 390000,
    image: "https://4men.com.vn/thumbs/2022/08/ao-polo-slimfit-stripe-color-po083-mau-den-21295-p.jpg",
    badgeText: "HOT",
  },
  {
    id: 9,
    name: "Quần short Relaxed",
    price: 320000,
    image: "https://product.hstatic.net/1000026602/product/img_6152_fb1b7d91184f437b9f517401fdaf091a_master.jpg",
    badgeText: "NEW",
  },
  {
    id: 10,
    name: "Áo khoác Bomber",
    price: 850000,
    image: "https://product.hstatic.net/1000360022/product/untitled-1__2__218c890a6c9c406a966b4ab805530b28.jpg",
    badgeText: "HOT",
  },
];

const mockBestsellers = [
  {
    id: 11,
    name: "Áo thun Oversize",
    price: 290000,
    image: "https://dosi-in.com/images/detailed/41/lnc_tr%C6%A1n_3.png",
    rank: 1,
  },
  {
    id: 12,
    name: "Quần jean Slim",
    price: 450000,
    image: "https://jeanthuanhai.com/wp-content/uploads/2023/07/z4450182316466-d5ba23a0926f883115ddd2acdf3909d6.jpg",
    rank: 2,
  },
  {
    id: 13,
    name: "Áo sơ mi Cuban",
    price: 420000,
    image: "https://product.hstatic.net/1000026602/product/dsc07958_d95ce0e4b1bd4bfc9b15636103f11454.jpg",
    rank: 3,
  },
  {
    id: 14,
    name: "Áo thun Basic",
    price: 280000,
    image: "https://dosi-in.com/images/detailed/42/CDL11_1.jpg",
    rank: 4,
  },
  {
    id: 15,
    name: "Quần jogger Cotton",
    price: 390000,
    image: "https://cf.shopee.vn/file/6d2258714d6226cd3b7247534ea229c1",
    rank: 5,
  },
];

// Format tiền VND
function formatCurrency(price) {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    minimumFractionDigits: 0,
  }).format(price);
}

// Format số
function formatNumber(number) {
  return new Intl.NumberFormat("vi-VN").format(number);
}

// Load data khi component được mount
onMounted(async () => {
  await new Promise((resolve) => setTimeout(resolve, 800));
  featuredProducts.value = mockFeaturedProducts;
  trending.value = mockTrending;
  bestsellers.value = mockBestsellers;
  isLoading.value = false;
});
</script>

<template>
  <div class="hot-products-page">
    <!-- Featured Banner -->
    <div class="featured-banner">
      <div class="banner-content">
        <h1>HOT & TRENDING</h1>
        <p class="subtitle">Khám phá những sản phẩm nổi bật nhất từ POLY</p>
      </div>
    </div>

    <!-- Loading State -->
    <div class="loading-container" v-if="isLoading">
      <div class="spinner"></div>
      <p>Đang tải sản phẩm...</p>
    </div>

    <div class="page-content" v-else>
      <!-- Featured Products -->
      <section class="section featured-products">
        <div class="section-header">
          <h2 class="section-title">ĐƯỢC QUAN TÂM NHIỀU NHẤT</h2>
          <p class="section-description">
            Sản phẩm được xem và mua nhiều nhất tháng này
          </p>
        </div>

        <div class="featured-grid">
          <div
            v-for="product in featuredProducts"
            :key="product.id"
            class="featured-product"
          >
            <div class="product-image">
              <img :src="product.image" :alt="product.name" />
            </div>
            <div class="product-info">
              <h3 class="product-name">{{ product.name }}</h3>
              <div class="product-price">
                {{ formatCurrency(product.price) }}
              </div>

              <div class="product-stats">
                <div class="stat">
                  <i class="bi bi-eye"></i>
                  <span>{{ formatNumber(product.views) }} lượt xem</span>
                </div>
                <div class="stat">
                  <i class="bi bi-bag-check"></i>
                  <span>{{ formatNumber(product.sold) }} đã bán</span>
                </div>
              </div>

              <button class="view-product-btn">XEM SẢN PHẨM</button>
            </div>
          </div>
        </div>
      </section>

      <!-- Trending Products -->
      <section class="section trending-section">
        <div class="section-header">
          <h2 class="section-title">SẢN PHẨM TRENDING</h2>
          <p class="section-description">
            Những sản phẩm đang được săn đón nhiều nhất
          </p>
        </div>

        <div class="trending-grid">
          <div
            v-for="product in trending"
            :key="product.id"
            class="trending-product"
          >
            <div class="product-image">
              <img :src="product.image" :alt="product.name" />
              <span
                class="badge"
                :class="product.badgeText === 'NEW' ? 'badge-new' : 'badge-hot'"
              >
                {{ product.badgeText }}
              </span>
            </div>

            <div class="product-details">
              <h3 class="product-name">{{ product.name }}</h3>
              <div class="product-price">
                {{ formatCurrency(product.price) }}
              </div>

              <div class="product-actions">
                <button class="add-to-cart">Thêm vào giỏ</button>
                <button class="quick-view">
                  <i class="bi bi-eye"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Bestsellers -->
      <section class="section bestsellers-section">
        <div class="section-header">
          <h2 class="section-title">TOP SẢN PHẨM BÁN CHẠY</h2>
          <p class="section-description">
            Top 5 sản phẩm bán chạy nhất trong tháng
          </p>
        </div>

        <div class="bestsellers-list">
          <div
            v-for="product in bestsellers"
            :key="product.id"
            class="bestseller-item"
          >
            <div class="rank-badge" :class="`rank-${product.rank}`">
              {{ product.rank }}
            </div>

            <div class="bestseller-image">
              <img :src="product.image" :alt="product.name" />
            </div>

            <div class="bestseller-info">
              <h3 class="product-name">{{ product.name }}</h3>
              <div class="product-price">
                {{ formatCurrency(product.price) }}
              </div>
              <button class="view-product-btn small">XEM NGAY</button>
            </div>
          </div>
        </div>
      </section>

      <!-- Email Signup -->
      <section class="section email-signup">
        <div class="signup-content">
          <h2>NHẬN THÔNG BÁO VỀ SẢN PHẨM MỚI</h2>
          <p>
            Đăng ký để nhận thông tin về các sản phẩm mới và ưu đãi đặc biệt
          </p>

          <div class="signup-form">
            <input type="email" placeholder="Email của bạn" />
            <button>ĐĂNG KÝ</button>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<style scoped>
.hot-products-page {
  padding-bottom: 40px;
}

.featured-banner {
  height: 200px;
  background-color: #000;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 40px;
}

.banner-content {
  text-align: center;
  color: white;
}

.banner-content h1 {
  font-size: 36px;
  margin-bottom: 10px;
  font-weight: 700;
}

.subtitle {
  font-size: 16px;
  opacity: 0.8;
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

.page-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

/* Section styles */
.section {
  margin-bottom: 60px;
}

.section-header {
  text-align: center;
  margin-bottom: 30px;
}

.section-title {
  font-size: 24px;
  margin-bottom: 10px;
  font-weight: 600;
}

.section-description {
  color: #666;
  margin-bottom: 20px;
}

/* Featured products */
.featured-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 30px;
}

.featured-product {
  display: flex;
  background-color: #fff;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.05);
  transition: transform 0.3s;
}

.featured-product:hover {
  transform: translateY(-5px);
}

.featured-product .product-image {
  flex: 0 0 40%;
  overflow: hidden;
}

.featured-product img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s;
}

.featured-product:hover img {
  transform: scale(1.05);
}

.product-info {
  flex: 1;
  padding: 20px;
  display: flex;
  flex-direction: column;
}

.product-name {
  font-size: 18px;
  margin-bottom: 10px;
  font-weight: 600;
}

.product-price {
  color: #000;
  font-weight: 700;
  font-size: 18px;
  margin-bottom: 15px;
}

.product-stats {
  display: flex;
  flex-direction: column;
  margin-bottom: 20px;
  gap: 8px;
}

.stat {
  display: flex;
  align-items: center;
  color: #666;
  font-size: 14px;
}

.stat i {
  margin-right: 8px;
  font-size: 16px;
}

.view-product-btn {
  margin-top: auto;
  background-color: #000;
  color: white;
  border: none;
  padding: 12px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s;
}

.view-product-btn:hover {
  background-color: #333;
}

.view-product-btn.small {
  padding: 8px 15px;
  font-size: 12px;
}

/* Trending products */
.trending-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.trending-product {
  background-color: #fff;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
  transition: transform 0.2s;
}

.trending-product:hover {
  transform: translateY(-5px);
}

.trending-product .product-image {
  position: relative;
  aspect-ratio: 3/4;
  overflow: hidden;
}

.trending-product img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s;
}

.trending-product:hover img {
  transform: scale(1.05);
}

.badge {
  position: absolute;
  top: 10px;
  left: 10px;
  padding: 5px 10px;
  color: white;
  font-size: 12px;
  font-weight: 600;
  border-radius: 3px;
}

.badge-new {
  background-color: #000;
}

.badge-hot {
  background-color: #e53637;
}

.product-details {
  padding: 15px;
}

.product-actions {
  display: flex;
  gap: 10px;
  margin-top: 10px;
  opacity: 0;
  transform: translateY(20px);
  transition: all 0.3s;
}

.trending-product:hover .product-actions {
  opacity: 1;
  transform: translateY(0);
}

.add-to-cart {
  flex: 1;
  background-color: #000;
  color: white;
  border: none;
  padding: 10px 0;
  font-size: 14px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.add-to-cart:hover {
  background-color: #333;
}

.quick-view {
  width: 40px;
  height: 40px;
  background-color: #fff;
  border: 1px solid #ddd;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: background-color 0.2s;
}

.quick-view:hover {
  background-color: #f5f5f5;
}

/* Bestsellers */
.bestsellers-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.bestseller-item {
  display: flex;
  align-items: center;
  background-color: #fff;
  border-radius: 8px;
  padding: 15px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  transition: transform 0.2s;
}

.bestseller-item:hover {
  transform: translateX(5px);
}

.rank-badge {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 16px;
  margin-right: 15px;
  color: white;
}

.rank-1 {
  background-color: #ffd700; /* Gold */
}

.rank-2 {
  background-color: #c0c0c0; /* Silver */
}

.rank-3 {
  background-color: #cd7f32; /* Bronze */
}

.rank-4,
.rank-5 {
  background-color: #666;
}

.bestseller-image {
  width: 80px;
  height: 80px;
  margin-right: 15px;
}

.bestseller-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 4px;
}

.bestseller-info {
  flex: 1;
  display: flex;
  flex-wrap: wrap;
  align-items: center;
}

.bestseller-info .product-name {
  width: 40%;
  margin-bottom: 0;
  margin-right: 10px;
  font-size: 16px;
}

.bestseller-info .product-price {
  width: 20%;
  margin-bottom: 0;
  text-align: right;
  font-size: 16px;
}

.bestseller-info .view-product-btn {
  margin-left: auto;
}

/* Email signup */
.email-signup {
  background-color: #f9f9f9;
  padding: 40px;
  border-radius: 8px;
  text-align: center;
}

.signup-content h2 {
  font-size: 24px;
  margin-bottom: 15px;
}

.signup-content p {
  color: #666;
  margin-bottom: 25px;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

.signup-form {
  display: flex;
  max-width: 500px;
  margin: 0 auto;
}

.signup-form input {
  flex: 1;
  padding: 15px;
  border: 1px solid #ddd;
  border-right: none;
  border-radius: 4px 0 0 4px;
  font-size: 16px;
}

.signup-form button {
  padding: 15px 25px;
  background-color: #000;
  color: white;
  border: none;
  border-radius: 0 4px 4px 0;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.signup-form button:hover {
  background-color: #333;
}

@media (max-width: 992px) {
  .featured-grid {
    grid-template-columns: 1fr;
  }

  .trending-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .bestseller-info .product-name,
  .bestseller-info .product-price {
    width: 100%;
    margin-bottom: 10px;
    text-align: left;
  }

  .bestseller-info .view-product-btn {
    margin-left: 0;
  }

  .signup-form {
    flex-direction: column;
    gap: 10px;
  }

  .signup-form input,
  .signup-form button {
    width: 100%;
    border-radius: 4px;
  }
}

@media (max-width: 576px) {
  .trending-grid {
    grid-template-columns: 1fr;
  }

  .featured-product {
    flex-direction: column;
  }

  .featured-product .product-image {
    flex: 0 0 200px;
  }

  .product-actions {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
