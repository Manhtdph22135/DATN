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
    total() { 
      return Math.max(0, this.subtotal + this.shippingFee - this.discount); 
    }
  },
  created() {
    this.loadUserInfo();
    this.loadCheckoutItems();
    this.fetchVouchers();
  },
  methods: {
    // ✅ LẤY THÔNG TIN TỪ API THEO accountId TRONG sessionStorage
    async loadUserInfo() {
      try {
        // 1. Lấy accountId từ sessionStorage
        const accountId = sessionStorage.getItem("accountId");

        if (accountId) {
          // 2. Gọi API get-customer-by-account/{accountId}
          const res = await axios.get(
            `https://localhost:7055/api/Customer/get-customer-by-account/${accountId}`
          );

          const c = res.data || {};

          // 3. Đổ dữ liệu vào shippingInfo
          this.shippingInfo.fullName = c.fullName || c.username || "";
          this.shippingInfo.email    = c.email || "";
          this.shippingInfo.phone    = c.phone || c.phoneNumber || "";
          this.shippingInfo.address  = c.address || "";
          // API không có city nên để trống, user tự điền
          this.shippingInfo.city     = "";
          return; // Có dữ liệu từ API rồi thì không cần fallback localStorage nữa
        }

        // 4. Fallback: nếu không có accountId, dùng dữ liệu localStorage cũ
        const userStr = localStorage.getItem("user");
        if (userStr) {
          const user = JSON.parse(userStr);
          this.shippingInfo.fullName = user.fullName || user.username || "";
          this.shippingInfo.email    = user.email || "";
          this.shippingInfo.phone    = user.phoneNumber || user.phone || "";
          this.shippingInfo.address  = user.address || "";
          this.shippingInfo.city     = user.city || "";
        }
      } catch (e) {
        console.error("Lỗi loadUserInfo:", e);
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
          ((p.startDate && p.startDate <= today) || (p.StartDate && p.StartDate <= today)) &&
          ((p.endDate && p.endDate >= today) || (p.EndDate && p.EndDate >= today))
        );
      } catch (err) { 
        console.error(err); 
      }
    },

    applySelectedVoucher() {
      if (!this.selectedVoucherId) { 
        this.discount = 0; 
        return; 
      }
      const v = this.availableVouchers.find(item => 
        (item.id || item.promotionId || item.PromotionId) === this.selectedVoucherId
      );
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
      return new Intl.NumberFormat("vi-VN", { 
        style: "currency", 
        currency: "VND" 
      }).format(price);
    },

    validateForm() {
      this.errors = {};
      let isValid = true;
      if (!this.shippingInfo.fullName) { 
        this.errors.fullName = "Nhập họ tên"; 
        isValid = false; 
      }
      if (!this.shippingInfo.phone) { 
        this.errors.phone = "Nhập số điện thoại"; 
        isValid = false; 
      }
      if (!this.shippingInfo.address) { 
        this.errors.address = "Nhập địa chỉ"; 
        isValid = false; 
      }
      if (!this.shippingInfo.city) { 
        this.errors.city = "Nhập Tỉnh/Thành"; 
        isValid = false; 
      }
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
      this.$router.push(`/order-success/${order.id}`);
    }
  }
};
</script>



<template>
  <div class="account-page">
    <div class="account-container">
      <h1 class="page-title">Tài khoản của tôi</h1>

      <div class="alert alert-success" v-if="successMessage">
        {{ successMessage }}
      </div>
      <div class="alert alert-danger" v-if="errorMessage">
        {{ errorMessage }}
      </div>

      <!-- Thông tin tài khoản -->
      <div class="account-section">
        <div class="section-header">
          <h2>Thông tin cá nhân</h2>
          <button v-if="!isEditing" @click="enableEditing" class="btn-edit">
            <i class="bi bi-pencil"></i> Chỉnh sửa
          </button>
        </div>

        <div class="profile-content">
          <!-- Form xem hoặc chỉnh sửa thông tin -->
          <form @submit.prevent="saveProfile" class="profile-form">
            <div class="form-row">
              <div class="form-group">
                <label for="username">Tên đăng nhập:</label>
                <input type="text" id="username" v-model="userProfile.username" :disabled="true" />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="fullName">Họ tên:</label>
                <input type="text" id="fullName" v-model="userProfile.fullName" :disabled="!isEditing" />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="email">Email:</label>
                <input type="email" id="email" v-model="userProfile.email" :disabled="!isEditing" />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="phone">Số điện thoại:</label>
                <input type="tel" id="phone" v-model="userProfile.phoneNumber" :disabled="!isEditing" />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="address">Địa chỉ:</label>
                <textarea id="address" v-model="userProfile.address" :disabled="!isEditing"></textarea>
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="dob">Ngày sinh:</label>
                <input type="date" id="dob" v-model="userProfile.dateOfBirth" :disabled="!isEditing" />
              </div>
            </div>

            <div class="form-row" v-if="isEditing">
              <div class="form-group gender-group">
                <label>Giới tính:</label>
                <div class="gender-options">
                  <label class="gender-option">
                    <input type="radio" v-model="userProfile.gender" value="female" :disabled="!isEditing" />
                    <span>Nữ</span>
                  </label>
                  <label class="gender-option">
                    <input type="radio" v-model="userProfile.gender" value="male" :disabled="!isEditing" />
                    <span>Nam</span>
                  </label>
                </div>
              </div>
            </div>
            <div class="form-row" v-else>
              <div class="form-group">
                <label>Giới tính:</label>
                <span>{{
                  userProfile.gender === "female" ? "Nữ" : "Nam"
                }}</span>
              </div>
            </div>

            <!-- Nút lưu/hủy -->
            <div class="button-group" v-if="isEditing">
              <button type="button" @click="cancelEditing" class="btn-cancel">
                Hủy
              </button>
              <button type="submit" class="btn-save" :disabled="isLoading">
                <span v-if="isLoading">
                  <i class="bi bi-arrow-repeat spinning"></i> Đang lưu...
                </span>
                <span v-else>Lưu thay đổi</span>
              </button>
            </div>
          </form>
        </div>
      </div>

      <!-- Đổi mật khẩu -->
      <div class="account-section">
        <div class="section-header">
          <h2>Mật khẩu</h2>
          <button v-if="!showChangePassword" @click="toggleChangePassword" class="btn-edit">
            <i class="bi bi-shield-lock"></i> Đổi mật khẩu
          </button>
        </div>

        <div class="password-content" v-if="showChangePassword">
          <form @submit.prevent="changePassword" class="password-form">
            <div class="form-group">
              <label for="currentPassword">Mật khẩu hiện tại:</label>
              <input type="password" id="currentPassword" v-model="changePasswordForm.currentPassword" />
              <p class="error-message" v-if="passwordErrors.currentPassword">
                {{ passwordErrors.currentPassword }}
              </p>
            </div>

            <div class="form-group">
              <label for="newPassword">Mật khẩu mới:</label>
              <input type="password" id="newPassword" v-model="changePasswordForm.newPassword" />
              <p class="error-message" v-if="passwordErrors.newPassword">
                {{ passwordErrors.newPassword }}
              </p>
            </div>

            <div class="form-group">
              <label for="confirmPassword">Xác nhận mật khẩu mới:</label>
              <input type="password" id="confirmPassword" v-model="changePasswordForm.confirmPassword" />
              <p class="error-message" v-if="passwordErrors.confirmPassword">
                {{ passwordErrors.confirmPassword }}
              </p>
            </div>

            <div class="button-group">
              <button type="button" @click="toggleChangePassword" class="btn-cancel">
                Hủy
              </button>
              <button type="submit" class="btn-save" :disabled="isLoading">
                <span v-if="isLoading">
                  <i class="bi bi-arrow-repeat spinning"></i> Đang xử lý...
                </span>
                <span v-else>Cập nhật mật khẩu</span>
              </button>
            </div>
          </form>
        </div>
        <div v-else class="password-placeholder">
          <p>••••••••</p>
          <p class="password-hint">
            Mật khẩu được giấu vì lý do bảo mật. Nhấn "Đổi mật khẩu" để cập
            nhật.
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.account-page {
  padding: 40px 20px;
}

.account-container {
  max-width: 800px;
  margin: 0 auto;
  background-color: white;
  border-radius: 8px;
  padding: 30px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  margin-bottom: 20px;
  color: #333;
}

.account-section {
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 1px solid #eee;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.section-header h2 {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.btn-edit {
  background: none;
  border: 1px solid #ddd;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  display: flex;
  align-items: center;
  gap: 5px;
  color: #555;
}

.btn-edit:hover {
  background-color: #f5f5f5;
}

.profile-form {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.form-row {
  display: flex;
  gap: 20px;
}

.form-group {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-weight: 500;
  color: #555;
  font-size: 14px;
}

.form-group input,
.form-group textarea {
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.form-group input:disabled,
.form-group textarea:disabled {
  background-color: #f9f9f9;
  color: #666;
}

.gender-options {
  display: flex;
  gap: 20px;
}

.gender-option {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

.button-group {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}

.btn-cancel {
  padding: 10px 20px;
  border: 1px solid #ddd;
  background-color: white;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.btn-save {
  padding: 10px 20px;
  border: none;
  background-color: #000;
  color: white;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  min-width: 120px;
}

.btn-save:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.alert {
  padding: 12px;
  border-radius: 4px;
  margin-bottom: 20px;
  font-size: 14px;
}

.alert-success {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}

.alert-danger {
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}

.password-placeholder {
  background-color: #f9f9f9;
  padding: 15px;
  border-radius: 4px;
}

.password-hint {
  font-size: 12px;
  color: #777;
  margin-top: 8px;
}

.error-message {
  color: #dc3545;
  font-size: 12px;
  margin-top: 4px;
}

.spinning {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }

  to {
    transform: rotate(360deg);
  }
}

@media (max-width: 768px) {
  .form-row {
    flex-direction: column;
    gap: 15px;
  }

  .account-container {
    padding: 20px;
  }
}
</style>
