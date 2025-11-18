<script setup>
import { ref, computed, onMounted } from "vue";
import bootstrap from "@/utils/bootstrapHelper";
import { productService } from "@/services/productService";
import { customerService } from "@/services/customerService";

// Search and suggestions state
const search = ref("");
const suggestions = ref([]);
const showSuggestions = ref(false);
let fetchTimeout = null;

// Cart state
const cart = ref([]);
const activeTab = ref("all");
const showProductGrid = ref(true);
const customer = ref(null);
const paymentMethod = ref("cash");
const customerSearch = ref("");
const customerSuggestions = ref([]);
const showCustomerSuggestions = ref(false);
const amountReceived = ref(0);
const discountPercent = ref(0);
const discountAmount = ref(0);
const orderNote = ref("");
const currentOrderId = ref("ĐH" + new Date().getTime().toString().slice(-6));

// dữ liệu hiển thị trong modal sau khi thanh toán
const paidTotal = ref(0);
const paidChange = ref(0);
const paidOrderCode = ref("");
const paidPaymentMethod = ref("cash");

// VietQR state
const vietQrImage = ref("");
const vietQrLink = ref("");
const pendingPayload = ref(null);

// categories & products
const categories = ref([{ id: "all", name: "Tất cả" }]);
const products = ref([]);

const showError = ref(false);
const errorMessage = ref("");
const isLoading = ref(false);

// Computed
const filteredProducts = computed(() => products.value);

const subtotal = computed(() =>
  cart.value.reduce((sum, item) => sum + item.price * item.quantity, 0)
);

const totalItems = computed(() =>
  cart.value.reduce((sum, item) => sum + item.quantity, 0)
);

const totalTax = computed(() => subtotal.value * 0.08);

const totalDiscount = computed(() => {
  let percentDiscountValue = 0;
  if (discountPercent.value > 0) {
    percentDiscountValue = (subtotal.value * discountPercent.value) / 100;
  }
  return Math.max(percentDiscountValue, discountAmount.value);
});

const grandTotal = computed(() => {
  return subtotal.value + totalTax.value - totalDiscount.value;
});

const changeAmount = computed(() => {
  const change = amountReceived.value - grandTotal.value;
  return change > 0 ? change : 0;
});

// Helpers: chuẩn hóa dữ liệu sản phẩm
function asArray(res) {
  if (Array.isArray(res)) return res;
  if (res && Array.isArray(res.$values)) return res.$values;
  if (res && Array.isArray(res.data)) return res.data;
  return [];
}

function mapProduct(p) {
  return {
    id: p.productId,
    productDetailId: p.productDetailId ?? null,
    productName: p.productName,
    price: p.price,
    category: p.categoryId ?? null,
    status: !!p.status,
    stock: p.stockQuantity ?? 0,
    image: p.image || "default-product-image.jpg",
    categoryName: p.categoryName,
    trademark: p.trademark,
    materialName: p.materialName,
    sizeName: p.sizeName,
    colorName: p.colorName,
    sizeId: p.sizeId ?? null,
    colorId: p.colorId ?? null,
    materialId: p.materialId ?? null,
  };
}

// Search sản phẩm
const fetchProducts = () => {
  clearTimeout(fetchTimeout);
  if (!search.value.trim()) {
    suggestions.value = [];
    return;
  }

  fetchTimeout = setTimeout(() => {
    try {
      const searchTerm = search.value.toLowerCase();
      suggestions.value = products.value.filter((product) =>
        product.productName.toLowerCase().includes(searchTerm)
      );
    } catch (error) {
      console.error("Error searching products:", error);
      suggestions.value = [];
    }
  }, 300);
};

// Search khách
const fetchCustomers = async () => {
  const term = customerSearch.value.trim();
  if (!term) {
    customerSuggestions.value = [];
    showCustomerSuggestions.value = false;
    isLoading.value = false;
    return;
  }

  isLoading.value = true;
  try {
    const response = await customerService.getCustomers();

    const customersArray = (
      Array.isArray(response)
        ? response
        : response && Array.isArray(response.$values)
        ? response.$values
        : response && Array.isArray(response.data)
        ? response.data
        : []
    ).map((c) => ({
      customerId: c.customerId || c.id,
      fullname: c.fullname || c.fullName || c.name || "Không rõ tên",
      phone: c.phone || c.phoneNumber || "",
      email: c.email || "",
      address: c.address || "",
      rankMember: c.rankMember || "",
      point: c.point || 0,
    }));

    customerSuggestions.value = customersArray.filter((c) => {
      const name = (c.fullname || "").toLowerCase();
      const phone = (c.phone || "").toString();
      const q = term.toLowerCase();
      return (name && name.includes(q)) || (phone && phone.includes(term));
    });

    showCustomerSuggestions.value = customerSuggestions.value.length > 0;
  } catch (err) {
    console.error(err);
    showError.value = true;
    errorMessage.value =
      "Không thể tải danh sách khách hàng. Vui lòng thử lại.";
    customerSuggestions.value = [];
    showCustomerSuggestions.value = false;
  } finally {
    isLoading.value = false;
  }
};

// chọn sản phẩm
const selectProduct = (product) => {
  if (!product || typeof product !== "object") {
    showToast("Sản phẩm không hợp lệ", "danger");
    return;
  }

  if (!product.stock || product.stock <= 0) {
    showToast("Sản phẩm đã hết hàng!", "warning");
    return;
  }

  const exist = cart.value.find(
    (i) => i.productDetailId === product.productDetailId
  );
  if (exist) {
    if (exist.quantity < product.stock) {
      exist.quantity += 1;
    } else {
      showToast("Số lượng đặt đã đạt tồn kho!", "warning");
    }
  } else {
    cart.value.push({
      id: product.id || null,
      productDetailId: product.productDetailId || null,
      productName: product.productName || "Sản phẩm không tên",
      price: product.price || 0,
      quantity: 1,
      image: product.image || "default-product-image.jpg",
      maxStock: product.stock,
      sizeId: product.sizeId ?? null,
      colorId: product.colorId ?? null,
      materialId: product.materialId ?? null,
    });
  }

  search.value = "";
  showSuggestions.value = false;
  suggestions.value = [];
};

// chọn khách
const selectCustomer = (c) => {
  customer.value = {
    id: c.customerId,
    fullname: c.fullname,
    phone: c.phone,
    email: c.email,
    address: c.address,
    rankMember: c.rankMember,
    point: c.point,
  };
  customerSearch.value = "";
  showCustomerSuggestions.value = false;
};

const removeCustomer = () => {
  customer.value = null;
};

const increaseQuantity = (item) => {
  if (item.quantity < item.maxStock) item.quantity += 1;
};

const decreaseQuantity = (item) => {
  if (item.quantity > 1) item.quantity -= 1;
};

const removeItem = (index) => {
  cart.value.splice(index, 1);
};

// chọn danh mục
const selectCategory = async (categoryId) => {
  activeTab.value = categoryId;
  try {
    const productsData =
      categoryId === "all"
        ? await productService.getProducts()
        : await productService.getProductsByCategory(categoryId);

    products.value = asArray(productsData).map(mapProduct);
  } catch (error) {
    console.error("Error fetching products:", error);
    products.value = [];
  }
};

const openNewCustomerModal = () => {
  const modal = new bootstrap.Modal(
    document.getElementById("newCustomerModal")
  );
  modal.show();
};

const hideCustomerSuggestions = () => {
  setTimeout(() => {
    showCustomerSuggestions.value = false;
  }, 200);
};

const hideSuggestions = () => {
  setTimeout(() => {
    showSuggestions.value = false;
  }, 200);
};

const formatCurrency = (value) => {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    maximumFractionDigits: 0,
  }).format(value ?? 0);
};

// VietQR
const generateVietQR = async (amount, orderCode) => {
  vietQrImage.value = "";
  vietQrLink.value = "";
  try {
    const payload = {
      accountNo: "19038235614012", // số tài khoản nhận
      accountName: "TRAN DUC MANH", // tên chủ TK
      acqId: "970407", // mã ngân hàng - ví dụ Techcombank
      amount: Math.round(amount),
      addInfo: `Thanh toan ${orderCode}`,
      format: "vietqr_net",
      template: "compact2",
    };

    const res = await fetch("https://api.vietqr.io/v2/generate", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });

    const data = await res.json();
    if (data.code === "00" && data.data) {
      vietQrImage.value = data.data.qrDataURL || "";
      vietQrLink.value = data.data.qrCode || "";
    } else {
      console.error("VietQR error:", data);
      showToast("Không tạo được mã VietQR", "danger");
    }
  } catch (err) {
    console.error("VietQR error:", err);
    showToast("Lỗi khi gọi VietQR", "danger");
  }
};

// build payload checkout
const buildCheckoutPayload = () => {
  return {
    orderCode: currentOrderId.value,
    customerId: customer.value?.id || null,
    cashierId: 1,
    paymentMethod: paymentMethod.value,
    amountReceived:
      paymentMethod.value === "cash"
        ? amountReceived.value
        : grandTotal.value,
    discountAmount: totalDiscount.value,
    discountPercent: discountPercent.value,
    note: orderNote.value,
    items: cart.value.map((i) => ({
      productDetailId: i.productDetailId,
      productId: i.id,
      productName: i.productName,
      quantity: i.quantity,
      unitPrice: i.price,
      totalPrice: i.price * i.quantity,
      sizeId: i.sizeId ?? null,
      colorId: i.colorId ?? null,
      materialId: i.materialId ?? null,
    })),
  };
};

// submit checkout tới API
const submitCheckout = async (payload) => {
  const API_BASE = "https://localhost:7055";

  const res = await fetch(`${API_BASE}/api/Checkout`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });

  if (!res.ok) throw new Error(await res.text());

  const data = await res.json();

  paidTotal.value = data.total ?? grandTotal.value;
  paidChange.value =
    data.change ?? Math.max(0, payload.amountReceived - (data.total ?? 0));
  paidOrderCode.value = data.orderCode ?? payload.orderCode;
  paidPaymentMethod.value = payload.paymentMethod;

  showToast("Thanh toán thành công! Đã trừ tồn kho.", "success");

  try {
    await fetchInitialData();
    await selectCategory(activeTab.value);
  } catch (err) {
    console.error("Error reloading categories/products:", err);
  }

  const qrModal = bootstrap.Modal.getInstance(
    document.getElementById("vietqrModal")
  );
  if (qrModal) qrModal.hide();

  const successModal = new bootstrap.Modal(
    document.getElementById("paymentSuccessModal")
  );
  successModal.show();
};

// xác nhận đã nhận tiền cho chuyển khoản
const confirmTransferPayment = async () => {
  if (!pendingPayload.value) {
    showToast("Không có đơn hàng chờ thanh toán.", "danger");
    return;
  }
  try {
    await submitCheckout(pendingPayload.value);
    pendingPayload.value = null;
  } catch (e) {
    console.error(e);
    showToast("Lỗi khi tạo hóa đơn: " + e.message, "danger");
  }
};

// Thanh toán
const processPayment = async () => {
  if (!customer.value) {
    showToast("Vui lòng chọn khách hàng hoặc thêm mới!", "warning");
    return;
  }

  if (cart.value.length === 0) {
    showToast("Vui lòng thêm sản phẩm vào giỏ hàng", "warning");
    return;
  }

  if (paymentMethod.value === "cash") {
    if (!amountReceived.value || amountReceived.value <= 0) {
      showToast("Vui lòng nhập số tiền khách đưa!", "warning");
      return;
    }
    if (amountReceived.value < grandTotal.value) {
      showToast(
        "Số tiền khách đưa phải lớn hơn hoặc bằng tổng thanh toán!",
        "warning"
      );
      return;
    }
  }

  // kiểm tra productDetailId
  for (const i of cart.value) {
    if (!i.productDetailId) {
      showToast(
        `Thiếu productDetailId cho sản phẩm "${i.productName}". Vui lòng kiểm tra lại dữ liệu sản phẩm.`,
        "danger"
      );
      return;
    }
  }

  if (!confirm("Xác nhận tạo đơn hàng và xử lý thanh toán?")) {
    return;
  }

  const payload = buildCheckoutPayload();

  // nếu là chuyển khoản thì hiển thị VietQR trước, chưa gọi Checkout
  if (paymentMethod.value === "transfer") {
    pendingPayload.value = payload;
    await generateVietQR(grandTotal.value, currentOrderId.value);

    const qrModal = new bootstrap.Modal(
      document.getElementById("vietqrModal")
    );
    qrModal.show();
    return;
  }

  // các phương thức khác: thanh toán ngay
  try {
    await submitCheckout(payload);
  } catch (e) {
    console.error(e);
    showToast("Lỗi khi xử lý thanh toán: " + e.message, "danger");
  }
};

// Toast helper
function showToast(message, type = "info") {
  const old = document.getElementById("poly-toast");
  if (old) old.remove();

  const toast = document.createElement("div");
  toast.id = "poly-toast";
  toast.className = `toast align-items-center text-bg-${type} border-0 position-fixed top-0 end-0 m-3`;
  toast.style.zIndex = 2000;
  toast.setAttribute("role", "alert");
  toast.setAttribute("aria-live", "assertive");
  toast.setAttribute("aria-atomic", "true");
  toast.innerHTML = `
    <div class="d-flex">
      <div class="toast-body">${message}</div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
  `;
  document.body.appendChild(toast);

  const bsToast = new bootstrap.Toast(toast, { delay: 2500 });
  bsToast.show();

  toast.addEventListener("hidden.bs.toast", () => {
    toast.remove();
  });
}

const startNewOrder = () => {
  cart.value = [];
  customer.value = null;
  paymentMethod.value = "cash";
  amountReceived.value = 0;
  discountPercent.value = 0;
  discountAmount.value = 0;
  orderNote.value = "";
  currentOrderId.value = "ĐH" + new Date().getTime().toString().slice(-6);

  paidTotal.value = 0;
  paidChange.value = 0;
  paidOrderCode.value = "";
  paidPaymentMethod.value = "cash";

  const modal = bootstrap.Modal.getInstance(
    document.getElementById("paymentSuccessModal")
  );
  if (modal) modal.hide();

  showProductGrid.value = true;
};

const fetchInitialData = async () => {
  try {
    const categoriesData = await productService.getCategories();
    categories.value = [
      { id: "all", name: "Tất cả" },
      ...asArray(categoriesData).map((cat) => ({
        id: cat.categoryId,
        name: cat.categoryName,
      })),
    ];

    const productsData = await productService.getProducts();
    products.value = asArray(productsData).map(mapProduct);
  } catch (error) {
    console.error("Error fetching initial data:", error);
  }
};

onMounted(() => {
  fetchInitialData();
});
</script>

<template>
  <div class="admin-page">
    <div class="page-header mb-3">
      <h2 class="page-title">Bán hàng tại quầy</h2>
      <div class="order-id">Đơn hàng: #{{ currentOrderId }}</div>
    </div>

    <div class="row">
      <!-- Left: Products -->
      <div class="col-lg-8 mb-4">
        <div class="card shadow-sm">
          <div
            class="card-header bg-white d-flex justify-content-between align-items-center py-3"
          >
            <ul class="nav nav-pills">
              <li class="nav-item" v-for="category in categories" :key="category.id">
                <a
                  class="nav-link"
                  :class="{ active: activeTab === category.id }"
                  href="#"
                  @click.prevent="selectCategory(category.id)"
                >
                  {{ category.name }}
                </a>
              </li>
            </ul>
            <div class="form-check form-switch">
              <input
                class="form-check-input"
                type="checkbox"
                v-model="showProductGrid"
                id="viewToggle"
              />
              <label class="form-check-label" for="viewToggle">
                {{ showProductGrid ? "Hiển thị ô lưới" : "Hiển thị danh sách" }}
              </label>
            </div>
          </div>

          <div class="card-body">
            <!-- Product search -->
            <div class="input-group mb-3 position-relative">
              <span class="input-group-text bg-white border-end-0">
                <i class="bi bi-search"></i>
              </span>
              <input
                type="text"
                class="form-control border-start-0"
                placeholder="Tìm kiếm sản phẩm theo tên..."
                v-model="search"
                @focus="showSuggestions = true"
                @input="fetchProducts"
                @blur="hideSuggestions"
              />
              <ul
                v-if="showSuggestions && suggestions.length"
                class="list-group position-absolute search-suggestions"
              >
                <li
                  v-for="product in suggestions"
                  :key="product.id"
                  class="list-group-item list-group-item-action d-flex align-items-center"
                  @mousedown.prevent="selectProduct(product)"
                >
                  <img :src="product.image" class="product-suggestion-img me-2" />
                  <div>
                    <div>{{ product.productName }}</div>
                    <div class="small text-muted">
                      {{ formatCurrency(product.price) }}
                    </div>
                  </div>
                </li>
              </ul>
            </div>

            <!-- Product grid view -->
            <div
              v-if="showProductGrid"
              class="row row-cols-2 row-cols-md-3 row-cols-lg-4 g-3"
            >
              <div v-for="product in filteredProducts" :key="product.id" class="col">
                <div
                  class="card h-100 product-card"
                  :class="{
                    'opacity-50': !product.status,
                    'pointer-events-none': !product.status,
                  }"
                  @click="product.status && selectProduct(product)"
                  :style="!product.status ? 'cursor: not-allowed;' : ''"
                >
                  <img
                    :src="product.image"
                    class="card-img-top product-img"
                    :alt="product.productName"
                  />
                  <div class="card-body">
                    <h6 class="card-title product-title">
                      {{ product.productName }}
                    </h6>
                    <div class="price">{{ formatCurrency(product.price) }}</div>
                    <div>Size {{ product.sizeName }}</div>
                    <div class="text-muted small">
                      Còn {{ product.stock }} sản phẩm
                    </div>
                    <div v-if="!product.status" class="mt-2">
                      <span class="badge bg-secondary">Ngưng bán</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Product list view -->
            <div v-else class="table-responsive">
              <table class="table table-hover align-middle">
                <thead>
                  <tr>
                    <th>Hình ảnh</th>
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
                      <img :src="product.image" class="product-list-img" />
                    </td>
                    <td>
                      {{ product.productName }}
                      <small>
                        <span
                          v-if="product.status"
                          class="badge bg-success"
                          >Đang bán</span
                        >
                        <span v-else class="badge bg-secondary">Ngưng bán</span>
                      </small>
                    </td>
                    <td>
                      <span class="badge bg-light text-dark">
                        {{ product.categoryName }}
                      </span>
                    </td>
                    <td>{{ formatCurrency(product.price) }}</td>
                    <td>{{ product.stock }}</td>
                    <td>
                      <button
                        class="btn btn-sm btn-primary"
                        @click="selectProduct(product)"
                        :disabled="!product.status"
                      >
                        <i class="bi bi-plus-circle"></i> Thêm
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <!-- Right: Cart -->
      <div class="col-lg-4">
        <div class="card shadow-sm">
          <div class="card-header bg-white py-3">
            <div class="d-flex align-items-center justify-content-between">
              <h5 class="mb-0">Giỏ hàng</h5>
              <span class="badge bg-primary rounded-pill"
                >{{ totalItems }} sản phẩm</span
              >
            </div>

            <!-- Customer selection -->
            <div class="mt-3 position-relative">
              <div v-if="!customer" class="input-group">
                <input
                  type="text"
                  class="form-control"
                  placeholder="Tìm kiếm khách hàng..."
                  v-model="customerSearch"
                  @focus="showCustomerSuggestions = true"
                  @input="fetchCustomers"
                  @blur="hideCustomerSuggestions"
                />
                <button class="btn btn-outline-secondary" @click="openNewCustomerModal">
                  <i class="bi bi-person-plus"></i>
                </button>
                <ul
                  v-if="showCustomerSuggestions && customerSuggestions.length"
                  class="list-group position-absolute customer-suggestions"
                >
                  <li
                    v-for="c in customerSuggestions"
                    :key="c.customerId"
                    class="list-group-item list-group-item-action suggestion-item d-flex align-items-center"
                    @mousedown.prevent="selectCustomer(c)"
                  >
                    <div class="me-2">
                      <i class="bi bi-person-circle fs-4 text-primary"></i>
                    </div>
                    <div class="flex-grow-1">
                      <div class="fw-semibold">{{ c.fullname }}</div>
                      <div class="small text-muted">
                        {{ c.phone }}
                        <span
                          v-if="c.rankMember"
                          class="rank-badge"
                          >{{ c.rankMember }}</span
                        >
                      </div>
                    </div>
                    <div class="text-end small text-muted">Chọn</div>
                  </li>
                </ul>
              </div>
              <div
                v-else
                class="selected-customer p-2 border rounded d-flex justify-content-between align-items-center"
              >
                <div>
                  <div><strong>{{ customer.fullname }}</strong></div>
                  <div class="text-muted small">{{ customer.phone }}</div>
                </div>
                <button
                  class="btn btn-sm btn-outline-danger"
                  @click="removeCustomer"
                >
                  <i class="bi bi-x"></i>
                </button>
              </div>
            </div>
          </div>

          <div class="card-body p-0">
            <!-- Cart items -->
            <div class="cart-items">
              <div v-if="cart.length === 0" class="text-center py-5">
                <i class="bi bi-cart fs-1 text-muted"></i>
                <p class="text-muted">Giỏ hàng trống</p>
              </div>
              <ul v-else class="list-group list-group-flush">
                <li v-for="(item, index) in cart" :key="index" class="list-group-item">
                  <div class="d-flex">
                    <img :src="item.image" class="cart-item-img me-2" />
                    <div class="flex-grow-1">
                      <div class="d-flex justify-content-between align-items-start">
                        <div class="cart-item-name">{{ item.productName }}</div>
                        <button
                          class="btn btn-sm text-danger"
                          @click="removeItem(index)"
                        >
                          <i class="bi bi-trash"></i>
                        </button>
                      </div>
                      <div
                        class="d-flex align-items-center justify-content-between mt-2"
                      >
                        <div class="item-price">
                          {{ formatCurrency(item.price) }}
                        </div>
                        <div class="quantity-controls">
                          <button
                            class="btn btn-sm btn-outline-secondary"
                            @click="decreaseQuantity(item)"
                            :disabled="item.quantity <= 1"
                          >
                            -
                          </button>
                          <span class="mx-2">{{ item.quantity }}</span>
                          <button
                            class="btn btn-sm btn-outline-secondary"
                            @click="increaseQuantity(item)"
                            :disabled="item.quantity >= item.maxStock"
                          >
                            +
                          </button>
                        </div>
                      </div>
                      <div class="text-end mt-1">
                        <span class="fw-bold">
                          {{ formatCurrency(item.price * item.quantity) }}
                        </span>
                      </div>
                    </div>
                  </div>
                </li>
              </ul>
            </div>

            <!-- Order summary -->
            <div class="p-3 border-top">
              <div class="row mb-2">
                <div class="col-6">Tạm tính:</div>
                <div class="col-6 text-end">{{ formatCurrency(subtotal) }}</div>
              </div>
              <div class="row mb-2">
                <div class="col-6">Thuế VAT (8%):</div>
                <div class="col-6 text-end">{{ formatCurrency(totalTax) }}</div>
              </div>

              <div class="discount-section mb-2 pb-2 border-bottom">
                <div class="row align-items-center mb-1">
                  <div class="col-6">Giảm giá (%):</div>
                  <div class="col-6">
                    <input
                      type="number"
                      class="form-control form-control-sm"
                      v-model="discountPercent"
                      min="0"
                      max="100"
                    />
                  </div>
                </div>
                <div class="row align-items-center">
                  <div class="col-6">Giảm tiền:</div>
                  <div class="col-6">
                    <input
                      type="number"
                      class="form-control form-control-sm"
                      v-model="discountAmount"
                      min="0"
                    />
                  </div>
                </div>
                <div class="row mt-1">
                  <div class="col-6">Tổng giảm:</div>
                  <div class="col-6 text-end text-danger">
                    {{ formatCurrency(totalDiscount) }}
                  </div>
                </div>
              </div>

              <div class="row mb-3">
                <div class="col-6 fw-bold">Tổng thanh toán:</div>
                <div class="col-6 text-end fw-bold fs-5">
                  {{ formatCurrency(grandTotal) }}
                </div>
              </div>

              <div class="row mb-3 align-items-center">
                <div class="col-5">Phương thức:</div>
                <div class="col-7">
                  <select
                    class="form-select form-select-sm"
                    v-model="paymentMethod"
                  >
                    <option value="cash">Tiền mặt</option>
                    <option value="card">Thẻ ATM/Visa</option>
                    <option value="transfer">Chuyển khoản</option>
                    <option value="momo">Ví MoMo</option>
                    <option value="vnpay">VNPay</option>
                  </select>
                </div>
              </div>

              <div
                class="row mb-3 align-items-center"
                v-if="paymentMethod === 'cash'"
              >
                <div class="col-5">Tiền khách đưa:</div>
                <div class="col-7">
                  <input
                    type="number"
                    class="form-control"
                    v-model="amountReceived"
                  />
                </div>
              </div>

              <div
                class="row mb-3"
                v-if="paymentMethod === 'cash' && amountReceived > 0"
              >
                <div class="col-5">Tiền thừa:</div>
                <div class="col-7 text-end fw-bold">
                  {{ formatCurrency(changeAmount) }}
                </div>
              </div>

              <div class="mb-3">
                <label class="form-label small">Ghi chú đơn hàng</label>
                <textarea
                  class="form-control form-control-sm"
                  v-model="orderNote"
                  rows="2"
                ></textarea>
              </div>

              <div class="d-grid gap-2">
                <button
                  class="btn btn-primary btn-lg"
                  @click="processPayment"
                  :disabled="cart.length === 0"
                >
                  <i class="bi bi-cash-stack me-2"></i> Thanh toán
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- VietQR Modal -->
    <div class="modal fade" id="vietqrModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Quét mã VietQR để thanh toán</h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body text-center">
            <p>Đơn hàng #{{ currentOrderId }}</p>
            <p>
              Số tiền:
              <strong>{{ formatCurrency(grandTotal) }}</strong>
            </p>
            <div v-if="vietQrImage" class="mb-3">
              <img
                :src="vietQrImage"
                alt="VietQR"
                class="img-fluid rounded"
                style="max-width: 260px"
              />
            </div>
            <div v-else class="text-muted small mb-3">
              Đang tạo mã QR...
            </div>
            <!-- <a
              v-if="vietQrLink"
              :href="vietQrLink"
              target="_blank"
              class="btn btn-outline-primary mb-3"
            >
              Mở VietQR
            </a> 
            <p class="small text-muted">
              Sau khi khách đã chuyển khoản thành công, nhấn
              "Xác nhận đã nhận tiền" để tạo hóa đơn và trừ tồn kho.
            </p>-->
          </div>
          <div class="modal-footer justify-content-between">
            <button
              type="button"
              class="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              Đóng
            </button>
            <button
              type="button"
              class="btn btn-success"
              @click="confirmTransferPayment"
            >
              Xác nhận đã nhận tiền
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Payment Success Modal -->
    <div
      class="modal fade"
      id="paymentSuccessModal"
      tabindex="-1"
      aria-hidden="true"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header bg-success text-white">
            <h5 class="modal-title">
              <i class="bi bi-check-circle me-2"></i> Thanh toán thành công
            </h5>
            <button
              type="button"
              class="btn-close btn-close-white"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body text-center py-4">
            <i class="bi bi-emoji-smile display-1 text-success mb-3"></i>
            <h4>
              Đơn hàng #{{
                paidOrderCode || currentOrderId
              }} đã được thanh toán
            </h4>
            <p class="mb-0">
              Tổng giá trị: {{ formatCurrency(paidTotal || grandTotal) }}
            </p>
            <p class="mb-3">
              Phương thức:
              {{
                paidPaymentMethod === "cash"
                  ? "Tiền mặt"
                  : paidPaymentMethod === "card"
                  ? "Thẻ ATM/Visa"
                  : paidPaymentMethod === "transfer"
                  ? "Chuyển khoản"
                  : paidPaymentMethod === "momo"
                  ? "Ví MoMo"
                  : "VNPay"
              }}
            </p>
            <p class="mb-3">
              Tiền thừa:
              {{ formatCurrency(paidChange || changeAmount) }}
            </p>
            <div class="d-flex justify-content-center gap-2">
              <button
                type="button"
                class="btn btn-secondary"
                data-bs-dismiss="modal"
              >
                <i class="bi bi-printer"></i> In hóa đơn
              </button>
              <button type="button" class="btn btn-primary" @click="startNewOrder">
                <i class="bi bi-plus-circle"></i> Tạo đơn mới
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- New Customer Modal -->
    <div class="modal fade" id="newCustomerModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Thêm khách hàng mới</h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <form>
              <div class="mb-3">
                <label class="form-label"
                  >Họ và tên <span class="text-danger">*</span></label
                >
                <input type="text" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label"
                  >Số điện thoại <span class="text-danger">*</span></label
                >
                <input type="tel" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Email</label>
                <input type="email" class="form-control" />
              </div>
              <div class="mb-3">
                <label class="form-label">Địa chỉ</label>
                <textarea class="form-control" rows="2"></textarea>
              </div>
            </form>
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
              class="btn btn-primary"
              data-bs-dismiss="modal"
            >
              Lưu khách hàng
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
}

.page-title {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 0;
}

.order-id {
  background-color: #f8f9fa;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  font-weight: 600;
}

.search-suggestions {
  top: 100%;
  left: 0;
  right: 0;
  z-index: 1000;
  max-height: 300px;
  overflow-y: auto;
}

.customer-suggestions {
  top: 100%;
  left: 0;
  right: 0;
  z-index: 1000;
  max-height: 200px;
  overflow-y: auto;
}

.product-card {
  cursor: pointer;
  transition: transform 0.3s, box-shadow 0.3s;
}

.product-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.product-img {
  height: 150px;
  object-fit: cover;
}

.product-list-img {
  width: 50px;
  height: 50px;
  object-fit: cover;
  border-radius: 4px;
}

.product-suggestion-img {
  width: 40px;
  height: 40px;
  object-fit: cover;
  border-radius: 4px;
}

.cart-items {
  max-height: 350px;
  overflow-y: auto;
}

.cart-item-img {
  width: 60px;
  height: 60px;
  object-fit: cover;
  border-radius: 4px;
}

.cart-item-name {
  font-weight: 500;
}

.item-price {
  color: #6c757d;
  font-size: 0.9rem;
}

.product-title {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.quantity-controls {
  display: flex;
  align-items: center;
}

.suggestion-item {
  padding: 0.5rem 1rem;
  cursor: pointer;
  transition: background-color 0.3s;
}

.suggestion-item:hover {
  background-color: #f1f1f1;
}

.rank-badge {
  background-color: #007bff;
  color: white;
  border-radius: 12px;
  padding: 0.2rem 0.5rem;
  font-size: 0.75rem;
  margin-left: 0.5rem;
}
</style>
