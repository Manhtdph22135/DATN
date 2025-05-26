<script setup>
import { RouterLink, RouterView } from "vue-router";
import { ref, onMounted, computed } from "vue";
import authService, { authState } from "./untility/authService";
import { useRouter } from "vue-router";

const router = useRouter();
const showUserDropdown = ref(false);

// Use the authState from authService for reactivity
const isLoggedIn = computed(() => authState.isLoggedIn.value);
const userData = computed(() => authState.user.value);

// Computed property to display user's name or username
const displayName = computed(() => {
  if (!userData.value) return "";
  return userData.value.fullName || userData.value.username || "Người dùng";
});

// Toggle user dropdown
function toggleUserDropdown() {
  showUserDropdown.value = !showUserDropdown.value;
}

// Close dropdown when clicking outside
function closeDropdownOnOutsideClick(event) {
  const dropdown = document.querySelector(".user-menu-dropdown");
  const userIcon = document.querySelector(".user-icon-clickable");
  if (
    dropdown &&
    !dropdown.contains(event.target) &&
    !userIcon.contains(event.target)
  ) {
    showUserDropdown.value = false;
  }
}

// Add event listener for outside clicks
onMounted(() => {
  document.addEventListener("click", closeDropdownOnOutsideClick);
});

// Handle logout
function logout() {
  authService.logout();
  router.push("/login");
  showUserDropdown.value = false;
}

// Navigate to Admin Dashboard
function goToAdminDashboard() {
  showUserDropdown.value = false;

  // Make sure the user has admin privileges
  const userData = authService.getUserData();
  if (userData) {
    userData.role = "admin"; // Set admin role
    authService.saveUserData(userData, true); // Save with updated role
  }

  router.push("/admin/dashboard");
}
</script>

<template>
  <!-- Admin Layout -->
  <div class="app-container" v-if="router.currentRoute.value.path.includes('/admin')">
    <div class="admin-layout">
      <!-- Sidebar -->
      <div class="sidebar">
        <div class="logo-container">
          <img src="@/components/img/logo.png" alt="POLY" class="logo" />
        </div>
        <div class="menu-title">Quản lý</div>
        <ul class="sidebar-menu">
          <li>
            <router-link to="/admin/dashboard" class="sidebar-link"><i class="bi bi-speedometer2 menu-icon"></i>
              Dashboard</router-link>
          </li>
          <li>
            <router-link to="/admin/nhanvien" class="sidebar-link"><i class="bi bi-people menu-icon"></i> Nhân
              viên</router-link>
          </li>
          <li>
            <router-link to="/admin/khachhang" class="sidebar-link"><i class="bi bi-person-vcard menu-icon"></i> Khách
              hàng</router-link>
          </li>
          <li>
            <router-link to="/admin/uudai" class="sidebar-link"><i class="bi bi-tags menu-icon"></i> Ưu
              đãi</router-link>
          </li>
          <li>
            <router-link to="/admin/sanpham" class="sidebar-link"><i class="bi bi-box-seam menu-icon"></i> Sản
              phẩm</router-link>
          </li>
          <li>
            <router-link to="/admin/donhang" class="sidebar-link"><i class="bi bi-bag menu-icon"></i> Đơn
              hàng</router-link>
          </li>
          <li>
            <router-link to="/admin/baiviet" class="sidebar-link"><i class="bi bi-newspaper menu-icon"></i> Bài
              viết</router-link>
          </li>
          <li>
            <router-link to="/admin/banhang" class="sidebar-link"><i class="bi bi-cart-check menu-icon"></i> Bán hàng
              tại
              quầy</router-link>
          </li>
          <li>
            <router-link to="/admin/hoadon" class="sidebar-link"><i class="bi bi-receipt menu-icon"></i> Hoá
              đơn</router-link>
          </li>
          <li>
            <router-link to="/admin/trahang" class="sidebar-link"><i class="bi bi-arrow-return-left menu-icon"></i> Trả
              hàng hoàn
              tiền</router-link>
          </li>
          <!-- <li>
            <router-link to="/admin/thongke" class="sidebar-link"><i class="bi bi-bar-chart menu-icon"></i> Thống
              kê</router-link>
          </li> -->
        </ul>
      </div>

      <!-- Content Area -->
      <div class="content-area">
        <!-- Admin Header -->
        <div class="admin-header">
          <div class="admin-header-right">
            <div class="admin-user-dropdown">
              <div class="user-icon-clickable" @click="toggleUserDropdown">
                <i class="bi bi-person-circle"></i>
                <span class="user-name">{{ displayName }}</span>
                <i class="bi bi-chevron-down"></i>
              </div>
              <!-- Dropdown menu -->
              <div class="user-menu-dropdown" v-if="showUserDropdown">
                <div class="dropdown-user-info">
                  <i class="bi bi-person-circle"></i>
                  <span>{{ displayName }}</span>
                </div>
                <div class="dropdown-divider"></div>
                <router-link to="/account" class="dropdown-item">
                  <i class="bi bi-gear"></i> Tài khoản của tôi
                </router-link>
                <router-link to="/orders" class="dropdown-item">
                  <i class="bi bi-bag"></i> Đơn hàng của tôi
                </router-link>
                <a href="#" class="dropdown-item" @click.prevent="goToAdminDashboard">
                  <i class="bi bi-speedometer2"></i> Dashboard
                </a>
                <div class="dropdown-divider"></div>
                <a href="#" class="dropdown-item logout-item" @click.prevent="logout">
                  <i class="bi bi-box-arrow-right"></i> Đăng xuất
                </a>
              </div>
            </div>
          </div>
        </div>
        <RouterView />
      </div>
    </div>
  </div>

  <!-- E-commerce Layout -->
  <div class="app-container" v-else>
    <!-- Header -->
    <header class="store-header">
      <div class="logo-container">
        <router-link to="/">
          <img src="@/components/img/logo.png" alt="POLY" class="logo" />
        </router-link>
      </div>
      <nav class="main-nav">
        <ul>
          <li><router-link to="/shop">SHOP ALL</router-link></li>
          <li><router-link to="/news">NEWS</router-link></li>
          <li><router-link to="/tops">TOPS</router-link></li>
          <li><router-link to="/bottoms">BOTTOMS</router-link></li>
          <li><router-link to="/sale">SALE</router-link></li>
          <li><router-link to="/hot">HOT</router-link></li>
          <li><router-link to="/contact">CONTACT</router-link></li>
        </ul>
      </nav>
      <div class="header-actions">
        <div class="search-icon">
          <i class="bi bi-search"></i>
        </div>
        <div class="cart-icon">
          <router-link to="/cart">
            <i class="bi bi-cart"></i>
          </router-link>
        </div>
        <!-- User Menu -->
        <div class="user-menu">
          <!-- Not logged in -> show login link -->
          <template v-if="!isLoggedIn">
            <router-link to="/login" class="login-link">
              <i class="bi bi-person"></i>
            </router-link>
          </template>
          <!-- Logged in -> show user menu dropdown -->
          <div class="user-dropdown" v-else>
            <div class="user-icon-clickable" @click="toggleUserDropdown">
              <i class="bi bi-person-circle"></i>
              <span class="user-name">{{ displayName }}</span>
              <i class="bi bi-chevron-down"></i>
            </div>
            <!-- Dropdown menu -->
            <div class="user-menu-dropdown" v-if="showUserDropdown">
              <div class="dropdown-user-info">
                <i class="bi bi-person-circle"></i>
                <span>{{ displayName }}</span>
              </div>
              <div class="dropdown-divider"></div>
              <router-link to="/account" class="dropdown-item">
                <i class="bi bi-gear"></i> Tài khoản của tôi
              </router-link>
              <router-link to="/orders" class="dropdown-item">
                <i class="bi bi-bag"></i> Đơn hàng của tôi
              </router-link>
              <a href="#" class="dropdown-item" @click.prevent="goToAdminDashboard">
                <i class="bi bi-speedometer2"></i> Dashboard
              </a>
              <div class="dropdown-divider"></div>
              <a href="#" class="dropdown-item logout-item" @click.prevent="logout">
                <i class="bi bi-box-arrow-right"></i> Đăng xuất
              </a>
            </div>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="store-content">
      <RouterView />
    </main>

    <!-- Footer -->
    <footer class="store-footer">
      <div class="footer-branding">
        <div class="footer-logo">POLY</div>
        <div class="footer-slogan">CHẤT RIÊNG TRONG TỪNG ĐƯỜNG MAY!</div>
        <div class="hanger-icon">
          <i class="bi bi-hanger"></i>
        </div>
      </div>
      <div class="footer-columns" style="display: flex; justify-content: center; align-items: flex-start; gap: 60px;">
        <div class="footer-column ms-5">
          <h3 style="font-weight: bold;">Information</h3>
          <p>Hà Nội : 123 Đường láng -Dist. Đống Đa</p>
          <p>-0975190369</p>
          <p>polyshop@gmail.com</p>
        </div>
        <div class="footer-column ms-5">
          <h3 style="font-weight: bold;">Service</h3>
          <p>Hướng dẫn mua hàng</p>
          <p>Quy cách đổi hàng</p>
        </div>
        <div class="footer-column ms-5">
          <h3 style="font-weight: bold;">Follow US On</h3>
          <div class="social-icons">
            <a href="#" class="social-icon"><i class="bi bi-facebook"></i></a>
            <a href="#" class="social-icon"><i class="bi bi-instagram"></i></a>
            <a href="#" class="social-icon"><i class="bi bi-tiktok"></i></a>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<style>
/* General styles */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: "Arial", sans-serif;
}

.app-container {
  width: 100%;
  min-height: 100vh;
}

/* Admin Layout Styles */
.admin-layout {
  display: flex;
  height: 100vh;
}

.sidebar {
  width: 250px;
  height: 100%;
  background-color: #f8f9fa;
  border-right: 1px solid #dee2e6;
  padding: 20px 0;
  position: fixed;
  left: 0;
  top: 0;
  bottom: 0;
  overflow-y: auto;
}

.logo-container {
  padding: 0 20px 20px;
  text-align: center;
}

.logo {
  width: 70px;
  height: auto;
}

.menu-title {
  font-size: 18px;
  font-weight: bold;
  padding: 10px 20px;
  margin-bottom: 10px;
}

.sidebar-menu {
  list-style-type: none;
  padding: 0;
}

.sidebar-menu li {
  padding: 0;
}

.sidebar-link {
  display: block;
  padding: 10px 20px;
  color: #333;
  text-decoration: none;
  border-bottom: 1px solid #dee2e6;
  transition: background-color 0.2s;
  display: flex;
  align-items: center;
}

.menu-icon {
  margin-right: 10px;
  font-size: 16px;
}

.sidebar-link:hover,
.sidebar-link.router-link-active {
  background-color: #e9ecef;
}

.content-area {
  flex: 1;
  margin-left: 250px;
  padding: 20px;
  overflow-y: auto;
}

.admin-header {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 20px;
  padding-bottom: 10px;
  border-bottom: 1px solid #dee2e6;
}

.admin-header-right {
  display: flex;
  align-items: center;
}

.admin-user-dropdown {
  position: relative;
}

/* E-commerce Layout Styles */
.store-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 15px 30px;
  background-color: #fff;
  border-bottom: 1px solid #eee;
}

.main-nav ul {
  display: flex;
  list-style-type: none;
  gap: 20px;
}

.main-nav a {
  text-decoration: none;
  color: #333;
  font-size: 14px;
  font-weight: 500;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 20px;
}

.search-icon,
.cart-icon {
  font-size: 20px;
  cursor: pointer;
}

/* User Menu Styles */
.user-menu {
  position: relative;
}

.login-link {
  color: #333;
  text-decoration: none;
  font-size: 20px;
}

.user-dropdown {
  position: relative;
}

.user-icon-clickable {
  display: flex;
  align-items: center;
  cursor: pointer;
  gap: 5px;
}

.user-name {
  font-size: 14px;
  margin-right: 5px;
}

.user-menu-dropdown {
  position: absolute;
  top: 100%;
  right: 0;
  width: 230px;
  background-color: white;
  border-radius: 5px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  z-index: 1000;
  margin-top: 10px;
}

.dropdown-user-info {
  display: flex;
  align-items: center;
  padding: 15px;
  gap: 10px;
  border-bottom: 1px solid #eee;
}

.dropdown-user-info i {
  font-size: 24px;
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 15px;
  color: #333;
  text-decoration: none;
  transition: background-color 0.2s;
}

.dropdown-item:hover {
  background-color: #f5f5f5;
}

.dropdown-divider {
  height: 1px;
  background-color: #eee;
  margin: 5px 0;
}

.logout-item {
  color: #dc3545;
}

.store-content {
  min-height: calc(100vh - 150px);
  padding: 30px;
  background-color: #f9f9f9;
}

/* Rest of the styles remain unchanged */
.store-footer {
  background-color: #f2f2f2;
  padding: 30px;
}

.footer-branding {
  text-align: center;
  margin-bottom: 30px;
}

.footer-logo {
  font-size: 30px;
  font-weight: bold;
  color: #333;
}

.footer-slogan {
  font-size: 14px;
  color: #777;
  margin: 10px 0;
}

.hanger-icon {
  font-size: 24px;
  margin: 10px 0;
}

.footer-columns {
  display: flex;
  justify-content: space-between;
  padding-top: 20px;
  border-top: 1px solid #ddd;
}

.footer-column {
  flex: 1;
  padding: 0 15px;
}

.footer-column h3 {
  font-size: 16px;
  margin-bottom: 15px;
}

.footer-column ul {
  list-style-type: none;
}

.footer-column li {
  margin-bottom: 8px;
}

.social-icons {
  display: flex;
  gap: 15px;
}

/* Admin tables */
.admin-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

.admin-table th,
.admin-table td {
  border: 1px solid #dee2e6;
  padding: 10px;
  text-align: center;
}

.admin-table th {
  background-color: #f8f9fa;
}

/* Admin page header */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-title {
  font-size: 24px;
  font-weight: bold;
}

.search-container {
  position: relative;
  width: 50%;
}

.search-input {
  width: 100%;
  padding: 10px 40px 10px 15px;
  border: none;
  border-radius: 20px;
  background-color: #f1f1f1;
}

.search-icon {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  color: #777;
}

.date-filters {
  display: flex;
  align-items: center;
  gap: 10px;
}

.add-button {
  width: 40px;
  height: 40px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 50%;
  font-size: 24px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
}

.filter-button {
  background-color: #ffc107;
  color: #333;
  border: none;
  border-radius: 20px;
  padding: 8px 20px;
  font-weight: bold;
  cursor: pointer;
}

/* Action buttons in tables */
.action-buttons {
  display: flex;
  gap: 10px;
  justify-content: center;
}

.edit-button {
  background-color: #ffeb3b;
  color: #333;
  border: none;
  border-radius: 50%;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.delete-button {
  background-color: #f44336;
  color: white;
  border: none;
  border-radius: 50%;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}
</style>
