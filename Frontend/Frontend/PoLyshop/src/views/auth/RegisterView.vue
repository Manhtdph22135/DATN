<template>
  <div class="register-container">
    <div class="register-card">
      <h1 class="register-title">ĐĂNG KÝ</h1>

      <div v-if="errorMessage" class="alert alert-danger">
        {{ errorMessage }}
      </div>
      <div v-if="successMessage" class="alert alert-success">
        {{ successMessage }}
      </div>

      <form @submit.prevent="register" class="register-form">
        <input
          type="text"
          id="firstName"
          v-model="firstName"
          class="form-input"
          placeholder="Họ"
          required
        />
        <input
          type="text"
          id="lastName"
          v-model="lastName"
          class="form-input"
          placeholder="Tên"
          required
        />

        <div class="gender-options">
          <label class="gender-option">
            <input type="radio" name="gender" value="female" v-model="gender" required/>
            <span>Nữ</span>
          </label>
          <label class="gender-option">
            <input type="radio" name="gender" value="male" v-model="gender" required/>
            <span>Nam</span>
          </label>
        </div>

        <input
          type="date"
          id="dateOfBirth"
          v-model="dateOfBirth"
          class="form-input"
          placeholder="mm/dd/yyyy"
          required
        />
        <input
          type="email"
          id="email"
          v-model="email"
          class="form-input"
          placeholder="Email (Dùng làm tên đăng nhập)"
          required
        />
        <input
          type="tel"
          id="phoneNumber"
          v-model="phoneNumber"
          class="form-input"
          placeholder="Số điện thoại"
        />
        <input
          type="text"
          id="address"
          v-model="address"
          class="form-input"
          placeholder="Địa chỉ"
        />

        <input
          type="password"
          id="password"
          v-model="password"
          class="form-input"
          placeholder="Mật khẩu"
          required
        />
        <input
          type="password"
          id="confirmPassword"
          v-model="confirmPassword"
          class="form-input"
          placeholder="Xác nhận mật khẩu"
          required
        />

        <button type="submit" class="register-button" :disabled="isLoading">
          <span v-if="isLoading">
            <i class="bi bi-arrow-repeat spinning"></i>
          </span>
          <span v-else>ĐĂNG KÝ</span>
        </button>
      </form>

      <div class="home-link">
        <router-link to="/">
          <i class="bi bi-arrow-left"></i> Quay lại trang chủ
        </router-link>
      </div>
    </div>
  </div>
</template>

<script>
// Giả định authService đã được fix và export đúng
import { authService } from "@/untility/authService"; 

export default {
  name: "RegisterView",
  data() {
    return {
      // Các trường dữ liệu từ form
      firstName: "",
      lastName: "",
      gender: "female", // 'male' hoặc 'female'
      dateOfBirth: "",
      phoneNumber: "", 
      email: "",
      address: "", 
      password: "",
      confirmPassword: "", 
      
      // Các trạng thái UI
      isLoading: false,
      errorMessage: "",
      successMessage: "",
      
      // Các trường bạn có thể bỏ qua hoặc giữ nguyên
      showPassword: false,
      showConfirmPassword: false,
      agreeTerms: true,
    };
  },
  computed: {
    isFormValid() {
      // Kiểm tra các trường bắt buộc
      return (
        this.firstName &&
        this.lastName &&
        this.email &&
        this.dateOfBirth &&
        this.password &&
        this.confirmPassword
      );
    },
    // fullName() { return `${this.firstName} ${this.lastName}`; }
  },
  methods: {
    // Hàm ánh xạ giới tính từ string sang boolean cho Backend C# (Male=True, Female=False)
    mapGenderToBoolean(genderString) {
        return genderString === 'male'; 
    },

    // RegisterView.vue - Trong phần methods: { register() }

async register() {
    // ... (Kiểm tra validation và mật khẩu)
    if (this.password !== this.confirmPassword) {
        this.errorMessage = "Mật khẩu và Xác nhận mật khẩu không khớp!";
        return;
    }

    this.isLoading = true;
    this.errorMessage = "";
    this.successMessage = "";

    try {
        // --- PHẦN ĐÃ SỬA: XỬ LÝ USERNAME ---
        let usernameToRegister = this.email;
        const parts = this.email.split('@');
        if (parts.length > 1) {
            // Lấy phần đầu tiên trước ký tự '@'
            usernameToRegister = parts[0]; 
        }
        // ------------------------------------

        const registerData = {
            // Account fields
            Username: usernameToRegister, // SỬ DỤNG USERNAME ĐÃ XỬ LÝ
            Password: this.password,
            ConfirmPassword: this.confirmPassword,
            RoleId: 3, 

            // Customer fields (Giữ nguyên)
            Ho: this.firstName,
            Ten: this.lastName,
            Email: this.email,
            Phone: this.phoneNumber,
            Sex: this.mapGenderToBoolean(this.gender), 
            Dob: this.dateOfBirth,
            Address: this.address,
        };

        // ... (Gọi API và xử lý kết quả)
        const response = await authService.register(registerData);

        this.successMessage = response.message || "Đăng ký thành công!";
        this.clearForm();
        setTimeout(() => {
            this.$router.push("/login");
        }, 2000);

    } catch (error) {
        // ... (Xử lý lỗi)
        this.errorMessage = error.message || "Đăng ký thất bại. Vui lòng thử lại.";
    } finally {
        this.isLoading = false;
    }
},
    clearForm() {
      // ... (code clear form giữ nguyên)
      this.firstName = "";
      this.lastName = "";
      this.gender = "female";
      this.dateOfBirth = "";
      this.phoneNumber = "";
      this.email = "";
      this.address = "";
      this.password = "";
      this.confirmPassword = "";
    },
  },
};
</script>

<style scoped>
/* ---------------------------------------------------------------------- */
/* PHẦN STYLES (CỦA BẠN)                          */
/* ---------------------------------------------------------------------- */
.register-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  padding: 20px;
  background-color: #fff;
}

.register-card {
  width: 100%;
  max-width: 500px;
  padding: 20px;
}

.register-title {
  font-size: 24px;
  font-weight: bold;
  text-align: center;
  margin-bottom: 30px;
}

.register-form {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.form-input {
  width: 100%;
  padding: 12px;
  border: none;
  background-color: #e9e9e9;
  border-radius: 3px;
  font-size: 16px;
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

.register-button {
  width: 100%;
  padding: 12px;
  background-color: #000;
  color: #fff;
  border: none;
  border-radius: 3px;
  font-size: 16px;
  font-weight: bold;
  cursor: pointer;
  margin-top: 10px;
}

.home-link {
  margin-top: 20px;
  text-align: left;
}

.home-link a {
  color: #000;
  text-decoration: none;
  display: flex;
  align-items: center;
  gap: 5px;
}

.home-link a:hover {
  text-decoration: underline;
}

.alert {
  padding: 10px;
  margin-bottom: 20px;
  border-radius: 3px;
}

.alert-danger {
  color: #721c24;
  background-color: #f8d7da;
  border: 1px solid #f5c6cb;
}

.alert-success {
  color: #155724;
  background-color: #d4edda;
  border: 1px solid #c3e6cb;
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
</style>