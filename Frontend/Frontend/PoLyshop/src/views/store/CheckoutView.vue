<template>
  <div class="checkout-container">
    <h1 class="page-title">Thanh Toán</h1>

    <div class="checkout-content">
      <!-- Checkout Steps -->
      <div class="checkout-steps">
        <div class="step active">
          <div class="step-number">1</div>
          <div class="step-text">Thông tin giao hàng</div>
        </div>
        <div class="step-divider"></div>
        <div class="step">
          <div class="step-number">2</div>
          <div class="step-text">Thanh toán</div>
        </div>
        <div class="step-divider"></div>
        <div class="step">
          <div class="step-number">3</div>
          <div class="step-text">Hoàn tất</div>
        </div>
      </div>

      <div class="checkout-layout">
        <!-- Checkout Form -->
        <div class="checkout-form">
          <!-- Shipping Information -->
          <div class="checkout-section">
            <h2 class="section-title">Thông tin giao hàng</h2>
            <div class="form-row">
              <div class="form-group">
                <label for="fullName"
                  >Họ tên <span class="required">*</span></label
                >
                <input
                  type="text"
                  id="fullName"
                  v-model="shippingInfo.fullName"
                  class="form-control"
                />
                <span v-if="errors.fullName" class="error-message">{{
                  errors.fullName
                }}</span>
              </div>
              <div class="form-group">
                <label for="phone"
                  >Số điện thoại <span class="required">*</span></label
                >
                <input
                  type="tel"
                  id="phone"
                  v-model="shippingInfo.phone"
                  class="form-control"
                />
                <span v-if="errors.phone" class="error-message">{{
                  errors.phone
                }}</span>
              </div>
            </div>

            <div class="form-group">
              <label for="email">Email <span class="required">*</span></label>
              <input
                type="email"
                id="email"
                v-model="shippingInfo.email"
                class="form-control"
              />
              <span v-if="errors.email" class="error-message">{{
                errors.email
              }}</span>
            </div>

            <div class="form-group">
              <label for="address"
                >Địa chỉ <span class="required">*</span></label
              >
              <input
                type="text"
                id="address"
                v-model="shippingInfo.address"
                class="form-control"
              />
              <span v-if="errors.address" class="error-message">{{
                errors.address
              }}</span>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="province"
                  >Tỉnh/Thành phố <span class="required">*</span></label
                >
                <select
                  id="province"
                  v-model="shippingInfo.province"
                  class="form-control"
                >
                  <option value="">Chọn Tỉnh/Thành phố</option>
                  <option value="hanoi">Hà Nội</option>
                  <option value="hochiminh">TP. Hồ Chí Minh</option>
                  <option value="danang">Đà Nẵng</option>
                </select>
                <span v-if="errors.province" class="error-message">{{
                  errors.province
                }}</span>
              </div>
              <div class="form-group">
                <label for="district"
                  >Quận/Huyện <span class="required">*</span></label
                >
                <select
                  id="district"
                  v-model="shippingInfo.district"
                  class="form-control"
                >
                  <option value="">Chọn Quận/Huyện</option>
                  <option value="district1">Quận 1</option>
                  <option value="district2">Quận 2</option>
                  <option value="district3">Quận 3</option>
                </select>
                <span v-if="errors.district" class="error-message">{{
                  errors.district
                }}</span>
              </div>
            </div>
          </div>

          <!-- Shipping Method -->
          <div class="checkout-section">
            <h2 class="section-title">Phương thức vận chuyển</h2>
            <div class="shipping-methods">
              <div
                class="shipping-method"
                :class="{ active: selectedShippingMethod === 'standard' }"
                @click="selectedShippingMethod = 'standard'"
              >
                <div class="shipping-radio">
                  <div
                    v-if="selectedShippingMethod === 'standard'"
                    class="radio-inner"
                  ></div>
                </div>
                <div class="shipping-info">
                  <div class="shipping-name">Giao hàng tiêu chuẩn</div>
                  <div class="shipping-time">Nhận hàng trong 3-5 ngày</div>
                </div>
                <div class="shipping-price">30.000đ</div>
              </div>

              <div
                class="shipping-method"
                :class="{ active: selectedShippingMethod === 'express' }"
                @click="selectedShippingMethod = 'express'"
              >
                <div class="shipping-radio">
                  <div
                    v-if="selectedShippingMethod === 'express'"
                    class="radio-inner"
                  ></div>
                </div>
                <div class="shipping-info">
                  <div class="shipping-name">Giao hàng nhanh</div>
                  <div class="shipping-time">Nhận hàng trong 1-2 ngày</div>
                </div>
                <div class="shipping-price">50.000đ</div>
              </div>
            </div>
          </div>

          <!-- Payment Method -->
          <div class="checkout-section">
            <h2 class="section-title">Phương thức thanh toán</h2>
            <div class="payment-methods">
              <div
                class="payment-method"
                :class="{ active: selectedPaymentMethod === 'cod' }"
                @click="selectedPaymentMethod = 'cod'"
              >
                <div class="payment-radio">
                  <div
                    v-if="selectedPaymentMethod === 'cod'"
                    class="radio-inner"
                  ></div>
                </div>
                <div class="payment-icon">
                  <i class="bi bi-cash"></i>
                </div>
                <div class="payment-name">Thanh toán khi nhận hàng (COD)</div>
              </div>

              <div
                class="payment-method"
                :class="{ active: selectedPaymentMethod === 'bank' }"
                @click="selectedPaymentMethod = 'bank'"
              >
                <div class="payment-radio">
                  <div
                    v-if="selectedPaymentMethod === 'bank'"
                    class="radio-inner"
                  ></div>
                </div>
                <div class="payment-icon">
                  <i class="bi bi-credit-card"></i>
                </div>
                <div class="payment-name">Chuyển khoản ngân hàng</div>
              </div>

              <div
                class="payment-method"
                :class="{ active: selectedPaymentMethod === 'momo' }"
                @click="selectedPaymentMethod = 'momo'"
              >
                <div class="payment-radio">
                  <div
                    v-if="selectedPaymentMethod === 'momo'"
                    class="radio-inner"
                  ></div>
                </div>
                <div class="payment-icon payment-momo">
                  <i class="bi bi-wallet2"></i>
                </div>
                <div class="payment-name">Ví MoMo</div>
              </div>
            </div>
          </div>

          <!-- Note -->
          <div class="checkout-section">
            <h2 class="section-title">Ghi chú</h2>
            <div class="form-group">
              <textarea
                v-model="orderNote"
                class="form-control"
                rows="3"
                placeholder="Ghi chú về đơn hàng, ví dụ: thời gian hay chỉ dẫn địa điểm giao hàng chi tiết hơn."
              ></textarea>
            </div>
          </div>
        </div>

        <!-- Order Summary -->
        <div class="order-summary">
          <h2 class="summary-title">Đơn hàng của bạn</h2>

          <div class="order-items">
            <div
              v-for="(item, index) in orderItems"
              :key="index"
              class="order-item"
            >
              <div class="item-image">
                <img :src="item.image" :alt="item.name" />
              </div>
              <div class="item-details">
                <div class="item-name">{{ item.name }}</div>
                <div class="item-variant">{{ item.variant }}</div>
                <div class="item-quantity-price">
                  <span class="item-quantity">x{{ item.quantity }}</span>
                  <span class="item-price">{{ formatPrice(item.price) }}</span>
                </div>
              </div>
            </div>
          </div>

          <div class="coupon-section">
            <div class="coupon-input">
              <input
                type="text"
                v-model="couponCode"
                placeholder="Mã giảm giá"
                class="form-control"
              />
              <button class="apply-coupon">Áp dụng</button>
            </div>
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
            <div class="summary-row">
              <div class="summary-label">Giảm giá</div>
              <div class="summary-value">{{ formatPrice(discount) }}</div>
            </div>
            <div class="summary-row total">
              <div class="summary-label">Tổng cộng</div>
              <div class="summary-value">{{ formatPrice(total) }}</div>
            </div>
          </div>

          <button class="checkout-button" @click="placeOrder">Đặt hàng</button>

          <div class="checkout-policies">
            <p>
              Bằng cách đặt hàng, bạn đồng ý với các
              <a href="#">Điều khoản và Điều kiện</a> của POLY.
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "CheckoutView",
  data() {
    return {
      shippingInfo: {
        fullName: "",
        phone: "",
        email: "",
        address: "",
        province: "",
        district: "",
      },
      errors: {},
      selectedShippingMethod: "standard",
      selectedPaymentMethod: "cod",
      orderNote: "",
      couponCode: "",
      orderItems: [],
      subtotal: 0,
      shippingFee: 30000,
      discount: 0,
    };
  },
  created() {
    // Load checkout items from localStorage
    this.loadCheckoutItems();
  },
  computed: {
    total() {
      return this.subtotal + this.shippingFee - this.discount;
    },
  },
  methods: {
    loadCheckoutItems() {
      // Try to get items from checkout storage first (selected items from cart)
      let items = JSON.parse(localStorage.getItem("checkoutItems") || "[]");

      // If no checkout items found, try to get all cart items
      if (items.length === 0) {
        const cartItems = JSON.parse(localStorage.getItem("cart") || "[]");
        items = cartItems.filter((item) => item.selected !== false);
      }

      // If still no items, use default items
      if (items.length === 0) {
        items = [
          {
            name: "Áo thun POLY Basic",
            variant: "Trắng / Size M",
            price: 280000,
            quantity: 1,
            image: "https://placehold.co/100x100",
          },
          {
            name: "Quần jean slim fit",
            variant: "Xanh đậm / Size 30",
            price: 450000,
            quantity: 2,
            image: "https://placehold.co/100x100",
          },
        ];
      }

      // Format items if needed
      this.orderItems = items.map((item) => {
        return {
          name: item.name,
          variant: item.category || item.variant || "",
          price: item.price,
          quantity: item.quantity,
          image: item.image || "https://placehold.co/100x100",
        };
      });

      // Calculate subtotal
      this.calculateSubtotal();
    },
    calculateSubtotal() {
      this.subtotal = this.orderItems.reduce((total, item) => {
        return total + item.price * item.quantity;
      }, 0);
    },
    formatPrice(price) {
      return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
        minimumFractionDigits: 0,
      }).format(price);
    },
    validateForm() {
      this.errors = {};
      let isValid = true;

      if (!this.shippingInfo.fullName.trim()) {
        this.errors.fullName = "Vui lòng nhập họ tên";
        isValid = false;
      }

      if (!this.shippingInfo.phone.trim()) {
        this.errors.phone = "Vui lòng nhập số điện thoại";
        isValid = false;
      } else if (!this.isValidPhone(this.shippingInfo.phone)) {
        this.errors.phone = "Số điện thoại không hợp lệ";
        isValid = false;
      }

      if (!this.shippingInfo.email.trim()) {
        this.errors.email = "Vui lòng nhập email";
        isValid = false;
      } else if (!this.isValidEmail(this.shippingInfo.email)) {
        this.errors.email = "Email không đúng định dạng";
        isValid = false;
      }

      if (!this.shippingInfo.address.trim()) {
        this.errors.address = "Vui lòng nhập địa chỉ";
        isValid = false;
      }

      if (!this.shippingInfo.province) {
        this.errors.province = "Vui lòng chọn tỉnh/thành phố";
        isValid = false;
      }

      if (!this.shippingInfo.district) {
        this.errors.district = "Vui lòng chọn quận/huyện";
        isValid = false;
      }

      return isValid;
    },
    isValidEmail(email) {
      const re =
        /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
      return re.test(String(email).toLowerCase());
    },
    isValidPhone(phone) {
      const re = /(84|0[3|5|7|8|9])+([0-9]{8})\b/;
      return re.test(String(phone));
    },
    placeOrder() {
      if (!this.validateForm()) {
        // Scroll to the top to show errors
        window.scrollTo(0, 0);
        return;
      }

      // Create order object with all necessary data
      const order = {
        id: "ORD-" + Date.now(), // Generate unique order ID
        date: new Date().toISOString(),
        status: "processing", // Initial status
        customer: {
          name: this.shippingInfo.fullName,
          email: this.shippingInfo.email,
          phone: this.shippingInfo.phone,
          address: this.shippingInfo.address,
          province: this.shippingInfo.province,
          district: this.shippingInfo.district,
        },
        shipping: {
          method: this.selectedShippingMethod,
          fee: this.selectedShippingMethod === "standard" ? 30000 : 50000,
        },
        payment: {
          method: this.selectedPaymentMethod,
        },
        items: this.orderItems,
        subtotal: this.subtotal,
        shippingFee: this.shippingFee,
        discount: this.discount,
        total: this.total,
        note: this.orderNote,
      };

      // Save to localStorage
      let userOrders = JSON.parse(localStorage.getItem("userOrders") || "[]");
      userOrders.push(order);
      localStorage.setItem("userOrders", JSON.stringify(userOrders));

      // Clear cart
      localStorage.removeItem("cart");

      alert("Đặt hàng thành công! Cám ơn bạn đã mua sắm tại POLY.");

      // Redirect to orders page
      this.$router.push("/orders");
    },
  },
};
</script>

<style scoped>
.checkout-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 30px 20px;
}

.page-title {
  text-align: center;
  font-size: 28px;
  margin-bottom: 30px;
}

/* Checkout Steps */
.checkout-steps {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 40px;
}

.step {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.step-number {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  background-color: #f0f0f0;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  margin-bottom: 8px;
}

.step.active .step-number {
  background-color: #000;
  color: white;
}

.step-divider {
  width: 80px;
  height: 1px;
  background-color: #ddd;
  margin: 0 20px;
}

.checkout-layout {
  display: flex;
  gap: 30px;
}

.checkout-form {
  flex: 1.5;
}

.checkout-section {
  background-color: white;
  border-radius: 8px;
  padding: 25px;
  margin-bottom: 25px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.section-title {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 20px;
  border-bottom: 1px solid #eee;
  padding-bottom: 10px;
}

.form-row {
  display: flex;
  gap: 20px;
  margin-bottom: 15px;
}

.form-group {
  flex: 1;
  margin-bottom: 15px;
}

.form-control {
  width: 100%;
  padding: 12px 15px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  transition: border-color 0.2s;
}

.form-control:focus {
  border-color: #333;
  outline: none;
}

label {
  display: block;
  margin-bottom: 8px;
  font-size: 14px;
  font-weight: 500;
}

.required {
  color: #e53637;
}

.error-message {
  font-size: 12px;
  color: #e53637;
  margin-top: 5px;
  display: block;
}

/* Shipping Methods */
.shipping-methods {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.shipping-method {
  display: flex;
  align-items: center;
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.shipping-method:hover {
  border-color: #aaa;
}

.shipping-method.active {
  border-color: #000;
  background-color: #f9f9f9;
}

.shipping-radio {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 2px solid #ddd;
  margin-right: 15px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.shipping-method.active .shipping-radio {
  border-color: #000;
}

.radio-inner {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background-color: #000;
}

.shipping-info {
  flex: 1;
}

.shipping-name {
  font-weight: 500;
  margin-bottom: 5px;
}

.shipping-time {
  font-size: 13px;
  color: #666;
}

.shipping-price {
  font-weight: 600;
}

/* Payment Methods */
.payment-methods {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.payment-method {
  display: flex;
  align-items: center;
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.payment-method:hover {
  border-color: #aaa;
}

.payment-method.active {
  border-color: #000;
  background-color: #f9f9f9;
}

.payment-radio {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 2px solid #ddd;
  margin-right: 15px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.payment-method.active .payment-radio {
  border-color: #000;
}

.payment-icon {
  width: 40px;
  height: 40px;
  background-color: #f0f0f0;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 15px;
  font-size: 18px;
}

.payment-momo {
  background-color: #d82d8b;
  color: white;
}

.payment-name {
  font-weight: 500;
}

/* Order Summary */
.order-summary {
  flex: 1;
  background-color: white;
  border-radius: 8px;
  padding: 25px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  align-self: flex-start;
  position: sticky;
  top: 20px;
}

.summary-title {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 20px;
  border-bottom: 1px solid #eee;
  padding-bottom: 10px;
}

.order-items {
  margin-bottom: 20px;
  max-height: 300px;
  overflow-y: auto;
}

.order-item {
  display: flex;
  margin-bottom: 15px;
  padding-bottom: 15px;
  border-bottom: 1px solid #f0f0f0;
}

.item-image {
  width: 60px;
  height: 60px;
  margin-right: 15px;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 4px;
}

.item-details {
  flex: 1;
}

.item-name {
  font-weight: 500;
  margin-bottom: 5px;
}

.item-variant {
  font-size: 12px;
  color: #666;
  margin-bottom: 5px;
}

.item-quantity-price {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
}

.item-price {
  font-weight: 500;
}

/* Coupon */
.coupon-section {
  margin-bottom: 20px;
}

.coupon-input {
  display: flex;
}

.apply-coupon {
  border: none;
  background-color: #000;
  color: white;
  padding: 0 15px;
  cursor: pointer;
  border-radius: 0 4px 4px 0;
  font-size: 14px;
}

.summary-details {
  margin-bottom: 20px;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 14px;
}

.summary-row.total {
  font-size: 18px;
  font-weight: 600;
  margin-top: 15px;
  padding-top: 15px;
  border-top: 1px solid #eee;
}

.checkout-button {
  width: 100%;
  padding: 15px;
  background-color: #000;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s;
  margin-bottom: 20px;
}

.checkout-button:hover {
  background-color: #333;
}

.checkout-policies {
  font-size: 12px;
  color: #666;
  text-align: center;
}

.checkout-policies a {
  color: #000;
  text-decoration: none;
}

@media (max-width: 992px) {
  .checkout-layout {
    flex-direction: column;
  }

  .order-summary {
    position: static;
  }
}

@media (max-width: 768px) {
  .form-row {
    flex-direction: column;
    gap: 0;
  }

  .step-divider {
    width: 40px;
  }
}
</style>
