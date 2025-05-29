<script setup>
import { ref, computed, onMounted } from "vue";

// States
const products = ref([]);
const isLoading = ref(true);
const selectedSort = ref("default");
const selectedFilters = ref({
  categories: [],
  sizes: [],
  colors: [],
  priceRange: [0, 2000000],
});

// Filters data
const filters = {
  categories: [
    { id: "jeans", name: "Quần jean" },
    { id: "shorts", name: "Quần short" },
    { id: "kaki", name: "Quần kaki" },
    { id: "jogger", name: "Quần jogger" },
    { id: "cargo", name: "Quần cargo" },
  ],
  sizes: ["28", "29", "30", "31", "32", "34", "36"],
  colors: [
    { name: "Đen", code: "#000000" },
    { name: "Xanh đậm", code: "#000080" },
    { name: "Xanh nhạt", code: "#87CEFA" },
    { name: "Xám", code: "#888888" },
    { name: "Trắng", code: "#FFFFFF" },
    { name: "Be", code: "#F5F5DC" },
  ],
};

// Mock products
const mockProducts = [
  {
    id: 1,
    name: "Quần jean slim fit",
    price: 450000,
    originalPrice: 450000,
    image: "https://4menshop.com/images/thumbs/2019/08/quan-jean-slimfit-xanh-bien-qj1653-14633-slide-products-5d64ff79a06ee.JPG",
    category: "jeans",
    sizes: ["29", "30", "31", "32"],
    colors: ["#000080", "#000000"],
    isNew: true,
    isSale: false,
  },
  {
    id: 2,
    name: "Quần short denim",
    price: 350000,
    originalPrice: 420000,
    image: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSbJKPHaebJNp0zE1pRaU1HPwK7lLhWsGbRUQ&s",
    category: "shorts",
    sizes: ["29", "30", "31", "32"],
    colors: ["#000080", "#87CEFA", "#000000"],
    isNew: false,
    isSale: true,
  },
  {
    id: 3,
    name: "Quần kaki dáng regular",
    price: 420000,
    originalPrice: 420000,
    image: "https://product.hstatic.net/200000886795/product/quan-kaki-nam-aristino-regular-fit-akk0040z__7__70b252499fbc48b8b078d67cebdf1008.jpg",
    category: "kaki",
    sizes: ["29", "30", "31", "32", "34"],
    colors: ["#000000", "#888888", "#F5F5DC"],
    isNew: false,
    isSale: false,
  },
  {
    id: 4,
    name: "Quần jogger thể thao",
    price: 380000,
    originalPrice: 380000,
    image: "https://www.sporter.vn/wp-content/uploads/2017/06/quan-jogger-nam-360s-lx-den-1-750x800.jpg",
    category: "jogger",
    sizes: ["29", "30", "31", "32"],
    colors: ["#000000", "#888888"],
    isNew: true,
    isSale: false,
  },
  {
    id: 5,
    name: "Quần cargo túi hộp",
    price: 550000,
    originalPrice: 650000,
    image: "https://zizoou.com/cdn/shop/files/Quan-tui-hop-kaki-cao-cap-Quan-jogger-unisex-vang-be-khaki-7-1.jpg?v=1698800965",
    category: "cargo",
    sizes: ["30", "31", "32", "34"],
    colors: ["#000000", "#888888", "#F5F5DC"],
    isNew: true,
    isSale: true,
  },
  {
    id: 6,
    name: "Quần jean rách gối",
    price: 480000,
    originalPrice: 480000,
    image: "https://product.hstatic.net/200000525319/product/jean_rach_2_goi_-_280k__3__affbf4b8691441d6bcb23c13a0a3ed46_master.jpg",
    category: "jeans",
    sizes: ["29", "30", "31", "32"],
    colors: ["#000080", "#000000"],
    isNew: false,
    isSale: false,
  },
  {
    id: 7,
    name: "Quần short kaki",
    price: 320000,
    originalPrice: 380000,
    image: "https://product.hstatic.net/1000253775/product/qsid0137_06f8df98b65b4ffe82851bcd5a011ff1_1024x1024.jpg",
    category: "shorts",
    sizes: ["29", "30", "31", "32"],
    colors: ["#F5F5DC", "#000000", "#888888"],
    isNew: false,
    isSale: true,
  },
  {
    id: 8,
    name: "Quần jean baggy",
    price: 520000,
    originalPrice: 520000,
    image: "https://zizoou.com/cdn/shop/products/Quan-Baggy-Jean-nam-nu-2b-1-Quan-ong-rong-xanh-classic-ZiZoou-Store.jpg?v=1680283265",
    category: "jeans",
    sizes: ["30", "31", "32", "34", "36"],
    colors: ["#000080", "#000000"],
    isNew: true,
    isSale: false,
  },
];

// Tải dữ liệu
onMounted(async () => {
  await new Promise((resolve) => setTimeout(resolve, 800));
  products.value = mockProducts;
  isLoading.value = false;
});

// Toggle filter
function toggleFilter(type, value) {
  const currentFilters = [...selectedFilters.value[type]];
  const index = currentFilters.indexOf(value);

  if (index === -1) {
    currentFilters.push(value);
  } else {
    currentFilters.splice(index, 1);
  }

  selectedFilters.value = {
    ...selectedFilters.value,
    [type]: currentFilters,
  };
}

// Check if filter is active
function isFilterActive(type, value) {
  return selectedFilters.value[type].includes(value);
}

// Filtered products
const filteredProducts = computed(() => {
  let result = [...products.value];

  // Filter by category
  if (selectedFilters.value.categories.length > 0) {
    result = result.filter((product) =>
      selectedFilters.value.categories.includes(product.category)
    );
  }

  // Filter by size
  if (selectedFilters.value.sizes.length > 0) {
    result = result.filter((product) =>
      product.sizes.some((size) => selectedFilters.value.sizes.includes(size))
    );
  }

  // Filter by color
  if (selectedFilters.value.colors.length > 0) {
    result = result.filter((product) =>
      product.colors.some((color) =>
        selectedFilters.value.colors.includes(color)
      )
    );
  }

  // Filter by price range
  result = result.filter(
    (product) =>
      product.price >= selectedFilters.value.priceRange[0] &&
      product.price <= selectedFilters.value.priceRange[1]
  );

  // Sort
  if (selectedSort.value === "price-asc") {
    result.sort((a, b) => a.price - b.price);
  } else if (selectedSort.value === "price-desc") {
    result.sort((a, b) => b.price - a.price);
  } else if (selectedSort.value === "name-asc") {
    result.sort((a, b) => a.name.localeCompare(b.name));
  } else if (selectedSort.value === "name-desc") {
    result.sort((a, b) => b.name.localeCompare(a.name));
  }

  return result;
});

// Format currency
function formatCurrency(price) {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    minimumFractionDigits: 0,
  }).format(price);
}

// Reset filters
function resetFilters() {
  selectedFilters.value = {
    categories: [],
    sizes: [],
    colors: [],
    priceRange: [0, 2000000],
  };
  selectedSort.value = "default";
}

// Toggle mobile filters
const showMobileFilters = ref(false);

function toggleMobileFilters() {
  showMobileFilters.value = !showMobileFilters.value;
}
</script>

<template>
  <div class="product-page">
    <div class="page-header">
      <h1>QUẦN</h1>
      <p class="subtitle">Tất cả các mẫu quần mới nhất từ POLY</p>
    </div>

    <div class="product-container">
      <!-- Mobile filter toggle -->
      <div class="mobile-filter-toggle">
        <button @click="toggleMobileFilters">
          <i class="bi bi-funnel-fill"></i> Bộ lọc
          <span
            v-if="Object.values(selectedFilters).flat().length > 0"
            class="filter-count"
          >
            {{ Object.values(selectedFilters).flat().length }}
          </span>
        </button>
      </div>

      <!-- Sidebar filters - Desktop -->
      <div
        class="filter-sidebar"
        :class="{ 'mobile-active': showMobileFilters }"
      >
        <div class="filter-header">
          <h3>BỘ LỌC</h3>
          <button class="close-filters" @click="toggleMobileFilters">
            <i class="bi bi-x-lg"></i>
          </button>
        </div>

        <!-- Categories -->
        <div class="filter-section">
          <h4>Loại quần</h4>
          <div class="filter-options">
            <div
              v-for="category in filters.categories"
              :key="category.id"
              class="filter-option"
              :class="{ active: isFilterActive('categories', category.id) }"
              @click="toggleFilter('categories', category.id)"
            >
              <span>{{ category.name }}</span>
              <i
                v-if="isFilterActive('categories', category.id)"
                class="bi bi-check-lg"
              ></i>
            </div>
          </div>
        </div>

        <!-- Sizes -->
        <div class="filter-section">
          <h4>Kích thước</h4>
          <div class="size-options">
            <div
              v-for="size in filters.sizes"
              :key="size"
              class="size-option"
              :class="{ active: isFilterActive('sizes', size) }"
              @click="toggleFilter('sizes', size)"
            >
              {{ size }}
            </div>
          </div>
        </div>

        <!-- Colors -->
        <div class="filter-section">
          <h4>Màu sắc</h4>
          <div class="color-options">
            <div
              v-for="color in filters.colors"
              :key="color.code"
              class="color-option"
              :class="{ active: isFilterActive('colors', color.code) }"
              @click="toggleFilter('colors', color.code)"
            >
              <span
                class="color-sample"
                :style="`background-color: ${color.code}`"
              ></span>
              <span class="color-name">{{ color.name }}</span>
            </div>
          </div>
        </div>

        <!-- Reset filters -->
        <button class="reset-button" @click="resetFilters">
          <i class="bi bi-arrow-counterclockwise"></i> Xóa bộ lọc
        </button>
      </div>

      <!-- Product list -->
      <div class="product-content">
        <!-- Sort options -->
        <div class="sort-container">
          <label for="sort">Sắp xếp:</label>
          <select id="sort" v-model="selectedSort" class="sort-select">
            <option value="default">Mặc định</option>
            <option value="price-asc">Giá tăng dần</option>
            <option value="price-desc">Giá giảm dần</option>
            <option value="name-asc">Tên A-Z</option>
            <option value="name-desc">Tên Z-A</option>
          </select>

          <div class="product-count">
            {{ filteredProducts.length }} sản phẩm
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
            <p>Không có sản phẩm nào phù hợp với bộ lọc</p>
            <button class="reset-button" @click="resetFilters">
              Xóa bộ lọc
            </button>
          </div>

          <div v-else class="products-grid">
            <div
              v-for="product in filteredProducts"
              :key="product.id"
              class="product-card"
            >
              <div class="product-image">
                <img :src="product.image" :alt="product.name" />
                <div class="product-tags">
                  <span v-if="product.isNew" class="tag new-tag">NEW</span>
                  <span v-if="product.isSale" class="tag sale-tag">SALE</span>
                </div>
              </div>

              <div class="product-details">
                <h3 class="product-name">{{ product.name }}</h3>
                <div class="product-price">
                  <span class="price">{{ formatCurrency(product.price) }}</span>
                  <span
                    v-if="product.originalPrice > product.price"
                    class="original-price"
                  >
                    {{ formatCurrency(product.originalPrice) }}
                  </span>
                </div>

                <div class="product-colors">
                  <div
                    v-for="color in product.colors"
                    :key="color"
                    class="color-dot"
                    :style="`background-color: ${color}`"
                  ></div>
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
      </div>
    </div>
  </div>
</template>

<style scoped>
.product-page {
  padding: 40px 0;
}

.page-header {
  text-align: center;
  margin-bottom: 40px;
}

.page-header h1 {
  font-size: 28px;
  margin-bottom: 10px;
  font-weight: 600;
}

.subtitle {
  color: #666;
  font-size: 16px;
}

.product-container {
  display: flex;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  position: relative;
}

/* Mobile filter toggle */
.mobile-filter-toggle {
  display: none;
  margin-bottom: 20px;
  padding: 0 20px;
}

.mobile-filter-toggle button {
  width: 100%;
  padding: 10px;
  background-color: #f5f5f5;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  cursor: pointer;
}

.filter-count {
  background-color: #000;
  color: white;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
}

/* Filter sidebar */
.filter-sidebar {
  width: 250px;
  margin-right: 30px;
}

.filter-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.filter-header h3 {
  font-size: 16px;
  font-weight: 600;
  margin: 0;
}

.close-filters {
  display: none;
  background: none;
  border: none;
  font-size: 20px;
  cursor: pointer;
}

.filter-section {
  margin-bottom: 25px;
}

.filter-section h4 {
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 10px;
  border-bottom: 1px solid #eee;
  padding-bottom: 8px;
}

.filter-options {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.filter-option {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 6px 10px;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.filter-option:hover {
  background-color: #f5f5f5;
}

.filter-option.active {
  background-color: #f0f0f0;
  font-weight: 500;
}

.size-options {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.size-option {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.size-option:hover {
  background-color: #f5f5f5;
}

.size-option.active {
  background-color: #000;
  border-color: #000;
  color: white;
}

.color-options {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.color-option {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  padding: 6px 10px;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.color-option:hover {
  background-color: #f5f5f5;
}

.color-option.active {
  background-color: #f0f0f0;
}

.color-sample {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 1px solid #ddd;
}

.reset-button {
  width: 100%;
  padding: 10px;
  background-color: #f5f5f5;
  border: 1px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  font-size: 14px;
  transition: background-color 0.2s;
}

.reset-button:hover {
  background-color: #e5e5e5;
}

/* Product content */
.product-content {
  flex: 1;
}

.sort-container {
  display: flex;
  align-items: center;
  margin-bottom: 30px;
  gap: 10px;
}

.sort-select {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.product-count {
  margin-left: auto;
  font-size: 14px;
  color: #666;
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

.no-products {
  text-align: center;
  padding: 40px 0;
}

.no-products i {
  font-size: 32px;
  color: #999;
  margin-bottom: 10px;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 30px;
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

.product-tags {
  position: absolute;
  top: 10px;
  left: 10px;
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.tag {
  display: inline-block;
  padding: 4px 8px;
  color: white;
  font-size: 12px;
  font-weight: 500;
}

.new-tag {
  background-color: #000;
}

.sale-tag {
  background-color: #e53637;
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
  margin-bottom: 10px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.price {
  font-weight: 600;
}

.original-price {
  color: #999;
  text-decoration: line-through;
  font-size: 14px;
}

.product-colors {
  display: flex;
  gap: 5px;
}

.color-dot {
  width: 15px;
  height: 15px;
  border-radius: 50%;
  border: 1px solid #ddd;
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

@media (max-width: 992px) {
  .products-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .product-container {
    flex-direction: column;
  }

  .filter-sidebar {
    position: fixed;
    top: 0;
    left: 0;
    width: 80%;
    max-width: 300px;
    height: 100%;
    background-color: white;
    z-index: 1000;
    padding: 20px;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
    overflow-y: auto;
    transform: translateX(-100%);
    transition: transform 0.3s;
  }

  .filter-sidebar.mobile-active {
    transform: translateX(0);
  }

  .mobile-filter-toggle {
    display: block;
  }

  .close-filters {
    display: block;
  }

  .sort-container {
    flex-wrap: wrap;
    gap: 10px;
  }

  .product-count {
    width: 100%;
    margin-left: 0;
  }
}

@media (max-width: 576px) {
  .products-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }

  .product-actions {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
