<script setup>
import { ref, onMounted } from "vue";

// Trạng thái và dữ liệu
const isLoading = ref(true);
const orders = ref([]);
const selectedOrder = ref(null);
const showOrderDetail = ref(false);
const errorMessage = ref("");

// Giả lập dữ liệu đơn hàng
const mockOrders = [
  {
    id: "DH001",
    date: "2024-05-15",
    status: "Đã giao hàng",
    total: 1450000,
    paymentMethod: "COD",
    items: [
      {
        id: 1,
        name: "Áo thun POLY Classic",
        quantity: 2,
        price: 350000,
        image: "https://placehold.co/80x80",
      },
      {
        id: 2,
        name: "Quần jean đen",
        quantity: 1,
        price: 750000,
        image: "https://placehold.co/80x80",
      },
    ],
    address: "123 Đường Láng, Đống Đa, Hà Nội",
    trackingCode: "POLY124578",
  },
  {
    id: "DH002",
    date: "2024-04-28",
    status: "Đang giao hàng",
    total: 550000,
    paymentMethod: "Chuyển khoản",
    items: [
      {
        id: 3,
        name: "Áo sơ mi trắng",
        quantity: 1,
        price: 550000,
        image: "https://placehold.co/80x80",
      },
    ],
    address: "45 Nguyễn Trãi, Thanh Xuân, Hà Nội",
    trackingCode: "POLY896543",
  },
  {
    id: "DH003",
    date: "2024-03-10",
    status: "Đã hủy",
    total: 1200000,
    paymentMethod: "COD",
    items: [
      {
        id: 4,
        name: "Áo khoác bomber",
        quantity: 1,
        price: 1200000,
        image: "https://placehold.co/80x80",
      },
    ],
    address: "78 Lê Trọng Tấn, Thanh Xuân, Hà Nội",
    trackingCode: "",
  },
];

// Tải dữ liệu đơn hàng khi component được mount
onMounted(async () => {
  try {
    // Giả lập delay load data
    await new Promise((resolve) => setTimeout(resolve, 800));

    // Lấy đơn hàng từ localStorage
    const userOrders = JSON.parse(localStorage.getItem("userOrders") || "[]");

    // Kết hợp dữ liệu từ localStorage với dữ liệu mẫu
    // Chỉ lấy dữ liệu mẫu nếu không có dữ liệu từ localStorage
    orders.value = userOrders.length > 0 ? userOrders : mockOrders;

    // Chuyển đổi status thành tiếng Việt và định dạng
    orders.value = orders.value.map((order) => {
      // Nếu là đơn từ localStorage, chuyển đổi trạng thái sang tiếng Việt
      if (order.status === "processing") {
        order.status = "Chờ xác nhận";
      }

      // Chuyển đổi phương thức thanh toán
      if (order.payment && order.payment.method) {
        switch (order.payment.method) {
          case "cod":
            order.paymentMethod = "COD";
            break;
          case "bank":
            order.paymentMethod = "Chuyển khoản";
            break;
          case "momo":
            order.paymentMethod = "Ví MoMo";
            break;
          default:
            order.paymentMethod = order.payment.method;
        }
      }

      // Tạo địa chỉ đầy đủ từ thông tin khách hàng nếu có
      if (order.customer) {
        order.address = `${order.customer.address}, ${order.customer.district}, ${order.customer.province}`;
      }

      return order;
    });

    isLoading.value = false;
  } catch (error) {
    console.error("Error loading orders:", error);
    errorMessage.value = "Không thể tải dữ liệu đơn hàng.";
    isLoading.value = false;
  }
});

// Xử lý hiển thị chi tiết đơn hàng
function showOrderDetails(order) {
  selectedOrder.value = order;
  showOrderDetail.value = true;
}

// Đóng chi tiết đơn hàng
function closeOrderDetails() {
  showOrderDetail.value = false;
}

// Format tiền tệ VND
function formatCurrency(value) {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    minimumFractionDigits: 0,
  }).format(value);
}

// Lấy class CSS cho trạng thái đơn hàng
function getStatusClass(status) {
  switch (status) {
    case "Đã giao hàng":
      return "status-delivered";
    case "Đang giao hàng":
      return "status-shipping";
    case "Đã hủy":
      return "status-cancelled";
    case "Chờ xác nhận":
      return "status-pending";
    default:
      return "";
  }
}
</script>

<template>
  <div class="orders-page">
    <div class="orders-container">
      <h1 class="page-title">Đơn hàng của tôi</h1>

      <div class="alert alert-danger" v-if="errorMessage">
        {{ errorMessage }}
      </div>

      <!-- Hiển thị danh sách đơn hàng -->
      <div class="orders-list" v-if="!isLoading">
        <div v-if="orders.length === 0" class="empty-orders">
          <i class="bi bi-bag-x empty-icon"></i>
          <p>Bạn chưa có đơn hàng nào</p>
          <router-link to="/shop" class="shop-now-btn"
            >Mua sắm ngay</router-link
          >
        </div>

        <div v-else class="order-items">
          <div
            v-for="order in orders"
            :key="order.id"
            class="order-item"
            @click="showOrderDetails(order)"
          >
            <div class="order-header">
              <div class="order-id">
                Mã đơn hàng: <strong>{{ order.id }}</strong>
              </div>
              <div :class="['order-status', getStatusClass(order.status)]">
                {{ order.status }}
              </div>
            </div>

            <div class="order-date">
              Ngày đặt: {{ new Date(order.date).toLocaleDateString("vi-VN") }}
            </div>

            <div class="order-summary">
              <div class="order-items-count">
                {{ order.items.reduce((sum, item) => sum + item.quantity, 0) }}
                sản phẩm
              </div>
              <div class="order-total">{{ formatCurrency(order.total) }}</div>
            </div>

            <button class="view-details-btn">
              Xem chi tiết <i class="bi bi-chevron-right"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Loading spinner -->
      <div v-if="isLoading" class="loading-container">
        <div class="spinner"></div>
        <p>Đang tải đơn hàng...</p>
      </div>

      <!-- Chi tiết đơn hàng modal -->
      <div class="order-detail-modal" v-if="showOrderDetail">
        <div class="modal-overlay" @click="closeOrderDetails"></div>
        <div class="modal-container">
          <div class="modal-header">
            <h2>Chi tiết đơn hàng #{{ selectedOrder.id }}</h2>
            <button class="close-btn" @click="closeOrderDetails">
              <i class="bi bi-x"></i>
            </button>
          </div>

          <div class="modal-body">
            <div class="order-info-section">
              <div class="info-group">
                <span class="info-label">Trạng thái:</span>
                <span
                  :class="['info-value', getStatusClass(selectedOrder.status)]"
                >
                  {{ selectedOrder.status }}
                </span>
              </div>

              <div class="info-group">
                <span class="info-label">Ngày đặt hàng:</span>
                <span class="info-value">{{
                  new Date(selectedOrder.date).toLocaleDateString("vi-VN")
                }}</span>
              </div>

              <div class="info-group">
                <span class="info-label">Phương thức thanh toán:</span>
                <span class="info-value">{{
                  selectedOrder.paymentMethod
                }}</span>
              </div>

              <div class="info-group">
                <span class="info-label">Địa chỉ giao hàng:</span>
                <span class="info-value">{{ selectedOrder.address }}</span>
              </div>

              <div class="info-group" v-if="selectedOrder.trackingCode">
                <span class="info-label">Mã vận đơn:</span>
                <span class="info-value tracking-code">{{
                  selectedOrder.trackingCode
                }}</span>
              </div>
            </div>

            <h3 class="section-title">Sản phẩm</h3>
            <div class="order-products">
              <div
                v-for="item in selectedOrder.items"
                :key="item.id"
                class="product-item"
              >
                <div class="product-image">
                  <img :src="item.image" :alt="item.name" />
                </div>
                <div class="product-info">
                  <div class="product-name">{{ item.name }}</div>
                  <div class="product-quantity">x{{ item.quantity }}</div>
                </div>
                <div class="product-price">
                  {{ formatCurrency(item.price) }}
                </div>
              </div>
            </div>

            <div class="order-summary-section">
              <div class="summary-row">
                <span>Tạm tính</span>
                <span>{{
                  formatCurrency(selectedOrder.subtotal || selectedOrder.total)
                }}</span>
              </div>
              <div class="summary-row">
                <span>Phí vận chuyển</span>
                <span>{{
                  formatCurrency(selectedOrder.shippingFee || 0)
                }}</span>
              </div>
              <div class="summary-row" v-if="selectedOrder.discount">
                <span>Giảm giá</span>
                <span>{{ formatCurrency(selectedOrder.discount) }}</span>
              </div>
              <div class="summary-row total-row">
                <span>Tổng cộng</span>
                <span>{{ formatCurrency(selectedOrder.total) }}</span>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button
              v-if="selectedOrder.status === 'Chờ xác nhận'"
              class="cancel-order-btn"
            >
              Hủy đơn hàng
            </button>
            <button
              v-if="
                ['Đã giao hàng', 'Đang giao hàng'].includes(
                  selectedOrder.status
                )
              "
              class="tracking-btn"
            >
              <i class="bi bi-truck"></i> Theo dõi đơn hàng
            </button>
            <button class="buy-again-btn">
              <i class="bi bi-arrow-repeat"></i> Mua lại
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.orders-page {
  padding: 40px 20px;
}

.orders-container {
  max-width: 800px;
  margin: 0 auto;
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  margin-bottom: 20px;
  color: #333;
}

.empty-orders {
  text-align: center;
  padding: 40px 0;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
  color: #999;
}

.shop-now-btn {
  display: inline-block;
  margin-top: 16px;
  padding: 10px 20px;
  background-color: #000;
  color: white;
  text-decoration: none;
  border-radius: 4px;
  font-weight: 500;
}

.order-items {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.order-item {
  background-color: white;
  border-radius: 8px;
  padding: 16px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
  cursor: pointer;
  transition: transform 0.2s;
}

.order-item:hover {
  transform: translateY(-2px);
}

.order-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
  align-items: center;
}

.order-id {
  font-size: 14px;
}

.order-status {
  font-size: 14px;
  font-weight: 500;
  padding: 4px 12px;
  border-radius: 20px;
}

.status-delivered {
  background-color: #d4edda;
  color: #155724;
}

.status-shipping {
  background-color: #cce5ff;
  color: #004085;
}

.status-cancelled {
  background-color: #f8d7da;
  color: #721c24;
}

.status-pending {
  background-color: #fff3cd;
  color: #856404;
}

.order-date {
  color: #666;
  font-size: 14px;
  margin-bottom: 12px;
}

.order-summary {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
  font-size: 14px;
}

.order-total {
  font-weight: 600;
}

.view-details-btn {
  width: 100%;
  padding: 8px;
  background: none;
  border: 1px solid #ddd;
  border-radius: 4px;
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 5px;
  cursor: pointer;
  font-size: 14px;
  margin-top: 8px;
}

.view-details-btn:hover {
  background-color: #f5f5f5;
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

.alert {
  padding: 12px;
  border-radius: 4px;
  margin-bottom: 20px;
  font-size: 14px;
}

.alert-danger {
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}

/* Modal Styles */
.order-detail-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1000;
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
}

.modal-container {
  position: relative;
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  background-color: white;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  z-index: 1001;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  border-bottom: 1px solid #eee;
}

.modal-header h2 {
  font-size: 18px;
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 20px;
  cursor: pointer;
  color: #666;
}

.modal-body {
  padding: 16px;
  overflow-y: auto;
}

.order-info-section {
  margin-bottom: 20px;
  padding-bottom: 20px;
  border-bottom: 1px solid #eee;
}

.info-group {
  margin-bottom: 10px;
  display: flex;
  align-items: baseline;
  flex-wrap: wrap;
}

.info-label {
  font-weight: 500;
  min-width: 150px;
  margin-right: 8px;
}

.tracking-code {
  font-weight: 600;
  color: #0056b3;
}

.section-title {
  font-size: 16px;
  margin-bottom: 12px;
}

.order-products {
  margin-bottom: 20px;
}

.product-item {
  display: flex;
  align-items: center;
  padding: 12px 0;
  border-bottom: 1px solid #eee;
}

.product-image {
  flex: 0 0 60px;
  height: 60px;
  margin-right: 12px;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 4px;
}

.product-info {
  flex: 1;
}

.product-name {
  margin-bottom: 4px;
}

.product-quantity {
  color: #666;
  font-size: 14px;
}

.product-price {
  font-weight: 600;
}

.order-summary-section {
  margin-top: 16px;
  padding-top: 16px;
  border-top: 1px solid #eee;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
}

.total-row {
  font-weight: 600;
  font-size: 16px;
  margin-top: 8px;
  padding-top: 8px;
  border-top: 1px solid #eee;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  padding: 16px;
  gap: 12px;
  border-top: 1px solid #eee;
}

.cancel-order-btn {
  padding: 10px 20px;
  background-color: #f8d7da;
  color: #721c24;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.tracking-btn,
.buy-again-btn {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
}

.tracking-btn {
  background-color: #e9ecef;
  color: #333;
}

.buy-again-btn {
  background-color: #000;
  color: white;
}

@media (max-width: 768px) {
  .info-label {
    min-width: 100%;
    margin-bottom: 4px;
  }

  .modal-footer {
    flex-direction: column;
  }

  .cancel-order-btn,
  .tracking-btn,
  .buy-again-btn {
    width: 100%;
  }
}
</style>
