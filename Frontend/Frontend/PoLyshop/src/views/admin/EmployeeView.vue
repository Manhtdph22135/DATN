<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý nhân viên</h2>
      <div class="header-actions">
        <div class="search-container">
          <input
            type="text"
            class="search-input"
            placeholder="Tìm kiếm nhân viên..."
            v-model="searchTerm"
          />
          <i class="bi bi-search search-icon"></i>
        </div>
        <button class="btn btn-success" @click="openAddModal">
          <i class="bi bi-plus"></i> Thêm nhân viên
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

      <div v-else-if="filteredEmployees.length === 0" class="text-center p-5">
        <p>Không tìm thấy nhân viên nào.</p>
      </div>

      <div v-else class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Họ và tên</th>
              <th>Email</th>
              <th>Số điện thoại</th>
              <th>Chức vụ</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="employee in filteredEmployees" :key="employee.id">
              <td>{{ employee.id }}</td>
              <td>{{ employee.fullName }}</td>
              <td>{{ employee.email }}</td>
              <td>{{ employee.phone }}</td>
              <td>{{ employee.position }}</td>
              <td>
                <span :class="getStatusClass(employee.status)">{{
                  employee.status
                }}</span>
              </td>
              <td class="action-buttons">
                <button
                  class="btn btn-sm btn-info me-1"
                  @click="viewEmployee(employee)"
                >
                  <i class="bi bi-eye"></i>
                </button>
                <button
                  class="btn btn-sm btn-warning me-1"
                  @click="editEmployee(employee)"
                >
                  <i class="bi bi-pencil"></i>
                </button>
                <button
                  class="btn btn-sm btn-danger"
                  @click="confirmDeleteEmployee(employee)"
                >
                  <i class="bi bi-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add/Edit Employee Modal -->
    <div class="modal fade" id="employeeModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ isEditing ? "Cập nhật nhân viên" : "Thêm nhân viên mới" }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveEmployee">
              <div class="mb-3">
                <label class="form-label">Họ và tên</label>
                <input
                  type="text"
                  class="form-control"
                  v-model="currentEmployee.fullName"
                  required
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Email</label>
                <input
                  type="email"
                  class="form-control"
                  v-model="currentEmployee.email"
                  required
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Số điện thoại</label>
                <input
                  type="tel"
                  class="form-control"
                  v-model="currentEmployee.phone"
                  required
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Chức vụ</label>
                <select
                  class="form-select"
                  v-model="currentEmployee.position"
                  required
                >
                  <option value="Quản lý">Quản lý</option>
                  <option value="Nhân viên bán hàng">Nhân viên bán hàng</option>
                  <option value="Nhân viên kho">Nhân viên kho</option>
                  <option value="Kế toán">Kế toán</option>
                </select>
              </div>
              <div class="mb-3">
                <label class="form-label">Trạng thái</label>
                <select class="form-select" v-model="currentEmployee.status">
                  <option value="Đang làm việc">Đang làm việc</option>
                  <option value="Tạm nghỉ">Tạm nghỉ</option>
                  <option value="Đã nghỉ việc">Đã nghỉ việc</option>
                </select>
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
              Bạn có chắc chắn muốn xóa nhân viên
              <strong>{{ currentEmployee.fullName }}</strong
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
              @click="deleteEmployee"
            >
              Xóa
            </button>
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
  name: "EmployeeView",
  setup() {
    const employees = ref([]);
    const loading = ref(true);
    const error = ref(null);
    const searchTerm = ref("");
    const currentEmployee = ref({
      id: null,
      fullName: "",
      email: "",
      phone: "",
      position: "Nhân viên bán hàng",
      status: "Đang làm việc",
    });
    const isEditing = ref(false);

    // Sample data for development
    const sampleEmployees = [
      {
      id: 1,
      fullName: "Phạm Minh Tuấn",
      email: "tuan.pham@example.com",
      phone: "0987654321",
      position: "Quản lý",
      status: "Đang làm việc",
      },
      {
      id: 2,
      fullName: "Đỗ Thị Hạnh",
      email: "hanh.do@example.com",
      phone: "0912345678",
      position: "Nhân viên bán hàng",
      status: "Đang làm việc",
      },
      {
      id: 3,
      fullName: "Vũ Quốc Dũng",
      email: "dung.vu@example.com",
      phone: "0934567890",
      position: "Nhân viên kho",
      status: "Tạm nghỉ",
      }
    ];

    const fetchEmployees = async () => {
      try {
        loading.value = true;
        // Uncomment the following line when API is ready
        // const response = await axios.get('/api/employees');
        // employees.value = response.data;

        // For development, use sample data
        setTimeout(() => {
          employees.value = sampleEmployees;
          loading.value = false;
        }, 500);
      } catch (err) {
        error.value = "Không thể tải dữ liệu nhân viên: " + err.message;
        loading.value = false;
      }
    };

    const filteredEmployees = computed(() => {
      if (!searchTerm.value) return employees.value;

      const term = searchTerm.value.toLowerCase();
      return employees.value.filter(
        (emp) =>
          emp.fullName.toLowerCase().includes(term) ||
          emp.email.toLowerCase().includes(term) ||
          emp.phone.includes(term) ||
          emp.position.toLowerCase().includes(term)
      );
    });

    const openAddModal = () => {
      isEditing.value = false;
      currentEmployee.value = {
        id: null,
        fullName: "",
        email: "",
        phone: "",
        position: "Nhân viên bán hàng",
        status: "Đang làm việc",
      };
      const modal = new bootstrap.Modal(
        document.getElementById("employeeModal")
      );
      modal.show();
    };

    const editEmployee = (employee) => {
      isEditing.value = true;
      currentEmployee.value = { ...employee };
      const modal = new bootstrap.Modal(
        document.getElementById("employeeModal")
      );
      modal.show();
    };

    const viewEmployee = (employee) => {
      // View employee details implementation
      console.log("View employee:", employee);
    };

    const confirmDeleteEmployee = (employee) => {
      currentEmployee.value = { ...employee };
      const modal = new bootstrap.Modal(document.getElementById("deleteModal"));
      modal.show();
    };

    const saveEmployee = async () => {
      try {
        if (isEditing.value) {
          // Uncomment when API is ready
          // await axios.put(`/api/employees/${currentEmployee.value.id}`, currentEmployee.value);

          // For development
          const index = employees.value.findIndex(
            (e) => e.id === currentEmployee.value.id
          );
          if (index !== -1) {
            employees.value[index] = { ...currentEmployee.value };
          }
        } else {
          // Uncomment when API is ready
          // const response = await axios.post('/api/employees', currentEmployee.value);
          // employees.value.push(response.data);

          // For development
          const newId = Math.max(0, ...employees.value.map((e) => e.id)) + 1;
          employees.value.push({
            ...currentEmployee.value,
            id: newId,
          });
        }

        // Close the modal
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("employeeModal")
        );
        modal.hide();
      } catch (err) {
        error.value =
          `Không thể ${isEditing.value ? "cập nhật" : "thêm"} nhân viên: ` +
          err.message;
      }
    };

    const deleteEmployee = async () => {
      try {
        // Uncomment when API is ready
        // await axios.delete(`/api/employees/${currentEmployee.value.id}`);

        // For development
        employees.value = employees.value.filter(
          (e) => e.id !== currentEmployee.value.id
        );

        // Close the modal
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("deleteModal")
        );
        modal.hide();
      } catch (err) {
        error.value = "Không thể xóa nhân viên: " + err.message;
      }
    };

    const getStatusClass = (status) => {
      switch (status) {
        case "Đang làm việc":
          return "badge bg-success";
        case "Tạm nghỉ":
          return "badge bg-warning";
        case "Đã nghỉ việc":
          return "badge bg-danger";
        default:
          return "badge bg-secondary";
      }
    };

    onMounted(() => {
      fetchEmployees();
    });

    return {
      employees,
      loading,
      error,
      searchTerm,
      currentEmployee,
      isEditing,
      filteredEmployees,
      openAddModal,
      editEmployee,
      viewEmployee,
      confirmDeleteEmployee,
      saveEmployee,
      deleteEmployee,
      getStatusClass,
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
</style>
