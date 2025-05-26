<script setup>
import { ref, onMounted, computed } from "vue";
import axios from "axios";
import bootstrap from "@/utils/bootstrapHelper";

const products = ref([]);
const searchTerm = ref("");
const error = ref(null);
const loading = ref(true);
const isEditing = ref(false);
const currentProduct = ref({
  id: null,
  productName: "",
  price: 0,
  description: "",
  category: "",
  stock: 0,
  image: "",
});

const categories = ref([
  "Áo thun",
  "Áo khoác",
  "Áo sơ mi",
  "Quần jeans",
  "Quần short",
  "Váy",
  "Đầm",
  "Phụ kiện",
]);

// Sample data for development
const sampleProducts = [
  {
    id: 1,
    productName: "Áo thun Unisex Cotton",
    price: 250000,
    description: "Áo thun chất liệu cotton cao cấp, form rộng thoải mái",
    stock: 100,
    category: "Áo thun",
    image: "https://media1.thehungryjpeg.com/thumbs/800_4262428_zzultqdefwrzpkyb25nsmpzexo4h46xkmpbv2rmb.png",
  },
  {
    id: 2,
    productName: "Quần Jeans Slim Fit",
    price: 450000,
    description: "Quần jeans form slim fit, màu xanh đậm",
    stock: 50,
    category: "Quần jeans",
    image: "https://vn-test-11.slatic.net/p/9bb2a97169e7673623ade19ccafeaff3.jpg",
  },
  {
    id: 3,
    productName: "Áo sơ mi Oxford",
    price: 350000,
    description: "Áo sơ mi chất liệu oxford, form suông, màu trắng",
    stock: 75,
    category: "Áo sơ mi",
    image: "https://product.hstatic.net/1000360022/product/ao-so-mi-linen-nam-tay-ngan-minimal-collection-form-regular__7__96ae3e35f57049438841a8a8459c336a.jpg",
  },
  {
    id: 4,
    productName: "Áo khoác Bomber",
    price: 650000,
    description: "Áo khoác bomber chống nước, màu đen",
    stock: 30,
    category: "Áo khoác",
    image: "https://product.hstatic.net/1000360022/product/untitled-1__2__218c890a6c9c406a966b4ab805530b28.jpg",
  },
];

const fetchProducts = async () => {
  try {
    loading.value = true;
    // Uncomment the following line when API is ready
    // const res = await axios.get("/api/Product");
    // products.value = res.data || [];

    // For development, use sample data
    setTimeout(() => {
      products.value = sampleProducts;
      loading.value = false;
    }, 500);
  } catch (err) {
    error.value = "Không thể lấy dữ liệu sản phẩm: " + err.message;
    loading.value = false;
  }
};

const filteredProducts = computed(() => {
  if (!searchTerm.value) return products.value;

  const search = searchTerm.value.toLowerCase();
  return products.value.filter(
    (product) =>
      product.productName.toLowerCase().includes(search) ||
      product.description.toLowerCase().includes(search) ||
      product.category.toLowerCase().includes(search)
  );
});

const openAddModal = () => {
  isEditing.value = false;
  currentProduct.value = {
    id: null,
    productName: "",
    price: 0,
    description: "",
    category: categories.value[0],
    stock: 0,
    image: "",
  };
  const modal = new bootstrap.Modal(document.getElementById("productModal"));
  modal.show();
};

const editProduct = (product) => {
  isEditing.value = true;
  currentProduct.value = { ...product };
  const modal = new bootstrap.Modal(document.getElementById("productModal"));
  modal.show();
};

const confirmDeleteProduct = (product) => {
  currentProduct.value = { ...product };
  const modal = new bootstrap.Modal(document.getElementById("deleteModal"));
  modal.show();
};

const saveProduct = async () => {
  try {
    if (isEditing.value) {
      // Uncomment when API is ready
      // await axios.put(`/api/Product/${currentProduct.value.id}`, currentProduct.value);

      // For development
      const index = products.value.findIndex(
        (p) => p.id === currentProduct.value.id
      );
      if (index !== -1) {
        products.value[index] = { ...currentProduct.value };
      }
    } else {
      // Uncomment when API is ready
      // const response = await axios.post('/api/Product', currentProduct.value);
      // products.value.push(response.data);

      // For development
      const newId = Math.max(0, ...products.value.map((p) => p.id)) + 1;
      products.value.push({
        ...currentProduct.value,
        id: newId,
        image: "https://placehold.co/100x100", // Default image for new products
      });
    }

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

const deleteProduct = async () => {
  try {
    // Uncomment when API is ready
    // await axios.delete(`/api/Product/${currentProduct.value.id}`);

    // For development
    products.value = products.value.filter(
      (p) => p.id !== currentProduct.value.id
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
          <input
            type="text"
            class="search-input"
            placeholder="Tìm kiếm sản phẩm..."
            v-model="searchTerm"
          />
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
              <th>ID</th>
              <th>Tên sản phẩm</th>
              <th>Danh mục</th>
              <th>Giá</th>
              <th>Tồn kho</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="product in filteredProducts" :key="product.id">
              <td>
                <img
                  :src="product.image"
                  :alt="product.productName"
                  class="product-thumbnail"
                />
              </td>
              <td>{{ product.id }}</td>
              <td>
                <div class="product-name">{{ product.productName }}</div>
                <small class="text-muted"
                  >{{ product.description.substring(0, 50) }}...</small
                >
              </td>
              <td>{{ product.category }}</td>
              <td>{{ formatCurrency(product.price) }}</td>
              <td>
                <span
                  class="badge"
                  :class="product.stock > 10 ? 'bg-success' : 'bg-warning'"
                >
                  {{ product.stock }}
                </span>
              </td>
              <td class="action-buttons">
                <button
                  class="btn btn-sm btn-primary me-1"
                  @click="editProduct(product)"
                >
                  <i class="bi bi-pencil"></i>
                </button>
                <button
                  class="btn btn-sm btn-danger"
                  @click="confirmDeleteProduct(product)"
                >
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
              {{ isEditing ? "Chỉnh sửa sản phẩm" : "Thêm sản phẩm mới" }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveProduct">
              <div class="row">
                <div class="col-md-6">
                  <div class="mb-3">
                    <label class="form-label">Tên sản phẩm</label>
                    <input
                      type="text"
                      class="form-control"
                      v-model="currentProduct.productName"
                      required
                    />
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Danh mục</label>
                    <select
                      class="form-select"
                      v-model="currentProduct.category"
                    >
                      <option v-for="cat in categories" :key="cat" :value="cat">
                        {{ cat }}
                      </option>
                    </select>
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Giá (VNĐ)</label>
                    <input
                      type="number"
                      class="form-control"
                      v-model="currentProduct.price"
                      min="0"
                      required
                    />
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Số lượng tồn kho</label>
                    <input
                      type="number"
                      class="form-control"
                      v-model="currentProduct.stock"
                      min="0"
                      required
                    />
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="mb-3">
                    <label class="form-label">Mô tả sản phẩm</label>
                    <textarea
                      class="form-control"
                      v-model="currentProduct.description"
                      rows="5"
                    ></textarea>
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Ảnh sản phẩm</label>
                    <input type="file" class="form-control" />
                    <small class="text-muted"
                      >Tính năng upload ảnh sẽ được triển khai sau.</small
                    >
                  </div>
                  <div v-if="isEditing && currentProduct.image" class="mb-3">
                    <label class="form-label">Ảnh hiện tại</label>
                    <div class="current-image">
                      <img
                        :src="currentProduct.image"
                        :alt="currentProduct.productName"
                        class="img-fluid"
                      />
                    </div>
                  </div>
                </div>
              </div>
              <div class="text-end mt-3">
                <button
                  type="button"
                  class="btn btn-secondary me-2"
                  data-bs-dismiss="modal"
                >
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
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <p>
              Bạn có chắc chắn muốn xóa sản phẩm
              <strong>{{ currentProduct.productName }}</strong
              >?
            </p>
            <p class="text-danger">Hành động này không thể hoàn tác.</p>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              data-bs-dismiss="modal"
            >
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
