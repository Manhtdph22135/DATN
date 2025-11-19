<template>
  <div class="checkout-container">
    <h1 class="page-title">Thanh Toán</h1>

    <div class="checkout-content">
      <div class="checkout-layout">
        
        <div class="checkout-form">
          
          <div class="checkout-section">
            <h2 class="section-title">Thông tin giao hàng</h2>
            
            <div class="form-row">
              <div class="form-group">
                <label>Họ tên <span class="req">*</span></label>
                <input type="text" v-model="shippingInfo.fullName" class="form-control" placeholder="Nguyễn Văn A">
                <span v-if="errors.fullName" class="err">{{ errors.fullName }}</span>
              </div>
              <div class="form-group">
                <label>SĐT <span class="req">*</span></label>
                <input type="tel" v-model="shippingInfo.phone" class="form-control" placeholder="09xxxxxxxx">
                <span v-if="errors.phone" class="err">{{ errors.phone }}</span>
              </div>
            </div>

            <div class="form-group">
              <label>Email</label>
              <input type="email" v-model="shippingInfo.email" class="form-control" placeholder="email@example.com">
            </div>

            <div class="form-group">
              <label>Tỉnh / Thành / Quận / Huyện <span class="req">*</span></label>
              <input 
                type="text" 
                v-model="shippingInfo.city" 
                class="form-control" 
                placeholder="VD: Hà Nội, Quận Cầu Giấy"
              >
              <span v-if="errors.city" class="err">{{ errors.city }}</span>
            </div>

            <div class="form-group">
              <label>Địa chỉ cụ thể <span class="req">*</span></label>
              <input 
                type="text" 
                v-model="shippingInfo.address" 
                class="form-control" 
                placeholder="Số nhà, tên đường..." 
              >
              <span v-if="errors.address" class="err">{{ errors.address }}</span>
            </div>
          </div>
          
          <div class="checkout-section">
            <h2 class="section-title">Vận chuyển</h2>
            <div class="shipping-method active">
              <div class="shipping-radio"><div class="radio-inner"></div></div>
              <div class="shipping-info">
                <div class="shipping-name">Giao hàng tiêu chuẩn</div>
                <div class="shipping-time">3-5 ngày</div>
              </div>
              <div class="shipping-price">{{ formatPrice(30000) }}</div>
            </div>
          </div>

          <div class="checkout-section">
            <h2 class="section-title">Thanh toán</h2>
            <div class="payment-methods">
              <div class="payment-method" :class="{ active: selectedPaymentMethod === 'cod' }" @click="selectedPaymentMethod = 'cod'">
                <div class="payment-radio"><div v-if="selectedPaymentMethod === 'cod'" class="radio-inner"></div></div>
                <div class="payment-name">Thanh toán khi nhận hàng (COD)</div>
              </div>
              <div class="payment-method" :class="{ active: selectedPaymentMethod === 'bank' }" @click="selectedPaymentMethod = 'bank'">
                <div class="payment-radio"><div v-if="selectedPaymentMethod === 'bank'" class="radio-inner"></div></div>
                <div class="payment-name">Chuyển khoản ngân hàng</div>
              </div>
            </div>
          </div>

          <div class="checkout-section mt-3">
             <label class="fw-bold mb-2">Ghi chú đơn hàng</label>
             <textarea v-model="orderNote" class="form-control" rows="2"></textarea>
          </div>
        </div>

        <div class="order-summary">
          <h2 class="summary-title">Đơn hàng của bạn</h2>
          
          <div class="order-items">
            <div v-for="(item, index) in orderItems" :key="index" class="order-item">
              <div class="item-image">
                <img :src="item.image" :alt="item.name" />
              </div>
              <div class="item-details">
                <div class="item-name">{{ item.name || item.productName }}</div>
                <div class="item-variant">
                    {{ item.sizeName || item.size || 'Mặc định' }} / 
                    {{ item.colorName || item.color || 'Mặc định' }}
                </div>
                <div class="item-quantity-price">
                  <span class="item-quantity">x{{ item.quantity }}</span>
                  <span class="item-price">{{ formatPrice(item.price) }}</span>
                </div>
              </div>
            </div>
          </div>

          <div class="coupon-section">
            <label class="form-label fw-bold" style="font-size: 14px;">Mã ưu đãi</label>
            <div v-if="availableVouchers.length > 0">
              <select class="form-select form-select-sm" v-model="selectedVoucherId" @change="applySelectedVoucher">
                <option :value="null">-- Chọn mã --</option>
                <option v-for="v in availableVouchers" :key="v.promotionId || v.id" :value="v.promotionId || v.id">
                  {{ v.code }} - {{ v.name }} (Giảm {{ v.discountValue }}%)
                </option>
              </select>
            </div>
            <div v-else class="text-muted small mb-2">Không có mã giảm giá.</div>
          </div>

          <div class="summary-details">
            <div class="summary-row">
              <div class="summary-label">Tạm tính</div>
              <div class="summary-value">{{ formatPrice(subtotal) }}</div>
            </div>
            <div class="summary-row">
              <div class="summary-label">Phí vận chuyển</div>
              <div class="summary-value">{{ formatPrice(shippingFee) }}</div>
            </div>
            <div class="summary-row text-success" v-if="discount > 0">
              <div class="summary-label">Giảm giá</div>
              <div class="summary-value">-{{ formatPrice(discount) }}</div>
            </div>
            <div class="summary-row total">
              <div class="summary-label">Tổng cộng</div>
              <div class="summary-value">{{ formatPrice(total) }}</div>
            </div>
          </div>

          <button class="checkout-button" @click="placeOrder">Đặt hàng</button>

          <div class="checkout-policies">
            <p>Bằng cách đặt hàng, bạn đồng ý với các <a href="#">Điều khoản và Điều kiện</a>.</p>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "CheckoutView",
  data() {
    return {
      shippingInfo: {
        fullName: "",
        phone: "",
        email: "",
        address: "",
        city: "" 
      },
      errors: {},
      selectedPaymentMethod: "cod",
      orderNote: "",
      orderItems: [],
      subtotal: 0,
      shippingFee: 30000,
      discount: 0,
      availableVouchers: [],
      selectedVoucherId: null
    };
  },
  computed: {
    total() { return Math.max(0, this.subtotal + this.shippingFee - this.discount); }
  },
  created() {
    this.loadUserInfo();
    this.loadCheckoutItems();
    this.fetchVouchers();
  },
  methods: {
    loadUserInfo() {
      const userStr = localStorage.getItem("user");
      if (userStr) {
        try {
          const user = JSON.parse(userStr);
          this.shippingInfo.fullName = user.fullName || user.username || "";
          this.shippingInfo.email = user.email || "";
          this.shippingInfo.phone = user.phoneNumber || user.phone || "";
          this.shippingInfo.address = user.address || "";
        } catch (e) {}
      }
    },

    loadCheckoutItems() {
      let items = JSON.parse(localStorage.getItem("checkoutItems") || "[]");
      if (items.length === 0) {
        const cartItems = JSON.parse(localStorage.getItem("polyshop_cart") || "[]");
        items = cartItems.filter((item) => item.selected !== false);
      }
      this.orderItems = items;
      this.subtotal = items.reduce((total, item) => total + item.price * item.quantity, 0);
    },

    async fetchVouchers() {
      try {
        const res = await axios.get("https://localhost:7055/api/Promotion");
        const all = res.data.$values || res.data || [];
        const today = new Date().toISOString().split('T')[0];
        this.availableVouchers = all.filter(p => 
            (p.productId === null || p.ProductID === null) && 
            (p.status === "Đang hoạt động" || p.Status === "Đang hoạt động") &&
            (p.startDate <= today || p.StartDate <= today) &&
            (p.endDate >= today || p.EndDate >= today)
        );
      } catch (err) { console.error(err); }
    },

    applySelectedVoucher() {
      if (!this.selectedVoucherId) { this.discount = 0; return; }
      const v = this.availableVouchers.find(item => (item.id || item.promotionId || item.PromotionId) === this.selectedVoucherId);
      if (v) {
        const val = Number(v.discountValue || v.DiscountValue || 0);
        const type = String(v.discountType || v.DiscountType);
        if (type === "1") {
          this.discount = Math.round((this.subtotal * val) / 100);
        } else {
          this.discount = val;
        }
        if (this.discount > this.subtotal) this.discount = this.subtotal;
      }
    },

    formatPrice(price) {
      return new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(price);
    },

    validateForm() {
      this.errors = {};
      let isValid = true;
      if (!this.shippingInfo.fullName) { this.errors.fullName = "Nhập họ tên"; isValid = false; }
      if (!this.shippingInfo.phone) { this.errors.phone = "Nhập số điện thoại"; isValid = false; }
      if (!this.shippingInfo.address) { this.errors.address = "Nhập địa chỉ"; isValid = false; }
      if (!this.shippingInfo.city) { this.errors.city = "Nhập Tỉnh/Thành"; isValid = false; }
      return isValid;
    },

    placeOrder() {
      if (!this.validateForm()) {
        window.scrollTo(0, 0);
        return;
      }
      
      const order = {
        id: "ORD-" + Date.now(),
        date: new Date().toISOString(),
        status: "pending",
        customer: {
           ...this.shippingInfo,
           fullAddress: `${this.shippingInfo.address}, ${this.shippingInfo.city}`
        },
        payment: { method: this.selectedPaymentMethod },
        items: this.orderItems,
        subtotal: this.subtotal,
        shippingFee: this.shippingFee,
        discount: this.discount,
        total: this.total,
        note: this.orderNote,
      };

      let userOrders = JSON.parse(localStorage.getItem("userOrders") || "[]");
      userOrders.push(order);
      localStorage.setItem("userOrders", JSON.stringify(userOrders));
      
      localStorage.removeItem("polyshop_cart");
      localStorage.removeItem("checkoutItems");

      alert("Đặt hàng thành công!");
      // Chuyển hướng sang trang chi tiết đơn hàng vừa đặt
      this.$router.push(`/order-success/${order.id}`);
    }
  }
};
</script>

<style scoped>
/* GIỮ NGUYÊN CSS CŨ CỦA BẠN ĐỂ GIAO DIỆN KHÔNG BỊ VỠ */
.checkout-container { max-width: 1200px; margin: 0 auto; padding: 30px 20px; }
.page-title { text-align: center; font-size: 28px; margin-bottom: 30px; }
.checkout-steps { display: flex; align-items: center; justify-content: center; margin-bottom: 40px; }
.step { display: flex; flex-direction: column; align-items: center; }
.step-number { width: 30px; height: 30px; border-radius: 50%; background-color: #f0f0f0; display: flex; align-items: center; justify-content: center; font-weight: bold; margin-bottom: 8px; }
.step.active .step-number { background-color: #000; color: white; }
.step-divider { width: 80px; height: 1px; background-color: #ddd; margin: 0 20px; }
.checkout-layout { display: flex; gap: 30px; }
.checkout-form { flex: 1.5; }
.checkout-section { background-color: white; border-radius: 8px; padding: 25px; margin-bottom: 25px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05); }
.section-title { font-size: 18px; font-weight: 600; margin-bottom: 20px; border-bottom: 1px solid #eee; padding-bottom: 10px; }
.form-row { display: flex; gap: 20px; margin-bottom: 15px; }
.form-group { flex: 1; margin-bottom: 15px; }
.form-control { width: 100%; padding: 12px 15px; border: 1px solid #ddd; border-radius: 4px; font-size: 14px; transition: border-color 0.2s; }
.form-control:focus { border-color: #333; outline: none; }
label { display: block; margin-bottom: 8px; font-size: 14px; font-weight: 500; }
.req { color: #e53637; }
.err { color: #e53637; font-size: 12px; margin-top: 5px; display: block; }

.order-summary { flex: 1; background-color: white; border-radius: 8px; padding: 25px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05); align-self: flex-start; position: sticky; top: 20px; }
.summary-title { font-size: 18px; font-weight: 600; margin-bottom: 20px; border-bottom: 1px solid #eee; padding-bottom: 10px; }
.order-items { margin-bottom: 20px; max-height: 300px; overflow-y: auto; }
.order-item { display: flex; margin-bottom: 15px; padding-bottom: 15px; border-bottom: 1px solid #f0f0f0; }
.item-image { width: 60px; height: 60px; margin-right: 15px; }
.item-image img { width: 100%; height: 100%; object-fit: cover; border-radius: 4px; }
.item-details { flex: 1; }
.item-name { font-weight: 500; margin-bottom: 5px; }
.item-variant { font-size: 12px; color: #666; margin-bottom: 5px; }
.item-quantity-price { display: flex; justify-content: space-between; font-size: 14px; }
.item-price { font-weight: 500; }

/* CSS Cho phần Vận chuyển & Thanh toán */
.shipping-method, .payment-method { display: flex; align-items: center; padding: 15px; border: 1px solid #ddd; border-radius: 8px; cursor: pointer; transition: all 0.2s; margin-bottom: 10px; }
.shipping-method.active, .payment-method.active { border-color: #000; background-color: #f9f9f9; }
.shipping-radio, .payment-radio { width: 20px; height: 20px; border-radius: 50%; border: 2px solid #ddd; margin-right: 15px; display: flex; align-items: center; justify-content: center; }
.shipping-method.active .shipping-radio, .payment-method.active .payment-radio { border-color: #000; }
.radio-inner { width: 10px; height: 10px; border-radius: 50%; background-color: #000; }
.shipping-info { flex: 1; }
.shipping-price { font-weight: 600; }

.summary-row { display: flex; justify-content: space-between; margin-bottom: 8px; font-size: 14px; }
.summary-row.total { font-size: 18px; font-weight: 600; margin-top: 15px; padding-top: 15px; border-top: 1px solid #eee; }
.checkout-button { width: 100%; padding: 15px; background-color: #000; color: white; border: none; border-radius: 4px; font-size: 16px; font-weight: 600; cursor: pointer; transition: background-color 0.2s; margin-bottom: 20px; }
.checkout-button:hover { background-color: #333; }
.checkout-policies { font-size: 12px; color: #666; text-align: center; }

@media (max-width: 992px) { .checkout-layout { flex-direction: column; } .order-summary { position: static; } }
@media (max-width: 768px) { .form-row { flex-direction: column; gap: 0; } }
</style>