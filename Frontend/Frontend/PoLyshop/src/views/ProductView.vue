<script setup>
import { ref, onMounted, computed } from "vue";
import axios from "axios";
import bootstrap from "@/utils/bootstrapHelper";

const getBootstrap = () => {
  if (bootstrap && bootstrap.Modal) return bootstrap;
  if (typeof window !== "undefined" && window.bootstrap) return window.bootstrap;
  return null;
};

const products = ref([]);
const categories = ref([]);
const sizes = ref([]);
const colors = ref([]);
const materials = ref([]);

const searchTerm = ref("");
const error = ref(null);
const loading = ref(true);
const isEditing = ref(false);
const isAdding = ref(false);
const selectedProduct = ref(false);

const defaultProductState = () => ({
  id: null,
  productId: null,
  productDetailId: null,
  productName: "",
  price: 0,
  createdAt: new Date().toISOString().slice(0, 10),
  updatedAt: null,
  status: true,
  categoryName: categories.value[0]?.categoryName || "",
  categoryId: categories.value[0]?.categoryId || null,
  image: "",
  tradeMark: "",
  materialName: materials.value[0]?.materialName || "",
  materialId: materials.value[0]?.materialId || null,
  sizeName: "",
  sizeId: null,
  colorName: colors.value[0]?.colorName || "",
  colorId: colors.value[0]?.colorId || null,
  stockQuantity: 0,
  sizeVariants: [
    {
      sizeName: "",
      sizeId: null,
      stockQuantity: 0,
    },
  ],
});

const currentProduct = ref(defaultProductState());

// Xử lý response .NET
const handleApiResponse = (res) => {
  return Array.isArray(res.data) ? res.data : res.data.$values || [];
};

// Lấy danh mục
const fetchCategories = async () => {
  try {
    const res = await axios.get("https://localhost:7055/api/ProductCategory");
    const raw = handleApiResponse(res);
    categories.value = raw.map((c) => ({
      categoryId: c.categoryId,
      categoryName: c.categoryName,
    }));
  } catch (err) {
    error.value = "Không thể lấy danh mục sản phẩm: " + err.message;
  }
};

// Lấy size (đi từ api/Sizes – nếu bạn dùng api/Product/Sizes thì đổi URL)
const fetchSizes = async () => {
  try {
    const res = await axios.get("https://localhost:7055/api/Sizes");
    const raw = handleApiResponse(res);
    sizes.value = raw.map((s) => ({
      sizeId: s.sizeId,
      sizeName: s.sizeName,
    }));
  } catch (err) {
    error.value = "Không thể lấy danh sách size: " + err.message;
  }
};

// Lấy màu
const fetchColors = async () => {
  try {
    const res = await axios.get("https://localhost:7055/api/Colors");
    const raw = handleApiResponse(res);
    colors.value = raw.map((c) => ({
      colorId: c.colorId,
      colorName: c.colorName,
    }));
  } catch (err) {
    error.value = "Không thể lấy danh sách màu sắc: " + err.message;
  }
};

// Lấy chất liệu
const fetchMaterials = async () => {
  try {
    const res = await axios.get("https://localhost:7055/api/Materials");
    const raw = handleApiResponse(res);
    materials.value = raw.map((m) => ({
      materialId: m.materialId,
      materialName: m.materialName,
    }));
  } catch (err) {
    error.value = "Không thể lấy danh sách chất liệu: " + err.message;
  }
};

// Lấy sản phẩm
const fetchProducts = async () => {
  try {
    loading.value = true;
    const res = await axios.get("https://localhost:7055/api/Product");
    const productArray = handleApiResponse(res);

    products.value = productArray.map((p) => {
      const stock = p.stockQuantity ?? 0;
      const originStatus =
        typeof p.status === "boolean" ? p.status : !!p.status;
      const isSelling = stock > 0 && originStatus;

      return {
        productId: p.productId,
        productDetailId: p.productDetailId || null,
        productName: p.productName,
        price: p.price,
        categoryName: p.categoryName || "",
        status: isSelling,
        image:
          p.image ||
          "https://placehold.co/100x100/orange/white?text=No+Image",
        stockQuantity: stock,
        sizeName: p.sizeName || "",
        colorName: p.colorName || "",
        materialName: p.materialName || "",
      };
    });

    loading.value = false;
  } catch (err) {
    error.value = "Không thể lấy dữ liệu sản phẩm: " + err.message;
    loading.value = false;
  }
};

onMounted(() => {
  fetchCategories();
  fetchProducts();
  fetchSizes();
  fetchColors();
  fetchMaterials();
});

const filteredProducts = computed(() => {
  if (!searchTerm.value) return products.value;

  const search = searchTerm.value.toLowerCase();
  return products.value.filter((product) => {
    const nameMatch = (product.productName || "")
      .toLowerCase()
      .includes(search);
    const categoryMatch = (product.categoryName || "")
      .toLowerCase()
      .includes(search);
    const statusString = product.status === true ? "đang bán" : "ngưng bán";
    const statusMatch = statusString.includes(search);
    return nameMatch || categoryMatch || statusMatch;
  });
});

// Mở modal thêm
const openAddModal = () => {
  const bs = getBootstrap();
  const modalElement = bs && bs.Modal
    ? new bs.Modal(document.getElementById("productModal"))
    : new window.bootstrap.Modal(document.getElementById("productModal"));
  modalElement.show();
  selectedProduct.value = false;
  currentProduct.value = defaultProductState();
  isAdding.value = true;
  error.value = null;
};

// Xem chi tiết
const detailsProduct = (product) => {
  currentProduct.value = {
    ...product,
    sizeVariants: [
      {
        sizeName: product.sizeName || "",
        sizeId: sizes.value.find((s) => s.sizeName === product.sizeName)
          ?.sizeId,
        stockQuantity: product.stockQuantity || 0,
      },
    ],
  };
  const modal = new bootstrap.Modal(document.getElementById("productModal"));
  modal.show();
  isEditing.value = false;
  selectedProduct.value = true;
  isAdding.value = false;
};

// Sửa sản phẩm
const editProduct = (product) => {
  isEditing.value = true;
  isAdding.value = false;
  selectedProduct.value = false;

  const mat = materials.value.find((m) => m.materialName === product.materialName);
  const col = colors.value.find((c) => c.colorName === product.colorName);
  const sz = sizes.value.find((s) => s.sizeName === product.sizeName);
  const cat = categories.value.find((c) => c.categoryName === product.categoryName);

  currentProduct.value = {
    ...defaultProductState(),
    productId: product.productId,
    productDetailId: product.productDetailId,
    productName: product.productName,
    price: product.price,
    status: product.status,
    categoryName: product.categoryName,
    categoryId: cat?.categoryId || null,
    image: product.image,
    materialName: product.materialName,
    materialId: mat?.materialId || null,
    sizeName: product.sizeName,
    sizeId: sz?.sizeId || null,
    colorName: product.colorName,
    colorId: col?.colorId || null,
    stockQuantity: product.stockQuantity,
    sizeVariants: [
      {
        sizeName: product.sizeName || "",
        sizeId: sz?.sizeId || null,
        stockQuantity: product.stockQuantity || 0,
      },
    ],
  };
  error.value = null;
  const modal = new bootstrap.Modal(document.getElementById("productModal"));
  modal.show();
};

const addSizeVariant = () => {
  if (!isEditing.value && !isAdding.value) return;
  currentProduct.value.sizeVariants.push({
    sizeName: "",
    sizeId: null,
    stockQuantity: 0,
  });
};

const removeSizeVariant = (index) => {
  if (!isEditing.value && !isAdding.value) return;
  if (currentProduct.value.sizeVariants.length <= 1) return;
  currentProduct.value.sizeVariants.splice(index, 1);
};

// Đọc file -> base64 để PREVIEW (nhưng KHÔNG gửi lên server nữa)
const handleImageChange = (event) => {
  const file = event.target.files?.[0];
  if (!file) return;

  const reader = new FileReader();
  reader.onload = () => {
    currentProduct.value.image = reader.result;
  };
  reader.readAsDataURL(file);
};

// Lưu sản phẩm
const saveProduct = async () => {
  const selectedCategory = categories.value.find(
    (c) => c.categoryName === currentProduct.value.categoryName
  );
  const selectedColor = colors.value.find(
    (c) => c.colorName === currentProduct.value.colorName
  );
  const selectedMaterial = materials.value.find(
    (m) => m.materialName === currentProduct.value.materialName
  );

  if (selectedCategory) {
    currentProduct.value.categoryId = selectedCategory.categoryId;
  }
  if (selectedColor) {
    currentProduct.value.colorId = selectedColor.colorId;
  }
  if (selectedMaterial) {
    currentProduct.value.materialId = selectedMaterial.materialId;
  }

  const totalStock = (currentProduct.value.sizeVariants || []).reduce(
    (sum, v) => sum + Number(v.stockQuantity || 0),
    0
  );
  currentProduct.value.status = totalStock > 0;

  try {
    if (isEditing.value && isAdding.value === false) {
      // CHỈNH SỬA
      const firstVariant =
        currentProduct.value.sizeVariants &&
        currentProduct.value.sizeVariants[0];

      if (!firstVariant) {
        throw new Error("Không có biến thể size được chọn.");
      }

      const selectedSize = sizes.value.find(
        (s) => s.sizeName === firstVariant.sizeName
      );
      if (selectedSize) {
        currentProduct.value.sizeId = selectedSize.sizeId;
      }

      const dataToSend = {
        products: {
          productId: currentProduct.value.productId,
          productName: currentProduct.value.productName,
          price: Number(currentProduct.value.price),
          status: currentProduct.value.status,
          categoryId: currentProduct.value.categoryId,
        },
        productDetails: {
          productDetailId: currentProduct.value.productDetailId,
          productId: currentProduct.value.productId,
          stockQuantity: Number(firstVariant.stockQuantity),
          // QUAN TRỌNG: không gửi base64 nữa để tránh văng server
          image: null,
        },
        sizes: {
          sizeId: currentProduct.value.sizeId,
        },
        colors: {
          colorId: currentProduct.value.colorId,
        },
        materials: {
          materialId: currentProduct.value.materialId,
        },
      };

      await axios.put(
        `https://localhost:7055/api/Product/${currentProduct.value.productId}`,
        dataToSend
      );

    } else {
      // THÊM MỚI
      const validVariants = (currentProduct.value.sizeVariants || []).filter(
        (v) => v.sizeName && Number(v.stockQuantity) >= 0
      );

      if (validVariants.length === 0) {
        throw new Error("Vui lòng thêm ít nhất một size và số lượng hợp lệ.");
      }

      await Promise.all(
        validVariants.map(async (variant) => {
          const selectedSize = sizes.value.find(
            (s) => s.sizeName === variant.sizeName
          );
          const sizeId = selectedSize ? selectedSize.sizeId : null;

          const variantStatus = Number(variant.stockQuantity) > 0;

          return axios.post("https://localhost:7055/api/Product", {
            products: {
              productName: currentProduct.value.productName,
              price: Number(currentProduct.value.price),
              categoryId: currentProduct.value.categoryId,
              status: variantStatus,
            },
            productDetails: {
              stockQuantity: Number(variant.stockQuantity),
              // QUAN TRỌNG: không gửi base64 nữa
              image: null,
            },
            sizes: {
              sizeId: sizeId,
            },
            colors: {
              colorId: currentProduct.value.colorId,
            },
            materials: {
              materialId: currentProduct.value.materialId,
            },
          });
        })
      );
    }

    const bs = getBootstrap();
    const modal = bs && bs.Modal && bs.Modal.getInstance
      ? bs.Modal.getInstance(document.getElementById("productModal"))
      : window.bootstrap?.Modal?.getInstance?.(document.getElementById("productModal"));
    modal && modal.hide();

    window.location.reload();
    currentProduct.value = defaultProductState();
    isEditing.value = false;
    selectedProduct.value = false;
    isAdding.value = false;
    error.value = null;
  } catch (err) {
    console.error(err);
    error.value =
      `Không thể ${isEditing.value ? "cập nhật" : "thêm"} sản phẩm: ` +
      (err.response?.data?.title || err.message);
  }
};

// Xác nhận xóa
const confirmDeleteProduct = (product) => {
  currentProduct.value = { ...product };
  const modal = new bootstrap.Modal(document.getElementById("deleteModal"));
  modal.show();
  isEditing.value = false;
  selectedProduct.value = false;
  isAdding.value = false;
};

// Xóa
const deleteProduct = async () => {
  try {
    await axios.delete(
      `https://localhost:7055/api/Product/${currentProduct.value.productId}`
    );

    await fetchProducts();

    const modal = bootstrap.Modal.getInstance(
      document.getElementById("deleteModal")
    );
    modal && modal.hide();
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
              <th>STT</th>
              <th>Tên sản phẩm</th>
              <th>Phân loại</th>
              <th>Giá</th>
              <th>Tồn kho</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(product, index) in filteredProducts"
              :key="product.productId || index"
            >
              <td>
                <img
                  :src="product.image"
                  :alt="product.productName"
                  class="product-thumbnail"
                />
              </td>
              <td>{{ index + 1 }}</td>
              <td>
                <div class="product-name">{{ product.productName }}</div>
                <div
                  class="text-muted small mt-1"
                  v-if="product.sizeName || product.colorName"
                >
                  Biến thể: **{{ product.sizeName }}** / **{{ product.colorName }}**
                </div>
              </td>
              <td>{{ product.categoryName }}</td>
              <td>{{ formatCurrency(product.price) }}</td>
              <td>
                <span
                  class="badge"
                  :class="product.stockQuantity > 10 ? 'bg-success' : 'bg-warning'"
                >
                  {{ product.stockQuantity }}
                </span>
              </td>
              <td>
                <small
                  class="text-white px-2 py-1 rounded"
                  :style="{
                    backgroundColor: product.status ? '#28a745' : '#dc3545',
                  }"
                >
                  {{ product.status ? "Đang bán" : "Ngưng bán" }}
                </small>
              </td>
              <td class="action-buttons">
                <button
                  class="btn btn-sm btn-info me-1"
                  @click="detailsProduct(product)"
                >
                  <i class="bi bi-eye"></i>
                </button>
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

    <!-- Modal sản phẩm -->
    <div class="modal fade" id="productModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{
                isEditing === true && isAdding === false
                  ? "Chỉnh sửa biến thể"
                  : selectedProduct === true && isEditing === false
                  ? "Chi tiết biến thể"
                  : "Thêm sản phẩm mới"
              }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <div v-if="error" class="alert alert-danger">{{ error }}</div>
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
                      :disabled="!isEditing && !isAdding"
                    />
                  </div>

                  <div class="mb-3">
                    <label class="form-label">Danh mục</label>
                    <select
                      class="form-select"
                      v-model="currentProduct.categoryName"
                      required
                      :disabled="!isEditing && !isAdding"
                    >
                      <option
                        v-for="cat in categories"
                        :key="cat.categoryId"
                        :value="cat.categoryName"
                      >
                        {{ cat.categoryName }}
                      </option>
                    </select>
                  </div>

                  <div class="mb-3">
                    <label class="form-label">Giá (VNĐ)</label>
                    <input
                      type="number"
                      class="form-control"
                      v-model.number="currentProduct.price"
                      min="0"
                      required
                      :disabled="!isEditing && !isAdding"
                    />
                  </div>

                  <div class="mb-3">
                    <label class="form-label">Chất liệu</label>
                    <select
                      class="form-select"
                      v-model="currentProduct.materialName"
                      :disabled="!isEditing && !isAdding"
                      required
                    >
                      <option value="" disabled>Chọn chất liệu</option>
                      <option
                        v-for="mat in materials"
                        :key="mat.materialId"
                        :value="mat.materialName"
                      >
                        {{ mat.materialName }}
                      </option>
                    </select>
                  </div>

                  <div class="mb-3">
                    <label class="form-label">Màu sắc</label>
                    <select
                      class="form-select"
                      v-model="currentProduct.colorName"
                      :disabled="!isEditing && !isAdding"
                      required
                    >
                      <option value="" disabled>Chọn màu sắc</option>
                      <option
                        v-for="cl in colors"
                        :key="cl.colorId"
                        :value="cl.colorName"
                      >
                        {{ cl.colorName }}
                      </option>
                    </select>
                  </div>
                </div>

                <div class="col-md-6">
                  <div
                    class="mb-3 border p-3 rounded bg-light"
                    :class="{'bg-warning-subtle': isEditing}"
                  >
                    <label class="form-label">
                      {{
                        isEditing
                          ? "Chỉnh sửa Size & Tồn kho (Chỉ áp dụng cho bản ghi hiện tại):"
                          : "Thêm các Size & Tồn kho:"
                      }}
                    </label>
                    <div
                      v-for="(variant, idx) in currentProduct.sizeVariants"
                      :key="idx"
                      class="d-flex gap-2 mb-2 align-items-center"
                    >
                      <select
                        class="form-select"
                        v-model="variant.sizeName"
                        :disabled="!isEditing && !isAdding"
                        required
                      >
                        <option value="" disabled>Chọn size</option>
                        <option
                          v-for="sz in sizes"
                          :key="sz.sizeId"
                          :value="sz.sizeName"
                        >
                          {{ sz.sizeName }}
                        </option>
                      </select>
                      <input
                        type="number"
                        class="form-control"
                        v-model.number="variant.stockQuantity"
                        min="0"
                        placeholder="Số lượng"
                        :disabled="!isEditing && !isAdding"
                        required
                      />
                      <button
                        type="button"
                        class="btn btn-outline-danger btn-sm"
                        @click="removeSizeVariant(idx)"
                        v-if="currentProduct.sizeVariants.length > 1 && isAdding"
                        :disabled="!isEditing && !isAdding"
                      >
                        <i class="bi bi-x"></i>
                      </button>
                    </div>
                    <button
                      type="button"
                      class="btn btn-outline-primary btn-sm mt-2"
                      @click="addSizeVariant"
                      :disabled="!isAdding"
                      v-if="isAdding"
                    >
                      + Thêm size khác
                    </button>
                    <div v-if="isEditing" class="alert alert-info mt-2 p-2">
                      **Chế độ Chỉnh sửa:** Chỉ thay đổi được **size và tồn kho**
                      của biến thể đang xem.
                    </div>
                  </div>

                  <div class="mb-3">
                    <label class="form-label">Trạng thái (Hệ thống tự động)</label>
                    <select
                      class="form-select"
                      :value="currentProduct.status"
                      disabled
                      required
                    >
                      <option :value="true">Đang bán</option>
                      <option :value="false">Ngưng bán</option>
                    </select>
                    <small class="text-muted d-block mt-1">
                      Trạng thái được tính tự động dựa trên tổng tồn kho.
                    </small>
                  </div>

                  <div class="mb-3">
                    <label class="form-label">Ảnh sản phẩm</label>
                    <input
                      type="file"
                      class="form-control"
                      :disabled="!isEditing && !isAdding"
                      @change="handleImageChange"
                      accept="image/*"
                    />
                    <small class="text-muted">
                      Ảnh hiện tại chỉ dùng để xem trước, chưa lưu xuống server.
                    </small>
                  </div>

                  <div
                    v-if="
                      currentProduct.image &&
                      (isEditing || isAdding || selectedProduct)
                    "
                    class="mb-3"
                  >
                    <label class="form-label">Ảnh xem trước</label>
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

              <div v-if="isEditing || isAdding" class="text-end mt-3">
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

    <!-- Modal xóa -->
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
              <strong>{{ currentProduct.productName }}</strong>?
              (Tất cả các biến thể Size/Màu/Chất liệu của sản phẩm này sẽ bị xóa)
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
            <button
              type="button"
              class="btn btn-danger"
              @click="deleteProduct"
            >
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
