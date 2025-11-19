<script setup>
import { ref, computed, onMounted } from "vue";
import axios from "axios";

// States (Giữ nguyên)
const products = ref([]);
const isLoading = ref(true);
const error = ref(null);
const selectedSort = ref("default");
const selectedFilters = ref({
  categories: [],
  sizes: [],
  colors: [],
  priceRange: [0, 2000000],
});
import { addToCart } from "@/utils/cartStore"; // Thêm dòng này

// 1. TẠO HỘP CHỨA BỘ LỌC ĐỘNG
const categories = ref([]); // Hộp chứa danh mục từ API
const sizes = ref([]); // Hộp chứa size từ API
const colors = ref([]); // Hộp chứa màu từ API

// 2. XÓA BỎ HOÀN TOÀN 'const filters' CŨ

// 3. CẬP NHẬT onMounted ĐỂ GỌI 4 API
onMounted(async () => {
  isLoading.value = true;
  error.value = null;
  try {
    // Gọi 4 API cùng lúc
    const [
      productRes, 
      categoryRes,
      sizeRes,
      colorRes
    ] = await Promise.all([
      axios.get("https://localhost:7055/api/Product"),
      axios.get("https://localhost:7055/api/ProductCategory"),
      axios.get("https://localhost:7055/api/Product/Sizes"), // GỌI API SIZE MỚI
      axios.get("https://localhost:7055/api/Product/Colors")  // GỌI API MÀU MỚI
    ]);

    products.value = productRes.data.$values || productRes.data || [];
    categories.value = categoryRes.data.$values || categoryRes.data || []; 
    sizes.value = sizeRes.data.$values || sizeRes.data || []; // Đổ size vào
    colors.value = colorRes.data.$values || colorRes.data || []; // Đổ màu vào

  } catch (err) {
    console.error(err);
    error.value = "Đã xảy ra lỗi khi tải dữ liệu.";
  } finally {
    isLoading.value = false;
  }
});

// 4. CẬP NHẬT BỘ LỌC ĐỘNG (Đọc từ ref động)
const displayCategories = computed(() => {
  if (!products.value) return [];
  const availableCategoryIds = new Set(products.value.map(p => p.categoryId));
  // Lấy 'categories' từ ref, và lọc theo trang (Áo/Quần)
  return categories.value.filter(cat => 
    availableCategoryIds.has(cat.categoryId) &&
    cat.categoryName.startsWith("Áo") // <-- Sửa "Áo" thành "Quần" cho trang Quần
  );
});

const displaySizes = computed(() => {
  if (!products.value) return [];
  const availableSizeIds = new Set(products.value.map(p => p.sizeId));
  // Lấy 'sizes' từ ref
  return sizes.value.filter(size => availableSizeIds.has(size.id));
});

const displayColors = computed(() => {
  if (!products.value) return [];
  const availableColorIds = new Set(products.value.map(p => p.colorId));
  // Lấy 'colors' từ ref
  return colors.value.filter(color => availableColorIds.has(color.id));
});


// 5. CẬP NHẬT filteredProducts (Dùng ID, đơn giản và nhanh)
const filteredProducts = computed(() => {
  if (!products.value || products.value.length === 0) {
    return [];
  }

  // --- BƯỚC A: GỘP SẢN PHẨM (Dùng ID) ---
  const productMap = new Map();
  for (const item of products.value) {
    // Đảm bảo API đã trả về các ID
    if (!item.categoryId || !item.sizeId || !item.colorId) continue; 

    if (!productMap.has(item.productId)) {
      productMap.set(item.productId, {
        ...item,
        categoryIds: new Set(),
        sizeIds: new Set(),
        colorIds: new Set()
      });
    }
    const prod = productMap.get(item.productId);
    prod.categoryIds.add(item.categoryId);
    prod.sizeIds.add(item.sizeId);
    prod.colorIds.add(item.colorId);
  }
  
  let result = Array.from(productMap.values()).map(prod => ({
    ...prod,
    categoryIds: Array.from(prod.categoryIds),
    sizeIds: Array.from(prod.sizeIds),
    colorIds: Array.from(prod.colorIds)
  }));
  
  // --- BƯỚC B: CHẠY BỘ LỌC ---

  // Lấy ra ID của các category thuộc trang "Áo"
  const pageCategoryIds = categories.value
    .filter(c => c.categoryName.startsWith("Áo")) // <-- Sửa "Áo" thành "Quần" cho trang Quần
    .map(c => c.categoryId);

  // Lọc bỏ tất cả sản phẩm không phải là "Áo"
  result = result.filter(p => p.categoryIds.some(id => pageCategoryIds.includes(id)));

  // Filter by category
  if (selectedFilters.value.categories.length > 0) {
    result = result.filter((product) =>
      product.categoryIds.some(id => selectedFilters.value.categories.includes(id))
    );
  }
  
  // Filter by size
  if (selectedFilters.value.sizes.length > 0) {
    result = result.filter((product) =>
      product.sizeIds.some(id => selectedFilters.value.sizes.includes(id))
    );
  }

  // Filter by color
  if (selectedFilters.value.colors.length > 0) {
    result = result.filter((product) =>
      product.colorIds.some(id => selectedFilters.value.colors.includes(id))
    );
  }
  
  // (Logic lọc Price, Sort y hệt như cũ)
  if (selectedFilters.value.priceRange.length > 0) {
     result = result.filter(
      (product) =>
        product.price >= selectedFilters.value.priceRange[0] &&
        product.price <= selectedFilters.value.priceRange[1]
    );
  }
  if (selectedSort.value === "price-asc") {
    result.sort((a, b) => parseFloat(a.price) - parseFloat(b.price)); // Thêm parseFloat
  } else if (selectedSort.value === "price-desc") {
    result.sort((a, b) => parseFloat(b.price) - parseFloat(a.price)); // Thêm parseFloat
  }

  return result;
});


// 6. GIỮ NGUYÊN CÁC HÀM CÒN LẠI
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
function isFilterActive(type, value) {
  return selectedFilters.value[type].includes(value);
}
function formatCurrency(price) {
  if (typeof price !== 'number') { return price; }
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    minimumFractionDigits: 0,
  }).format(price);
}
function resetFilters() {
  selectedFilters.value = {
    categories: [],
    sizes: [],
    colors: [],
    priceRange: [0, 2000000],
  };
  selectedSort.value = "default";
}
const showMobileFilters = ref(false);
function toggleMobileFilters() {
  showMobileFilters.value = !showMobileFilters.value;
}
const colorNameMap = {
  // Bạn phải đảm bảo tên ở đây (ví dụ "Đen")
  // GIỐNG HỆT 100% với tên trong cột 'ColorName' của SQL

  // Màu từ trang Áo
  "Đen": "#000000",
  "Trắng": "#FFFFFF",
  "Xám": "#888888",
  "Xanh Navy": "#000080",
  "Xanh lá": "#008000",
  "Đỏ": "#FF0000",
  "Xanh Dương": "#0000FF",
  "Cam": "#FFA500",
  "Vàng": "#FFFF00",
  "Hồng": "#FFC0CB",
  "Tím": "#800080",
  "Nâu": "#A52A2A",
  "Be": "#F5F5DC",
  "Xám": "#808080",
  

  // Màu từ trang Quần
  "Xanh đậm": "#000080",
  "Xanh nhạt": "#87CEFA",
  "Be": "#F5F5DC",

  // (Nếu có màu khác, bạn tự thêm vào đây)
};
// ***** THAY THẾ HÀM CŨ BẰNG HÀM NÀY *****
function getColorCodeById(id) {
  if (!colors.value) return '#FFF'; // Trả về nếu chưa tải
  
  // 1. Tìm đối tượng màu bằng ID (ví dụ: tìm ID 1)
  const colorObj = colors.value.find(c => c.id === id);
  if (!colorObj) return '#FFF'; // Không tìm thấy

  // 2. Lấy TÊN của nó (ví dụ: "Đỏ")
  const colorName = colorObj.name.trim(); // Lấy tên "Đỏ" và cắt khoảng trắng

  // 3. Dùng tên đó để tra cứu trong "bản đồ"
  return colorNameMap[colorName] || '#FFF'; // Trả về #FF0000 hoặc màu trắng nếu không có
}
// ***** HẾT PHẦN THAY THẾ *****

// Hàm này lấy ID (ví dụ: 1) và trả về tên (ví dụ: "S")
function getSizeNameById(id) {
  if (!sizes.value) return '?'; // Trả về '?' nếu 'sizes' (từ API) chưa tải
  const sizeObj = sizes.value.find(s => s.id === id);
  return sizeObj ? sizeObj.name : '?'; // Trả về 'S', 'M', 'L'...
}
// Thêm hàm này vào script setup
const handleAddToCart = (product) => {
  // Tạo một object sản phẩm để thêm vào giỏ
  // Vì ở trang danh sách chưa chọn size/màu, ta lấy cái đầu tiên hoặc để mặc định
  const cartItem = {
    productId: product.productId,
    productName: product.productName,
    price: product.price,
    image: product.image,
    categoryName: product.categoryName || 'Thời trang',
    
    // Logic lấy tên size/màu:
    // Lấy ID đầu tiên trong mảng sizeIds, sau đó tìm tên của nó
    sizeId: product.sizeIds && product.sizeIds.length > 0 ? product.sizeIds[0] : null,
    sizeName: product.sizeIds && product.sizeIds.length > 0 ? getSizeNameById(product.sizeIds[0]) : 'Mặc định',
    
    colorId: product.colorIds && product.colorIds.length > 0 ? product.colorIds[0] : null,
    colorName: product.colorIds && product.colorIds.length > 0 ? getColorNameById(product.colorIds[0]) : 'Mặc định' 
    // (Bạn cần viết thêm hàm getColorNameById tương tự getSizeNameById nếu chưa có, 
    // hoặc đơn giản là hardcode tạm nếu muốn test nhanh)
  };

  addToCart(cartItem);
};

// Hàm helper lấy tên màu (nếu bạn chưa có)
function getColorNameById(id) {
    if (!colors.value) return '?';
    const colorObj = colors.value.find(c => c.id === id);
    return colorObj ? colorObj.name : '?';
}
</script>

<template>
  <div class="product-page">
    <div class="page-header">
      <h1>ÁO</h1> <p class="subtitle">Tất cả các mẫu áo mới nhất từ POLY</p>
    </div>

    <div class="product-container">
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

        <div class="filter-section">
          <h4>Loại áo</h4> <div class="filter-options">
            <div
              v-for="category in displayCategories"
              :key="category.categoryId"
              class="filter-option"
              :class="{ active: isFilterActive('categories', category.categoryId) }"
              @click="toggleFilter('categories', category.categoryId)"
            >
              <span>{{ category.categoryName }}</span>
              <i
                v-if="isFilterActive('categories', category.categoryId)"
                class="bi bi-check-lg"
              ></i>
            </div>
          </div>
        </div>

        <div class="filter-section">
  <h4>Kích thước</h4>
  <div class="size-options">
    <div
      v-for="size in displaySizes" :key="size.id" class="size-option"
      :class="{ active: isFilterActive('sizes', size.id) }" @click="toggleFilter('sizes', size.id)" >
      {{ size.name }} </div>
  </div>
</div>

        <div class="filter-section">
  <h4>Màu sắc</h4>
  <div class="color-options">
                  <div
                    v-for="color in displayColors"
                    :key="color.id"
                    class="color-option"
                    :class="{ active: isFilterActive('colors', color.id) }"
                    @click="toggleFilter('colors', color.id)"
                  >
                    <span
                      class="color-sample"
                      :style="`background-color: ${colorNameMap[color.name.trim()] || '#FFF'}`"
                    ></span> <span class="color-name">{{ color.name }}</span>
                  </div>
                </div>
</div>

        <button class="reset-button" @click="resetFilters">
          <i class="bi bi-arrow-counterclockwise"></i> Xóa bộ lọc
        </button>
      </div>

      <div class="product-content">
        <div class="sort-container">
          <label for="sort">Sắp xếp:</label>
          <select id="sort" v-model="selectedSort" class="sort-select">
            <option value="default">Mặc định</option>
            <option value="price-asc">Giá tăng dần</option>
            <option value="price-desc">Giá giảm dần</option>
          </select>

          <div class="product-count">
            {{ filteredProducts.length }} sản phẩm
          </div>
        </div>

        <div class="loading-container" v-if="isLoading">
          <div class="spinner"></div>
          <p>Đang tải sản phẩm...</p>
        </div>

        <div class="no-products" v-else-if="error">
          <i class="bi bi-exclamation-triangle-fill" style="color: red"></i>
          <p style="color: red">{{ error }}</p>
        </div>

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
              :key="product.productId"
              class="product-card"
            >
              <div class="product-image">
                <img :src="product.image" :alt="product.productName" />
                <div class="product-tags">
                  <span v-if="product.isNew" class="tag new-tag">NEW</span>
                  <span v-if="product.isSale" class="tag sale-tag">SALE</span>
                </div>
              </div>

              <div class="product-details">
                <h3 class="product-name">{{ product.productName }}</h3>
                
                <div class="product-price">
                  <span class="price">{{ formatCurrency(product.price) }}</span>
                  <span
                    v-if="product.originalPrice > product.price"
                    class="original-price"
                  >
                    {{ formatCurrency(product.originalPrice) }}
                  </span>
                </div>

                <div class="product-colors" style="margin-top: 10px; display: flex; flex-wrap: wrap; gap: 6px;">
                  <span
                    v-for="colorId in product.colorIds" :key="colorId"
                    class="color-dot"
                    :style="`background-color: ${getColorCodeById(colorId)}; width: 15px; height: 15px; border-radius: 50%; border: 1px solid #ddd; display: inline-block;`"
                  ></span> </div>
                <div class="product-card-sizes" style="margin-top: 10px; display: flex; flex-wrap: wrap; gap: 6px;">
                  <span 
                    v-for="sizeId in product.sizeIds" :key="sizeId" 
                    style="font-size: 12px; border: 1px solid #ddd; padding: 2px 7px; border-radius: 3px; text-transform: uppercase;"
                  >
                    {{ getSizeNameById(sizeId) }} </span>
                </div>
                </div>

              <div class="product-actions">
  <button class="add-to-cart" @click="handleAddToCart(product)">Thêm vào giỏ</button>
  
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
