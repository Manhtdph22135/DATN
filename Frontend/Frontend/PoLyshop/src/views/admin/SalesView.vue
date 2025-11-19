<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý bán hàng</h2>
      <div class="header-actions">
        <button class="btn btn-primary" @click="startNewOrder">
          <i class="bi bi-plus-circle"></i> Tạo đơn mới
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

      <div v-else class="sales-dashboard">
        <div class="row">
          <!-- Product Selection Panel -->
          <div class="col-lg-8">
            <div class="card mb-4">
              <div
                class="card-header d-flex justify-content-between align-items-center"
              >
                <h5 class="mb-0">Chọn sản phẩm</h5>
                <div class="product-search">
                  <div class="input-group">
                    <input
                      type="text"
                      class="form-control"
                      placeholder="Tìm kiếm sản phẩm..."
                      v-model="searchTerm"
                    />
                    <span class="input-group-text">
                      <i class="bi bi-search"></i>
                    </span>
                  </div>
                </div>
              </div>
              <div class="card-body">
                <div class="filter-container mb-3">
                  <div class="filter-tags">
                    <button
                      v-for="category in categories"
                      :key="category"
                      class="btn btn-sm me-2"
                      :class="
                        selectedCategory === category
                          ? 'btn-primary'
                          : 'btn-outline-secondary'
                      "
                      @click="selectCategory(category)"
                    >
                      {{ category }}
                    </button>
                    <button
                      class="btn btn-sm"
                      :class="
                        selectedCategory === ''
                          ? 'btn-primary'
                          : 'btn-outline-secondary'
                      "
                      @click="selectCategory('')"
                    >
                      Tất cả
                    </button>
                  </div>
                </div>

                <div
                  v-if="filteredProducts.length === 0"
                  class="text-center p-4"
                >
                  <p class="text-muted">Không tìm thấy sản phẩm nào.</p>
                </div>

                <div v-else class="product-grid">
                  <div
                    v-for="product in filteredProducts"
                    :key="product.id"
                    class="product-item"
                    @click="addToCart(product)"
                    :class="{ 'out-of-stock': product.stock <= 0 }"
                  >
                    <div class="product-image">
                      <img :src="product.image" :alt="product.name" />
                      <span
                        v-if="product.stock <= 0"
                        class="out-of-stock-label"
                      >
                        Hết hàng
                      </span>
                    </div>
                    <div class="product-info">
                      <div class="product-name">{{ product.name }}</div>
                      <div class="product-price">
                        {{ formatCurrency(product.price) }}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Recently Added Products -->
            <div class="card">
              <div class="card-header">
                <h5 class="mb-0">Sản phẩm thêm gần đây</h5>
              </div>
              <div class="card-body">
                <div class="recent-products">
                  <div
                    v-for="product in recentlyAddedProducts"
                    :key="product.id"
                    class="recent-product"
                    @click="addToCart(product)"
                  >
                    <img
                      :src="product.image"
                      :alt="product.name"
                      class="recent-product-image"
                    />
                    <div class="recent-product-info">
                      <div class="recent-product-name">{{ product.name }}</div>
                      <div class="recent-product-price">
                        {{ formatCurrency(product.price) }}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Cart Panel -->
          <div class="col-lg-4">
            <div class="card cart-panel">
              <div class="card-header">
                <h5 class="mb-0">Giỏ hàng</h5>
              </div>
              <div class="card-body p-0">
                <div v-if="cart.length === 0" class="text-center p-4">
                  <i
                    class="bi bi-cart text-muted"
                    style="font-size: 2.5rem"
                  ></i>
                  <p class="mt-3 text-muted">Giỏ hàng trống</p>
                </div>
                <div v-else>
                  <div class="cart-items">
                    <div
                      v-for="(item, index) in cart"
                      :key="item.id"
                      class="cart-item"
                    >
                      <div class="item-image">
                        <img :src="item.image" :alt="item.name" />
                      </div>
                      <div class="item-details">
                        <div class="item-name">{{ item.name }}</div>
                        <div class="item-price">
                          {{ formatCurrency(item.price) }}
                        </div>
                      </div>
                      <div class="item-quantity">
                        <button
                          class="btn btn-sm btn-outline-secondary"
                          @click="decrementItem(index)"
                        >
                          <i class="bi bi-dash"></i>
                        </button>
                        <span class="quantity">{{ item.quantity }}</span>
                        <button
                          class="btn btn-sm btn-outline-secondary"
                          @click="incrementItem(index)"
                        >
                          <i class="bi bi-plus"></i>
                        </button>
                      </div>
                      <div class="item-total">
                        {{ formatCurrency(item.price * item.quantity) }}
                      </div>
                      <button
                        class="btn btn-sm btn-danger item-remove"
                        @click="removeFromCart(index)"
                      >
                        <i class="bi bi-x"></i>
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <div class="card-footer">
                <div class="cart-summary">
                  <div class="summary-row">
                    <span>Tạm tính:</span>
                    <span>{{ formatCurrency(subtotal) }}</span>
                  </div>
                  <div class="summary-row">
                    <span>Giảm giá:</span>
                    <div class="discount-input">
                      <input
                        type="number"
                        class="form-control form-control-sm"
                        v-model="discount"
                        min="0"
                      />
                      <select
                        class="form-select form-select-sm"
                        v-model="discountType"
                      >
                        <option value="percent">%</option>
                        <option value="amount">VNĐ</option>
                      </select>
                    </div>
                  </div>
                  <div class="summary-row">
                    <span>Thuế ({{ taxRate * 100 }}%):</span>
                    <span>{{ formatCurrency(taxAmount) }}</span>
                  </div>
                  <div class="summary-row total-row">
                    <span>Tổng cộng:</span>
                    <span>{{ formatCurrency(total) }}</span>
                  </div>
                </div>

                <div class="payment-actions">
                  <div class="customer-select mb-3">
                    <select class="form-select" v-model="selectedCustomerId">
                      <option value="">-- Chọn khách hàng --</option>
                      <option
                        v-for="customer in customers"
                        :key="customer.id"
                        :value="customer.id"
                      >
                        {{ customer.name }} ({{ customer.phone }})
                      </option>
                    </select>
                  </div>
                  <div class="payment-methods mb-3">
                    <div class="form-check form-check-inline">
                      <input
                        class="form-check-input"
                        type="radio"
                        id="cash"
                        value="cash"
                        v-model="paymentMethod"
                      />
                      <label class="form-check-label" for="cash">
                        <i class="bi bi-cash"></i> Tiền mặt
                      </label>
                    </div>
                    <div class="form-check form-check-inline">
                      <input
                        class="form-check-input"
                        type="radio"
                        id="card"
                        value="card"
                        v-model="paymentMethod"
                      />
                      <label class="form-check-label" for="card">
                        <i class="bi bi-credit-card"></i> Thẻ
                      </label>
                    </div>
                    <div class="form-check form-check-inline">
                      <input
                        class="form-check-input"
                        type="radio"
                        id="transfer"
                        value="transfer"
                        v-model="paymentMethod"
                      />
                      <label class="form-check-label" for="transfer">
                        <i class="bi bi-bank"></i> Chuyển khoản
                      </label>
                    </div>
                  </div>
                  <div class="d-grid gap-2">
                    <button
                      class="btn btn-lg btn-success"
                      @click="processOrder"
                      :disabled="cart.length === 0"
                    >
                      <i class="bi bi-check-circle"></i> Thanh toán
                    </button>
                    <button
                      class="btn btn-outline-danger"
                      @click="clearCart"
                      :disabled="cart.length === 0"
                    >
                      <i class="bi bi-trash"></i> Xóa giỏ hàng
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Order Complete Modal -->
    <div
      class="modal fade"
      id="orderCompleteModal"
      tabindex="-1"
      aria-hidden="true"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header bg-success text-white">
            <h5 class="modal-title">
              <i class="bi bi-check-circle-fill me-2"></i> Đơn hàng hoàn thành
            </h5>
            <button
              type="button"
              class="btn-close btn-close-white"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <div class="text-center mb-4">
              <div class="order-complete-icon">
                <i class="bi bi-check-circle-fill"></i>
              </div>
              <h4>Cảm ơn!</h4>
              <p>Đơn hàng đã được tạo thành công</p>
              <h5 class="mt-3">
                Mã đơn hàng:
                <span class="text-primary">#{{ currentOrderId }}</span>
              </h5>
            </div>

            <div class="order-summary">
              <div class="summary-row">
                <span>Tổng cộng:</span>
                <span class="fw-bold">{{ formatCurrency(total) }}</span>
              </div>
              <div class="summary-row">
                <span>Phương thức thanh toán:</span>
                <span>{{ getPaymentMethodText(paymentMethod) }}</span>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              Đóng
            </button>
            <button type="button" class="btn btn-primary" @click="printReceipt">
              <i class="bi bi-printer"></i> In hóa đơn
            </button>
            <button
              type="button"
              class="btn btn-success"
              data-bs-dismiss="modal"
              @click="startNewOrder"
            >
              <i class="bi bi-plus-circle"></i> Đơn hàng mới
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Barcode Scanner Modal -->
    <div
      class="modal fade"
      id="barcodeScannerModal"
      tabindex="-1"
      aria-hidden="true"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Quét mã vạch</h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body text-center">
            <div class="barcode-scanner-container">
              <div class="scanner-placeholder">
                <i class="bi bi-upc-scan"></i>
                <p>Camera quét mã vạch sẽ hiển thị ở đây</p>
                <small class="text-muted"
                  >Tính năng này sẽ được triển khai sau</small
                >
              </div>
            </div>
            <div class="manual-entry mt-3">
              <div class="input-group">
                <input
                  type="text"
                  class="form-control"
                  placeholder="Nhập mã sản phẩm..."
                  v-model="barcodeInput"
                />
                <button class="btn btn-primary" @click="searchByBarcode">
                  Tìm kiếm
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import axios from "axios";
import * as bootstrap from "bootstrap";

export default {
  name: "SalesView",
  setup() {
    const loading = ref(true);
    const error = ref(null);
    const searchTerm = ref("");
    const selectedCategory = ref("");
    const products = ref([]);
    const cart = ref([]);
    const customers = ref([]);
    const selectedCustomerId = ref("");
    const discount = ref(0);
    const discountType = ref("percent");
    const taxRate = ref(0.08);
    const paymentMethod = ref("cash");
    const currentOrderId = ref(10001);
    const barcodeInput = ref("");

    // Categories from products
    const categories = computed(() => {
      const uniqueCategories = new Set(products.value.map((p) => p.category));
      return Array.from(uniqueCategories);
    });

    // Recently added products (just the last 5 products for demo)
    const recentlyAddedProducts = computed(() => {
      return [...products.value].slice(0, 5);
    });

    // Sample data for development
    const sampleProducts = [
      {
        id: 1,
        name: "Áo thun Unisex Cotton",
        price: 250000,
        category: "Áo thun",
        image: "https://placehold.co/100x100",
        stock: 100,
        barcode: "P00001",
      },
      {
        id: 2,
        name: "Quần Jeans Slim Fit",
        price: 450000,
        category: "Quần jeans",
        image: "https://placehold.co/100x100",
        stock: 50,
        barcode: "P00002",
      },
      {
        id: 3,
        name: "Áo sơ mi Oxford",
        price: 350000,
        category: "Áo sơ mi",
        image: "https://placehold.co/100x100",
        stock: 75,
        barcode: "P00003",
      },
      {
        id: 4,
        name: "Áo khoác Bomber",
        price: 650000,
        category: "Áo khoác",
        image: "https://placehold.co/100x100",
        stock: 30,
        barcode: "P00004",
      },
      {
        id: 5,
        name: "Đầm suông dáng A",
        price: 550000,
        category: "Đầm",
        image: "https://placehold.co/100x100",
        stock: 45,
        barcode: "P00005",
      },
      {
        id: 6,
        name: "Quần short kaki",
        price: 280000,
        category: "Quần short",
        image: "https://placehold.co/100x100",
        stock: 60,
        barcode: "P00006",
      },
      {
        id: 7,
        name: "Áo sơ mi kẻ sọc",
        price: 320000,
        category: "Áo sơ mi",
        image: "https://placehold.co/100x100",
        stock: 0,
        barcode: "P00007",
      },
      {
        id: 8,
        name: "Quần jogger thể thao",
        price: 300000,
        category: "Quần dài",
        image: "https://placehold.co/100x100",
        stock: 80,
        barcode: "P00008",
      },
    ];

    // Sample customers
    const sampleCustomers = [
      {
        id: 1,
        name: "Nguyễn Văn A",
        phone: "0901234567",
        membership: "gold",
      },
      {
        id: 2,
        name: "Trần Thị B",
        phone: "0911234567",
        membership: "silver",
      },
      {
        id: 3,
        name: "Lê Văn C",
        phone: "0921234567",
        membership: "bronze",
      },
    ];

    // Filtered products based on search and category
    const filteredProducts = computed(() => {
      let result = products.value;

      // Filter by category if selected
      if (selectedCategory.value) {
        result = result.filter((p) => p.category === selectedCategory.value);
      }

      // Filter by search term
      if (searchTerm.value) {
        const term = searchTerm.value.toLowerCase();
        result = result.filter(
          (p) =>
            p.name.toLowerCase().includes(term) ||
            p.barcode.toLowerCase().includes(term)
        );
      }

      return result;
    });

    // Cart calculations
    const subtotal = computed(() => {
      return cart.value.reduce(
        (sum, item) => sum + item.price * item.quantity,
        0
      );
    });

    const discountAmount = computed(() => {
      if (discountType.value === "percent") {
        return (subtotal.value * discount.value) / 100;
      } else {
        return Number(discount.value);
      }
    });

    const taxAmount = computed(() => {
      return (subtotal.value - discountAmount.value) * taxRate.value;
    });

    const total = computed(() => {
      return subtotal.value - discountAmount.value + taxAmount.value;
    });

    // Functions
    const fetchProducts = async () => {
      try {
        loading.value = true;
        // Uncomment when API is ready
        // const response = await axios.get('/api/products');
        // products.value = response.data;

        // For development
        setTimeout(() => {
          products.value = sampleProducts;
          customers.value = sampleCustomers;
          loading.value = false;
        }, 500);
      } catch (err) {
        error.value = "Không thể tải dữ liệu sản phẩm: " + err.message;
        loading.value = false;
      }
    };

    const selectCategory = (category) => {
      selectedCategory.value = category;
    };

    const addToCart = (product) => {
      if (product.stock <= 0) return;

      const existingItem = cart.value.find((item) => item.id === product.id);
      if (existingItem) {
        // Don't exceed available stock
        if (existingItem.quantity < product.stock) {
          existingItem.quantity++;
        }
      } else {
        cart.value.push({
          id: product.id,
          name: product.name,
          price: product.price,
          quantity: 1,
          image: product.image,
        });
      }
    };

    const incrementItem = (index) => {
      const item = cart.value[index];
      const product = products.value.find((p) => p.id === item.id);

      // Don't exceed available stock
      if (item.quantity < product.stock) {
        item.quantity++;
      }
    };

    const decrementItem = (index) => {
      if (cart.value[index].quantity > 1) {
        cart.value[index].quantity--;
      } else {
        removeFromCart(index);
      }
    };

    const removeFromCart = (index) => {
      cart.value.splice(index, 1);
    };

    const clearCart = () => {
      cart.value = [];
      discount.value = 0;
      selectedCustomerId.value = "";
    };

    const processOrder = async () => {
      try {
        // Uncomment when API is ready
        // const orderData = {
        //   items: cart.value,
        //   customerId: selectedCustomerId.value || null,
        //   subtotal: subtotal.value,
        //   discount: discountAmount.value,
        //   tax: taxAmount.value,
        //   total: total.value,
        //   paymentMethod: paymentMethod.value,
        //   createdAt: new Date()
        // };
        // const response = await axios.post('/api/orders', orderData);
        // currentOrderId.value = response.data.id;

        // For development
        currentOrderId.value = Math.floor(10000 + Math.random() * 90000);

        // Show success modal
        const modal = new bootstrap.Modal(
          document.getElementById("orderCompleteModal")
        );
        modal.show();
      } catch (err) {
        error.value = "Không thể xử lý đơn hàng: " + err.message;
      }
    };

    const printReceipt = () => {
      console.log("Print receipt for order:", currentOrderId.value);
      // This would connect to a receipt printer API in a real implementation
      alert("Tính năng in hóa đơn sẽ được triển khai sau");
    };

    const startNewOrder = () => {
      clearCart();
      // Close any open modals
      const orderModal = bootstrap.Modal.getInstance(
        document.getElementById("orderCompleteModal")
      );
      if (orderModal) {
        orderModal.hide();
      }
    };

    const searchByBarcode = () => {
      if (!barcodeInput.value) return;

      const product = products.value.find(
        (p) => p.barcode.toLowerCase() === barcodeInput.value.toLowerCase()
      );

      if (product) {
        addToCart(product);

        // Close modal
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("barcodeScannerModal")
        );
        if (modal) {
          modal.hide();
        }

        barcodeInput.value = "";
      } else {
        alert("Không tìm thấy sản phẩm với mã này");
      }
    };

    const formatCurrency = (value) => {
      return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
      }).format(value);
    };

    const getPaymentMethodText = (method) => {
      switch (method) {
        case "cash":
          return "Tiền mặt";
        case "card":
          return "Thẻ tín dụng/ghi nợ";
        case "transfer":
          return "Chuyển khoản";
        default:
          return "Không xác định";
      }
    };

    onMounted(() => {
      fetchProducts();

      // Check if Bootstrap JS is loaded for modals
      if (typeof bootstrap === "undefined") {
        console.error("Bootstrap JS is not loaded. Modals will not work.");
      }
    });

    return {
      loading,
      error,
      products,
      searchTerm,
      selectedCategory,
      categories,
      cart,
      customers,
      selectedCustomerId,
      discount,
      discountType,
      taxRate,
      paymentMethod,
      currentOrderId,
      barcodeInput,
      recentlyAddedProducts,
      filteredProducts,
      subtotal,
      discountAmount,
      taxAmount,
      total,
      selectCategory,
      addToCart,
      incrementItem,
      decrementItem,
      removeFromCart,
      clearCart,
      processOrder,
      printReceipt,
      startNewOrder,
      searchByBarcode,
      formatCurrency,
      getPaymentMethodText,
    };
  },
};
</script>

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

.content-container {
  background-color: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.sales-dashboard {
  display: flex;
  flex-direction: column;
}

.product-search {
  min-width: 280px;
}

.filter-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.product-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
  gap: 12px;
  max-height: 400px;
  overflow-y: auto;
}

.product-item {
  border: 1px solid #ddd;
  border-radius: 6px;
  overflow: hidden;
  cursor: pointer;
  transition: transform 0.2s ease;
}

.product-item:hover {
  transform: translateY(-3px);
  box-shadow: 0 5px 10px rgba(0, 0, 0, 0.1);
}

.product-item.out-of-stock {
  opacity: 0.7;
}

.product-image {
  height: 100px;
  position: relative;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.out-of-stock-label {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  color: white;
  display: flex;
  justify-content: center;
  align-items: center;
  font-weight: bold;
}

.product-info {
  padding: 8px;
  text-align: center;
}

.product-name {
  font-size: 0.9rem;
  margin-bottom: 5px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.product-price {
  font-weight: bold;
  color: #e53935;
}

.recent-products {
  display: flex;
  overflow-x: auto;
  gap: 15px;
  padding: 10px 0;
}

.recent-product {
  min-width: 100px;
  border: 1px solid #eee;
  border-radius: 6px;
  padding: 5px;
  cursor: pointer;
  transition: transform 0.2s ease;
}

.recent-product:hover {
  transform: translateY(-3px);
}

.recent-product-image {
  width: 100%;
  height: 80px;
  object-fit: cover;
  border-radius: 4px;
}

.recent-product-info {
  padding: 5px 0;
  text-align: center;
}

.recent-product-name {
  font-size: 0.8rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.recent-product-price {
  font-size: 0.8rem;
  font-weight: bold;
  color: #e53935;
}

.cart-panel {
  position: sticky;
  top: 20px;
}

.cart-items {
  max-height: 400px;
  overflow-y: auto;
}

.cart-item {
  display: flex;
  align-items: center;
  padding: 10px 15px;
  border-bottom: 1px solid #eee;
}

.item-image {
  width: 50px;
  height: 50px;
  margin-right: 10px;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 4px;
}

.item-details {
  flex-grow: 1;
  min-width: 0;
}

.item-name {
  font-size: 0.9rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.item-price {
  font-size: 0.8rem;
  color: #777;
}

.item-quantity {
  display: flex;
  align-items: center;
  margin: 0 10px;
}

.quantity {
  margin: 0 8px;
  min-width: 20px;
  text-align: center;
}

.item-total {
  font-weight: bold;
  margin-right: 10px;
  white-space: nowrap;
}

.item-remove {
  padding: 0.25rem 0.5rem;
}

.cart-summary {
  padding: 15px;
  border-bottom: 1px solid #eee;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.total-row {
  font-size: 1.2rem;
  font-weight: bold;
  margin-top: 10px;
  border-top: 1px solid #eee;
  padding-top: 10px;
}

.discount-input {
  display: flex;
  max-width: 150px;
}

.payment-actions {
  padding: 15px;
}

.order-complete-icon {
  font-size: 4rem;
  color: #28a745;
  margin-bottom: 15px;
}

.barcode-scanner-container {
  height: 200px;
  border: 2px dashed #ccc;
  border-radius: 5px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.scanner-placeholder {
  text-align: center;
  color: #777;
}

.scanner-placeholder i {
  font-size: 3rem;
  margin-bottom: 10px;
}

.order-summary {
  border: 1px solid #eee;
  border-radius: 5px;
  padding: 15px;
  margin-top: 20px;
}

@media (max-width: 992px) {
  .cart-panel {
    position: static;
    margin-top: 20px;
  }
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: stretch;
  }

  .product-grid {
    grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  }
}
</style>
