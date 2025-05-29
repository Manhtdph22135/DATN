<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý khách hàng</h2>
      <div class="header-actions">
        <div class="search-container">
          <input
            type="text"
            class="search-input"
            placeholder="Tìm kiếm khách hàng..."
            v-model="searchTerm"
          />
          <i class="bi bi-search search-icon"></i>
        </div>
        <button class="btn btn-success" @click="openAddModal">
          <i class="bi bi-plus"></i> Thêm khách hàng
        </button>
      </div>
    </div>

    <div class="content-container">
      <div class="customer-filters mb-4">
        <div class="filter-group">
          <div class="member-filter">
            <label>Hạng thành viên:</label>
            <select v-model="membershipFilter" class="form-select">
              <option value="">Tất cả hạng</option>
              <option value="bronze">Thành viên Đồng</option>
              <option value="silver">Thành viên Bạc</option>
              <option value="gold">Thành viên Vàng</option>
              <option value="platinum">Thành viên Bạch Kim</option>
            </select>
          </div>
          <div class="status-filter">
            <label>Trạng thái:</label>
            <select v-model="statusFilter" class="form-select">
              <option value="">Tất cả trạng thái</option>
              <option value="active">Đang hoạt động</option>
              <option value="inactive">Không hoạt động</option>
            </select>
          </div>
        </div>
      </div>

      <div v-if="loading" class="text-center p-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Đang tải...</span>
        </div>
      </div>

      <div v-else-if="error" class="alert alert-danger">
        {{ error }}
      </div>

      <div v-else-if="filteredCustomers.length === 0" class="text-center p-5">
        <p>Không tìm thấy khách hàng nào.</p>
      </div>

      <div v-else class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Họ và tên</th>
              <th>Email</th>
              <th>Số điện thoại</th>
              <th>Hạng thành viên</th>
              <th>Đơn hàng</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="customer in filteredCustomers" :key="customer.id">
              <td>{{ customer.id }}</td>
              <td>{{ customer.fullName }}</td>
              <td>{{ customer.email }}</td>
              <td>{{ customer.phone }}</td>
              <td>
                <span
                  class="badge"
                  :class="getMembershipClass(customer.membership)"
                >
                  {{ getMembershipText(customer.membership) }}
                </span>
              </td>
              <td>{{ customer.orderCount }} đơn</td>
              <td>
                <span
                  class="badge"
                  :class="
                    customer.status === 'active' ? 'bg-success' : 'bg-secondary'
                  "
                >
                  {{
                    customer.status === "active"
                      ? "Hoạt động"
                      : "Không hoạt động"
                  }}
                </span>
              </td>
              <td class="action-buttons">
                <button
                  class="btn btn-sm btn-info me-1"
                  @click="viewCustomer(customer)"
                >
                  <i class="bi bi-eye"></i>
                </button>
                <button
                  class="btn btn-sm btn-primary me-1"
                  @click="editCustomer(customer)"
                >
                  <i class="bi bi-pencil"></i>
                </button>
                <button
                  v-if="customer.status === 'active'"
                  class="btn btn-sm btn-warning"
                  @click="toggleCustomerStatus(customer)"
                  title="Vô hiệu hóa tài khoản"
                >
                  <i class="bi bi-person-x"></i>
                </button>
                <button
                  v-else
                  class="btn btn-sm btn-success"
                  @click="toggleCustomerStatus(customer)"
                  title="Kích hoạt tài khoản"
                >
                  <i class="bi bi-person-check"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add/Edit Customer Modal -->
    <div class="modal fade" id="customerModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ isEditing ? "Cập nhật khách hàng" : "Thêm khách hàng mới" }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveCustomer">
              <div class="row mb-3">
                <div class="col-md-6">
                  <label class="form-label">Họ và tên</label>
                  <input
                    type="text"
                    class="form-control"
                    v-model="currentCustomer.fullName"
                    required
                  />
                </div>
                <div class="col-md-6">
                  <label class="form-label">Email</label>
                  <input
                    type="email"
                    class="form-control"
                    v-model="currentCustomer.email"
                    required
                  />
                </div>
              </div>
              <div class="row mb-3">
                <div class="col-md-6">
                  <label class="form-label">Số điện thoại</label>
                  <input
                    type="tel"
                    class="form-control"
                    v-model="currentCustomer.phone"
                    required
                  />
                </div>
                <div class="col-md-6">
                  <label class="form-label">Ngày sinh</label>
                  <input
                    type="date"
                    class="form-control"
                    v-model="currentCustomer.birthday"
                  />
                </div>
              </div>
              <div class="row mb-3">
                <div class="col-md-6">
                  <label class="form-label">Hạng thành viên</label>
                  <select
                    class="form-select"
                    v-model="currentCustomer.membership"
                  >
                    <option value="bronze">Thành viên Đồng</option>
                    <option value="silver">Thành viên Bạc</option>
                    <option value="gold">Thành viên Vàng</option>
                    <option value="platinum">Thành viên Bạch Kim</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Trạng thái</label>
                  <select class="form-select" v-model="currentCustomer.status">
                    <option value="active">Hoạt động</option>
                    <option value="inactive">Không hoạt động</option>
                  </select>
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label">Địa chỉ</label>
                <textarea
                  class="form-control"
                  v-model="currentCustomer.address"
                  rows="2"
                ></textarea>
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

    <!-- View Customer Details Modal -->
    <div
      class="modal fade"
      id="customerDetailsModal"
      tabindex="-1"
      aria-hidden="true"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              Thông tin khách hàng: {{ currentCustomer.fullName }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <!-- Customer Profile -->
            <div class="customer-profile mb-4">
              <div class="row">
                <div class="col-lg-3 col-md-4">
                  <div class="customer-avatar">
                    <img
                      :src="
                        currentCustomer.avatar ||
                        'https://placehold.co/150x150?text=Avatar'
                      "
                      :alt="currentCustomer.fullName"
                      class="img-fluid rounded"
                    />
                    <div class="text-center mt-2">
                      <span
                        class="badge px-3 py-2"
                        :class="getMembershipClass(currentCustomer.membership)"
                      >
                        {{ getMembershipText(currentCustomer.membership) }}
                      </span>
                    </div>
                  </div>
                </div>
                <div class="col-lg-9 col-md-8">
                  <div class="customer-info">
                    <div class="info-row">
                      <span class="info-label">Họ và tên:</span>
                      <span class="info-value">{{
                        currentCustomer.fullName
                      }}</span>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Email:</span>
                      <span class="info-value">{{
                        currentCustomer.email
                      }}</span>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Số điện thoại:</span>
                      <span class="info-value">{{
                        currentCustomer.phone
                      }}</span>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Ngày sinh:</span>
                      <span class="info-value">{{
                        formatDate(currentCustomer.birthday)
                      }}</span>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Địa chỉ:</span>
                      <span class="info-value">{{
                        currentCustomer.address
                      }}</span>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Ngày đăng ký:</span>
                      <span class="info-value">{{
                        formatDate(currentCustomer.registerDate)
                      }}</span>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Trạng thái:</span>
                      <span class="info-value">
                        <span
                          class="badge"
                          :class="
                            currentCustomer.status === 'active'
                              ? 'bg-success'
                              : 'bg-secondary'
                          "
                        >
                          {{
                            currentCustomer.status === "active"
                              ? "Hoạt động"
                              : "Không hoạt động"
                          }}
                        </span>
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Customer Stats -->
            <div class="customer-stats mb-4">
              <h6 class="section-title">Thống kê mua hàng</h6>
              <div class="stats-cards">
                <div class="stat-card">
                  <div class="stat-value">{{ currentCustomer.orderCount }}</div>
                  <div class="stat-label">Tổng đơn hàng</div>
                </div>
                <div class="stat-card">
                  <div class="stat-value">
                    {{ formatCurrency(currentCustomer.totalSpent) }}
                  </div>
                  <div class="stat-label">Tổng chi tiêu</div>
                </div>
                <div class="stat-card">
                  <div class="stat-value">{{ currentCustomer.points }}</div>
                  <div class="stat-label">Điểm tích lũy</div>
                </div>
              </div>
            </div>

            <!-- Recent Orders -->
            <div class="recent-orders">
              <h6 class="section-title">Đơn hàng gần đây</h6>
              <div
                v-if="
                  !currentCustomer.recentOrders ||
                  currentCustomer.recentOrders.length === 0
                "
                class="text-center py-3"
              >
                <p class="text-muted">Khách hàng chưa có đơn hàng nào</p>
              </div>
              <div v-else class="table-responsive">
                <table class="table table-bordered table-striped">
                  <thead>
                    <tr>
                      <th>Mã đơn</th>
                      <th>Ngày đặt</th>
                      <th>Tổng tiền</th>
                      <th>Trạng thái</th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="order in currentCustomer.recentOrders"
                      :key="order.id"
                    >
                      <td>#{{ order.id }}</td>
                      <td>{{ formatDate(order.orderDate) }}</td>
                      <td>{{ formatCurrency(order.total) }}</td>
                      <td>
                        <span
                          class="badge"
                          :class="getOrderStatusClass(order.status)"
                        >
                          {{ getOrderStatusText(order.status) }}
                        </span>
                      </td>
                      <td>
                        <button class="btn btn-sm btn-info">
                          <i class="bi bi-box-arrow-up-right"></i> Xem
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-primary me-2"
              @click="editCustomer(currentCustomer)"
            >
              <i class="bi bi-pencil"></i> Sửa
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
</template>

<script>
import { ref, computed, onMounted } from "vue";
import axios from "axios";
import * as bootstrap from "bootstrap";

// Create a custom instance of axios for this component
const customerApi = axios.create({
  baseURL: "", // Empty baseURL to prevent automatic prefixing
});

export default {
  name: "CustomerView",
  setup() {
    const customers = ref([]);
    const loading = ref(true);
    const error = ref(null);
    const searchTerm = ref("");
    const membershipFilter = ref("");
    const statusFilter = ref("");
    const isEditing = ref(false);
    const currentCustomer = ref({
      id: null,
      fullName: "",
      email: "",
      phone: "",
      birthday: "",
      address: "",
      membership: "bronze",
      status: "active",
      registerDate: "",
      orderCount: 0,
      totalSpent: 0,
      points: 0,
      avatar: "",
      recentOrders: [],
    });

    const fetchCustomers = async () => {
      try {
        loading.value = true;

        // In development mode, use sample data
        setTimeout(() => {
          customers.value = sampleCustomers;
          loading.value = false;
        }, 500);

        // NOTE: Real API implementation will be added later
        // Example of future implementation:
        // const response = await axios.get('/api/customers');
        // customers.value = response.data;
      } catch (err) {
        error.value = "Không thể tải dữ liệu khách hàng: " + err.message;
        loading.value = false;
      }
    };

    const filteredCustomers = computed(() => {
      let result = customers.value;

      // Filter by search term
      if (searchTerm.value) {
        const term = searchTerm.value.toLowerCase();
        result = result.filter(
          (customer) =>
            customer.fullName.toLowerCase().includes(term) ||
            customer.email.toLowerCase().includes(term) ||
            customer.phone.includes(term)
        );
      }

      // Filter by membership
      if (membershipFilter.value) {
        result = result.filter(
          (customer) => customer.membership === membershipFilter.value
        );
      }

      // Filter by status
      if (statusFilter.value) {
        result = result.filter(
          (customer) => customer.status === statusFilter.value
        );
      }

      return result;
    });

    const openAddModal = () => {
      isEditing.value = false;
      currentCustomer.value = {
        id: null,
        fullName: "",
        email: "",
        phone: "",
        birthday: "",
        address: "",
        membership: "bronze",
        status: "active",
        registerDate: new Date().toISOString().split("T")[0],
        orderCount: 0,
        totalSpent: 0,
        points: 0,
        recentOrders: [],
      };
      const modal = new bootstrap.Modal(
        document.getElementById("customerModal")
      );
      modal.show();
    };

    const editCustomer = (customer) => {
      isEditing.value = true;
      currentCustomer.value = { ...customer };

      // Close details modal if it's open
      const detailsModal = bootstrap.Modal.getInstance(
        document.getElementById("customerDetailsModal")
      );
      if (detailsModal) {
        detailsModal.hide();
      }

      // Show edit modal
      setTimeout(() => {
        const modal = new bootstrap.Modal(
          document.getElementById("customerModal")
        );
        modal.show();
      }, 500);
    };

    const viewCustomer = (customer) => {
      currentCustomer.value = { ...customer };
      const modal = new bootstrap.Modal(
        document.getElementById("customerDetailsModal")
      );
      modal.show();
    };

    const toggleCustomerStatus = async (customer) => {
      try {
        const newStatus = customer.status === "active" ? "inactive" : "active";

        // Uncomment when API is ready
        // await axios.patch(`/api/customers/${customer.id}/status`, { status: newStatus });

        // For development
        const index = customers.value.findIndex((c) => c.id === customer.id);
        if (index !== -1) {
          customers.value[index].status = newStatus;
        }
      } catch (err) {
        error.value =
          "Không thể thay đổi trạng thái khách hàng: " + err.message;
      }
    };

    const saveCustomer = async () => {
      try {
        if (isEditing.value) {
          // Uncomment when API is ready
          // await axios.put(`/api/customers/${currentCustomer.value.id}`, currentCustomer.value);

          // For development
          const index = customers.value.findIndex(
            (c) => c.id === currentCustomer.value.id
          );
          if (index !== -1) {
            customers.value[index] = { ...currentCustomer.value };
          }
        } else {
          // Uncomment when API is ready
          // const response = await axios.post('/api/customers', currentCustomer.value);
          // customers.value.push(response.data);

          // For development
          const newId = Math.max(0, ...customers.value.map((c) => c.id)) + 1;
          customers.value.push({
            ...currentCustomer.value,
            id: newId,
            avatar: "https://placehold.co/150x150?text=User", // Default avatar
          });
        }

        // Close the modal
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("customerModal")
        );
        modal.hide();
      } catch (err) {
        error.value =
          `Không thể ${isEditing.value ? "cập nhật" : "thêm"} khách hàng: ` +
          err.message;
      }
    };

    const formatDate = (dateString) => {
      if (!dateString) return "N/A";
      const options = { year: "numeric", month: "2-digit", day: "2-digit" };
      return new Date(dateString).toLocaleDateString("vi-VN", options);
    };

    const formatCurrency = (value) => {
      return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
      }).format(value);
    };

    const getMembershipClass = (membership) => {
      switch (membership) {
        case "bronze":
          return "bg-bronze";
        case "silver":
          return "bg-silver";
        case "gold":
          return "bg-gold";
        case "platinum":
          return "bg-platinum";
        default:
          return "bg-secondary";
      }
    };

    const getMembershipText = (membership) => {
      switch (membership) {
        case "bronze":
          return "Đồng";
        case "silver":
          return "Bạc";
        case "gold":
          return "Vàng";
        case "platinum":
          return "Bạch Kim";
        default:
          return "Không xác định";
      }
    };

    const getOrderStatusClass = (status) => {
      switch (status) {
        case "pending":
          return "bg-warning";
        case "processing":
          return "bg-primary";
        case "shipped":
          return "bg-info";
        case "delivered":
          return "bg-success";
        case "cancelled":
          return "bg-danger";
        default:
          return "bg-secondary";
      }
    };

    const getOrderStatusText = (status) => {
      switch (status) {
        case "pending":
          return "Chờ xác nhận";
        case "processing":
          return "Đang xử lý";
        case "shipped":
          return "Đã giao hàng";
        case "delivered":
          return "Đã nhận hàng";
        case "cancelled":
          return "Đã hủy";
        default:
          return "Không xác định";
      }
    };

    onMounted(() => {
      fetchCustomers();

      // Check if Bootstrap JS is loaded for modals
      if (typeof bootstrap === "undefined") {
        console.error("Bootstrap JS is not loaded. Modals will not work.");
      }
    });

    return {
      customers,
      loading,
      error,
      searchTerm,
      membershipFilter,
      statusFilter,
      isEditing,
      currentCustomer,
      filteredCustomers,
      openAddModal,
      editCustomer,
      viewCustomer,
      toggleCustomerStatus,
      saveCustomer,
      formatDate,
      formatCurrency,
      getMembershipClass,
      getMembershipText,
      getOrderStatusClass,
      getOrderStatusText,
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

.customer-filters {
  display: flex;
  margin-bottom: 20px;
}

.filter-group {
  display: flex;
  gap: 15px;
  align-items: flex-end;
  flex-wrap: wrap;
}

.member-filter,
.status-filter {
  min-width: 180px;
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

/* Customer Details Styling */
.customer-profile {
  padding: 15px;
  background-color: #f9f9f9;
  border-radius: 10px;
}

.customer-avatar {
  text-align: center;
}

.customer-info {
  padding: 10px;
}

.info-row {
  margin-bottom: 8px;
  display: flex;
}

.info-label {
  font-weight: bold;
  min-width: 120px;
}

.section-title {
  font-weight: bold;
  margin-bottom: 15px;
  padding-bottom: 5px;
  border-bottom: 2px solid #f0f0f0;
}

.stats-cards {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 15px;
  margin-bottom: 20px;
}

.stat-card {
  background: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 8px;
  padding: 15px;
  text-align: center;
  transition: transform 0.2s ease;
}

.stat-card:hover {
  transform: translateY(-5px);
}

.stat-value {
  font-size: 1.5rem;
  font-weight: bold;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 0.9rem;
  color: #6c757d;
}

/* Membership badge colors */
.bg-bronze {
  background-color: #cd7f32;
  color: white;
}

.bg-silver {
  background-color: #c0c0c0;
  color: white;
}

.bg-gold {
  background-color: #ffd700;
  color: #212529;
}

.bg-platinum {
  background: linear-gradient(45deg, #e5e4e2, #ffffff);
  color: #212529;
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: stretch;
  }

  .header-actions {
    margin-top: 10px;
  }

  .filter-group {
    flex-direction: column;
    align-items: stretch;
  }
}
</style>
