<script setup>
import { ref, computed, onMounted } from "vue";
import bootstrap from "@/utils/bootstrapHelper";
import { productService } from "@/services/productService";

// Search and suggestions state
const search = ref("");
const suggestions = ref([]);
const showSuggestions = ref(false);
let fetchTimeout = null;

// Cart state
const cart = ref([]);
const activeTab = ref("all"); // all, clothing, accessories, etc.
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

// Replace static categories and products with refs
const categories = ref([{ id: "all", name: "Tất cả" }]);
const products = ref([]);

// Sample data for development
const sampleProducts = [
  {
    id: 1,
    productName: "Áo thun nam basic",
    price: 250000,
    category: "tshirt",
    stock: 100,
    image: "https://dosi-in.com/images/detailed/41/lnc_tr%C6%A1n_3.png",
  },
  {
    id: 2,
    productName: "Quần jeans nam slim fit",
    price: 450000,
    category: "pants",
    stock: 50,
    image: "https://vn-test-11.slatic.net/p/9bb2a97169e7673623ade19ccafeaff3.jpg",
  },
  {
    id: 3,
    productName: "Áo sơ mi Minimalist",
    price: 350000,
    category: "shirt",
    stock: 75,
    image: "https://product.hstatic.net/1000360022/product/ao-so-mi-linen-nam-tay-ngan-minimal-collection-form-regular__7__96ae3e35f57049438841a8a8459c336a.jpg",
  },
  {
    id: 4,
    productName: "Váy liền thân",
    price: 550000,
    category: "dress",
    stock: 25,
    image: "https://file.hstatic.net/200000503583/file/mau-vay-lien-than-dep-cardina__20_.jpg_b05b34e6d8e540d693df036aca23fbbd.jpg",
  },
  {
    id: 5,
    productName: "Áo polo nam",
    price: 320000,
    category: "tshirt",
    stock: 60,
    image: "https://4men.com.vn/thumbs/2022/08/ao-polo-slimfit-stripe-color-po083-mau-den-21295-p.jpg",
  },
  {
    id: 6,
    productName: "Quần short kaki",
    price: 250000,
    category: "pants",
    stock: 45,
    image: "https://4men.com.vn/thumbs/2021/02/quan-short-kaki-tron-qs009-19221-p.png",
  },
  {
    id: 7,
    productName: "Thắt lưng da",
    price: 180000,
    category: "accessories",
    stock: 30,
    image: "https://lh4.googleusercontent.com/n6g1hX2jMqfCirizMT5tBNsLhkdP9FjjNMX851fyxNmm8S-6gCRwhqxfx6XNpIeFJy6uStQnZjjBTqupqV-4TjmTJlAhVBKWKLB1aDWYVGDgCHyVt9_0kd_ovBCLXdW9X5PaRpJYhlSxHUpHkiB3Z7w",
  },
  {
    id: 8,
    productName: "Áo khoác denim",
    price: 650000,
    category: "shirt",
    stock: 20,
    image: "https://product.hstatic.net/1000360022/product/z3907027303949_e9fb89295d4938b8c37bec9aecb06c46_c26702be229b4874a4b17bfc711825b9.jpg",
  },
  {
    id: 9,
    productName: "Túi xách nữ",
    price: 450000,
    category: "accessories",
    stock: 15,
    image: "https://louiskimmi.vn/wp-content/uploads/2022/10/z3770072061822_f6866e936dfba3450397e713827657fc.jpg",
  },
  {
    id: 10,
    productName: "Đầm dự tiệc",
    price: 850000,
    category: "dress",
    stock: 10,
    image: "https://cdn.kkfashion.vn/16217-home_default/dam-xoe-du-tiec-nguc-xep-ly-hl19-20.jpg",
  },
];

const sampleCustomers = [
  {
    id: 1,
    name: "Nguyễn Văn An",
    phone: "0912345678",
    email: "an.nguyen@gmail.com",
    address: "12 Nguyễn Trãi, Q1, TP.HCM",
  },
  {
    id: 2,
    name: "Trần Thị Bình",
    phone: "0987654321",
    email: "binh.tran@gmail.com",
    address: "34 Lê Duẩn, Q3, TP.HCM",
  },
  {
    id: 3,
    name: "Lê Hoàng Cường",
    phone: "0938123456",
    email: "cuong.le@gmail.com",
    address: "56 Hai Bà Trưng, Q5, TP.HCM",
  },
  {
    id: 4,
    name: "Phạm Thị Dung",
    phone: "0909123123",
    email: "dung.pham@gmail.com",
    address: "78 Võ Thị Sáu, Q10, TP.HCM",
  },
  {
    id: 5,
    name: "Võ Minh Đức",
    phone: "0977123456",
    email: "duc.vo@gmail.com",
    address: "90 Pasteur, Q1, TP.HCM",
  },
];

// Computed properties
const filteredProducts = computed(() => {
  return products.value;
});

const subtotal = computed(() => {
  return cart.value.reduce((sum, item) => sum + item.price * item.quantity, 0);
});

const totalItems = computed(() => {
  return cart.value.reduce((sum, item) => sum + item.quantity, 0);
});

const totalTax = computed(() => {
  return subtotal.value * 0.08; // 8% VAT
});

const totalDiscount = computed(() => {
  // Calculate both percent and fixed discount
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
  return amountReceived.value - grandTotal.value > 0
    ? amountReceived.value - grandTotal.value
    : 0;
});

// Methods
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

const fetchCustomers = () => {
  if (!customerSearch.value.trim()) {
    customerSuggestions.value = [];
    return;
  }

  try {
    const searchTerm = customerSearch.value.toLowerCase();
    customerSuggestions.value = sampleCustomers.filter(
      (c) =>
        c.name.toLowerCase().includes(searchTerm) ||
        c.phone.includes(searchTerm)
    );
  } catch {
    customerSuggestions.value = [];
  }
};

const selectProduct = (product) => {
  if (product.stock <= 0) {
    alert('Sản phẩm đã hết hàng!');
    return;
  }

  search.value = "";
  showSuggestions.value = false;
  suggestions.value = [];

  const existingItem = cart.value.find((item) => item.id === product.id);
  if (existingItem) {
    if (existingItem.quantity < product.stock) {
      existingItem.quantity += 1;
    } else {
      alert('Số lượng đặt hàng đã đạt giới hạn tồn kho!');
    }
  } else {
    cart.value.push({
      id: product.id,
      productName: product.productName,
      price: product.price,
      quantity: 1,
      image: product.image,
      maxStock: product.stock
    });
  }
};

const selectCustomer = (c) => {
  customer.value = c;
  customerSearch.value = "";
  showCustomerSuggestions.value = false;
};

const removeCustomer = () => {
  customer.value = null;
};

const increaseQuantity = (item) => {
  item.quantity += 1;
};

const decreaseQuantity = (item) => {
  if (item.quantity > 1) {
    item.quantity -= 1;
  }
};

const removeItem = (index) => {
  cart.value.splice(index, 1);
};

const selectCategory = async (categoryId) => {
  activeTab.value = categoryId;
  try {
    if (categoryId === "all") {
      const productsData = await productService.getProducts();
      products.value = productsData.map((product) => ({
        id: product.productId,
        productName: product.productName,
        price: product.price,
        category: product.categoryId,
        status: product.status,
        stock: product.stockQuantity,
        image: product.image || "default-product-image.jpg",
        categoryName: product.categoryName,
        trademark: product.trademark,
        materialName: product.materialName,
        sizeName: product.sizeName,
        colorName: product.colorName,
      }));
    } else {
      const productsData = await productService.getProductsByCategory(categoryId);
      products.value = productsData.$values.map((product) => ({
        id: product.productId,
        productName: product.productName,
        price: product.price,
        category: product.categoryId,
        status: product.status,
        stock: product.stockQuantity,
        image: product.image || "default-product-image.jpg",
        categoryName: product.categoryName,
        trademark: product.trademark,
        materialName: product.materialName,
        sizeName: product.sizeName,
        colorName: product.colorName,
      }));
    }
  } catch (error) {
    console.error("Error fetching products:", error);
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
  }).format(value);
};

const processPayment = () => {
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
      showToast("Số tiền khách đưa phải lớn hơn hoặc bằng tổng thanh toán!", "warning");
      return;
    }
  }

  if (!confirm("Xác nhận đã thanh toán và tạo hóa đơn cho đơn hàng này?")) {
    return;
  }

  const order = {
    orderId: currentOrderId.value,
    customer: customer.value,
    items: cart.value,
    subtotal: subtotal.value,
    tax: totalTax.value,
    discount: totalDiscount.value,
    total: grandTotal.value,
    paymentMethod: paymentMethod.value,
    amountReceived: amountReceived.value,
    change: changeAmount.value,
    note: orderNote.value,
    date: new Date().toISOString(),
  };

  console.log("Processing order:", order);

  const modal = new bootstrap.Modal(
    document.getElementById("paymentSuccessModal")
  );
  modal.show();

  showToast("Thanh toán thành công!", "success");
};

// Toast helper
function showToast(message, type = "info") {
  // Remove existing toast if any
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
  // Reset all order data
  cart.value = [];
  customer.value = null;
  paymentMethod.value = "cash";
  amountReceived.value = 0;
  discountPercent.value = 0;
  discountAmount.value = 0;
  orderNote.value = "";
  currentOrderId.value = "ĐH" + new Date().getTime().toString().slice(-6);

  // Close modal if open
  const modal = bootstrap.Modal.getInstance(
    document.getElementById("paymentSuccessModal")
  );
  if (modal) {
    modal.hide();
  }

  // Switch view to product grid
  showProductGrid.value = true;
};

const fetchInitialData = async () => {
  try {
    // Fetch categories
    const categoriesData = await productService.getCategories();
    categories.value = [
      { id: "all", name: "Tất cả" },
      ...categoriesData.$values.map((cat) => ({
        id: cat.categoryId,
        name: cat.categoryName,
      })),
    ];
    // Fetch all products
    const productsData = await productService.getProducts();
    products.value = productsData.map((product) => ({
      id: product.productId,
      productName: product.productName,
      price: product.price,
      category: product.categoryId,
      status: product.status,
      stock: product.stockQuantity,
      image: product.image || "default-product-image.jpg",
      categoryName: product.categoryName,
      trademark: product.trademark,
      materialName: product.materialName,
      sizeName: product.sizeName,
      colorName: product.colorName,
    }));
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
      <!-- Left side: Products selection -->
      <div class="col-lg-8 mb-4">
        <div class="card shadow-sm">
          <div class="card-header bg-white d-flex justify-content-between align-items-center py-3">
            <ul class="nav nav-pills">
              <li class="nav-item" v-for="category in categories" :key="category.id">
                <a class="nav-link" :class="{ active: activeTab === category.id }" href="#"
                  @click.prevent="selectCategory(category.id)">
                  {{ category.name }}
                </a>
              </li>
            </ul>
            <div class="form-check form-switch">
              <input class="form-check-input" type="checkbox" v-model="showProductGrid" id="viewToggle" />
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
              <input type="text" class="form-control border-start-0" placeholder="Tìm kiếm sản phẩm theo tên..."
                v-model="search" @focus="showSuggestions = true" @input="fetchProducts" @blur="hideSuggestions" />
              <ul v-if="showSuggestions && suggestions.length" class="list-group position-absolute search-suggestions">
                <li v-for="product in suggestions" :key="product.id"
                  class="list-group-item list-group-item-action d-flex align-items-center"
                  @mousedown.prevent="selectProduct(product)">
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
            <div v-if="showProductGrid" class="row row-cols-2 row-cols-md-3 row-cols-lg-4 g-3">
              <div v-for="product in filteredProducts" :key="product.id" class="col">
                <div
                  class="card h-100 product-card"
                  :class="{ 'opacity-50': !product.status, 'pointer-events-none': !product.status }"
                  @click="product.status && selectProduct(product)"
                  :style="!product.status ? 'cursor: not-allowed;' : ''"
                >
                  <img :src="product.image" class="card-img-top product-img" :alt="product.productName" />
                  <div class="card-body">
                    <h6 class="card-title product-title">
                      {{ product.productName }}
                    </h6>
                    <div class="price">{{ formatCurrency(product.price) }}</div>
                    <div> Size {{ product.sizeName }}</div>
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
                          >
                          Đang bán
                          </span>
                          <span
                          v-else
                          class="badge bg-secondary"
                          >
                          Ngưng bán
                          </span>
                        </small>
                    </td>
                    <td>
                      <span class="badge bg-light text-dark">
                        {{
                          product.categoryName
                        }}
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

      <!-- Right side: Cart and checkout -->
      <div class="col-lg-4">
        <div class="card shadow-sm">
          <div class="card-header bg-white py-3">
            <div class="d-flex align-items-center justify-content-between">
              <h5 class="mb-0">Giỏ hàng</h5>
              <span class="badge bg-primary rounded-pill">{{ totalItems }} sản phẩm</span>
            </div>

            <!-- Customer selection -->
            <div class="mt-3 position-relative">
              <div v-if="!customer" class="input-group">
                <input type="text" class="form-control" placeholder="Tìm kiếm khách hàng..." v-model="customerSearch"
                  @focus="showCustomerSuggestions = true" @input="fetchCustomers" @blur="hideCustomerSuggestions" />
                <button class="btn btn-outline-secondary" @click="openNewCustomerModal">
                  <i class="bi bi-person-plus"></i>
                </button>
                <ul v-if="showCustomerSuggestions && customerSuggestions.length"
                  class="list-group position-absolute customer-suggestions">
                  <li v-for="c in customerSuggestions" :key="c.id" class="list-group-item list-group-item-action"
                    @mousedown.prevent="selectCustomer(c)">
                    {{ c.name }} - {{ c.phone }}
                  </li>
                </ul>
              </div>
              <div v-else
                class="selected-customer p-2 border rounded d-flex justify-content-between align-items-center">
                <div>
                  <div>
                    <strong>{{ customer.name }}</strong>
                  </div>
                  <div class="text-muted small">{{ customer.phone }}</div>
                </div>
                <button class="btn btn-sm btn-outline-danger" @click="removeCustomer">
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
                        <button class="btn btn-sm text-danger" @click="removeItem(index)">
                          <i class="bi bi-trash"></i>
                        </button>
                      </div>
                      <div class="d-flex align-items-center justify-content-between mt-2">
                        <div class="item-price">
                          {{ formatCurrency(item.price) }}
                        </div>
                        <div class="quantity-controls">
                          <button class="btn btn-sm btn-outline-secondary" @click="decreaseQuantity(item)">
                            -
                          </button>
                          <span class="mx-2">{{ item.quantity }}</span>
                          <button class="btn btn-sm btn-outline-secondary" @click="increaseQuantity(item)">
                            +
                          </button>
                        </div>
                      </div>
                      <div class="text-end mt-1">
                        <span class="fw-bold">{{
                          formatCurrency(item.price * item.quantity)
                        }}</span>
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

              <!-- Discount section -->
              <div class="discount-section mb-2 pb-2 border-bottom">
                <div class="row align-items-center mb-1">
                  <div class="col-6">Giảm giá (%):</div>
                  <div class="col-6">
                    <input type="number" class="form-control form-control-sm" v-model="discountPercent" min="0"
                      max="100" />
                  </div>
                </div>
                <div class="row align-items-center">
                  <div class="col-6">Giảm tiền:</div>
                  <div class="col-6">
                    <input type="number" class="form-control form-control-sm" v-model="discountAmount" min="0" />
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

              <!-- Payment options -->
              <div class="row mb-3 align-items-center">
                <div class="col-5">Phương thức:</div>
                <div class="col-7">
                  <select class="form-select form-select-sm" v-model="paymentMethod">
                    <option value="cash">Tiền mặt</option>
                    <option value="card">Thẻ ATM/Visa</option>
                    <option value="transfer">Chuyển khoản</option>
                    <option value="momo">Ví MoMo</option>
                    <option value="vnpay">VNPay</option>
                  </select>
                </div>
              </div>

              <div class="row mb-3 align-items-center" v-if="paymentMethod === 'cash'">
                <div class="col-5">Tiền khách đưa:</div>
                <div class="col-7">
                  <input type="number" class="form-control" v-model="amountReceived" />
                </div>
              </div>

              <div class="row mb-3" v-if="paymentMethod === 'cash' && amountReceived > 0">
                <div class="col-5">Tiền thừa:</div>
                <div class="col-7 text-end fw-bold">
                  {{ formatCurrency(changeAmount) }}
                </div>
              </div>

              <div class="mb-3">
                <label class="form-label small">Ghi chú đơn hàng</label>
                <textarea class="form-control form-control-sm" v-model="orderNote" rows="2"></textarea>
              </div>

              <!-- Checkout button -->
              <div class="d-grid gap-2">
                <button class="btn btn-primary btn-lg" @click="processPayment" :disabled="cart.length === 0">
                  <i class="bi bi-cash-stack me-2"></i> Thanh toán
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Payment Success Modal -->
    <div class="modal fade" id="paymentSuccessModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header bg-success text-white">
            <h5 class="modal-title">
              <i class="bi bi-check-circle me-2"></i> Thanh toán thành công
            </h5>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body text-center py-4">
            <i class="bi bi-emoji-smile display-1 text-success mb-3"></i>
            <h4>Đơn hàng #{{ currentOrderId }} đã được thanh toán</h4>
            <p class="mb-0">Tổng giá trị: {{ formatCurrency(grandTotal) }}</p>
            <p class="mb-3">
              Phương thức:
              {{
                paymentMethod === "cash"
                  ? "Tiền mặt"
                  : paymentMethod === "card"
                    ? "Thẻ ATM/Visa"
                    : paymentMethod === "transfer"
                      ? "Chuyển khoản"
                      : paymentMethod === "momo"
                        ? "Ví MoMo"
                        : "VNPay"
              }}
            </p>
            <div class="d-flex justify-content-center gap-2">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
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
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <form>
              <div class="mb-3">
                <label class="form-label">Họ và tên <span class="text-danger">*</span></label>
                <input type="text" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Số điện thoại <span class="text-danger">*</span></label>
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
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
              Hủy
            </button>
            <button type="button" class="btn btn-primary" data-bs-dismiss="modal">
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
</style>
