<template>
  <div class="cart-container">
    <h1 class="page-title">Giỏ Hàng</h1>

    <div class="cart-content">
      <div class="cart-items">
        <table class="cart-table">
          <thead>
            <tr>
              <th class="select-col">
                <input type="checkbox" v-model="isAllSelected" />
              </th>
              <th class="product-col">Sản phẩm</th>
              <th class="detail-col">Chi tiết</th>
              <th class="price-col">Giá</th>
              <th class="quantity-col">Số lượng</th>
              <th class="action-col">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in cartItems" :key="item.uniqueId || index">
              <td class="select-col">
                <input type="checkbox" v-model="item.selected" />
              </td>
              <td class="product-col">
                <div class="product-image">
                  <img :src="item.image" :alt="item.name" />
                </div>
              </td>
              <td class="detail-col">
                <div class="product-name">{{ item.name }}</div>
                <div class="product-category">
                  Phân Loại: {{ item.category }}
                </div>
                <div class="product-details">
                  Size: <strong>{{ item.sizeName }}</strong> - Màu: <strong>{{ item.colorName }}</strong>
                </div>
              </td>
              <td class="price-col">{{ formatPrice(item.price) }}</td>
              <td class="quantity-col">
                <div class="quantity-selector">
                  <button class="quantity-btn" @click="decreaseQty(index)">-</button>
                  <input type="text" class="quantity-input" :value="item.quantity" readonly />
                  <button class="quantity-btn" @click="increaseQty(index)">+</button>
                </div>
              </td>
              <td class="action-col">
                <div class="action-buttons">
                  <button class="remove-button" @click="removeItemFromCart(index)">
                    Xóa
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <div v-if="cartItems.length === 0" class="empty-cart">
          <i class="bi bi-cart-x" style="font-size: 48px; color: #ccc;"></i>
          <p>Giỏ hàng của bạn trống</p>
          <router-link to="/shop" class="continue-shopping">Tiếp tục mua sắm</router-link>
        </div>

        <div class="cart-actions" v-if="cartItems.length > 0">
          <div class="select-all">
            <input type="checkbox" v-model="isAllSelected" />
            <span>Chọn tất cả ({{ cartItems.length }} sản phẩm)</span>
          </div>
          <div class="cart-summary">
            <div class="cart-total">
              <span>Tổng tiền: </span>
              <span class="total-price">{{ formatPrice(totalPrice) }}</span>
            </div>
            <button class="checkout-button" @click="handleCheckout">Đặt hàng</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
// Import các hàm từ Store
import { cartState, removeFromCart, updateQuantity, calculateTotal } from "@/utils/cartStore";

export default {
  name: "CartView",
  computed: {
    // 1. Lấy danh sách sản phẩm từ Store
    cartItems() {
      return cartState.items;
    },
    // 2. Tính tổng tiền (Store đã tính sẵn, ta chỉ việc gọi)
    totalPrice() {
      return calculateTotal();
    },
    // 3. Logic cho nút "Chọn tất cả"
    isAllSelected: {
      get() {
        return this.cartItems.length > 0 && this.cartItems.every((item) => item.selected);
      },
      set(value) {
        this.cartItems.forEach((item) => (item.selected = value));
      },
    },
  },
  methods: {
    formatPrice(price) {
      return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
        minimumFractionDigits: 0,
      }).format(price);
    },
    // Các hàm thao tác gọi sang Store
    increaseQty(index) {
      updateQuantity(index, 1);
    },
    decreaseQty(index) {
      updateQuantity(index, -1);
    },
    removeItemFromCart(index) {
      if (confirm("Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?")) {
        removeFromCart(index);
      }
    },
    handleCheckout() {
      if (this.totalPrice === 0) {
        alert("Vui lòng chọn ít nhất một sản phẩm để thanh toán");
        return;
      }
      // Lưu sản phẩm đã chọn vào LocalStorage tạm để trang Checkout dùng
      const checkoutItems = this.cartItems.filter((item) => item.selected);
      localStorage.setItem("checkoutItems", JSON.stringify(checkoutItems));
      
      // Chuyển hướng (đảm bảo bạn có route này)
      this.$router.push("/checkout"); 
    },
  },
};
</script>

<style scoped>
/* Giữ nguyên style cũ của bạn, nó đã rất đẹp rồi */
.cart-container { max-width: 1200px; margin: 0 auto; padding: 30px 20px; }
.page-title { text-align: center; font-size: 28px; margin-bottom: 30px; }
.cart-table { width: 100%; border-collapse: collapse; }
.cart-table th { padding: 15px; text-align: center; background-color: #f9f9f9; border-bottom: 1px solid #ddd; }
.cart-table td { padding: 15px; border-bottom: 1px solid #ddd; vertical-align: middle; }
.select-col { width: 5%; text-align: center; }
.product-col { width: 15%; }
.product-image { width: 80px; height: 80px; margin: 0 auto; }
.product-image img { width: 100%; height: 100%; object-fit: cover; border-radius: 4px;}
.detail-col { width: 30%; }
.product-name { font-weight: bold; margin-bottom: 5px; }
.product-category, .product-details { font-size: 14px; color: #666; margin-bottom: 5px; }
.price-col { width: 15%; text-align: center; font-weight: bold; }
.quantity-col { width: 15%; text-align: center; }
.quantity-selector { display: flex; align-items: center; justify-content: center; width: 100px; margin: 0 auto; }
.quantity-btn { width: 30px; height: 30px; background-color: #f5f5f5; border: 1px solid #ddd; cursor: pointer; display: flex; align-items: center; justify-content: center; }
.quantity-input { flex: 1; height: 30px; border: 1px solid #ddd; text-align: center; width: 30px; border-left: none; border-right: none;}
.action-col { width: 20%; text-align: center; }
.action-buttons { display: flex; flex-direction: column; gap: 5px; align-items: center; }
.voucher-button { background-color: #4caf50; color: white; border: none; padding: 5px 10px; border-radius: 4px; cursor: pointer; font-size: 12px;}
.remove-button { background-color: #f44336; color: white; border: none; padding: 5px 10px; border-radius: 4px; cursor: pointer; font-size: 12px;}
.empty-cart { text-align: center; padding: 50px 0; }
.continue-shopping { display: inline-block; margin-top: 20px; padding: 10px 20px; background-color: #333; color: white; text-decoration: none; border-radius: 4px; }
.cart-actions { display: flex; justify-content: space-between; align-items: center; padding: 20px 0; border-top: 2px solid #eee; margin-top: 20px;}
.select-all { display: flex; align-items: center; gap: 10px; }
.cart-summary { display: flex; align-items: center; gap: 20px; }
.cart-total { font-size: 18px; font-weight: bold; }
.total-price { color: #f44336; font-size: 20px; margin-left: 5px;}
.checkout-button { background-color: #ff5722; color: white; border: none; padding: 12px 30px; border-radius: 4px; font-weight: bold; cursor: pointer; }
@media (max-width: 768px) { .cart-table { display: block; overflow-x: auto; } .cart-actions { flex-direction: column; gap: 15px; align-items: flex-end;} }
</style>