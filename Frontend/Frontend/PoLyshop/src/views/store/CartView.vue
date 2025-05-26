<template>
  <div class="cart-container">
    <h1 class="page-title">Giỏ Hàng</h1>

    <div class="cart-content">
      <div class="cart-items">
        <table class="cart-table">
          <thead>
            <tr>
              <th class="select-col"></th>
              <th class="product-col">Sản phẩm</th>
              <th class="detail-col">Chi tiết</th>
              <th class="price-col">Giá</th>
              <th class="quantity-col">Số lượng</th>
              <th class="action-col">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in cartItems" :key="index">
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
                  Phân Loại Hàng: {{ item.category }}
                </div>
                <div class="product-details">Áo, cỡ, màu, chất liệu</div>
              </td>
              <td class="price-col">{{ formatPrice(item.price) }}</td>
              <td class="quantity-col">
                <div class="quantity-selector">
                  <button class="quantity-btn" @click="decreaseQuantity(index)">
                    -
                  </button>
                  <input type="text" class="quantity-input" v-model="item.quantity" readonly />
                  <button class="quantity-btn" @click="increaseQuantity(index)">
                    +
                  </button>
                </div>
              </td>
              <td class="action-col">
                <div class="action-buttons">
                  <button class="voucher-button">Voucher</button>
                  <button class="remove-button" @click="removeItem(index)">
                    Xóa
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <div v-if="cartItems.length === 0" class="empty-cart">
          <p>Giỏ hàng của bạn trống</p>
          <router-link to="/shop" class="continue-shopping">Tiếp tục mua sắm</router-link>
        </div>

        <div class="cart-actions">
          <div class="select-all">
            <input type="checkbox" v-model="selectAll" @change="toggleSelectAll" />
            <span>Tất cả</span>
          </div>
          <div class="cart-summary">
            <div class="cart-total">
              <span>Tổng tiền: </span>
              <span class="total-price">{{
                formatPrice(calculateTotal())
              }}</span>
            </div>
            <button class="checkout-button" @click="checkout">Đặt hàng</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "CartView",
  data() {
    return {
      cartItems: [],
      selectAll: true,
    };
  },
  created() {
    // Load cart items from localStorage when component is created
    this.loadCartFromStorage();
  },
  methods: {
    loadCartFromStorage() {
      const savedCart = localStorage.getItem("cart");

      if (savedCart) {
        this.cartItems = JSON.parse(savedCart);
        this.cartItems.forEach((item) => {
          if (item.selected === undefined) {
            item.selected = true;
          }
        });
      } else {
        // Khi localStorage chưa có dữ liệu thì tạo dữ liệu mặc định
        this.cartItems = [
          {
            name: "Áo thun POLY Basic",
            category: "Áo / Size M / Trắng",
            image:"https://media1.thehungryjpeg.com/thumbs/800_4262428_zzultqdefwrzpkyb25nsmpzexo4h46xkmpbv2rmb.png", // nhớ xử lý ảnh đúng
            price: 280000,
            quantity: 1,
            selected: true,
          },
          {
            name: "Quần jean slim fit",
            category: "Quần / Size 30 / Xanh đậm",
            image: "https://jeanthuanhai.com/wp-content/uploads/2023/07/z4450182316466-d5ba23a0926f883115ddd2acdf3909d6.jpgFron",
            price: 450000,
            quantity: 2,
            selected: true,
          },
        ];

        // Lưu lại dữ liệu mẫu vào localStorage để lần sau load có sẵn
        this.saveCartToStorage();
      }
    },
    saveCartToStorage() {
      localStorage.setItem("cart", JSON.stringify(this.cartItems));
    },
    formatPrice(price) {
      return price.toLocaleString("vi-VN") + "đ";
    },
    increaseQuantity(index) {
      this.cartItems[index].quantity++;
      this.saveCartToStorage();
    },
    decreaseQuantity(index) {
      if (this.cartItems[index].quantity > 1) {
        this.cartItems[index].quantity--;
        this.saveCartToStorage();
      }
    },
    removeItem(index) {
      this.cartItems.splice(index, 1);
      this.saveCartToStorage();
    },
    calculateTotal() {
      return this.cartItems
        .filter((item) => item.selected)
        .reduce((total, item) => total + item.price * item.quantity, 0);
    },
    toggleSelectAll() {
      this.cartItems.forEach((item) => {
        item.selected = this.selectAll;
      });
      this.saveCartToStorage();
    },
    checkout() {
      if (this.cartItems.filter((item) => item.selected).length === 0) {
        alert("Vui lòng chọn ít nhất một sản phẩm để thanh toán");
        return;
      }

      // Save selected items to a temp storage for checkout
      const checkoutItems = this.cartItems.filter((item) => item.selected);
      localStorage.setItem("checkoutItems", JSON.stringify(checkoutItems));

      // Navigate to checkout page
      this.$router.push("/checkout");
    },
  },
  watch: {
    cartItems: {
      handler() {
        this.selectAll =
          this.cartItems.length > 0 &&
          this.cartItems.every((item) => item.selected);

        // Save changes to localStorage
        this.saveCartToStorage();
      },
      deep: true,
    },
  },
};
</script>

<style scoped>
.cart-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 30px 20px;
}

.page-title {
  text-align: center;
  font-size: 28px;
  margin-bottom: 30px;
}

.cart-table {
  width: 100%;
  border-collapse: collapse;
}

.cart-table th {
  padding: 15px;
  text-align: center;
  background-color: #f9f9f9;
  border-bottom: 1px solid #ddd;
}

.cart-table td {
  padding: 15px;
  border-bottom: 1px solid #ddd;
  vertical-align: middle;
}

.select-col {
  width: 5%;
  text-align: center;
}

.product-col {
  width: 15%;
}

.product-image {
  width: 100px;
  height: 100px;
  margin: 0 auto;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.detail-col {
  width: 30%;
}

.product-name {
  font-weight: bold;
  margin-bottom: 5px;
}

.product-category,
.product-details {
  font-size: 14px;
  color: #666;
  margin-bottom: 5px;
}

.price-col {
  width: 15%;
  text-align: center;
  font-weight: bold;
}

.quantity-col {
  width: 15%;
  text-align: center;
}

.quantity-selector {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 120px;
  margin: 0 auto;
}

.quantity-btn {
  width: 30px;
  height: 30px;
  background-color: #f5f5f5;
  border: 1px solid #ddd;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
}

.quantity-input {
  flex: 1;
  height: 30px;
  border: 1px solid #ddd;
  text-align: center;
  width: 40px;
}

.action-col {
  width: 20%;
  text-align: center;
}

.action-buttons {
  display: flex;
  flex-direction: column;
  gap: 5px;
  align-items: center;
}

.voucher-button {
  background-color: #4caf50;
  color: white;
  border: none;
  padding: 5px 15px;
  border-radius: 4px;
  cursor: pointer;
  width: 80px;
}

.remove-button {
  background-color: #f44336;
  color: white;
  border: none;
  padding: 5px 15px;
  border-radius: 4px;
  cursor: pointer;
  width: 80px;
}

.empty-cart {
  text-align: center;
  padding: 50px 0;
}

.continue-shopping {
  display: inline-block;
  margin-top: 20px;
  padding: 10px 20px;
  background-color: #333;
  color: white;
  text-decoration: none;
  border-radius: 4px;
}

.cart-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 0;
}

.select-all {
  display: flex;
  align-items: center;
  gap: 10px;
}

.cart-summary {
  display: flex;
  align-items: center;
  gap: 20px;
}

.cart-total {
  font-size: 18px;
  font-weight: bold;
}

.total-price {
  color: #f44336;
}

.checkout-button {
  background-color: #4caf50;
  color: white;
  border: none;
  padding: 12px 25px;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
}

@media (max-width: 768px) {
  .cart-table {
    display: block;
    overflow-x: auto;
  }

  .cart-actions {
    flex-direction: column;
    gap: 15px;
  }
}
</style>
