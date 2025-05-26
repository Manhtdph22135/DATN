<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý ưu đãi</h2>
      <div class="header-actions">
        <div class="search-container">
          <input
            type="text"
            class="search-input"
            placeholder="Tìm kiếm ưu đãi..."
            v-model="searchTerm"
          />
          <i class="bi bi-search search-icon"></i>
        </div>
        <button class="btn btn-success" @click="openAddModal">
          <i class="bi bi-plus"></i> Thêm ưu đãi
        </button>
      </div>
    </div>

    <div class="content-container">
      <div v-if="loading" class="text-center p-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Đang tải...</span>
        </div>
      </div>

      <div v-else-if="error" class="alert alert-danger">
        {{ error }}
      </div>

      <div v-else-if="filteredDiscounts.length === 0" class="text-center p-5">
        <p>Không tìm thấy ưu đãi nào.</p>
      </div>

      <div v-else class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>Mã</th>
              <th>Tên ưu đãi</th>
              <th>Mã giảm giá</th>
              <th>Phần trăm</th>
              <th>Ngày bắt đầu</th>
              <th>Ngày kết thúc</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="discount in filteredDiscounts" :key="discount.id">
              <td>{{ discount.id }}</td>
              <td>{{ discount.name }}</td>
              <td>
                <span class="badge bg-info">{{ discount.code }}</span>
              </td>
              <td>{{ discount.percentage }}%</td>
              <td>{{ formatDate(discount.startDate) }}</td>
              <td>{{ formatDate(discount.endDate) }}</td>
              <td>
                <span :class="getStatusClass(discount)">{{
                  getStatusText(discount)
                }}</span>
              </td>
              <td class="action-buttons">
                <button
                  class="btn btn-sm btn-info me-1"
                  @click="viewDiscount(discount)"
                >
                  <i class="bi bi-eye"></i>
                </button>
                <button
                  class="btn btn-sm btn-warning me-1"
                  @click="editDiscount(discount)"
                >
                  <i class="bi bi-pencil"></i>
                </button>
                <button
                  class="btn btn-sm btn-danger"
                  @click="confirmDeleteDiscount(discount)"
                >
                  <i class="bi bi-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add/Edit Discount Modal -->
    <div class="modal fade" id="discountModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ isEditing ? "Chỉnh sửa ưu đãi" : "Thêm ưu đãi mới" }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveDiscount">
              <div class="mb-3">
                <label class="form-label">Tên ưu đãi</label>
                <input
                  type="text"
                  class="form-control"
                  v-model="currentDiscount.name"
                  required
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Mã giảm giá</label>
                <input
                  type="text"
                  class="form-control"
                  v-model="currentDiscount.code"
                  required
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Phần trăm giảm giá (%)</label>
                <input
                  type="number"
                  class="form-control"
                  v-model="currentDiscount.percentage"
                  min="0"
                  max="100"
                  required
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Mô tả</label>
                <textarea
                  class="form-control"
                  v-model="currentDiscount.description"
                  rows="3"
                ></textarea>
              </div>
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Ngày bắt đầu</label>
                  <input
                    type="date"
                    class="form-control"
                    v-model="currentDiscount.startDate"
                    required
                  />
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Ngày kết thúc</label>
                  <input
                    type="date"
                    class="form-control"
                    v-model="currentDiscount.endDate"
                    required
                  />
                </div>
              </div>
              <div class="text-end">
                <button
                  type="button"
                  class="btn btn-secondary me-2"
                  data-bs-dismiss="modal"
                >
                  Hủy
                </button>
                <button type="submit" class="btn btn-primary">Lưu</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div class="modal fade" id="deleteModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Xác nhận xóa</h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <p>
              Bạn có chắc chắn muốn xóa ưu đãi
              <strong>{{ currentDiscount.name }}</strong
              >?
            </p>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              Hủy
            </button>
            <button
              type="button"
              class="btn btn-danger"
              @click="deleteDiscount"
            >
              Xóa
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- View Discount Details Modal -->
    <div class="modal fade" id="viewModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Chi tiết ưu đãi</h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <div class="card">
              <div class="card-body">
                <div class="mb-4 text-center">
                  <h4 class="card-title">{{ currentDiscount.name }}</h4>
                  <span class="badge bg-primary fs-5">{{
                    currentDiscount.code
                  }}</span>
                </div>

                <div class="discount-details">
                  <div class="row mb-3">
                    <div class="col-5 text-muted">Giảm giá:</div>
                    <div class="col-7 fw-bold">
                      {{ currentDiscount.percentage }}%
                    </div>
                  </div>

                  <div class="row mb-3">
                    <div class="col-5 text-muted">Ngày bắt đầu:</div>
                    <div class="col-7">
                      {{ formatDate(currentDiscount.startDate) }}
                    </div>
                  </div>

                  <div class="row mb-3">
                    <div class="col-5 text-muted">Ngày kết thúc:</div>
                    <div class="col-7">
                      {{ formatDate(currentDiscount.endDate) }}
                    </div>
                  </div>

                  <div class="row mb-3">
                    <div class="col-5 text-muted">Trạng thái:</div>
                    <div class="col-7">
                      <span :class="getStatusClass(currentDiscount)">
                        {{ getStatusText(currentDiscount) }}
                      </span>
                    </div>
                  </div>

                  <div class="row mb-3" v-if="currentDiscount.description">
                    <div class="col-12 text-muted mb-1">Mô tả:</div>
                    <div class="col-12 p-3 bg-light rounded">
                      {{ currentDiscount.description }}
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="text-end mt-3">
              <button
                type="button"
                class="btn btn-warning me-2"
                @click="editFromView()"
              >
                <i class="bi bi-pencil"></i> Chỉnh sửa
              </button>
              <button
                type="button"
                class="btn btn-secondary"
                data-bs-dismiss="modal"
              >
                Đóng
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import axios from "axios";
// Import Bootstrap helper
import bootstrap from "@/utils/bootstrapHelper";

export default {
  name: "DiscountView",
  setup() {
    const discounts = ref([]);
    const loading = ref(true);
    const error = ref(null);
    const searchTerm = ref("");
    const currentDiscount = ref({
      id: null,
      name: "",
      code: "",
      percentage: 10,
      description: "",
      startDate: new Date().toISOString().split("T")[0],
      endDate: new Date(new Date().setMonth(new Date().getMonth() + 1))
        .toISOString()
        .split("T")[0],
    });
    const isEditing = ref(false);

    // Sample data for development
    const sampleDiscounts = [
      {
        id: 1,
        name: "Khuyến mãi hè",
        code: "SUMMER2023",
        percentage: 20,
        description: "Giảm giá cho các sản phẩm mùa hè",
        startDate: "2023-06-01",
        endDate: "2023-08-31",
      },
      {
        id: 2,
        name: "Khuyến mãi tháng 9",
        code: "SEPT2023",
        percentage: 15,
        description: "Giảm giá nhân dịp khai giảng",
        startDate: "2023-09-01",
        endDate: "2023-09-30",
      },
      {
        id: 3,
        name: "Black Friday",
        code: "BLACK2023",
        percentage: 30,
        description: "Giảm giá Black Friday",
        startDate: "2023-11-24",
        endDate: "2023-11-26",
      },
    ];

    const fetchDiscounts = async () => {
      try {
        loading.value = true;
        // Uncomment when API is ready
        // const response = await axios.get('/api/Discount');
        // discounts.value = response.data;

        // For development, use sample data
        setTimeout(() => {
          discounts.value = sampleDiscounts;
          loading.value = false;
        }, 500);
      } catch (err) {
        error.value = "Không thể tải dữ liệu ưu đãi: " + err.message;
        loading.value = false;
      }
    };

    const filteredDiscounts = computed(() => {
      if (!searchTerm.value) return discounts.value;

      const term = searchTerm.value.toLowerCase();
      return discounts.value.filter(
        (discount) =>
          discount.name.toLowerCase().includes(term) ||
          discount.code.toLowerCase().includes(term) ||
          discount.description?.toLowerCase().includes(term)
      );
    });

    const openAddModal = () => {
      isEditing.value = false;
      currentDiscount.value = {
        id: null,
        name: "",
        code: "",
        percentage: 10,
        description: "",
        startDate: new Date().toISOString().split("T")[0],
        endDate: new Date(new Date().setMonth(new Date().getMonth() + 1))
          .toISOString()
          .split("T")[0],
      };
      const modal = new bootstrap.Modal(
        document.getElementById("discountModal")
      );
      modal.show();
    };

    const editDiscount = (discount) => {
      isEditing.value = true;
      currentDiscount.value = { ...discount };
      const modal = new bootstrap.Modal(
        document.getElementById("discountModal")
      );
      modal.show();
    };

    const viewDiscount = (discount) => {
      currentDiscount.value = { ...discount };
      const modal = new bootstrap.Modal(document.getElementById("viewModal"));
      modal.show();
    };

    const editFromView = () => {
      // Close view modal and open edit modal
      const viewModal = bootstrap.Modal.getInstance(
        document.getElementById("viewModal")
      );
      viewModal.hide();

      setTimeout(() => {
        isEditing.value = true;
        const editModal = new bootstrap.Modal(
          document.getElementById("discountModal")
        );
        editModal.show();
      }, 500);
    };

    const confirmDeleteDiscount = (discount) => {
      currentDiscount.value = { ...discount };
      const modal = new bootstrap.Modal(document.getElementById("deleteModal"));
      modal.show();
    };

    const saveDiscount = async () => {
      try {
        if (isEditing.value) {
          // Uncomment when API is ready
          // await axios.put(`/api/Discount/${currentDiscount.value.id}`, currentDiscount.value);

          // For development
          const index = discounts.value.findIndex(
            (d) => d.id === currentDiscount.value.id
          );
          if (index !== -1) {
            discounts.value[index] = { ...currentDiscount.value };
          }
        } else {
          // Uncomment when API is ready
          // const response = await axios.post('/api/Discount', currentDiscount.value);
          // discounts.value.push(response.data);

          // For development
          const newId = Math.max(0, ...discounts.value.map((d) => d.id)) + 1;
          discounts.value.push({
            ...currentDiscount.value,
            id: newId,
          });
        }

        // Close the modal
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("discountModal")
        );
        modal.hide();
      } catch (err) {
        error.value =
          `Không thể ${isEditing.value ? "cập nhật" : "thêm"} ưu đãi: ` +
          err.message;
      }
    };

    const deleteDiscount = async () => {
      try {
        // Uncomment when API is ready
        // await axios.delete(`/api/Discount/${currentDiscount.value.id}`);

        // For development
        discounts.value = discounts.value.filter(
          (d) => d.id !== currentDiscount.value.id
        );

        // Close the modal
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("deleteModal")
        );
        modal.hide();
      } catch (err) {
        error.value = "Không thể xóa ưu đãi: " + err.message;
      }
    };

    const formatDate = (dateString) => {
      const options = { year: "numeric", month: "2-digit", day: "2-digit" };
      return new Date(dateString).toLocaleDateString("vi-VN", options);
    };

    const getStatusClass = (discount) => {
      const now = new Date();
      const startDate = new Date(discount.startDate);
      const endDate = new Date(discount.endDate);

      if (now < startDate) {
        return "badge bg-warning";
      } else if (now > endDate) {
        return "badge bg-danger";
      } else {
        return "badge bg-success";
      }
    };

    const getStatusText = (discount) => {
      const now = new Date();
      const startDate = new Date(discount.startDate);
      const endDate = new Date(discount.endDate);

      if (now < startDate) {
        return "Sắp diễn ra";
      } else if (now > endDate) {
        return "Đã kết thúc";
      } else {
        return "Đang diễn ra";
      }
    };

    onMounted(() => {
      fetchDiscounts();
    });

    return {
      discounts,
      loading,
      error,
      searchTerm,
      currentDiscount,
      isEditing,
      filteredDiscounts,
      openAddModal,
      editDiscount,
      viewDiscount,
      editFromView,
      confirmDeleteDiscount,
      saveDiscount,
      deleteDiscount,
      formatDate,
      getStatusClass,
      getStatusText,
    };
  },
};
</script>

<style scoped>
.admin-page {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  flex-wrap: wrap;
}

.page-title {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 10px;
}

.header-actions {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
}

.search-container {
  position: relative;
  min-width: 250px;
}

.search-input {
  width: 100%;
  padding: 10px 15px;
  padding-right: 40px;
  border-radius: 5px;
  border: 1px solid #ddd;
}

.search-icon {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  color: #777;
}

.content-container {
  background-color: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
}

.admin-table th,
.admin-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

.admin-table th {
  background-color: #f5f5f5;
  font-weight: bold;
}

.admin-table tr:hover {
  background-color: #f9f9f9;
}

.action-buttons {
  white-space: nowrap;
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: stretch;
  }

  .header-actions {
    margin-top: 10px;
  }
}

.discount-details {
  padding: 10px;
}

.badge.fs-5 {
  font-size: 1rem !important;
  padding: 8px 15px;
}
</style>
