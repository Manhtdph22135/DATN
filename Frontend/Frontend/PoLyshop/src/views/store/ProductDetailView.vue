<template>
  <div class="product-detail-container">
    <div class="product-detail">
      <div class="product-images">
        <div class="main-image">
          <img :src="product.mainImage" alt="Product Image" />
        </div>
        <div class="thumbnail-images">
          <div v-for="(image, index) in product.images" :key="index" class="thumbnail">
            <img :src="image" alt="Product Thumbnail" />
          </div>
        </div>
      </div>

      <div class="product-info">
        <h1 class="product-name">{{ product.name }}</h1>
        <div class="product-price">{{ formatPrice(product.price) }}</div>

        <div class="product-details">
          <div class="detail-item">{{ product.gender }}</div>
          <div class="detail-item">{{ product.protection }}</div>
        </div>

        <div class="product-options">
          <div class="option-section">
            <div class="option-label">Size</div>
            <div class="size-options">
              <button v-for="size in product.sizes" :key="size" class="size-option"
                :class="{ active: selectedSize === size }" @click="selectedSize = size">
                {{ size }}
              </button>
            </div>
          </div>

          <div class="option-section">
            <div class="option-label">Color</div>
            <div class="color-options">
              <button v-for="(color, index) in product.colors" :key="index" class="color-option"
                :style="{ backgroundColor: color }" :class="{ active: selectedColor === color }"
                @click="selectedColor = color"></button>
            </div>
          </div>

          <div class="option-section">
            <div class="option-label">Quantity:</div>
            <div class="quantity-selector">
              <button class="quantity-btn" @click="decreaseQuantity">
                <i class="bi bi-dash"></i>
              </button>
              <input type="text" class="quantity-input" v-model="quantity" readonly />
              <button class="quantity-btn" @click="increaseQuantity">
                <i class="bi bi-plus"></i>
              </button>
            </div>
          </div>
        </div>

        <button class="add-to-cart-btn" @click="addToCart">
          Thêm vào giỏ hàng
        </button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "ProductDetailView",
  data() {
    return {
      product: {
        name: "Basic Tee",
        price: 200000,
        gender: "Unisex",
        protection: "UV Protection",
        mainImage: "https://media1.thehungryjpeg.com/thumbs/800_4262428_zzultqdefwrzpkyb25nsmpzexo4h46xkmpbv2rmb.png",
        images: [
          "https://media1.thehungryjpeg.com/thumbs/800_4262428_zzultqdefwrzpkyb25nsmpzexo4h46xkmpbv2rmb.png",
          "https://media1.thehungryjpeg.com/thumbs/800_4262428_zzultqdefwrzpkyb25nsmpzexo4h46xkmpbv2rmb.png",
        ],
        sizes: ["M", "L", "XL"],
        colors: ["#fffff", "#b71c1c", "#4CAF50"],
      },
      selectedSize: "M",
      selectedColor: "#b71c1c",
      quantity: 1,
    };
  },
  methods: {
    formatPrice(price) {
      return price.toLocaleString("vi-VN") + " VND";
    },
    increaseQuantity() {
      this.quantity++;
    },
    decreaseQuantity() {
      if (this.quantity > 1) {
        this.quantity--;
      }
    },
    addToCart() {
      // In a real app, this would call a store action or API
      console.log("Added to cart:", {
        product: this.product.name,
        size: this.selectedSize,
        color: this.selectedColor,
        quantity: this.quantity,
      });

      // Show success message
      alert("Product added to cart!");
    },
  },
};
</script>

<style scoped>
.product-detail-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 40px 20px;
}

.product-detail {
  display: flex;
  gap: 40px;
}

.product-images {
  flex: 1;
}

.main-image {
  width: 100%;
  margin-bottom: 20px;
}

.main-image img {
  width: 100%;
  height: auto;
  object-fit: cover;
}

.thumbnail-images {
  display: flex;
  gap: 10px;
}

.thumbnail {
  width: 80px;
  height: 80px;
  cursor: pointer;
}

.thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.product-info {
  flex: 1;
  padding: 20px;
}

.product-name {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 10px;
}

.product-price {
  font-size: 20px;
  color: #f44336;
  font-weight: bold;
  margin-bottom: 20px;
}

.product-details {
  margin-bottom: 20px;
}

.detail-item {
  margin-bottom: 5px;
}

.product-options {
  margin-bottom: 30px;
}

.option-section {
  margin-bottom: 20px;
}

.option-label {
  font-weight: bold;
  margin-bottom: 10px;
}

.size-options {
  display: flex;
  gap: 10px;
}

.size-option {
  width: 40px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid #ddd;
  background-color: #f5f5f5;
  cursor: pointer;
}

.size-option.active {
  background-color: #333;
  color: white;
}

.color-options {
  display: flex;
  gap: 10px;
}

.color-option {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  border: 1px solid #ddd;
  cursor: pointer;
}

.color-option.active {
  border: 2px solid #333;
}

.quantity-selector {
  display: flex;
  align-items: center;
  width: 120px;
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
  width: 40px;
  height: 30px;
  border: 1px solid #ddd;
  text-align: center;
}

.add-to-cart-btn {
  width: 100%;
  padding: 15px;
  background-color: #333;
  color: white;
  border: none;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s;
}

.add-to-cart-btn:hover {
  background-color: #000;
}

@media (max-width: 768px) {
  .product-detail {
    flex-direction: column;
  }
}
</style>
