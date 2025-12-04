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
              <th>Tên đăng nhập</th>
              <th>Mật khẩu</th>
              <th>Chức vụ</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="employee in filteredEmployees" :key="employee.id">
              <td>{{ employee.id }}</td>
              <td>{{ employee.fullName }}</td>
              <td>{{ employee.username }}</td>
              <td>{{ employee.passwordHash }}</td>
              <td>{{ employee.position }}</td>
              <td>
                <span :class="getStatusClass(employee.status)">{{
                  employee.status
                }}</span>
              </td>
              <td class="action-buttons">
                <!-- <button
                  class="btn btn-sm btn-info me-1"
                  @click="viewEmployee(employee)"
                >
                  <i class="bi bi-eye"></i>
                </button> -->
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
              <!-- <div class="mb-3">
                <label class="form-label">Họ và tên</label>
                <input
                  type="text"
                  class="form-control"
                  v-model="currentEmployee.fullName"
                  required
                />
              </div> -->
              <div class="mb-3">
                <label class="form-label">Tên đăng nhập</label>
                <input
                  type="text"
                  class="form-control"
                  v-model="currentEmployee.username"
                  required
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Mật khẩu</label>
                <input
                  type="password"
                  class="form-control"
                  v-model="currentEmployee.passwordHash"
                  required
                />
              </div>
              <!-- <div class="mb-3">
                <label class="form-label">Số điện thoại</label>
                <input
                  type="tel"
                  class="form-control"
                  v-model="currentEmployee.phone"
                  required
                />
              </div> -->
              <div class="mb-3">
                <label class="form-label">Chức vụ</label>
                <select
                  class="form-select"
                  v-model="currentEmployee.roleId"
                  required
                >
                  <option value="1">Quản lý</option>
                  <option value="2">Nhân viên</option>
                </select>
              </div>
              <!-- <div class="mb-3">
                <label class="form-label">Trạng thái</label>
                <select class="form-select" v-model="currentEmployee.status">
                  <option value="Đang làm việc">Đang làm việc</option>
                  <option value="Tạm nghỉ">Tạm nghỉ</option>
                  <option value="Đã nghỉ việc">Đã nghỉ việc</option>
                </select>
              </div> -->
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
      username: "",
      passwordHash: "",
      roleId: 0,
    });
    const isEditing = ref(false);

    // Sample data for development
    const sampleEmployees = []; // kept as a fallback if API is unavailable

    const filteredEmployees = computed(() => {
      if (!searchTerm.value) return employees.value;

      const term = searchTerm.value.toLowerCase();
      return employees.value.filter((emp) => {
        return (
          (emp.fullName || "").toLowerCase().includes(term) ||
          (emp.username || "").toLowerCase().includes(term) ||
          (emp.passwordHash || "").toLowerCase().includes(term) ||
          (emp.position || "").toLowerCase().includes(term)
        );
      });
    });

    const fetchEmployees = async () => {
      try {
        loading.value = true;

        // call the provided API
        const response = await axios.get(
          "https://localhost:7055/api/Auth/get-account"
        );

        // normalize response: support direct array, { data: [...] }, { $values: [...] } or single object
        const respData = response.data;

        // helper to map account object from API to the employee model used by the UI
        const mapAccountToEmployee = (acc) => {
          return {
            id: acc.accountId ?? acc.id ?? null,
            fullName: acc.fullName ?? acc.username ?? "",
            username: acc.username ?? "",
            passwordHash: acc.passwordHash ?? acc.password ?? "",
            position:
              acc.position ??
              (typeof acc.roleId === "number"
                ? acc.roleId === 1
                  ? "Quản lý"
                  : "Nhân viên bán hàng"
                : "Nhân viên bán hàng"),
            status: acc.status ?? "Đang làm việc",
          };
        };

        let sourceArray = [];

        if (Array.isArray(respData)) {
          sourceArray = respData;
        } else if (respData && Array.isArray(respData.data)) {
          sourceArray = respData.data;
        } else if (respData && Array.isArray(respData.$values)) {
          sourceArray = respData.$values;
        } else if (respData) {
          sourceArray = [respData];
        } else {
          sourceArray = [];
        }

        employees.value = sourceArray.map(mapAccountToEmployee);
      } catch (err) {
        // fallback to sample data if API fails and expose error message
        employees.value = sampleEmployees;
        error.value =
          "Không thể tải dữ liệu nhân viên: " + (err && err.message ? err.message : "");
      } finally {
        loading.value = false;
      }
    };

    const openAddModal = () => {
      isEditing.value = false;
      currentEmployee.value = {
        id: null,
        username: "",
        password: "",
        roleId: 0,
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
        // Update employee via API
        await axios.put(
        `https://localhost:7055/api/Auth/update-account-nhanvien/${currentEmployee.value.id}`,
        {
          username: currentEmployee.value.username,
          password: currentEmployee.value.passwordHash,
          roleId: parseInt(currentEmployee.value.roleId),
        }
        );

        const index = employees.value.findIndex(
        (e) => e.id === currentEmployee.value.id
        );
        if (index !== -1) {
        employees.value[index] = { ...currentEmployee.value };
        }
      } else {
        // Add new employee via API
        const response = await axios.post(
        "https://localhost:7055/api/Auth/add-account-nhanvien",
        {
          username: currentEmployee.value.username,
          password: currentEmployee.value.passwordHash,
          roleId: parseInt(currentEmployee.value.roleId),
        }
        );

        const newEmployee = {
        ...currentEmployee.value,
        id: response.data.accountId ?? response.data.id,
        };
        employees.value.push(newEmployee);
      }

      // Close the modal
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("employeeModal")
      );
      modal.hide();

      // Reload data after save
      await fetchEmployees();
      } catch (err) {
      error.value = "Không thể lưu nhân viên: " + err.message;
      }
    };

    const deleteEmployee = async () => {
      try {
        // Delete employee via API
        await axios.delete(
          `https://localhost:7055/api/Auth/delete-account-nhanvien/${currentEmployee.value.id}`
        );

        employees.value = employees.value.filter(
          (e) => e.id !== currentEmployee.value.id
        );

        // Close the modal
        const modalDelete = bootstrap.Modal.getInstance(
          document.getElementById("deleteModal")
        );
        modalDelete.hide();
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
