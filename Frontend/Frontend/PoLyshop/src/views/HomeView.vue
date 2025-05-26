<script setup>
import { ref, onMounted } from "vue";
import axios from "axios";

const products = ref([]);
const error = ref(null);

onMounted(async () => {
  try {
    const res = await axios.get("https://localhost:7055/trang-chu");
    products.value = res.data.$values || [];
  } catch (err) {
    error.value = err.message;
  }
});

// Hàm chia mảng thành từng nhóm 4 sản phẩm
const chunk = (array, size) => {
  const result = [];
  for (let i = 0; i < array.length; i += size) {
    result.push(array.slice(i, i + size));
  }
  return result;
};

// Ảnh mặc định
const defaultImage = "../src/components/img/ao1.webp";
</script>

<template>
  <div class="container py-5">
    <h2 class="text-center mb-4">Sản phẩm mới</h2>

    <div v-if="error" class="alert alert-danger text-center">
      {{ error }}
    </div>

    <div v-else-if="products.length === 0">
      <div class="text-center">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Đang tải...</span>
        </div>
      </div>
    </div>

    <div
      v-else
      id="productCarousel"
      class="carousel slide"
      data-bs-ride="carousel"
    >
      <div class="carousel-inner">
        <div
          class="carousel-item"
          :class="{ active: index === 0 }"
          v-for="(group, index) in chunk(products, 4)"
          :key="index"
        >
          <div class="row row-cols-1 row-cols-sm-2 row-cols-md-4 g-4">
            <div class="col" v-for="(item, i) in group" :key="i">
              <div class="card h-100 shadow-sm">
                <img
                  :src="item.image || defaultImage"
                  class="card-img-top"
                  alt="Ảnh sản phẩm"
                  style="height: 250px; object-fit: cover"
                />
                <div class="card-body text-center">
                  <h5 class="card-title">{{ item.productName }}</h5>
                  <p class="card-text text-danger">{{ item.price }}$</p>
                  <button class="btn btn-dark">Add to Cart</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <button
        class="carousel-control-prev"
        type="button"
        data-bs-target="#productCarousel"
        data-bs-slide="prev"
      >
        <span
          class="carousel-control-prev-icon bg-dark p-2"
          aria-hidden="true"
        ></span>
        <span class="visually-hidden">Previous</span>
      </button>
      <button
        class="carousel-control-next"
        type="button"
        data-bs-target="#productCarousel"
        data-bs-slide="next"
      >
        <span
          class="carousel-control-next-icon bg-dark p-2"
          aria-hidden="true"
        ></span>
        <span class="visually-hidden">Next</span>
      </button>
    </div>
  </div>
</template>
