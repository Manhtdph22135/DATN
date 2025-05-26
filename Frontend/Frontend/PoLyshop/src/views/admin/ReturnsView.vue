<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý đơn trả hàng</h2>
      <div class="header-actions">
        <div class="search-container">
          <input
            type="text"
            class="search-input"
            placeholder="Tìm kiếm đơn trả hàng..."
            v-model="searchTerm"
          />
          <i class="bi bi-search search-icon"></i>
        </div>
        <div class="filter-container">
          <select v-model="statusFilter" class="form-select">
            <option value="">Tất cả trạng thái</option>
            <option value="pending">Chờ xác nhận</option>
            <option value="approved">Đã chấp nhận</option>
            <option value="rejected">Đã từ chối</option>
            <option value="completed">Đã hoàn thành</option>
          </select>
        </div>
      </div>
    </div>

    <div class="content-container">
      <div class="return-filters mb-4">
        <div class="filter-group">
          <div class="reason-filter">
            <label>Lý do:</label>
            <select v-model="reasonFilter" class="form-select">
              <option value="">Tất cả lý do</option>
              <option value="damaged">Hàng bị hỏng</option>
              <option value="wrong_item">Hàng không đúng mẫu</option>
              <option value="size_issue">Sai kích cỡ</option>
              <option value="quality_issue">Chất lượng kém</option>
              <option value="other">Lý do khác</option>
            </select>
          </div>
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
      </div>

      <div v-if="loading" class="text-center p-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Đang tải...</span>
        </div>
      </div>

      <div v-else-if="error" class="alert alert-danger">
        {{ error }}
      </div>

      <div v-else-if="filteredReturns.length === 0" class="text-center p-5">
        <p>Không tìm thấy đơn trả hàng nào.</p>
      </div>

      <div v-else class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>Mã trả</th>
              <th>Đơn hàng</th>
              <th>Khách hàng</th>
              <th>Ngày yêu cầu</th>
              <th>Lý do</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="returnItem in filteredReturns" :key="returnItem.id">
              <td>#{{ returnItem.id }}</td>
              <td>#{{ returnItem.orderId }}</td>
              <td>{{ returnItem.customerName }}</td>
              <td>{{ formatDate(returnItem.requestDate) }}</td>
              <td>{{ getReasonText(returnItem.reason) }}</td>
              <td>
                <span class="badge" :class="getStatusClass(returnItem.status)">
                  {{ getStatusText(returnItem.status) }}
                </span>
              </td>
              <td class="action-buttons">
                <button
                  class="btn btn-sm btn-info me-1"
                  @click="viewReturnDetails(returnItem)"
                >
                  <i class="bi bi-eye"></i>
                </button>
                <button
                  class="btn btn-sm btn-success me-1"
                  @click="approveReturn(returnItem)"
                  v-if="returnItem.status === 'pending'"
                >
                  <i class="bi bi-check-circle"></i>
                </button>
                <button
                  class="btn btn-sm btn-danger"
                  @click="rejectReturn(returnItem)"
                  v-if="returnItem.status === 'pending'"
                >
                  <i class="bi bi-x-circle"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Return Details Modal -->
    <div
      class="modal fade"
      id="returnDetailsModal"
      tabindex="-1"
      aria-hidden="true"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              Chi tiết đơn trả hàng #{{ currentReturn.id }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <div class="row mb-4">
              <div class="col-md-6">
                <h6>Thông tin khách hàng</h6>
                <p><strong>Tên:</strong> {{ currentReturn.customerName }}</p>
                <p><strong>Email:</strong> {{ currentReturn.customerEmail }}</p>
                <p>
                  <strong>Số điện thoại:</strong>
                  {{ currentReturn.customerPhone }}
                </p>
              </div>
              <div class="col-md-6">
                <h6>Thông tin đơn trả</h6>
                <p>
                  <strong>Ngày yêu cầu:</strong>
                  {{ formatDate(currentReturn.requestDate) }}
                </p>
                <p>
                  <strong>Đơn hàng gốc:</strong>
                  #{{ currentReturn.orderId }}
                </p>
                <p>
                  <strong>Trạng thái:</strong>
                  <span
                    class="badge"
                    :class="getStatusClass(currentReturn.status)"
                  >
                    {{ getStatusText(currentReturn.status) }}
                  </span>
                </p>
              </div>
            </div>

            <div class="return-reason mb-4">
              <h6>Lý do trả hàng</h6>
              <div class="reason-box">
                <strong>{{ getReasonText(currentReturn.reason) }}:</strong>
                {{ currentReturn.reasonDetail }}
              </div>
            </div>

            <h6>Sản phẩm trả lại</h6>
            <div class="table-responsive">
              <table class="table table-striped">
                <thead>
                  <tr>
                    <th>Sản phẩm</th>
                    <th>Giá</th>
                    <th>Số lượng</th>
                    <th>Trạng thái</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in currentReturn.items" :key="item.id">
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
                    <td>
                      <span
                        class="badge"
                        :class="
                          item.condition === 'good'
                            ? 'bg-success'
                            : item.condition === 'damaged'
                            ? 'bg-danger'
                            : 'bg-warning'
                        "
                      >
                        {{
                          item.condition === "good"
                            ? "Tốt"
                            : item.condition === "damaged"
                            ? "Hư hỏng"
                            : "Kiểm tra"
                        }}
                      </span>
                    </td>
                  </tr>
                </tbody>
                <tfoot>
                  <tr>
                    <td colspan="3" class="text-end">
                      <strong>Tổng tiền hoàn trả:</strong>
                    </td>
                    <td>{{ formatCurrency(currentReturn.refundAmount) }}</td>
                  </tr>
                </tfoot>
              </table>
            </div>

            <div class="mb-3" v-if="currentReturn.status === 'pending'">
              <h6>Ghi chú xử lý</h6>
              <textarea
                v-model="processNote"
                rows="3"
                class="form-control"
                placeholder="Nhập ghi chú xử lý..."
              ></textarea>
            </div>

            <div class="mb-3" v-if="currentReturn.adminNote">
              <h6>Ghi chú từ quản trị viên</h6>
              <div class="note-box">
                {{ currentReturn.adminNote }}
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <div class="me-auto" v-if="currentReturn.status === 'pending'">
              <button
                class="btn btn-success me-2"
                @click="confirmProcess('approved')"
              >
                Chấp nhận trả hàng
              </button>
              <button
                class="btn btn-danger"
                @click="confirmProcess('rejected')"
              >
                Từ chối trả hàng
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

    <!-- Confirmation Modal -->
    <div class="modal fade" id="confirmModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              Xác nhận
              {{ processingAction === "approved" ? "chấp nhận" : "từ chối" }}
              đơn trả hàng
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <p>
              Bạn có chắc chắn muốn
              {{ processingAction === "approved" ? "chấp nhận" : "từ chối" }}
              đơn trả hàng #{{ currentReturn.id }}?
            </p>
            <p v-if="!processNote" class="text-warning">
              <i class="bi bi-exclamation-triangle"></i> Bạn chưa nhập ghi chú
              xử lý
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
              :class="
                processingAction === 'approved'
                  ? 'btn btn-success'
                  : 'btn btn-danger'
              "
              @click="processReturn"
            >
              {{ processingAction === "approved" ? "Chấp nhận" : "Từ chối" }}
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

export default {
  name: "ReturnsView",
  setup() {
    const returns = ref([]);
    const loading = ref(true);
    const error = ref(null);
    const searchTerm = ref("");
    const statusFilter = ref("");
    const reasonFilter = ref("");
    const startDate = ref("");
    const endDate = ref("");
    const currentReturn = ref({});
    const processNote = ref("");
    const processingAction = ref("");

    // Sample data
    const sampleReturns = [
      {
        id: 101,
        orderId: 1001,
        customerName: "Nguyễn Văn A",
        customerEmail: "nguyenvana@example.com",
        customerPhone: "0901234567",
        requestDate: "2023-10-18",
        status: "pending",
        reason: "damaged",
        reasonDetail: "Sản phẩm bị rách ở phần tay áo phải khi mở hộp.",
        refundAmount: 250000,
        items: [
          {
            id: 1,
            name: "Áo thun Unisex Cotton",
            price: 250000,
            quantity: 1,
            condition: "damaged",
            image: "https://placehold.co/100x100",
          },
        ],
      },
      {
        id: 102,
        orderId: 1002,
        customerName: "Trần Thị B",
        customerEmail: "tranthib@example.com",
        customerPhone: "0911234567",
        requestDate: "2023-10-15",
        status: "approved",
        reason: "wrong_item",
        reasonDetail: "Nhận được áo màu xanh thay vì màu đen như đã đặt.",
        adminNote:
          "Kiểm tra kho hàng, đúng là đã gửi nhầm màu. Chấp nhận đổi hàng.",
        refundAmount: 350000,
        items: [
          {
            id: 3,
            name: "Áo sơ mi Oxford",
            price: 350000,
            quantity: 1,
            condition: "good",
            image: "https://placehold.co/100x100",
          },
        ],
      },
      {
        id: 103,
        orderId: 1003,
        customerName: "Lê Văn C",
        customerEmail: "levanc@example.com",
        customerPhone: "0921234567",
        requestDate: "2023-10-12",
        status: "completed",
        reason: "size_issue",
        reasonDetail: "Size quá nhỏ, cần đổi size lớn hơn.",
        adminNote: "Đã nhận lại hàng và gửi size mới theo yêu cầu.",
        refundAmount: 0, // No refund because this is an exchange
        items: [
          {
            id: 4,
            name: "Áo khoác Bomber",
            price: 650000,
            quantity: 1,
            condition: "good",
            image: "https://placehold.co/100x100",
          },
        ],
      },
      {
        id: 104,
        orderId: 1004,
        customerName: "Phạm Thị D",
        customerEmail: "phamthid@example.com",
        customerPhone: "0931234567",
        requestDate: "2023-10-10",
        status: "rejected",
        reason: "other",
        reasonDetail: "Không thích sản phẩm, muốn đổi mẫu khác.",
        adminNote: "Từ chối yêu cầu do không phù hợp với chính sách đổi trả.",
        refundAmount: 0,
        items: [
          {
            id: 2,
            name: "Quần Jeans Slim Fit",
            price: 450000,
            quantity: 1,
            condition: "good",
            image: "https://placehold.co/100x100",
          },
        ],
      },
    ];

    const fetchReturns = async () => {
      try {
        loading.value = true;
        // Uncomment when API is ready
        // const response = await axios.get('/api/returns');
        // returns.value = response.data;

        // For development
        setTimeout(() => {
          returns.value = sampleReturns;
          loading.value = false;
        }, 500);
      } catch (err) {
        error.value = "Không thể tải dữ liệu đơn trả hàng: " + err.message;
        loading.value = false;
      }
    };

    const filteredReturns = computed(() => {
      let result = returns.value;

      // Filter by search term
      if (searchTerm.value) {
        const term = searchTerm.value.toLowerCase();
        result = result.filter(
          (returnItem) =>
            returnItem.customerName.toLowerCase().includes(term) ||
            returnItem.id.toString().includes(term) ||
            returnItem.orderId.toString().includes(term)
        );
      }

      // Filter by status
      if (statusFilter.value) {
        result = result.filter(
          (returnItem) => returnItem.status === statusFilter.value
        );
      }

      // Filter by reason
      if (reasonFilter.value) {
        result = result.filter(
          (returnItem) => returnItem.reason === reasonFilter.value
        );
      }

      // Filter by date range
      if (startDate.value && endDate.value) {
        const start = new Date(startDate.value);
        const end = new Date(endDate.value);
        result = result.filter((returnItem) => {
          const requestDate = new Date(returnItem.requestDate);
          return requestDate >= start && requestDate <= end;
        });
      }

      return result;
    });

    const viewReturnDetails = (returnItem) => {
      currentReturn.value = { ...returnItem };
      processNote.value = "";
      const modal = new bootstrap.Modal(
        document.getElementById("returnDetailsModal")
      );
      modal.show();
    };

    const approveReturn = (returnItem) => {
      currentReturn.value = { ...returnItem };
      processNote.value = "";
      const modal = new bootstrap.Modal(
        document.getElementById("returnDetailsModal")
      );
      modal.show();
    };

    const rejectReturn = (returnItem) => {
      currentReturn.value = { ...returnItem };
      processNote.value = "";
      const modal = new bootstrap.Modal(
        document.getElementById("returnDetailsModal")
      );
      modal.show();
    };

    const confirmProcess = (action) => {
      processingAction.value = action;
      const detailsModal = bootstrap.Modal.getInstance(
        document.getElementById("returnDetailsModal")
      );
      detailsModal.hide();

      setTimeout(() => {
        const confirmModal = new bootstrap.Modal(
          document.getElementById("confirmModal")
        );
        confirmModal.show();
      }, 500);
    };

    const processReturn = async () => {
      try {
        // Uncomment when API is ready
        // await axios.put(`/api/returns/${currentReturn.value.id}/process`, {
        //   status: processingAction.value,
        //   adminNote: processNote.value
        // });

        // For development
        const index = returns.value.findIndex(
          (r) => r.id === currentReturn.value.id
        );
        if (index !== -1) {
          returns.value[index].status = processingAction.value;
          returns.value[index].adminNote = processNote.value;
        }

        // Close the modal
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("confirmModal")
        );
        modal.hide();
      } catch (err) {
        error.value = "Không thể xử lý đơn trả hàng: " + err.message;
      }
    };

    const applyFilters = () => {
      // This function is called when the filter button is clicked
      console.log("Applying filters:", {
        startDate: startDate.value,
        endDate: endDate.value,
        reason: reasonFilter.value,
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
        case "approved":
          return "bg-primary";
        case "rejected":
          return "bg-danger";
        case "completed":
          return "bg-success";
        default:
          return "bg-secondary";
      }
    };

    const getStatusText = (status) => {
      switch (status) {
        case "pending":
          return "Chờ xác nhận";
        case "approved":
          return "Đã chấp nhận";
        case "rejected":
          return "Đã từ chối";
        case "completed":
          return "Đã hoàn thành";
        default:
          return "Không xác định";
      }
    };

    const getReasonText = (reason) => {
      switch (reason) {
        case "damaged":
          return "Hàng bị hỏng";
        case "wrong_item":
          return "Hàng không đúng mẫu";
        case "size_issue":
          return "Sai kích cỡ";
        case "quality_issue":
          return "Chất lượng kém";
        case "other":
          return "Lý do khác";
        default:
          return "Không xác định";
      }
    };

    onMounted(() => {
      fetchReturns();

      // Check if Bootstrap JS is loaded for modals
      if (typeof bootstrap === "undefined") {
        console.error("Bootstrap JS is not loaded. Modals will not work.");
      }
    });

    return {
      returns,
      loading,
      error,
      searchTerm,
      statusFilter,
      reasonFilter,
      startDate,
      endDate,
      currentReturn,
      processNote,
      processingAction,
      filteredReturns,
      viewReturnDetails,
      approveReturn,
      rejectReturn,
      confirmProcess,
      processReturn,
      applyFilters,
      formatDate,
      formatCurrency,
      getStatusClass,
      getStatusText,
      getReasonText,
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

.return-filters {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  gap: 15px;
  align-items: flex-end;
  flex-wrap: wrap;
  width: 100%;
  justify-content: space-between;
}

.reason-filter {
  min-width: 220px;
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

.admin-table tr:hover {
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

.reason-box,
.note-box {
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 5px;
  border: 1px solid #dee2e6;
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

  .date-range {
    flex-direction: column;
    align-items: stretch;
  }
}
</style>
