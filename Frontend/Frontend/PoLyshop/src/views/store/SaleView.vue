<script setup>
import { ref, computed, onMounted } from "vue";

const saleProducts = ref([]);
const isLoading = ref(true);
const selectedCategories = ref([]);
const selectedPriceRange = ref([0, 2000000]);
const sortBy = ref("discount"); // discount, priceAsc, priceDesc

// Các nhóm sản phẩm
const categories = [
  { id: "tops", name: "Áo" },
  { id: "bottoms", name: "Quần" },
  { id: "accessories", name: "Phụ kiện" },
];

// Mock data cho các sản phẩm khuyến mãi
const mockSaleProducts = [
  {
    id: 1,
    name: "Áo thun POLY Basic",
    price: 280000,
    originalPrice: 350000,
    discount: 20,
    image: "https://placehold.co/300x400",
    category: "tops",
  },
  {
    id: 2,
    name: "Quần jean slim fit",
    price: 450000,
    originalPrice: 550000,
    discount: 18,
    image: "https://placehold.co/300x400",
    category: "bottoms",
  },
  {
    id: 3,
    name: "Áo hoodie nỉ Classic",
    price: 550000,
    originalPrice: 650000,
    discount: 15,
    image: "https://placehold.co/300x400",
    category: "tops",
  },
  {
    id: 4,
    name: "Quần short denim",
    price: 350000,
    originalPrice: 420000,
    discount: 17,
    image: "https://placehold.co/300x400",
    category: "bottoms",
  },
  {
    id: 5,
    name: "Túi đeo chéo mini",
    price: 240000,
    originalPrice: 320000,
    discount: 25,
    image: "https://placehold.co/300x400",
    category: "accessories",
  },
  {
    id: 6,
    name: "Áo khoác Bomber",
    price: 750000,
    originalPrice: 950000,
    discount: 21,
    image: "https://placehold.co/300x400",
    category: "tops",
  },
  {
    id: 7,
    name: "Quần cargo túi hộp",
    price: 550000,
    originalPrice: 650000,
    discount: 15,
    image: "https://placehold.co/300x400",
    category: "bottoms",
  },
  {
    id: 8,
    name: "Nón bucket",
    price: 180000,
    originalPrice: 240000,
    discount: 25,
    image: "https://placehold.co/300x400",
    category: "accessories",
  },
  {
    id: 9,
    name: "Áo sơ mi cộc tay",
    price: 320000,
    originalPrice: 380000,
    discount: 16,
    image: "https://placehold.co/300x400",
    category: "tops",
  },
  {
    id: 10,
    name: "Quần short kaki",
    price: 320000,
    originalPrice: 380000,
    discount: 16,
    image: "https://placehold.co/300x400",
    category: "bottoms",
  },
  {
    id: 11,
    name: "Vớ cổ cao",
    price: 60000,
    originalPrice: 90000,
    discount: 33,
    image: "https://placehold.co/300x400",
    category: "accessories",
  },
  {
    id: 12,
    name: "Áo polo trơn",
    price: 290000,
    originalPrice: 380000,
    discount: 24,
    image: "https://placehold.co/300x400",
    category: "tops",
  },
];

// Tải sản phẩm khi component được mount
onMounted(async () => {
  await new Promise((resolve) => setTimeout(resolve, 800));
  saleProducts.value = mockSaleProducts;
  isLoading.value = false;
});

// Lọc sản phẩm
const filteredProducts = computed(() => {
  let result = [...saleProducts.value];

  // Lọc theo danh mục
  if (selectedCategories.value.length > 0) {
    result = result.filter((product) =>
      selectedCategories.value.includes(product.category)
    );
  }

  // Lọc theo khoảng giá
  result = result.filter(
    (product) =>
      product.price >= selectedPriceRange.value[0] &&
      product.price <= selectedPriceRange.value[1]
  );

  // Sắp xếp
  if (sortBy.value === "discount") {
    result.sort((a, b) => b.discount - a.discount);
  } else if (sortBy.value === "priceAsc") {
    result.sort((a, b) => a.price - b.price);
  } else if (sortBy.value === "priceDesc") {
    result.sort((a, b) => b.price - a.price);
  }

  return result;
});

// Format tiền tệ
function formatCurrency(price) {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    minimumFractionDigits: 0,
  }).format(price);
}

// Toggle lọc danh mục
function toggleCategory(categoryId) {
  const index = selectedCategories.value.indexOf(categoryId);
  if (index === -1) {
    selectedCategories.value.push(categoryId);
  } else {
    selectedCategories.value.splice(index, 1);
  }
}

// Kiểm tra danh mục đã được chọn chưa
function isCategorySelected(categoryId) {
  return selectedCategories.value.includes(categoryId);
}
</script>

<template>
  <div class="sale-page">
    <div class="sale-banner">
      <div class="banner-content">
        <h1>GIẢM GIÁ</h1>
        <p class="subtitle">
          Cơ hội sở hữu những sản phẩm POLY với mức giá tốt nhất
        </p>
      </div>
    </div>

    <div class="sale-container">
      <!-- Filters -->
      <div class="filter-bar">
        <div class="category-filters">
          <button
            v-for="category in categories"
            :key="category.id"
            class="category-filter"
            :class="{ active: isCategorySelected(category.id) }"
            @click="toggleCategory(category.id)"
          >
            {{ category.name }}
          </button>
        </div>

        <div class="sort-options">
          <label for="sort">Sắp xếp:</label>
          <select id="sort" v-model="sortBy">
            <option value="discount">% Giảm giá cao nhất</option>
            <option value="priceAsc">Giá tăng dần</option>
            <option value="priceDesc">Giá giảm dần</option>
          </select>
        </div>
      </div>

      <!-- Loading state -->
      <div class="loading-container" v-if="isLoading">
        <div class="spinner"></div>
        <p>Đang tải sản phẩm...</p>
      </div>

      <!-- Products grid -->
      <div v-else>
        <div v-if="filteredProducts.length === 0" class="no-products">
          <i class="bi bi-exclamation-circle"></i>
          <p>Không tìm thấy sản phẩm khuyến mãi phù hợp</p>
        </div>

        <div v-else class="products-grid">
          <div
            v-for="product in filteredProducts"
            :key="product.id"
            class="product-card"
          >
            <div class="product-image">
              <img :src="product.image" :alt="product.name" />
              <div class="discount-tag">-{{ product.discount }}%</div>
            </div>

            <div class="product-details">
              <h3 class="product-name">{{ product.name }}</h3>
              <div class="product-price">
                <span class="sale-price">{{
                  formatCurrency(product.price)
                }}</span>
                <span class="original-price">{{
                  formatCurrency(product.originalPrice)
                }}</span>
              </div>
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

      <!-- Promotional section -->
      <div class="promo-section">
        <div class="promo-header">
          <h2>KHUYẾN MÃI ĐANG DIỄN RA</h2>
          <p>Các chương trình khuyến mãi đặc biệt từ POLY</p>
        </div>

        <div class="promo-cards">
          <div class="promo-card">
            <div class="promo-icon">
              <i class="bi bi-gift"></i>
            </div>
            <div class="promo-content">
              <h3>Mua 2 Tặng 1</h3>
              <p>Khi mua 2 sản phẩm áo thun bất kỳ - Tặng ngay 1 vớ cao cổ</p>
              <span class="promo-date">Đến hết 30/06/2024</span>
            </div>
          </div>

          <div class="promo-card">
            <div class="promo-icon">
              <i class="bi bi-currency-exchange"></i>
            </div>
            <div class="promo-content">
              <h3>Giảm 100K</h3>
              <p>Cho đơn hàng từ 600K khi thanh toán qua ZaloPay</p>
              <span class="promo-date">Đến hết 15/06/2024</span>
            </div>
          </div>

          <div class="promo-card">
            <div class="promo-icon">
              <i class="bi bi-truck"></i>
            </div>
            <div class="promo-content">
              <h3>Freeship Extra</h3>
              <p>Miễn phí vận chuyển cho đơn hàng từ 500K</p>
              <span class="promo-date">Áp dụng toàn quốc</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.sale-page {
  padding-bottom: 40px;
}

.sale-banner {
  width: 100%;
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

.sale-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

/* Filter bar */
.filter-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.category-filters {
  display: flex;
  gap: 10px;
}

.category-filter {
  padding: 8px 16px;
  background: none;
  border: 1px solid #ddd;
  border-radius: 20px;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 14px;
}

.category-filter:hover {
  background-color: #f5f5f5;
}

.category-filter.active {
  background-color: #000;
  color: white;
  border-color: #000;
}

.sort-options {
  display: flex;
  align-items: center;
  gap: 10px;
}

.sort-options select {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

/* Loading state */
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

.no-products {
  text-align: center;
  padding: 40px 0;
}

.no-products i {
  font-size: 32px;
  color: #999;
  margin-bottom: 10px;
}

/* Products grid */
.products-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
  margin-bottom: 60px;
}

.product-card {
  position: relative;
  transition: transform 0.2s;
}

.product-card:hover {
  transform: translateY(-5px);
}

.product-image {
  position: relative;
  aspect-ratio: 3/4;
  overflow: hidden;
  background-color: #f9f9f9;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.discount-tag {
  position: absolute;
  top: 10px;
  right: 10px;
  background-color: #e53637;
  color: white;
  padding: 5px 10px;
  font-size: 14px;
  font-weight: 600;
}

.product-details {
  padding: 15px 0;
}

.product-name {
  font-size: 16px;
  margin-bottom: 8px;
  font-weight: 500;
}

.product-price {
  display: flex;
  align-items: center;
  gap: 8px;
}

.sale-price {
  font-weight: 600;
  color: #e53637;
}

.original-price {
  color: #999;
  text-decoration: line-through;
  font-size: 14px;
}

.product-actions {
  display: flex;
  gap: 10px;
  opacity: 0;
  transform: translateY(20px);
  transition: all 0.3s;
}

.product-card:hover .product-actions {
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

/* Promo section */
.promo-section {
  padding: 40px 0;
}

.promo-header {
  text-align: center;
  margin-bottom: 30px;
}

.promo-header h2 {
  font-size: 24px;
  margin-bottom: 10px;
  font-weight: 600;
}

.promo-header p {
  color: #666;
}

.promo-cards {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.promo-card {
  background-color: #f9f9f9;
  padding: 20px;
  border-radius: 8px;
  display: flex;
  gap: 15px;
  transition: transform 0.2s;
}

.promo-card:hover {
  transform: translateY(-5px);
}

.promo-icon {
  font-size: 30px;
  color: #e53637;
}

.promo-content h3 {
  font-size: 18px;
  margin-bottom: 10px;
  font-weight: 600;
}

.promo-content p {
  font-size: 14px;
  color: #666;
  margin-bottom: 10px;
}

.promo-date {
  display: block;
  font-size: 12px;
  color: #999;
}

@media (max-width: 992px) {
  .products-grid {
    grid-template-columns: repeat(3, 1fr);
  }

  .promo-cards {
    grid-template-columns: repeat(2, 1fr);
    gap: 15px;
  }
}

@media (max-width: 768px) {
  .filter-bar {
    flex-direction: column;
    align-items: flex-start;
    gap: 15px;
  }

  .products-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .promo-cards {
    grid-template-columns: 1fr;
  }

  .product-actions {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (max-width: 576px) {
  .products-grid {
    grid-template-columns: 1fr;
    gap: 15px;
  }

  .category-filters {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    width: 100%;
  }

  .category-filter {
    width: 100%;
    text-align: center;
  }
}
</style>
