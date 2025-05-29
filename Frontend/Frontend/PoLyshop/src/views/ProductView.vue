<script setup>
import { ref, onMounted, computed } from "vue";
import axios from "axios";
import bootstrap from "@/utils/bootstrapHelper";

const products = ref([]);
const categories = ref([]);
const searchTerm = ref("");
const error = ref(null);
const loading = ref(true);
const isEditing = ref(false);
const isAdding = ref(false);
const selectedProduct = ref(true);
const currentProduct = ref({
  id: null,
  productId: null,
  productName: "",
  price: 0,
  createdAt: new Date().toISOString().slice(0, 10),
  updatedAt: null,
  status: true,
  categoryName: "",
  image: "",
  tradeMark: "",
  materialName: "",
  sizeName: "",
  colorName: "",
  stockQuantity: 0,
});
const sizes = ref([
  { sizeId: 1, sizeName: 'S' },
  { sizeId: 2, sizeName: 'M' },
  { sizeId: 3, sizeName: 'L' },
  { sizeId: 4, sizeName: 'XL' },
  { sizeId: 5, sizeName: 'XXL' },
]);

const colors = ref([
  { colorId: 1, colorName: 'Đỏ' },
  { colorId: 2, colorName: 'Xanh dương' },
  { colorId: 3, colorName: 'Xanh lá' },
  { colorId: 4, colorName: 'Đen' },
  { colorId: 5, colorName: 'Trắng' },
  { colorId: 6, colorName: 'Vàng' },
  { colorId: 7, colorName: 'Cam' },
  { colorId: 8, colorName: 'Tím' },
  { colorId: 9, colorName: 'Hồng' },
  { colorId: 10, colorName: 'Xám' },
]);

const materials = ref([
  { materialId: 1, materialName: 'Cotton' },
  { materialId: 2, materialName: 'Polyester' },
  { materialId: 3, materialName: 'Rayon' },
  { materialId: 4, materialName: 'Wool' },
  { materialId: 5, materialName: 'Silk' },
  { materialId: 6, materialName: 'Leather' },
  { materialId: 7, materialName: 'Denim' },
  { materialId: 8, materialName: 'Linen' },
  { materialId: 9, materialName: 'Acrylic' },
  { materialId: 10, materialName: 'Nylon' },
]);

const fetchCategories = async () => {
  try {
    const res = await axios.get("https://localhost:7055/api/ProductCategory");
    const raw = Array.isArray(res.data.$values) ? res.data.$values : [];
    categories.value = raw.map(c => ({
      categoryId: c.categoryId,
      categoryName: c.categoryName
    }));
  } catch (err) {
    error.value = "Không thể lấy danh mục sản phẩm: " + err.message;
  }
};
onMounted(() => {
  fetchCategories();
  fetchProducts();
});

const fetchProducts = async () => {
  try {
    loading.value = true;
    const res = await axios.get("https://localhost:7055/api/Product");

    const productArray = Array.isArray(res.data) ? res.data : res.data.$values || [];

    products.value = productArray.map(p => ({
      productId: p.productId,
      productName: p.productName,
      price: p.price,
      categoryName: p.categoryName || "",
      status: typeof p.status === "boolean" ? p.status : !!p.status,
      image: p.image || "https://placehold.co/100x100/orange/white?text=Coming+Soon",
      productDetailId: p.productDetailId || null,
      stockQuantity: p.stockQuantity ?? 0,
      sizeName: p.sizeName || "",
      colorName: p.colorName || "",
      materialName: p.materialName || ""
    }));

    loading.value = false;
  } catch (err) {
    error.value = "Không thể lấy dữ liệu sản phẩm: " + err.message;
    loading.value = false;
  }
};

const filteredProducts = computed(() => {
  if (!searchTerm.value) return products.value;

  const search = searchTerm.value.toLowerCase();
  return products.value.filter((product) => {
    const nameMatch = (product.productName || "").toLowerCase().includes(search);
    const categoryMatch = (product.categoryName || "").toLowerCase().includes(search);
    // Map boolean status to Vietnamese string for searching
    const statusString = product.status === true ? "đang bán" : "ngưng bán";
    const statusMatch = statusString.includes(search);
    return nameMatch || categoryMatch || statusMatch;
  });
});

const openAddModal = () => {
  isEditing.value = false;
  isAdding.value = true;
  selectedProduct.value = false;
  currentProduct.value = {
    id: null,
    productId: null,
    productName: "",
    price: 0,
    createdAt: new Date().toISOString().slice(0, 10),
    updatedAt: null,
    status: true,
    categoryName: categories.value[0] || "",
    image: "",
    tradeMark: "",
    materialName: "",
    sizeName: "",
    colorName: "",
    stockQuantity: 0,
  };
  const modal = new bootstrap.Modal(document.getElementById("productModal"));
  modal.show();
};

const detailsProduct = (product) => {
  currentProduct.value = { ...product };
  const modal = new bootstrap.Modal(document.getElementById("productModal"));
  modal.show();
  isEditing.value = false; // Set to view mode
  selectedProduct.value = true; // Set to view mode
};

const editProduct = (product) => {
  isEditing.value = true;
  isAdding.value = false;
  currentProduct.value = {
    productId: product.productId,
    productDetailId: product.productDetailId, // ⚠️ Cần thiết
    productName: product.productName,
    price: product.price,
    status: product.status,
    categoryName: product.categoryName,
    image: product.image,
    materialName: product.materialName,
    sizeName: product.sizeName,
    colorName: product.colorName,
    stockQuantity: product.stockQuantity
  };
  const modal = new bootstrap.Modal(document.getElementById("productModal"));
  modal.show();
};

const saveProduct = async () => {
  // Ensure correct IDs are set based on names
  const selectedCategory = categories.value.find(c => c.categoryName === currentProduct.value.categoryName);
  const selectedSize = sizes.value.find(s => s.sizeName === currentProduct.value.sizeName);
  const selectedColor = colors.value.find(c => c.colorName === currentProduct.value.colorName);
  const selectedMaterial = materials.value.find(m => m.materialName === currentProduct.value.materialName);

  // Set IDs if found
  if (selectedCategory) {
    currentProduct.value.categoryId = selectedCategory.categoryId;
  }
  if (selectedSize) {
    currentProduct.value.sizeId = selectedSize.sizeId;
  }
  if (selectedColor) {
    currentProduct.value.colorId = selectedColor.colorId;
  }
  if (selectedMaterial) {
    currentProduct.value.materialId = selectedMaterial.materialId;
  }

  console.log("📦 PUT DATA:", {
    products: {
      productId: currentProduct.value.productId,
      productName: currentProduct.value.productName,
      price: currentProduct.value.price,
      status: currentProduct.value.status,
      categoryId: currentProduct.value.categoryId
    },
    productDetails: {
      productDetailId: currentProduct.value.productDetailId,
      productId: currentProduct.value.productId,
      stockQuantity: currentProduct.value.stockQuantity,
      image: currentProduct.value.image
    }
  });

  try {
    if (isEditing.value && isAdding.value === false) {
      // Use the correct ID for update (prefer id, fallback to productId)
      const updateId = currentProduct.value.id || currentProduct.value.productId;
      await axios.put(`https://localhost:7055/api/Product/${currentProduct.value.productId}`, {
        products: {
          productId: currentProduct.value.productId,
          productName: currentProduct.value.productName,
          price: currentProduct.value.price,
          status: currentProduct.value.status,
          categoryId: currentProduct.value.categoryId
        },
        productDetails: {
          productDetailId: currentProduct.value.productDetailId,
          productId: currentProduct.value.productId,
          stockQuantity: currentProduct.value.stockQuantity,
          image: currentProduct.value.image
        },
        sizes: {
          sizeId: currentProduct.value.sizeId
        },
        colors: {
          colorId: currentProduct.value.colorId
        },
        materials: {
          materialId: currentProduct.value.materialId
        }
      });
    } else {
      // Add new product
      const res = await axios.post('https://localhost:7055/api/Product', {
        products: {
          productName: currentProduct.value.productName,
          price: currentProduct.value.price,
          categoryId: currentProduct.value.categoryId,
          status: currentProduct.value.status === true || currentProduct.value.status === "true"
        },
        productDetails: {
          stockQuantity: currentProduct.value.stockQuantity,
          image: currentProduct.value.image || "https://placehold.co/100x100/orange/white?text=Coming+Soon"
        },
        sizes: {
          sizeId: currentProduct.value.sizeId
        },
        colors: {
          colorId: currentProduct.value.colorId
        },
        materials: {
          materialId: currentProduct.value.materialId
        }
      });
      if (res.data && res.data.productId) {
        currentProduct.value.productId = res.data.productId;
      }
    }
    // Refresh product list
    await fetchProducts();
    // Reset current product
    currentProduct.value = {
      id: null,
      productId: null,
      productName: "",
      price: 0,
      createdAt: new Date().toISOString().slice(0, 10),
      updatedAt: null,
      status: true,
      categoryName: categories.value[0] || "",
      image: "",
      tradeMark: "",
      materialName: "",
      sizeName: "",
      colorName: "",
      stockQuantity: 0,
    };
    isEditing.value = false;
    selectedProduct.value = false;
    isAdding.value = false;
    error.value = null;
    // Close the modal
    const modal = bootstrap.Modal.getInstance(
      document.getElementById("productModal")
    );
    modal.hide();
  } catch (err) {
    error.value =
      `Không thể ${isEditing.value ? "cập nhật" : "thêm"} sản phẩm: ` +
      err.message;
  }
};

const confirmDeleteProduct = (product) => {
  currentProduct.value = { ...product };
  const modal = new bootstrap.Modal(document.getElementById("deleteModal"));
  modal.show();
  isEditing.value = false; // Set to view mode
  selectedProduct.value = false; // Set to view mode
  isAdding.value = false; // Ensure not in add mode
};

const deleteProduct = async () => {
  try {
    await axios.delete(`https://localhost:7055/api/Product/${currentProduct.value.productId}`);
    products.value = products.value.filter(
      (p) => p.productId !== currentProduct.value.productId
    );

    // Close the modal
    const modal = bootstrap.Modal.getInstance(
      document.getElementById("deleteModal")
    );
    modal.hide();
  } catch (err) {
    error.value = "Không thể xóa sản phẩm: " + err.message;
  }
};

const formatCurrency = (price) => {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(price);
};

onMounted(fetchProducts);
</script>

<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý sản phẩm</h2>
      <div class="header-actions">
        <div class="search-container">
          <input type="text" class="search-input" placeholder="Tìm kiếm sản phẩm..." v-model="searchTerm" />
          <i class="bi bi-search search-icon"></i>
        </div>
        <button class="btn btn-success" @click="openAddModal">
          <i class="bi bi-plus"></i> Thêm sản phẩm
        </button>
      </div>
    </div>

    <div class="content-container">
      <div v-if="loading" class="text-center p-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Đang tải...</span>
        </div>
      </div>

      <div v-else-if="error" class="alert alert-danger">
        {{ error }}
      </div>

      <div v-else-if="filteredProducts.length === 0" class="text-center p-5">
        <p>Không tìm thấy sản phẩm nào.</p>
      </div>

      <div v-else class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>Ảnh</th>
              <th>STT</th>
              <th>Tên sản phẩm</th>
              <th>Danh mục</th>
              <th>Giá</th>
              <th>Tồn kho</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(product, index) in filteredProducts" :key="product.id">
              <td>
                <img :src="product.image" :alt="product.productName" class="product-thumbnail" />
              </td>
              <td>{{ index + 1 }}</td>
              <td>
                <div class="product-name">{{ product.productName }}</div>
                <small class="text-white px-2 py-1 rounded" :style="{
                  backgroundColor:
                    (typeof product.status === 'string'
                      ? product.status === 'true'
                      : product.status === true)
                      ? '#28a745'
                      : '#dc3545'
                }">
                  {{
                    typeof product.status === 'string'
                      ? (product.status === "true" ? "Đang bán" : (product.status === "false" ? "Ngưng bán" :
                        product.status))
                      : (product.status === true ? "Đang bán" : "Ngưng bán")
                  }}
                </small>
              </td>
              <td>{{ product.categoryName }}</td>
              <td>{{ formatCurrency(product.price) }}</td>
              <td>
                <span class="badge" :class="product.stockQuantity > 10 ? 'bg-success' : 'bg-warning'">
                  {{ product.stockQuantity }}
                </span>
              </td>
              <td class="action-buttons">
                <button class="btn btn-sm btn-info me-1" @click="detailsProduct(product)">
                  <i class="bi bi-eye"></i>
                </button>
                <button class="btn btn-sm btn-primary me-1" @click="editProduct(product)">
                  <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-danger" @click="confirmDeleteProduct(product)">
                  <i class="bi bi-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add/Edit Product Modal -->
    <div class="modal fade" id="productModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ isEditing === true && isAdding === false ? "Chỉnh sửa sản phẩm" : selectedProduct === true && isEditing
                === false ? "Chi tết sản phẩm" : "Thêm sản phẩm mới" }}
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveProduct">
              <div class="row">
                <div class="col-md-6">
                  <div class="mb-3">
                    <label class="form-label">Tên sản phẩm</label>
                    <input type="text" class="form-control" v-model="currentProduct.productName" required
                      :disabled="!isEditing && !isAdding" />
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Danh mục</label>
                    <select class="form-select" v-model="currentProduct.categoryName" required
                      :disabled="!isEditing && !isAdding">
                      <option v-for="cat in categories" :key="cat.categoryId" :value="cat.categoryName">
                        {{ cat.categoryName }}
                      </option>
                    </select>
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Giá (VNĐ)</label>
                    <input type="number" class="form-control" v-model="currentProduct.price" min="0" required
                      :disabled="!isEditing && !isAdding" />
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Số lượng tồn kho</label>
                    <input type="number" class="form-control" v-model="currentProduct.stockQuantity" min="0" required
                      :disabled="!isEditing && !isAdding" />
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Chất liệu</label>
                    <select class="form-select" v-model="currentProduct.materialName"
                      :disabled="!isEditing && !isAdding" required>
                      <option value="" disabled>Chọn chất liệu</option>
                      <option v-for="mat in materials" :key="mat.materialId" :value="mat.materialName">
                        {{ mat.materialName }}
                      </option>
                    </select>
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Size</label>
                    <select class="form-select" v-model="currentProduct.sizeName" :disabled="!isEditing && !isAdding"
                      required>
                      <option value="" disabled>Chọn size</option>
                      <option v-for="sz in sizes" :key="sz.sizeId" :value="sz.sizeName">
                        {{ sz.sizeName }}
                      </option>
                    </select>
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Màu sắc</label>
                    <select class="form-select" v-model="currentProduct.colorName" :disabled="!isEditing && !isAdding"
                      required>
                      <option value="" disabled>Chọn màu sắc</option>
                      <option v-for="cl in colors" :key="cl.colorId" :value="cl.colorName">
                        {{ cl.colorName }}
                      </option>
                    </select>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="mb-3">
                    <label class="form-label">Trạng thái</label>
                    <select class="form-select" v-model="currentProduct.status" :disabled="!isEditing && !isAdding"
                      required>
                      <option :value="true">Đang bán</option>
                      <option :value="false">Ngưng bán</option>
                    </select>
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Ảnh sản phẩm</label>
                    <input type="file" class="form-control" :disabled="!isEditing && !isAdding" />
                    <small class="text-muted">Tính năng upload ảnh sẽ được triển khai sau.</small>
                  </div>
                  <div v-if="isEditing && currentProduct.image || selectedProduct && currentProduct.image" class="mb-3">
                    <label class="form-label">Ảnh hiện tại</label>
                    <div class="current-image">
                      <img :src="currentProduct.image" :alt="currentProduct.productName" class="img-fluid" />
                    </div>
                  </div>
                </div>
              </div>
              <div v-if="isEditing  || isAdding" class="text-end mt-3">
                <button type="button" class="btn btn-secondary me-2" data-bs-dismiss="modal">
                  Hủy
                </button>
                <button type="submit" class="btn btn-primary">Lưu</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div class="modal fade" id="deleteModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Xác nhận xóa</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <p>
              Bạn có chắc chắn muốn xóa sản phẩm
              <strong>{{ currentProduct.productName }}</strong>?
            </p>
            <p class="text-danger">Hành động này không thể hoàn tác.</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
              Hủy
            </button>
            <button type="button" class="btn btn-danger" @click="deleteProduct">
              Xóa
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.admin-page {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  flex-wrap: wrap;
}

.page-title {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 10px;
}

.header-actions {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
}

.search-container {
  position: relative;
  min-width: 250px;
}

.search-input {
  width: 100%;
  padding: 10px 15px;
  padding-right: 40px;
  border-radius: 5px;
  border: 1px solid #ddd;
}

.search-icon {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  color: #777;
}

.content-container {
  background-color: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
}

.admin-table th,
.admin-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

.admin-table th {
  background-color: #f5f5f5;
  font-weight: bold;
}

.admin-table tr:hover {
  background-color: #f9f9f9;
}

.action-buttons {
  white-space: nowrap;
}

.product-thumbnail {
  width: 60px;
  height: 60px;
  object-fit: cover;
  border-radius: 4px;
}

.product-name {
  font-weight: 500;
  font-size: large;
}

.current-image {
  max-width: 200px;
  margin-top: 10px;
  border: 1px solid #ddd;
  padding: 5px;
  border-radius: 4px;
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: stretch;
  }

  .header-actions {
    margin-top: 10px;
  }
}
</style>
