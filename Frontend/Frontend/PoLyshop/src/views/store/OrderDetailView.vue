<template>
  <div class="order-detail-page">
    <div class="container">
      <div v-if="order" class="receipt-card">
        
        <div class="receipt-header text-center">
          <div class="icon-success mb-3">
            <i class="bi bi-check-circle-fill text-success" style="font-size: 3rem;"></i>
          </div>
          <h2 class="fw-bold">Đặt hàng thành công!</h2>
          <p class="text-muted">Cảm ơn bạn đã mua sắm tại POLY. Dưới đây là hóa đơn của bạn.</p>
        </div>

        <div class="receipt-body">
          <div class="d-flex justify-content-between mb-4 border-bottom pb-3">
            <div>
              <small class="text-muted d-block">Mã đơn hàng</small>
              <span class="fw-bold">#{{ order.id }}</span>
            </div>
            <div class="text-end">
              <small class="text-muted d-block">Ngày đặt</small>
              <span>{{ formatDate(order.date) }}</span>
            </div>
          </div>

          <div class="mb-4">
            <h6 class="fw-bold border-bottom pb-2 mb-3">Thông tin giao hàng</h6>
            <div class="customer-info">
              <p class="mb-1"><strong>Người nhận:</strong> {{ order.customer.fullName }}</p>
              <p class="mb-1"><strong>SĐT:</strong> {{ order.customer.phone }}</p>
              <p class="mb-1"><strong>Địa chỉ:</strong> {{ order.customer.address }}, {{ order.customer.city }}</p>
              <p class="mb-0 fst-italic text-muted" v-if="order.note">
                <strong>Ghi chú:</strong> {{ order.note }}
              </p>
            </div>
          </div>

          <div class="mb-4">
            <h6 class="fw-bold border-bottom pb-2 mb-3">Sản phẩm</h6>
            <div class="item-list">
              <div v-for="(item, index) in order.items" :key="index" class="d-flex align-items-center mb-3">
                <img :src="item.image" class="rounded border me-3" style="width: 60px; height: 60px; object-fit: cover;">
                <div class="flex-grow-1">
                  <div class="fw-bold" style="font-size: 0.95rem;">{{ item.name }}</div>
                  <div class="text-muted small">
                    {{ item.sizeName || item.size }} / {{ item.colorName || item.color }}
                  </div>
                </div>
                <div class="text-end">
                  <div class="small text-muted">x{{ item.quantity }}</div>
                  <div class="fw-bold">{{ formatPrice(item.price) }}</div>
                </div>
              </div>
            </div>
          </div>

          <div class="total-section bg-light p-3 rounded">
            <div class="d-flex justify-content-between mb-2">
              <span>Tạm tính:</span>
              <span>{{ formatPrice(order.subtotal) }}</span>
            </div>
            <div class="d-flex justify-content-between mb-2">
              <span>Phí vận chuyển:</span>
              <span>{{ formatPrice(order.shippingFee) }}</span>
            </div>
            <div class="d-flex justify-content-between mb-2 text-success" v-if="order.discount > 0">
              <span>Giảm giá:</span>
              <span>-{{ formatPrice(order.discount) }}</span>
            </div>
            <hr>
            <div class="d-flex justify-content-between fs-5 fw-bold">
              <span>Tổng cộng:</span>
              <span class="text-danger">{{ formatPrice(order.total) }}</span>
            </div>
            <div class="text-center mt-2 small text-muted">
              Phương thức: {{ order.payment.method === 'cod' ? 'Thanh toán khi nhận hàng (COD)' : 'Chuyển khoản / Ví điện tử' }}
            </div>
          </div>
        </div>

        <div class="receipt-footer mt-4 text-center">
          <router-link to="/" class="btn btn-dark w-100 py-2">Tiếp tục mua sắm</router-link>
          <div class="mt-2">
            <small class="text-muted">Mọi thắc mắc vui lòng liên hệ hotline: 0947838677</small>
          </div>
        </div>
      </div>

      <div v-else class="text-center py-5">
        <h3>Không tìm thấy đơn hàng</h3>
        <router-link to="/" class="btn btn-primary mt-3">Về trang chủ</router-link>
      </div>
    </div>
  </div>
</template>

<script>
import { useRoute } from 'vue-router'; // Dùng để lấy ID từ URL

export default {
  name: "OrderDetailView",
  data() {
    return {
      order: null
    };
  },
  created() {
    // 1. Lấy ID từ URL (ví dụ: /order-success/ORD-123456)
    const orderId = this.$route.params.id;

    // 2. Lấy danh sách đơn hàng của khách từ LocalStorage
    const ordersStr = localStorage.getItem("userOrders");
    
    if (ordersStr) {
      const orders = JSON.parse(ordersStr);
      // 3. Tìm đúng đơn hàng vừa đặt
      this.order = orders.find(o => o.id == orderId);
    }
  },
  methods: {
    formatPrice(price) {
      return new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(price);
    },
    formatDate(dateString) {
      return new Date(dateString).toLocaleString("vi-VN");
    }
  }
};
</script>

<style scoped>
.order-detail-page {
  background-color: #f3f4f6;
  min-height: 100vh;
  padding: 40px 0;
}
.receipt-card {
  max-width: 600px;
  margin: 0 auto;
  background: #fff;
  padding: 30px;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.08);
}
</style>