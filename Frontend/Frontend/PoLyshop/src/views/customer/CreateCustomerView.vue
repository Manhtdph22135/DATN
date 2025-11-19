<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-md-8">
        <div class="card shadow">
          <div class="card-header bg-primary text-white">
            <h4 class="mb-0">Thêm khách hàng mới</h4>
          </div>
          <div class="card-body">
            <form @submit.prevent="saveCustomer">
              <div class="row mb-3">
                <div class="col-md-6">
                  <label class="form-label"
                    >Họ và tên <span class="text-danger">*</span></label
                  >
                  <input
                    type="text"
                    class="form-control"
                    v-model="customer.fullName"
                    required
                  />
                  <div class="invalid-feedback" v-if="errors.fullName">
                    {{ errors.fullName }}
                  </div>
                </div>
                <div class="col-md-6">
                  <label class="form-label"
                    >Email <span class="text-danger">*</span></label
                  >
                  <input
                    type="email"
                    class="form-control"
                    v-model="customer.email"
                    required
                  />
                  <div class="invalid-feedback" v-if="errors.email">
                    {{ errors.email }}
                  </div>
                </div>
              </div>

              <div class="row mb-3">
                <div class="col-md-6">
                  <label class="form-label"
                    >Số điện thoại <span class="text-danger">*</span></label
                  >
                  <input
                    type="tel"
                    class="form-control"
                    v-model="customer.phone"
                    required
                  />
                  <div class="invalid-feedback" v-if="errors.phone">
                    {{ errors.phone }}
                  </div>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Ngày sinh</label>
                  <input
                    type="date"
                    class="form-control"
                    v-model="customer.birthday"
                  />
                </div>
              </div>

              <div class="row mb-3">
                <div class="col-md-6">
                  <label class="form-label">Hạng thành viên</label>
                  <select class="form-select" v-model="customer.membership">
                    <option value="bronze">Thành viên Đồng</option>
                    <option value="silver">Thành viên Bạc</option>
                    <option value="gold">Thành viên Vàng</option>
                    <option value="platinum">Thành viên Bạch Kim</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Trạng thái</label>
                  <select class="form-select" v-model="customer.status">
                    <option value="active">Hoạt động</option>
                    <option value="inactive">Không hoạt động</option>
                  </select>
                </div>
              </div>

              <div class="mb-3">
                <label class="form-label">Địa chỉ</label>
                <textarea
                  class="form-control"
                  v-model="customer.address"
                  rows="3"
                  placeholder="Nhập địa chỉ đầy đủ"
                ></textarea>
              </div>

              <div class="mb-3">
                <label class="form-label">Ảnh đại diện</label>
                <input
                  type="text"
                  class="form-control"
                  v-model="customer.avatar"
                  placeholder="URL hình ảnh"
                />
                <small class="text-muted"
                  >Nhập URL hình ảnh hoặc để trống</small
                >
              </div>

              <div class="mb-3 form-check">
                <input
                  type="checkbox"
                  class="form-check-input"
                  id="agreeTerms"
                  v-model="agreeTerms"
                  required
                />
                <label class="form-check-label" for="agreeTerms">
                  Tôi đồng ý với <a href="#">điều khoản dịch vụ</a> và
                  <a href="#">chính sách bảo mật</a>
                </label>
              </div>

              <div class="d-flex justify-content-between">
                <router-link to="/admin/khachhang" class="btn btn-secondary">
                  <i class="bi bi-arrow-left"></i> Quay lại
                </router-link>
                <button
                  type="submit"
                  class="btn btn-primary"
                  :disabled="!agreeTerms || isSubmitting"
                >
                  <span
                    v-if="isSubmitting"
                    class="spinner-border spinner-border-sm me-1"
                  ></span>
                  <i v-else class="bi bi-check-circle me-1"></i>
                  Lưu thông tin
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";

export default {
  name: "CreateCustomerView",
  setup() {
    const router = useRouter();
    const isSubmitting = ref(false);
    const agreeTerms = ref(false);
    const errors = ref({});

    const customer = ref({
      fullName: "",
      email: "",
      phone: "",
      birthday: "",
      address: "",
      membership: "bronze",
      status: "active",
      avatar: "",
      registerDate: new Date().toISOString().split("T")[0],
      orderCount: 0,
      totalSpent: 0,
      points: 0,
    });

    const validateForm = () => {
      const newErrors = {};

      // Validate full name
      if (!customer.value.fullName.trim()) {
        newErrors.fullName = "Họ tên không được để trống";
      } else if (customer.value.fullName.length < 2) {
        newErrors.fullName = "Họ tên phải có ít nhất 2 ký tự";
      }

      // Validate email
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!customer.value.email.trim()) {
        newErrors.email = "Email không được để trống";
      } else if (!emailRegex.test(customer.value.email)) {
        newErrors.email = "Email không hợp lệ";
      }

      // Validate phone
      const phoneRegex = /^(0|\+84)[3|5|7|8|9][0-9]{8}$/;
      if (!customer.value.phone.trim()) {
        newErrors.phone = "Số điện thoại không được để trống";
      } else if (!phoneRegex.test(customer.value.phone)) {
        newErrors.phone = "Số điện thoại không hợp lệ";
      }

      errors.value = newErrors;
      return Object.keys(newErrors).length === 0;
    };

    const saveCustomer = async () => {
      if (!validateForm()) {
        return;
      }

      isSubmitting.value = true;

      try {
        // Replace with actual API call when available
        // await axios.post("/api/customers", customer.value);

        // For development purposes, we'll simulate a successful API call
        setTimeout(() => {
          isSubmitting.value = false;
          alert("Đã thêm khách hàng thành công!");
          router.push("/customers");
        }, 1000);
      } catch (err) {
        isSubmitting.value = false;
        alert("Lỗi khi thêm khách hàng: " + err.message);
      }
    };

    return {
      customer,
      agreeTerms,
      errors,
      isSubmitting,
      saveCustomer,
    };
  },
};
</script>

<style scoped>
.card {
  border-radius: 10px;
  overflow: hidden;
}

.card-header {
  background: linear-gradient(135deg, #007bff, #0056b3);
}

.form-label {
  font-weight: 500;
}

.invalid-feedback {
  display: block;
}

textarea {
  resize: none;
}
</style>
