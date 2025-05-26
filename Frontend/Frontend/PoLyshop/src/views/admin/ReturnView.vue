<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý trả hàng</h2>
      <div class="header-actions">
        <div class="search-container">
          <input
            type="text"
            class="search-input"
            placeholder="Tìm kiếm theo mã đơn hàng hoặc khách hàng..."
            v-model="searchTerm"
          />
          <i class="bi bi-search search-icon"></i>
        </div>
        <button class="btn btn-success" @click="openAddModal">
          <i class="bi bi-plus"></i> Thêm đơn trả hàng
        </button>
      </div>
    </div>

    <div class="content-container">
      <div class="return-filters mb-4">
        <div class="filter-group">
          <div class="status-filter">
            <label>Trạng thái:</label>
            <select v-model="statusFilter" class="form-select">
              <option value="">Tất cả trạng thái</option>
              <option value="pending">Chờ xử lý</option>
              <option value="processing">Đang xử lý</option>
              <option value="approved">Đã chấp nhận</option>
              <option value="rejected">Đã từ chối</option>
              <option value="completed">Hoàn thành</option>
            </select>
          </div>
          <div class="date-filter">
            <label>Thời gian:</label>
            <select v-model="dateFilter" class="form-select">
              <option value="">Tất cả thời gian</option>
              <option value="today">Hôm nay</option>
              <option value="yesterday">Hôm qua</option>
              <option value="thisWeek">Tuần này</option>
              <option value="thisMonth">Tháng này</option>
              <option value="lastMonth">Tháng trước</option>
            </select>
          </div>
          <div class="reason-filter">
            <label>Lý do trả hàng:</label>
            <select v-model="reasonFilter" class="form-select">
              <option value="">Tất cả lý do</option>
              <option value="damaged">Hàng bị hư hỏng</option>
              <option value="wrong-item">Giao nhầm hàng</option>
              <option value="size-issue">Vấn đề về kích thước</option>
              <option value="quality-issue">Chất lượng không đạt</option>
              <option value="other">Lý do khác</option>
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

      <div v-else-if="filteredReturns.length === 0" class="text-center p-5">
        <p>Không tìm thấy đơn trả hàng nào.</p>
      </div>

      <div v-else class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>Mã đơn hàng</th>
              <th>Khách hàng</th>
              <th>Ngày yêu cầu</th>
              <th>Sản phẩm</th>
              <th>Lý do trả</th>
              <th>Số lượng</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="returnOrder in filteredReturns" :key="returnOrder.id">
              <td>
                <span class="order-id">#{{ returnOrder.orderId }}</span>
              </td>
              <td>{{ returnOrder.customerName }}</td>
              <td>{{ formatDate(returnOrder.requestDate) }}</td>
              <td>
                <div class="product-cell">
                  <img
                    v-if="returnOrder.productImage"
                    :src="returnOrder.productImage"
                    :alt="returnOrder.productName"
                    class="product-thumbnail"
                  />
                  <div>
                    <div class="product-name">
                      {{ returnOrder.productName }}
                    </div>
                    <div class="product-variant text-muted">
                      {{ returnOrder.productVariant }}
                    </div>
                  </div>
                </div>
              </td>
              <td>{{ getReasonText(returnOrder.reason) }}</td>
              <td>{{ returnOrder.quantity }}</td>
              <td>
                <span class="badge" :class="getStatusClass(returnOrder.status)">
                  {{ getStatusText(returnOrder.status) }}
                </span>
              </td>
              <td class="action-buttons">
                <button
                  class="btn btn-sm btn-info me-1"
                  @click="viewReturn(returnOrder)"
                  title="Xem chi tiết"
                >
                  <i class="bi bi-eye"></i>
                </button>
                <button
                  class="btn btn-sm btn-primary me-1"
                  @click="processReturn(returnOrder)"
                  title="Xử lý trả hàng"
                >
                  <i class="bi bi-gear"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add Return Modal -->
    <div class="modal fade" id="returnModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{
                isEditing ? "Cập nhật đơn trả hàng" : "Thêm đơn trả hàng mới"
              }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveReturn">
              <div class="row mb-3">
                <div class="col-md-6">
                  <label class="form-label">Mã đơn hàng</label>
                  <input
                    type="text"
                    class="form-control"
                    v-model="currentReturn.orderId"
                    required
                  />
                </div>
                <div class="col-md-6">
                  <label class="form-label">Khách hàng</label>
                  <input
                    type="text"
                    class="form-control"
                    v-model="currentReturn.customerName"
                    required
                  />
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label">Sản phẩm</label>
                <input
                  type="text"
                  class="form-control"
                  v-model="currentReturn.productName"
                  required
                />
              </div>
              <div class="row mb-3">
                <div class="col-md-6">
                  <label class="form-label"
                    >Chi tiết sản phẩm (màu sắc, kích cỡ)</label
                  >
                  <input
                    type="text"
                    class="form-control"
                    v-model="currentReturn.productVariant"
                  />
                </div>
                <div class="col-md-6">
                  <label class="form-label">Số lượng</label>
                  <input
                    type="number"
                    class="form-control"
                    v-model="currentReturn.quantity"
                    min="1"
                    required
                  />
                </div>
              </div>
              <div class="row mb-3">
                <div class="col-md-6">
                  <label class="form-label">Lý do trả hàng</label>
                  <select
                    class="form-select"
                    v-model="currentReturn.reason"
                    required
                  >
                    <option value="damaged">Hàng bị hư hỏng</option>
                    <option value="wrong-item">Giao nhầm hàng</option>
                    <option value="size-issue">Vấn đề về kích thước</option>
                    <option value="quality-issue">Chất lượng không đạt</option>
                    <option value="other">Lý do khác</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Trạng thái</label>
                  <select class="form-select" v-model="currentReturn.status">
                    <option value="pending">Chờ xử lý</option>
                    <option value="processing">Đang xử lý</option>
                    <option value="approved">Đã chấp nhận</option>
                    <option value="rejected">Đã từ chối</option>
                    <option value="completed">Hoàn thành</option>
                  </select>
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label">Hình ảnh sản phẩm</label>
                <input
                  type="text"
                  class="form-control"
                  placeholder="URL hình ảnh"
                  v-model="currentReturn.productImage"
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Mô tả chi tiết</label>
                <textarea
                  class="form-control"
                  v-model="currentReturn.description"
                  rows="3"
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

    <!-- View Return Modal -->
    <div
      class="modal fade"
      id="returnDetailModal"
      tabindex="-1"
      aria-hidden="true"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Chi tiết đơn trả hàng</h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <div class="return-details">
              <div class="row mb-4">
                <div class="col-md-6">
                  <div class="mb-3">
                    <strong>Mã đơn hàng:</strong> #{{ currentReturn.orderId }}
                  </div>
                  <div class="mb-3">
                    <strong>Khách hàng:</strong>
                    {{ currentReturn.customerName }}
                  </div>
                  <div class="mb-3">
                    <strong>Thời gian yêu cầu:</strong>
                    {{ formatDate(currentReturn.requestDate) }}
                  </div>
                  <div>
                    <strong>Trạng thái:</strong>
                    <span
                      class="badge ms-2"
                      :class="getStatusClass(currentReturn.status)"
                    >
                      {{ getStatusText(currentReturn.status) }}
                    </span>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="mb-3">
                    <strong>Lý do trả hàng:</strong>
                    {{ getReasonText(currentReturn.reason) }}
                  </div>
                  <div class="mb-3">
                    <strong>Số lượng:</strong> {{ currentReturn.quantity }}
                  </div>
                  <div v-if="currentReturn.refundAmount" class="mb-3">
                    <strong>Hoàn tiền:</strong>
                    {{ formatCurrency(currentReturn.refundAmount) }}
                  </div>
                  <div v-if="currentReturn.handlingStaff" class="mb-3">
                    <strong>Nhân viên xử lý:</strong>
                    {{ currentReturn.handlingStaff }}
                  </div>
                </div>
              </div>

              <hr />

              <div class="row">
                <div class="col-md-4">
                  <div class="product-image mb-3">
                    <img
                      :src="
                        currentReturn.productImage ||
                        'https://placehold.co/400x300?text=No+Image'
                      "
                      alt="Sản phẩm"
                      class="img-fluid rounded"
                    />
                  </div>
                </div>
                <div class="col-md-8">
                  <div class="mb-3">
                    <strong>Sản phẩm:</strong> {{ currentReturn.productName }}
                  </div>
                  <div class="mb-3">
                    <strong>Chi tiết sản phẩm:</strong>
                    {{ currentReturn.productVariant }}
                  </div>
                  <div class="mb-3">
                    <strong>Mô tả chi tiết:</strong>
                    <p class="description mt-2">
                      {{
                        currentReturn.description || "Không có mô tả chi tiết"
                      }}
                    </p>
                  </div>
                </div>
              </div>

              <hr />

              <div v-if="currentReturn.notes">
                <h6 class="mb-3">Ghi chú xử lý</h6>
                <div class="card bg-light p-3">
                  <p class="mb-0">{{ currentReturn.notes }}</p>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button
              v-if="['pending', 'processing'].includes(currentReturn.status)"
              class="btn btn-primary me-2"
              @click="processReturn(currentReturn)"
            >
              <i class="bi bi-gear"></i> Xử lý đơn hàng
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

    <!-- Process Return Modal -->
    <div
      class="modal fade"
      id="processReturnModal"
      tabindex="-1"
      aria-hidden="true"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              Xử lý đơn trả hàng #{{ currentReturn.orderId }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="submitProcessReturn">
              <div class="mb-3">
                <label class="form-label">Trạng thái</label>
                <select
                  class="form-select"
                  v-model="currentReturn.status"
                  required
                >
                  <option value="processing">Đang xử lý</option>
                  <option value="approved">Chấp nhận</option>
                  <option value="rejected">Từ chối</option>
                  <option value="completed">Hoàn thành</option>
                </select>
              </div>
              <div class="mb-3">
                <label class="form-label">Số tiền hoàn trả (VNĐ)</label>
                <input
                  type="number"
                  class="form-control"
                  v-model="currentReturn.refundAmount"
                  min="0"
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Nhân viên xử lý</label>
                <input
                  type="text"
                  class="form-control"
                  v-model="currentReturn.handlingStaff"
                  required
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Ghi chú xử lý</label>
                <textarea
                  class="form-control"
                  v-model="currentReturn.notes"
                  rows="3"
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
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import bootstrap from "@/utils/bootstrapHelper";
import axios from "axios";

const returns = ref([]);
const loading = ref(true);
const error = ref(null);
const searchTerm = ref("");
const statusFilter = ref("");
const dateFilter = ref("");
const reasonFilter = ref("");
const isEditing = ref(false);
const currentReturn = ref({
  id: null,
  orderId: "",
  customerName: "",
  productName: "",
  productVariant: "",
  productImage: "",
  requestDate: "",
  reason: "damaged",
  quantity: 1,
  status: "pending",
  description: "",
  refundAmount: null,
  handlingStaff: "",
  notes: "",
});

// Sample data for development
const sampleReturns = [
  {
    id: 1,
    orderId: "10045",
    customerName: "Nguyễn Văn Nam",
    productName: "Áo Thun Nam Cổ Tròn",
    productVariant: "Màu: Đen, Size: L",
    productImage: "https://placehold.co/300x200?text=Black+Tshirt",
    requestDate: "2023-11-10T08:15:00",
    reason: "size-issue",
    quantity: 1,
    status: "completed",
    description: "Áo quá rộng so với size bình thường tôi mặc.",
    refundAmount: 250000,
    handlingStaff: "Trần Minh",
    notes: "Khách hàng đã gửi trả sản phẩm, đã hoàn tiền qua ví điện tử.",
  },
  {
    id: 2,
    orderId: "10089",
    customerName: "Trần Thị Hương",
    productName: "Váy Liền Thân Nữ",
    productVariant: "Màu: Hồng, Size: M",
    productImage: "https://placehold.co/300x200?text=Pink+Dress",
    requestDate: "2023-11-15T14:30:00",
    reason: "wrong-item",
    quantity: 1,
    status: "approved",
    description: "Tôi đặt váy màu đỏ nhưng nhận được màu hồng.",
    refundAmount: 450000,
    handlingStaff: "Nguyễn Thành",
    notes: "Xác nhận lỗi của shop. Đang chờ khách gửi trả hàng.",
  },
  {
    id: 3,
    orderId: "10102",
    customerName: "Lê Hoàng Long",
    productName: "Giày Thể Thao Nam",
    productVariant: "Màu: Trắng, Size: 42",
    productImage: "https://placehold.co/300x200?text=White+Sneakers",
    requestDate: "2023-11-18T09:45:00",
    reason: "damaged",
    quantity: 1,
    status: "processing",
    description: "Giày bị hư ở phần đế, có vẻ như bị lỗi sản xuất.",
    refundAmount: null,
    handlingStaff: "Nguyễn Thành",
    notes: "Đã yêu cầu khách gửi thêm hình ảnh chi tiết lỗi sản phẩm.",
  },
  {
    id: 4,
    orderId: "10120",
    customerName: "Phạm Thị Mai",
    productName: "Túi Xách Nữ",
    productVariant: "Màu: Nâu",
    productImage: "https://placehold.co/300x200?text=Brown+Handbag",
    requestDate: "2023-11-20T16:10:00",
    reason: "quality-issue",
    quantity: 1,
    status: "pending",
    description:
      "Chất liệu túi không giống như mô tả, trông rẻ tiền hơn nhiều.",
    refundAmount: null,
    handlingStaff: "",
    notes: "",
  },
  {
    id: 5,
    orderId: "10135",
    customerName: "Hoàng Minh Tuấn",
    productName: "Quần Jeans Nam",
    productVariant: "Màu: Xanh đậm, Size: 32",
    productImage: "https://placehold.co/300x200?text=Blue+Jeans",
    requestDate: "2023-11-22T10:20:00",
    reason: "other",
    quantity: 2,
    status: "rejected",
    description: "Tôi mua nhầm size, muốn đổi sang size 30.",
    refundAmount: null,
    handlingStaff: "Trần Minh",
    notes:
      "Từ chối do quá hạn đổi trả (30 ngày). Đã liên hệ khách hàng giải thích.",
  },
];

const fetchReturns = async () => {
  try {
    loading.value = true;

    // In development mode, use sample data
    setTimeout(() => {
      returns.value = sampleReturns;
      loading.value = false;
    }, 500);

    // NOTE: Real API implementation will be added later
    // Example of future implementation:
    // const response = await axios.get('/api/return-orders');
    // returns.value = response.data;
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
      (returnOrder) =>
        returnOrder.orderId.toString().includes(term) ||
        returnOrder.customerName.toLowerCase().includes(term) ||
        returnOrder.productName.toLowerCase().includes(term)
    );
  }

  // Filter by status
  if (statusFilter.value) {
    result = result.filter(
      (returnOrder) => returnOrder.status === statusFilter.value
    );
  }

  // Filter by reason
  if (reasonFilter.value) {
    result = result.filter(
      (returnOrder) => returnOrder.reason === reasonFilter.value
    );
  }

  // Filter by date
  if (dateFilter.value) {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const yesterday = new Date(today);
    yesterday.setDate(yesterday.getDate() - 1);

    const thisWeekStart = new Date(today);
    thisWeekStart.setDate(today.getDate() - today.getDay());

    const thisMonthStart = new Date(today.getFullYear(), today.getMonth(), 1);

    const lastMonthStart = new Date(
      today.getFullYear(),
      today.getMonth() - 1,
      1
    );
    const lastMonthEnd = new Date(today.getFullYear(), today.getMonth(), 0);

    result = result.filter((returnOrder) => {
      const date = new Date(returnOrder.requestDate);
      switch (dateFilter.value) {
        case "today":
          return date >= today;
        case "yesterday":
          return date >= yesterday && date < today;
        case "thisWeek":
          return date >= thisWeekStart;
        case "thisMonth":
          return date >= thisMonthStart;
        case "lastMonth":
          return date >= lastMonthStart && date <= lastMonthEnd;
        default:
          return true;
      }
    });
  }

  return result;
});

const openAddModal = () => {
  isEditing.value = false;
  currentReturn.value = {
    id: null,
    orderId: "",
    customerName: "",
    productName: "",
    productVariant: "",
    productImage: "",
    requestDate: new Date().toISOString(),
    reason: "damaged",
    quantity: 1,
    status: "pending",
    description: "",
    refundAmount: null,
    handlingStaff: "",
    notes: "",
  };
  const modal = new bootstrap.Modal(document.getElementById("returnModal"));
  modal.show();
};

const viewReturn = (returnOrder) => {
  currentReturn.value = { ...returnOrder };
  const modal = new bootstrap.Modal(
    document.getElementById("returnDetailModal")
  );
  modal.show();
};

const processReturn = (returnOrder) => {
  currentReturn.value = { ...returnOrder };

  // Close details modal if open
  const detailModal = bootstrap.Modal.getInstance(
    document.getElementById("returnDetailModal")
  );
  if (detailModal) {
    detailModal.hide();
  }

  // Show process modal
  setTimeout(() => {
    const modal = new bootstrap.Modal(
      document.getElementById("processReturnModal")
    );
    modal.show();
  }, 500);
};

const submitProcessReturn = async () => {
  try {
    // Uncomment when API is ready
    // await axios.put(`/api/return-orders/${currentReturn.value.id}/process`, currentReturn.value);

    // For development
    const index = returns.value.findIndex(
      (r) => r.id === currentReturn.value.id
    );
    if (index !== -1) {
      returns.value[index] = { ...currentReturn.value };
    }

    // Close the modal
    const modal = bootstrap.Modal.getInstance(
      document.getElementById("processReturnModal")
    );
    modal.hide();
  } catch (err) {
    error.value = "Không thể cập nhật trạng thái đơn trả hàng: " + err.message;
  }
};

const saveReturn = async () => {
  try {
    if (isEditing.value) {
      // Uncomment when API is ready
      // await axios.put(`/api/return-orders/${currentReturn.value.id}`, currentReturn.value);

      // For development
      const index = returns.value.findIndex(
        (r) => r.id === currentReturn.value.id
      );
      if (index !== -1) {
        returns.value[index] = { ...currentReturn.value };
      }
    } else {
      // Uncomment when API is ready
      // const response = await axios.post('/api/return-orders', currentReturn.value);
      // returns.value.push(response.data);

      // For development
      const newId = Math.max(0, ...returns.value.map((r) => r.id)) + 1;
      returns.value.push({
        ...currentReturn.value,
        id: newId,
      });
    }

    // Close the modal
    const modal = bootstrap.Modal.getInstance(
      document.getElementById("returnModal")
    );
    modal.hide();
  } catch (err) {
    error.value =
      `Không thể ${isEditing.value ? "cập nhật" : "thêm"} đơn trả hàng: ` +
      err.message;
  }
};

const formatDate = (dateString) => {
  if (!dateString) return "N/A";
  const options = {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  };
  return new Date(dateString).toLocaleDateString("vi-VN", options);
};

const formatCurrency = (value) => {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(value);
};

const getReasonText = (reason) => {
  const reasons = {
    damaged: "Hàng bị hư hỏng",
    "wrong-item": "Giao nhầm hàng",
    "size-issue": "Vấn đề về kích thước",
    "quality-issue": "Chất lượng không đạt",
    other: "Lý do khác",
  };
  return reasons[reason] || reason;
};

const getStatusClass = (status) => {
  switch (status) {
    case "pending":
      return "bg-warning";
    case "processing":
      return "bg-primary";
    case "approved":
      return "bg-info";
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
      return "Chờ xử lý";
    case "processing":
      return "Đang xử lý";
    case "approved":
      return "Đã chấp nhận";
    case "rejected":
      return "Đã từ chối";
    case "completed":
      return "Hoàn thành";
    default:
      return "Không xác định";
  }
};

onMounted(() => {
  fetchReturns();
});
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

.return-filters {
  display: flex;
  margin-bottom: 20px;
}

.filter-group {
  display: flex;
  gap: 15px;
  align-items: flex-end;
  flex-wrap: wrap;
}

.status-filter,
.date-filter,
.reason-filter {
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

.order-id {
  font-weight: 500;
}

.product-cell {
  display: flex;
  align-items: center;
}

.product-thumbnail {
  width: 40px;
  height: 40px;
  object-fit: cover;
  border-radius: 4px;
  margin-right: 10px;
}

.product-name {
  font-weight: 500;
  max-width: 180px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.product-variant {
  font-size: 0.85rem;
}

.action-buttons {
  white-space: nowrap;
}

.description {
  background-color: #f8f9fa;
  padding: 10px;
  border-radius: 5px;
  font-size: 0.9rem;
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
