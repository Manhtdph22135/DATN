<script setup>
import { ref, onMounted, reactive } from "vue";
import { authState } from "@/untility/authService";

const userProfile = reactive({
  username: "",
  fullName: "",
  email: "",
  phoneNumber: "",
  address: "",
  dateOfBirth: "",
  gender: "female",
});

const isEditing = ref(false);
const isLoading = ref(false);
const successMessage = ref("");
const errorMessage = ref("");

// Tải thông tin người dùng khi component được tạo
onMounted(() => {
  loadUserProfile();
});

// Lấy thông tin từ localStorage/authState
function loadUserProfile() {
  const userData = authState.user.value;
  if (userData) {
    userProfile.username = userData.username || "";
    userProfile.fullName = userData.fullName || "";
    userProfile.email = userData.email || "";
    userProfile.phoneNumber = userData.phoneNumber || "";
    userProfile.address = userData.address || "";
    userProfile.dateOfBirth = userData.dateOfBirth || "";
    userProfile.gender = userData.gender === false ? "female" : "male";
  }
}

// Bật chế độ chỉnh sửa
function enableEditing() {
  isEditing.value = true;
}

// Hủy chỉnh sửa
function cancelEditing() {
  isEditing.value = false;
  loadUserProfile(); // Tải lại thông tin ban đầu
  successMessage.value = "";
  errorMessage.value = "";
}

// Lưu thông tin cập nhật
async function saveProfile() {
  isLoading.value = true;
  successMessage.value = "";
  errorMessage.value = "";

  try {
    // Giả lập API call để cập nhật thông tin
    // Trong thực tế, bạn sẽ gọi API cập nhật thông tin
    await new Promise((resolve) => setTimeout(resolve, 1000));

    // Giả lập thành công
    successMessage.value = "Thông tin tài khoản đã được cập nhật!";
    isEditing.value = false;

    // Cập nhật lại authState nếu cần
    // authState.user.value = { ...authState.user.value, ...userProfile };
  } catch (error) {
    errorMessage.value = "Có lỗi xảy ra khi cập nhật thông tin.";
  } finally {
    isLoading.value = false;
  }
}

// Đổi mật khẩu
const changePasswordForm = reactive({
  currentPassword: "",
  newPassword: "",
  confirmPassword: "",
});

const showChangePassword = ref(false);
const passwordErrors = ref({});

function toggleChangePassword() {
  showChangePassword.value = !showChangePassword.value;
  changePasswordForm.currentPassword = "";
  changePasswordForm.newPassword = "";
  changePasswordForm.confirmPassword = "";
  passwordErrors.value = {};
}

async function changePassword() {
  // Kiểm tra dữ liệu đầu vào
  passwordErrors.value = {};

  if (!changePasswordForm.currentPassword) {
    passwordErrors.value.currentPassword = "Vui lòng nhập mật khẩu hiện tại";
  }

  if (!changePasswordForm.newPassword) {
    passwordErrors.value.newPassword = "Vui lòng nhập mật khẩu mới";
  } else if (changePasswordForm.newPassword.length < 6) {
    passwordErrors.value.newPassword = "Mật khẩu phải có ít nhất 6 ký tự";
  }

  if (!changePasswordForm.confirmPassword) {
    passwordErrors.value.confirmPassword = "Vui lòng xác nhận mật khẩu";
  } else if (
    changePasswordForm.newPassword !== changePasswordForm.confirmPassword
  ) {
    passwordErrors.value.confirmPassword = "Xác nhận mật khẩu không khớp";
  }

  // Nếu có lỗi, không tiếp tục
  if (Object.keys(passwordErrors.value).length > 0) {
    return;
  }

  isLoading.value = true;

  try {
    // Giả lập API call để đổi mật khẩu
    await new Promise((resolve) => setTimeout(resolve, 1000));

    // Giả lập thành công
    successMessage.value = "Mật khẩu đã được cập nhật thành công!";
    showChangePassword.value = false;
  } catch (error) {
    errorMessage.value = "Có lỗi xảy ra khi đổi mật khẩu.";
  } finally {
    isLoading.value = false;
  }
}
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
                <input
                  type="text"
                  id="username"
                  v-model="userProfile.username"
                  :disabled="true"
                />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="fullName">Họ tên:</label>
                <input
                  type="text"
                  id="fullName"
                  v-model="userProfile.fullName"
                  :disabled="!isEditing"
                />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="email">Email:</label>
                <input
                  type="email"
                  id="email"
                  v-model="userProfile.email"
                  :disabled="!isEditing"
                />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="phone">Số điện thoại:</label>
                <input
                  type="tel"
                  id="phone"
                  v-model="userProfile.phoneNumber"
                  :disabled="!isEditing"
                />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="address">Địa chỉ:</label>
                <textarea
                  id="address"
                  v-model="userProfile.address"
                  :disabled="!isEditing"
                ></textarea>
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="dob">Ngày sinh:</label>
                <input
                  type="date"
                  id="dob"
                  v-model="userProfile.dateOfBirth"
                  :disabled="!isEditing"
                />
              </div>
            </div>

            <div class="form-row" v-if="isEditing">
              <div class="form-group gender-group">
                <label>Giới tính:</label>
                <div class="gender-options">
                  <label class="gender-option">
                    <input
                      type="radio"
                      v-model="userProfile.gender"
                      value="female"
                      :disabled="!isEditing"
                    />
                    <span>Nữ</span>
                  </label>
                  <label class="gender-option">
                    <input
                      type="radio"
                      v-model="userProfile.gender"
                      value="male"
                      :disabled="!isEditing"
                    />
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
          <button
            v-if="!showChangePassword"
            @click="toggleChangePassword"
            class="btn-edit"
          >
            <i class="bi bi-shield-lock"></i> Đổi mật khẩu
          </button>
        </div>

        <div class="password-content" v-if="showChangePassword">
          <form @submit.prevent="changePassword" class="password-form">
            <div class="form-group">
              <label for="currentPassword">Mật khẩu hiện tại:</label>
              <input
                type="password"
                id="currentPassword"
                v-model="changePasswordForm.currentPassword"
              />
              <p class="error-message" v-if="passwordErrors.currentPassword">
                {{ passwordErrors.currentPassword }}
              </p>
            </div>

            <div class="form-group">
              <label for="newPassword">Mật khẩu mới:</label>
              <input
                type="password"
                id="newPassword"
                v-model="changePasswordForm.newPassword"
              />
              <p class="error-message" v-if="passwordErrors.newPassword">
                {{ passwordErrors.newPassword }}
              </p>
            </div>

            <div class="form-group">
              <label for="confirmPassword">Xác nhận mật khẩu mới:</label>
              <input
                type="password"
                id="confirmPassword"
                v-model="changePasswordForm.confirmPassword"
              />
              <p class="error-message" v-if="passwordErrors.confirmPassword">
                {{ passwordErrors.confirmPassword }}
              </p>
            </div>

            <div class="button-group">
              <button
                type="button"
                @click="toggleChangePassword"
                class="btn-cancel"
              >
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
