<template>
  <div class="product-list-container">
    <div class="hero-section">
      <div class="hero-content">
        <h1 class="hero-title">NEW SEASON DROP!</h1>
        <button class="shop-now-btn">SHOP NOW</button>
      </div>
    </div>

    <div class="category-section">
      <div class="category-header">
        <h2 class="category-title">TOP</h2>
        <div class="category-bottom">BOTTOM</div>
      </div>

      <div class="category-content">
        <div class="category-column">
          <div class="product-item">
            <div class="product-images">
              <img
                src="@/components/img/anh1.webp"
                alt="Product front"
                class="product-image"
              />
            </div>
            <div class="product-like">
              <i class="bi bi-heart"></i>
            </div>
            <div class="product-thumbnails">
              <img
                src="@/components/img/hutdi.webp"
                alt="Thumbnail 1"
                class="product-thumbnail"
              />
              <img
                src="@/components/img/sipcacdieu.webp"
                alt="Thumbnail 2"
                class="product-thumbnail"
              />
              <img
                src="@/components/img/aothun.webp"
                alt="Thumbnail 3"
                class="product-thumbnail"
              />
            </div>
            <div class="product-info">
              <h3 class="product-name">MORE PRODUCTS</h3>
              <button class="more-btn">
                <i class="bi bi-arrow-right"></i>
              </button>
            </div>
          </div>
        </div>

        <div class="category-column">
          <div class="product-item">
            <div class="product-images">
              <img
                src="@/components/img/ao2.webp"
                alt="Product front"
                class="product-image"
              />
            </div>
            <div class="product-like">
              <i class="bi bi-heart"></i>
            </div>
            <div class="product-thumbnails">
              <img
                src="@/components/img/dsc04625-6188.jpg"
                alt="Thumbnail 1"
                class="product-thumbnail"
              />
              <img
                src="@/components/img/jean4.webp"
                alt="Thumbnail 2"
                class="product-thumbnail"
              />
              <img
                src="@/components/img/ao3.webp"
                alt="Thumbnail 3"
                class="product-thumbnail"
              />
            </div>
            <div class="product-info">
              <h3 class="product-name">MORE PRODUCTS</h3>
              <button class="more-btn">
                <i class="bi bi-arrow-right"></i>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="products-section">
      <h2 class="section-title">New Goods</h2>

      <div class="products-grid">
        <div
          v-for="(product, index) in products"
          :key="index"
          class="product-card"
        >
          <router-link :to="`/product/${product.id}`" class="product-link">
            <div class="product-image-container">
              <img
                :src="product.image"
                :alt="product.name"
                class="product-image"
              />
            </div>
            <div class="product-details">
              <h3 class="product-name">{{ product.name }}</h3>
              <div class="product-price">{{ formatPrice(product.price) }}</div>
            </div>
          </router-link>
        </div>
      </div>

      <h2 class="section-title">T-Shirts</h2>

      <div class="products-grid">
        <div
          v-for="(product, index) in tShirts"
          :key="index"
          class="product-card"
        >
          <router-link :to="`/product/${product.id}`" class="product-link">
            <div class="product-image-container">
              <img
                :src="product.image"
                :alt="product.name"
                class="product-image"
              />
            </div>
            <div class="product-details">
              <h3 class="product-name">{{ product.name }}</h3>
              <div class="product-price">{{ formatPrice(product.price) }}</div>
            </div>
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "ProductListView",
  data() {
    return {
      products: [
        {
          id: 1,
          name: "Men's shirt",
          price: 500000,
          image: "https://dongphuchaianh.vn/wp-content/uploads/2022/08/Cac-mau-ao-so-mi-nam-dep-nhat-mau-trang.jpeg",
        },
        {
          id: 2,
          name: "Varsity Classic",
          price: 650000,
          image: "https://www.varsitybuilt.com/wp-content/uploads/2023/08/varsity-classic-vintage-black-heather-white-copy-scaled-thegem-blog-timeline-large.webp",
        },
        {
          id: 3,
          name: "Denim Jacket",
          price: 720000,
          image: "https://media.istockphoto.com/photos/denim-jacket-with-path-picture-id475570172?k=6&m=475570172&s=612x612&w=0&h=Ws0rOv8zv7IMCu6hyxtsp3m5nR40zpRKAuoHks99R3U=",
        },
      ],
      tShirts: [
        {
          id: 4,
          name: "Basic Tee",
          price: 200000,
          image: "https://media1.thehungryjpeg.com/thumbs/800_4262428_zzultqdefwrzpkyb25nsmpzexo4h46xkmpbv2rmb.png",
        },
        {
          id: 5,
          name: "Graphic T-Shirt",
          price: 250000,
          image: "https://img.freepik.com/premium-photo/owl-t-shirt-graphic-design-illustration_625492-1072.jpg",
        },
        {
          id: 6,
          name: "Oversized Tee",
          price: 280000,
          image: "https://png.pngtree.com/png-clipart/20230429/ourlarge/pngtree-black-oversized-fit-t-shirt-mockup-png-image_6740829.png",
        },
      ],
    };
  },
  methods: {
    formatPrice(price) {
      return price.toLocaleString("vi-VN") + " đ";
    },
  },
};
</script>

<style scoped>
.product-list-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

/* Hero Section */
.hero-section {
  background-image: url("@/components/img/banner.webp");
  background-size: cover;
  background-position: center;
  height: 500px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 50px;
}

.hero-content {
  text-align: center;
  color: white;
}

.hero-title {
  font-size: 48px;
  font-weight: bold;
  margin-bottom: 20px;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
}

.shop-now-btn {
  background-color: white;
  color: black;
  border: none;
  padding: 10px 30px;
  font-size: 18px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s;
}

.shop-now-btn:hover {
  background-color: #f5f5f5;
}

/* Category Section */
.category-section {
  margin-bottom: 50px;
}

.category-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 30px;
}

.category-title,
.category-bottom {
  font-size: 24px;
  font-weight: bold;
}

.category-content {
  display: flex;
  gap: 30px;
}

.category-column {
  flex: 1;
}

.product-item {
  position: relative;
}

.product-images {
  position: relative;
  margin-bottom: 10px;
}

.product-image {
  width: 100%;
  height: auto;
  display: block;
}

.product-like {
  position: absolute;
  top: 15px;
  right: 15px;
  width: 40px;
  height: 40px;
  background-color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.product-like i {
  color: #f44336;
  font-size: 20px;
}

.product-thumbnails {
  display: flex;
  gap: 5px;
  margin-bottom: 10px;
}

.product-thumbnail {
  width: 50px;
  height: 50px;
  object-fit: cover;
  cursor: pointer;
}

.product-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.product-name {
  font-size: 16px;
  font-weight: bold;
}

.more-btn {
  background-color: #4285f4;
  color: white;
  border: none;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

/* Products Section */
.section-title {
  font-size: 24px;
  font-weight: bold;
  margin: 40px 0 20px;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 30px;
  margin-bottom: 50px;
}

.product-card {
  overflow: hidden;
  transition: transform 0.3s;
}

.product-card:hover {
  transform: translateY(-5px);
}

.product-link {
  text-decoration: none;
  color: inherit;
}

.product-image-container {
  width: 100%;
  aspect-ratio: 1;
  margin-bottom: 10px;
  overflow: hidden;
}

.product-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s;
}

.product-card:hover .product-image {
  transform: scale(1.05);
}

.product-details {
  padding: 10px 0;
}

.product-name {
  font-size: 16px;
  font-weight: bold;
  margin-bottom: 5px;
}

.product-price {
  color: #f44336;
  font-weight: bold;
}

@media (max-width: 768px) {
  .hero-title {
    font-size: 36px;
  }

  .category-content {
    flex-direction: column;
  }

  .products-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 480px) {
  .products-grid {
    grid-template-columns: 1fr;
  }
}
</style>
