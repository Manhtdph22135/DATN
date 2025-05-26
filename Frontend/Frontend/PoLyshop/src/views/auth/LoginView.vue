<template>
  <div class="login-container">
    <div class="login-card">
      <h1 class="login-title">ĐĂNG NHẬP</h1>

      <div v-if="errorMessage" class="alert alert-danger" role="alert">
        {{ errorMessage }}
      </div>

      <form @submit.prevent="login" class="login-form">
        <div class="form-group">
          <label for="username">Username:</label>
          <input type="text" id="username" v-model="username" class="form-input" required />
        </div>

        <div class="form-group">
          <label for="password">Password:</label>
          <div style="position: relative;">
            <input :type="showPassword ? 'text' : 'password'" id="password" v-model="password" class="form-input"
              required />
            <button type="button" @click="togglePassword"
              style="position: absolute; right: 10px; top: 50%; transform: translateY(-50%); background: none; border: none; cursor: pointer;"
              tabindex="-1">
              <span v-if="showPassword">🙈</span>
              <span v-else>👁️</span>
            </button>
          </div>
        </div>
        <div class="form-group" style="display: flex; align-items: center;">
          <input type="checkbox" id="rememberMe" v-model="rememberMe" style="margin-right: 8px;" />
          <label for="rememberMe" style="margin: 0;">Ghi nhớ đăng nhập</label>
        </div>

        <button type="submit" class="login-button" :disabled="isLoading">
          <span v-if="isLoading">
            <i class="bi bi-arrow-repeat spinning"></i> Đang xử lý...
          </span>
          <span v-else>ĐĂNG NHẬP</span>
        </button>
      </form>

      <div class="login-links">
        <router-link to="/forgot-password">Quên mật khẩu?</router-link>
        <span>hoặc</span>
        <router-link to="/register">Đăng ký</router-link>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import authService from "@/untility/authService";

export default {
  name: "LoginView",
  data() {
    return {
      username: "",
      password: "",
      rememberMe: false,
      showPassword: false,
      isLoading: false,
      errorMessage: "",
    };
  },
  methods: {
    togglePassword() {
      this.showPassword = !this.showPassword;
    },
    async login() {
      this.isLoading = true;
      this.errorMessage = "";

      try {
        // Call the real API
        const response = await axios.post("https://localhost:7055/api/Auth/login", {
          Username: this.username,
          PasswordHash: this.password,
        });

        const loginData = response.data;

        if (loginData && loginData.message === "Login successful!") {
          // Save user data
          authService.saveUserData(loginData, this.rememberMe);

          this.$emit("login-success");
          alert("Đăng nhập thành công!");

          const redirectUrl = this.$route.query.redirect || "/shop";
          this.$router.push(redirectUrl);
        } else {
          this.errorMessage = "Đăng nhập thất bại. Dữ liệu không hợp lệ.";
        }
      } catch (error) {
        if (error.response) {
          if (error.response.status === 401) {
            this.errorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
          } else if (error.response.data && error.response.data.message) {
            this.errorMessage = error.response.data.message;
          } else {
            this.errorMessage =
              "Đăng nhập thất bại. Vui lòng kiểm tra thông tin đăng nhập.";
          }
        } else {
          this.errorMessage =
            "Không thể kết nối đến máy chủ. Vui lòng thử lại sau.";
        }
      } finally {
        this.isLoading = false;
      }
    },
  },
};
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  padding: 20px;
  background-color: #fff;
}

.login-card {
  width: 100%;
  max-width: 400px;
  padding: 20px;
}

.login-title {
  font-size: 24px;
  font-weight: bold;
  text-align: center;
  margin-bottom: 30px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: normal;
}

.form-input {
  width: 100%;
  padding: 12px;
  border: none;
  background-color: #e9e9e9;
  border-radius: 3px;
  font-size: 16px;
}

.login-button {
  width: 100%;
  padding: 12px;
  background-color: #e9e9e9;
  color: #000;
  border: none;
  border-radius: 3px;
  font-size: 16px;
  cursor: pointer;
  margin-top: 10px;
  font-weight: bold;
}

.login-links {
  display: flex;
  justify-content: center;
  margin-top: 20px;
  gap: 10px;
}

.login-links a {
  color: #000;
  text-decoration: none;
}

.login-links a:hover {
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
