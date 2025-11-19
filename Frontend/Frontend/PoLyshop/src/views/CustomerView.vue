<script setup>
import { ref, onMounted, computed } from "vue";
import axios from "axios";

const customers = ref([]); // Danh sách khách hàng
const searchTerm = ref(""); // Từ khóa tìm kiếm
const error = ref(null); // Lỗi khi lấy dữ liệu
const loading = ref(false); // Trạng thái loading

// Sample data for development
const sampleCustomers = [
  {
    customerId: 1,
    fullName: "Nguyễn Văn An",
    email: "nguyenvana@gmail.com",
    phone: "0901234567",
  },
  {
    customerId: 2,
    fullName: "Trần Thị Bình",
    email: "tranthib@gmail.com",
    phone: "0911234567",
  },
  {
    customerId: 3,
    fullName: "Lê Văn Cường",
    email: "levanc@gmail.com",
    phone: "0921234567",
  },
  {
    customerId: 4,
    fullName: "Phạm Thị Dung",
    email: "phamthid@gmail.com",
    phone: "0931234567",
  },
];

// Hàm lấy dữ liệu khách hàng (sử dụng dữ liệu mẫu)
const fetchCustomers = async () => {
  try {
    loading.value = true;

    // For development - using sample data
    setTimeout(() => {
      customers.value = sampleCustomers;
      loading.value = false;
    }, 500);

    // API implementation (commented out for development)
    // const res = await axios.get('/api/Customer')
    // customers.value = res.data || []
  } catch (err) {
    error.value = "Không thể lấy dữ liệu khách hàng.";
    loading.value = false;
  }
};

// Hàm lọc khách hàng theo tên hoặc email
const filteredCustomers = computed(() => {
  return customers.value.filter((customer) => {
    const search = searchTerm.value.toLowerCase();
    return (
      customer.fullName?.toLowerCase().includes(search) ||
      customer.email?.toLowerCase().includes(search)
    );
  });
});

// Lấy dữ liệu khách hàng khi component được mount
onMounted(fetchCustomers);
</script>

<template>
  <div class="container py-5">
    <h3 class="text-center">Quản lý Khách Hàng</h3>

    <!-- Tìm kiếm -->
    <div class="d-flex flex-grow-1 me-3 mb-3">
      <input
        type="text"
        class="form-control"
        placeholder="Tìm theo tên hoặc email"
        v-model="searchTerm"
      />
      <button class="btn btn-outline-secondary ms-2">
        <i class="bi bi-search"></i>
      </button>
    </div>

    <!-- Thêm khách hàng -->
    <div class="d-flex justify-content-between align-items-center mb-3 mt-3">
      <router-link to="/create-customer" class="btn btn-success">
        <i class="bi bi-plus"></i> Thêm Khách Hàng
      </router-link>
    </div>

    <!-- Loading indicator -->
    <div v-if="loading" class="text-center py-3">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Đang tải...</span>
      </div>
    </div>

    <!-- Hiển thị danh sách khách hàng -->
    <div class="row" v-else>
      <div v-if="error" class="alert alert-danger text-center">
        {{ error }}
      </div>

      <div v-if="filteredCustomers.length === 0" class="text-center">
        <p>Không có khách hàng nào.</p>
      </div>

      <div
        class="col-md-5 m-2 p-3 border rounded"
        v-for="customer in filteredCustomers"
        :key="customer.customerId"
      >
        <p><strong>Tên khách hàng:</strong> {{ customer.fullName }}</p>
        <p><strong>Email:</strong> {{ customer.email }}</p>
        <p><strong>Số điện thoại:</strong> {{ customer.phone }}</p>
        <router-link
          :to="'/customer-details/' + customer.customerId"
          class="btn btn-primary"
          >Chi Tiết</router-link
        >
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Style cho các phần tử trong view */
</style>
