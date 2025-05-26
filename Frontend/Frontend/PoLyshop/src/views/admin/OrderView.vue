<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý đơn hàng</h2>
      <div class="header-actions">
        <div class="search-container">
          <input
            type="text"
            class="search-input"
            placeholder="Tìm kiếm đơn hàng..."
            v-model="searchTerm"
          />
          <i class="bi bi-search search-icon"></i>
        </div>
        <div class="filter-container">
          <select v-model="statusFilter" class="form-select">
            <option value="">Tất cả trạng thái</option>
            <option value="pending">Chờ xác nhận</option>
            <option value="processing">Đang xử lý</option>
            <option value="shipped">Đã giao hàng</option>
            <option value="delivered">Đã nhận hàng</option>
            <option value="cancelled">Đã hủy</option>
          </select>
        </div>
      </div>
    </div>

    <div class="content-container">
      <div class="order-filters mb-4">
        <div class="date-range">
          <div class="date-input">
            <label>Từ ngày:</label>
            <input type="date" class="form-control" v-model="startDate" />
          </div>
          <div class="date-input">
            <label>Đến ngày:</label>
            <input type="date" class="form-control" v-model="endDate" />
          </div>
          <button class="btn btn-primary" @click="applyFilters">
            <i class="bi bi-filter"></i> Lọc
          </button>
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

      <div v-else-if="filteredOrders.length === 0" class="text-center p-5">
        <p>Không tìm thấy đơn hàng nào.</p>
      </div>

      <div v-else class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>Mã đơn</th>
              <th>Khách hàng</th>
              <th>Ngày đặt</th>
              <th>Tổng tiền</th>
              <th>Trạng thái</th>
              <th>Thanh toán</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="order in filteredOrders"
              :key="order.id"
              @click="viewOrderDetails(order)"
              class="order-row"
            >
              <td>#{{ order.id }}</td>
              <td>{{ order.customerName }}</td>
              <td>{{ formatDate(order.orderDate) }}</td>
              <td>{{ formatCurrency(order.totalAmount) }}</td>
              <td>
                <span class="badge" :class="getStatusClass(order.status)">
                  {{ getStatusText(order.status) }}
                </span>
              </td>
              <td>
                <span
                  class="badge"
                  :class="order.isPaid ? 'bg-success' : 'bg-warning'"
                >
                  {{ order.isPaid ? "Đã thanh toán" : "Chưa thanh toán" }}
                </span>
              </td>
              <td class="action-buttons">
                <button
                  class="btn btn-sm btn-info me-1"
                  @click.stop="viewOrderDetails(order)"
                >
                  <i class="bi bi-eye"></i>
                </button>
                <button
                  class="btn btn-sm btn-primary me-1"
                  @click.stop="updateStatus(order)"
                >
                  <i class="bi bi-arrow-clockwise"></i>
                </button>
                <button
                  class="btn btn-sm btn-danger"
                  @click.stop="confirmCancelOrder(order)"
                  v-if="order.status === 'pending'"
                >
                  <i class="bi bi-x-circle"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Order Details Modal -->
    <div
      class="modal fade"
      id="orderDetailsModal"
      tabindex="-1"
      aria-hidden="true"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              Chi tiết đơn hàng #{{ currentOrder.id }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <div class="order-info mb-4">
              <div class="row">
                <div class="col-md-6">
                  <h6>Thông tin khách hàng</h6>
                  <p><strong>Tên:</strong> {{ currentOrder.customerName }}</p>
                  <p>
                    <strong>Email:</strong> {{ currentOrder.customerEmail }}
                  </p>
                  <p>
                    <strong>Số điện thoại:</strong>
                    {{ currentOrder.customerPhone }}
                  </p>
                </div>
                <div class="col-md-6">
                  <h6>Thông tin đơn hàng</h6>
                  <p>
                    <strong>Ngày đặt:</strong>
                    {{ formatDate(currentOrder.orderDate) }}
                  </p>
                  <p>
                    <strong>Trạng thái:</strong>
                    <span
                      class="badge"
                      :class="getStatusClass(currentOrder.status)"
                    >
                      {{ getStatusText(currentOrder.status) }}
                    </span>
                  </p>
                  <p>
                    <strong>Thanh toán:</strong>
                    <span
                      class="badge"
                      :class="currentOrder.isPaid ? 'bg-success' : 'bg-warning'"
                    >
                      {{
                        currentOrder.isPaid
                          ? "Đã thanh toán"
                          : "Chưa thanh toán"
                      }}
                    </span>
                  </p>
                </div>
              </div>
              <div class="row mt-3">
                <div class="col-12">
                  <h6>Địa chỉ giao hàng</h6>
                  <p>{{ currentOrder.shippingAddress }}</p>
                </div>
              </div>
            </div>

            <h6>Sản phẩm</h6>
            <div class="table-responsive">
              <table class="table table-striped">
                <thead>
                  <tr>
                    <th>Sản phẩm</th>
                    <th>Giá</th>
                    <th>Số lượng</th>
                    <th>Tổng</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in currentOrder.items" :key="item.id">
                    <td>
                      <div class="product-cell">
                        <img
                          :src="item.image"
                          :alt="item.name"
                          class="product-thumbnail me-2"
                        />
                        <div>{{ item.name }}</div>
                      </div>
                    </td>
                    <td>{{ formatCurrency(item.price) }}</td>
                    <td>{{ item.quantity }}</td>
                    <td>{{ formatCurrency(item.price * item.quantity) }}</td>
                  </tr>
                </tbody>
                <tfoot>
                  <tr>
                    <td colspan="3" class="text-end">
                      <strong>Tạm tính:</strong>
                    </td>
                    <td>{{ formatCurrency(currentOrder.subtotal) }}</td>
                  </tr>
                  <tr>
                    <td colspan="3" class="text-end">
                      <strong>Phí giao hàng:</strong>
                    </td>
                    <td>{{ formatCurrency(currentOrder.shippingFee) }}</td>
                  </tr>
                  <tr>
                    <td colspan="3" class="text-end">
                      <strong>Giảm giá:</strong>
                    </td>
                    <td>{{ formatCurrency(currentOrder.discount) }}</td>
                  </tr>
                  <tr>
                    <td colspan="3" class="text-end">
                      <strong>Tổng cộng:</strong>
                    </td>
                    <td class="fw-bold">
                      {{ formatCurrency(currentOrder.totalAmount) }}
                    </td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>
          <div class="modal-footer">
            <div
              class="status-update"
              v-if="currentOrder.status !== 'cancelled'"
            >
              <select v-model="newStatus" class="form-select me-2">
                <option value="pending">Chờ xác nhận</option>
                <option value="processing">Đang xử lý</option>
                <option value="shipped">Đã giao hàng</option>
                <option value="delivered">Đã nhận hàng</option>
                <option value="cancelled">Đã hủy</option>
              </select>
              <button class="btn btn-primary" @click="saveStatus">
                Cập nhật trạng thái
              </button>
            </div>
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

    <!-- Cancel Order Confirmation Modal -->
    <div
      class="modal fade"
      id="cancelOrderModal"
      tabindex="-1"
      aria-hidden="true"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Xác nhận hủy đơn hàng</h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <p>
              Bạn có chắc chắn muốn hủy đơn hàng
              <strong>#{{ currentOrder.id }}</strong
              >?
            </p>
            <p class="text-danger">Hành động này không thể hoàn tác.</p>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              Đóng
            </button>
            <button type="button" class="btn btn-danger" @click="cancelOrder">
              Hủy đơn hàng
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
  name: "OrderView",
  setup() {
    const orders = ref([]);
    const loading = ref(true);
    const error = ref(null);
    const searchTerm = ref("");
    const statusFilter = ref("");
    const startDate = ref("");
    const endDate = ref("");
    const currentOrder = ref({
      id: null,
      customerName: "",
      customerEmail: "",
      customerPhone: "",
      orderDate: "",
      status: "",
      isPaid: false,
      shippingAddress: "",
      items: [],
      subtotal: 0,
      shippingFee: 0,
      discount: 0,
      totalAmount: 0,
    });
    const newStatus = ref("");

    // Sample data
    const sampleOrders = [
      {
        id: 1001,
        customerName: "Nguyễn Văn An",
        customerEmail: "nguyenvana@example.com",
        customerPhone: "0901234567",
        orderDate: "2023-10-15",
        status: "pending",
        isPaid: false,
        shippingAddress: "123 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh",
        items: [
          {
            id: 1,
            name: "Áo thun Unisex Cotton",
            price: 250000,
            quantity: 2,
            image: "https://media1.thehungryjpeg.com/thumbs/800_4262428_zzultqdefwrzpkyb25nsmpzexo4h46xkmpbv2rmb.png",
          },
          {
            id: 2,
            name: "Quần jeans slim fit",
            price: 450000,
            quantity: 1,
            image: "https://vn-test-11.slatic.net/p/9bb2a97169e7673623ade19ccafeaff3.jpg",
          },
        ],
        subtotal: 950000,
        shippingFee: 30000,
        discount: 0,
        totalAmount: 980000,
      },
      {
        id: 1002,
        customerName: "Trần Thị Bình",
        customerEmail: "tranthib@example.com",
        customerPhone: "0911234567",
        orderDate: "2023-10-14",
        status: "processing",
        isPaid: true,
        shippingAddress: "456 Lê Lợi, Quận 5, TP. Hồ Chí Minh",
        items: [
          {
            id: 3,
            name: "Áo sơ mi Oxford",
            price: 350000,
            quantity: 1,
            image: "https://product.hstatic.net/1000026602/product/dsc07958_d95ce0e4b1bd4bfc9b15636103f11454.jpg",
          },
        ],
        subtotal: 350000,
        shippingFee: 30000,
        discount: 0,
        totalAmount: 380000,
      },
      {
        id: 1003,
        customerName: "Lê Văn Cường",
        customerEmail: "levanc@example.com",
        customerPhone: "0921234567",
        orderDate: "2023-10-10",
        status: "delivered",
        isPaid: true,
        shippingAddress: "789 Điện Biên Phủ, Quận 3, TP. Hồ Chí Minh",
        items: [
          {
            id: 4,
            name: "Áo khoác bomber",
            price: 650000,
            quantity: 1,
            image: "https://product.hstatic.net/1000360022/product/untitled-1__2__218c890a6c9c406a966b4ab805530b28.jpg",
          },
          {
            id: 1,
            name: "Áo thun Unisex Cotton",
            price: 250000,
            quantity: 3,
            image: "https://dosi-in.com/images/detailed/41/lnc_tr%C6%A1n_3.png",
          },
        ],
        subtotal: 1400000,
        shippingFee: 0, // Miễn phí giao hàng
        discount: 100000, // Đã áp dụng giảm giá
        totalAmount: 1300000,
      },
    ];

    const fetchOrders = async () => {
      try {
        loading.value = true;
        // Uncomment when API is ready
        // const response = await axios.get('/api/orders');
        // orders.value = response.data;

        // For development
        setTimeout(() => {
          orders.value = sampleOrders;
          loading.value = false;
        }, 500);
      } catch (err) {
        error.value = "Không thể tải dữ liệu đơn hàng: " + err.message;
        loading.value = false;
      }
    };

    const filteredOrders = computed(() => {
      let result = orders.value;

      // Filter by search term
      if (searchTerm.value) {
        const term = searchTerm.value.toLowerCase();
        result = result.filter(
          (order) =>
            order.customerName.toLowerCase().includes(term) ||
            order.id.toString().includes(term)
        );
      }

      // Filter by status
      if (statusFilter.value) {
        result = result.filter((order) => order.status === statusFilter.value);
      }

      // Filter by date range
      if (startDate.value && endDate.value) {
        const start = new Date(startDate.value);
        const end = new Date(endDate.value);
        result = result.filter((order) => {
          const orderDate = new Date(order.orderDate);
          return orderDate >= start && orderDate <= end;
        });
      }

      return result;
    });

    const viewOrderDetails = (order) => {
      currentOrder.value = { ...order };
      newStatus.value = order.status;
      const modal = new bootstrap.Modal(
        document.getElementById("orderDetailsModal")
      );
      modal.show();
    };

    const updateStatus = (order) => {
      currentOrder.value = { ...order };
      newStatus.value = order.status;
      const modal = new bootstrap.Modal(
        document.getElementById("orderDetailsModal")
      );
      modal.show();
    };

    const confirmCancelOrder = (order) => {
      currentOrder.value = { ...order };
      const modal = new bootstrap.Modal(
        document.getElementById("cancelOrderModal")
      );
      modal.show();
    };

    const saveStatus = async () => {
      try {
        // Uncomment when API is ready
        // await axios.put(`/api/orders/${currentOrder.value.id}/status`, { status: newStatus.value });

        // For development
        const index = orders.value.findIndex(
          (o) => o.id === currentOrder.value.id
        );
        if (index !== -1) {
          orders.value[index].status = newStatus.value;
        }

        // Close the modal
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("orderDetailsModal")
        );
        modal.hide();
      } catch (err) {
        error.value = "Không thể cập nhật trạng thái đơn hàng: " + err.message;
      }
    };

    const cancelOrder = async () => {
      try {
        // Uncomment when API is ready
        // await axios.put(`/api/orders/${currentOrder.value.id}/cancel`);

        // For development
        const index = orders.value.findIndex(
          (o) => o.id === currentOrder.value.id
        );
        if (index !== -1) {
          orders.value[index].status = "cancelled";
        }

        // Close the modal
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("cancelOrderModal")
        );
        modal.hide();
      } catch (err) {
        error.value = "Không thể hủy đơn hàng: " + err.message;
      }
    };

    const applyFilters = () => {
      // This function is called when the filter button is clicked
      console.log("Applying filters:", {
        startDate: startDate.value,
        endDate: endDate.value,
      });
    };

    const formatDate = (dateString) => {
      const options = { year: "numeric", month: "2-digit", day: "2-digit" };
      return new Date(dateString).toLocaleDateString("vi-VN", options);
    };

    const formatCurrency = (value) => {
      return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
      }).format(value);
    };

    const getStatusClass = (status) => {
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

    const getStatusText = (status) => {
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
      fetchOrders();
    });

    return {
      orders,
      loading,
      error,
      searchTerm,
      statusFilter,
      startDate,
      endDate,
      currentOrder,
      newStatus,
      filteredOrders,
      viewOrderDetails,
      updateStatus,
      confirmCancelOrder,
      saveStatus,
      cancelOrder,
      applyFilters,
      formatDate,
      formatCurrency,
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

.filter-container {
  min-width: 180px;
}

.content-container {
  background-color: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.order-filters {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
}

.date-range {
  display: flex;
  gap: 15px;
  align-items: flex-end;
  flex-wrap: wrap;
}

.date-input {
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

.order-row {
  cursor: pointer;
}

.order-row:hover {
  background-color: #f9f9f9;
}

.action-buttons {
  white-space: nowrap;
}

.product-cell {
  display: flex;
  align-items: center;
}

.product-thumbnail {
  width: 50px;
  height: 50px;
  object-fit: cover;
  border-radius: 4px;
}

.status-update {
  display: flex;
  align-items: center;
  margin-right: auto;
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: stretch;
  }

  .header-actions {
    margin-top: 10px;
  }

  .date-range {
    flex-direction: column;
    align-items: stretch;
  }
}
</style>
