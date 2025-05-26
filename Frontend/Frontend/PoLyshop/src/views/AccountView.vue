<script setup>
import { ref, onMounted, computed } from "vue";
import axios from "axios";

const accounts = ref([]); // Danh sách tài khoản
const searchTerm = ref(""); // Từ khóa tìm kiếm
const error = ref(null); // Lỗi khi lấy dữ liệu
const loading = ref(false); // Trạng thái loading

// Sample data for development
const sampleAccounts = [
  {
    AccountId: 1,
    Username: "admin",
    RoleName: "Admin",
    CreateAt: "2023-01-15T08:30:00",
  },
  {
    AccountId: 2,
    Username: "staff1",
    RoleName: "Nhân viên",
    CreateAt: "2023-02-20T10:15:00",
  },
  {
    AccountId: 3,
    Username: "user123",
    RoleName: "Khách hàng",
    CreateAt: "2023-03-05T14:45:00",
  },
  {
    AccountId: 4,
    Username: "manager1",
    RoleName: "Quản lý",
    CreateAt: "2023-04-10T09:30:00",
  },
];

// Hàm lấy dữ liệu tài khoản
const fetchAccounts = async () => {
  try {
    loading.value = true;

    // For development - using sample data
    setTimeout(() => {
      accounts.value = sampleAccounts;
      loading.value = false;
    }, 500);

    // API implementation (commented out for development)
    // const res = await axios.get("/api/Auth");
    // accounts.value = res.data || [];
  } catch (err) {
    error.value = "Không thể lấy dữ liệu tài khoản.";
    loading.value = false;
  }
};

// Hàm lọc tài khoản theo tên hoặc vai trò
const filteredAccounts = computed(() => {
  return accounts.value.filter((account) => {
    const search = searchTerm.value.toLowerCase();
    return (
      account.Username?.toLowerCase().includes(search) ||
      account.RoleName?.toLowerCase().includes(search)
    );
  });
});

// Lấy dữ liệu tài khoản khi component được mount
onMounted(fetchAccounts);
</script>

<template>
  <div class="container py-5">
    <h3 class="text-center">Quản lý Tài Khoản</h3>

    <!-- Tìm kiếm -->
    <div class="d-flex flex-grow-1 me-3 mb-3">
      <input
        type="text"
        class="form-control"
        placeholder="Tìm theo tên tài khoản hoặc vai trò"
        v-model="searchTerm"
      />
      <button class="btn btn-outline-secondary ms-2">
        <i class="fas fa-search"></i>
      </button>
    </div>

    <!-- Thêm tài khoản -->
    <div class="d-flex justify-content-between align-items-center mb-3 mt-3">
      <router-link to="/create-account" class="btn btn-success">
        <i class="fas fa-plus"></i> Thêm Tài Khoản
      </router-link>
    </div>

    <!-- Loading indicator -->
    <div v-if="loading" class="text-center py-3">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Đang tải...</span>
      </div>
    </div>

    <!-- Hiển thị danh sách tài khoản -->
    <div class="row" v-else>
      <div v-if="error" class="alert alert-danger text-center">
        {{ error }}
      </div>

      <div v-if="filteredAccounts.length === 0" class="text-center">
        <p>Không có tài khoản nào.</p>
      </div>

      <div
        class="col-md-5 m-2 p-3 border rounded"
        v-for="account in filteredAccounts"
        :key="account.AccountId"
      >
        <p><strong>Tên tài khoản:</strong> {{ account.Username }}</p>
        <p><strong>Vai trò:</strong> {{ account.RoleName }}</p>
        <p>
          <strong>Ngày tạo:</strong>
          {{ new Date(account.CreateAt).toLocaleDateString() }}
        </p>
        <router-link
          :to="'/account-details/' + account.AccountId"
          class="btn btn-primary"
          >Chi Tiết</router-link
        >
      </div>
    </div>
  </div>
</template>
