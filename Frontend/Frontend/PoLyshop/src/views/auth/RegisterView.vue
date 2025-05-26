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
            <input type="radio" name="gender" value="female" v-model="gender" />
            <span>Nữ</span>
          </label>
          <label class="gender-option">
            <input type="radio" name="gender" value="male" v-model="gender" />
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
          placeholder="Email"
          required
        />

        <input
          type="password"
          id="password"
          v-model="password"
          class="form-input"
          placeholder="Mật khẩu"
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
import authService from "@/untility/authService";

export default {
  name: "RegisterView",
  data() {
    return {
      firstName: "",
      lastName: "",
      gender: "female",
      dateOfBirth: "",
      phoneNumber: "",
      email: "",
      address: "",
      password: "",
      confirmPassword: "",
      showPassword: false,
      showConfirmPassword: false,
      agreeTerms: true, // Default to true to simplify form
      isLoading: false,
      errorMessage: "",
      successMessage: "",
    };
  },
  computed: {
    isFormValid() {
      return (
        this.firstName &&
        this.lastName &&
        this.email &&
        this.dateOfBirth &&
        this.password
      );
    },
    fullName() {
      return `${this.firstName} ${this.lastName}`;
    },
  },
  methods: {
    async register() {
      if (!this.isFormValid) return;

      this.isLoading = true;
      this.errorMessage = "";
      this.successMessage = "";

      try {
        console.log("Form submitted with values:", {
          firstName: this.firstName,
          lastName: this.lastName,
          email: this.email,
          gender: this.gender,
        });

        // Prepare simpler data for registration to match API requirements
        const registerData = {
          userName: this.email,
          password: this.password,
          confirmPassword: this.password, // Use same password for confirmation
        };

        // TEMPORARY: Use mock registration for testing UI flow
        // Remove this and uncomment the real API call when backend is ready
        await this.mockRegistration();
        return;

        // Call register API with error handling
        console.log("Calling register API with:", registerData);
        const response = await authService.register(registerData);

        // Show success message
        this.successMessage =
          "Đăng ký thành công! Bạn sẽ được chuyển đến trang đăng nhập.";

        // Clear form data
        this.clearForm();

        // Redirect to login page after a short delay
        setTimeout(() => {
          this.$router.push("/login");
        }, 2000);
      } catch (error) {
        console.error("Registration failed:", error);

        // Detailed error handling
        if (error.response) {
          if (error.response.status === 500) {
            this.errorMessage = "Lỗi máy chủ. Vui lòng thử lại sau.";
          } else if (error.response.data && error.response.data.message) {
            // Direct message from API
            this.errorMessage = error.response.data.message;
          } else if (
            error.response.data &&
            typeof error.response.data === "object"
          ) {
            // Try to extract error messages
            const errorMessages = [];
            for (const key in error.response.data) {
              if (typeof error.response.data[key] === "string") {
                errorMessages.push(error.response.data[key]);
              } else if (Array.isArray(error.response.data[key])) {
                errorMessages.push(error.response.data[key].join(", "));
              }
            }
            this.errorMessage =
              errorMessages.join("\n") ||
              "Đăng ký thất bại. Vui lòng kiểm tra lại thông tin.";
          } else {
            this.errorMessage = `Đăng ký thất bại (${error.response.status}). Vui lòng thử lại.`;
          }
        } else if (error.request) {
          // Network error - no response received
          this.errorMessage =
            "Không thể kết nối đến máy chủ. Vui lòng kiểm tra kết nối mạng và thử lại.";
        } else {
          this.errorMessage =
            "Có lỗi xảy ra: " + (error.message || "Lỗi không xác định");
        }
      } finally {
        this.isLoading = false;
      }
    },
    clearForm() {
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
    // TEMPORARY: Mock registration function for UI testing
    async mockRegistration() {
      // Simulate API delay
      await new Promise((resolve) => setTimeout(resolve, 1000));

      // Check for validation errors
      if (!this.firstName || !this.lastName) {
        this.errorMessage = "Vui lòng nhập đầy đủ họ và tên.";
        return;
      }

      if (!this.email || !this.email.includes("@")) {
        this.errorMessage = "Địa chỉ email không hợp lệ.";
        return;
      }

      if (this.password.length < 6) {
        this.errorMessage = "Mật khẩu phải có ít nhất 6 ký tự.";
        return;
      }

      // Simulate successful registration
      this.successMessage =
        "Đăng ký thành công! Bạn sẽ được chuyển đến trang đăng nhập.";
      this.clearForm();

      // Redirect to login page after a short delay
      setTimeout(() => {
        this.$router.push("/login");
      }, 2000);
    },
  },
};
</script>

<style scoped>
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
