<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý Hoá đơn</h2>

      <!-- Filter actions -->
      <div class="header-actions">
        <div class="search-container">
          <input
            type="text"
            class="search-input"
            placeholder="Tìm kiếm số hoá đơn, khách hàng..."
            v-model="searchTerm"
            @input="filterBills"
          />
          <div class="search-icon">
            <i class="bi bi-search"></i>
          </div>
        </div>

        <div class="date-filters">
          <div class="date-filter">
            <span>Từ Ngày:</span>
            <input
              type="date"
              class="form-control date-input"
              v-model="filters.fromDate"
              @change="filterBills"
            />
          </div>

          <div class="date-filter">
            <span>Đến Ngày:</span>
            <input
              type="date"
              class="form-control date-input"
              v-model="filters.toDate"
              @change="filterBills"
            />
          </div>
        </div>

        <div class="status-filter">
          <select
            class="form-select"
            v-model="filters.status"
            @change="filterBills"
          >
            <option value="all">Tất cả trạng thái</option>
            <option value="paid">Đã thanh toán</option>
            <option value="pending">Chờ thanh toán</option>
            <option value="cancelled">Đã hủy</option>
          </select>
        </div>
      </div>
    </div>

    <button class="filter-button" @click="resetFilters">Làm mới bộ lọc</button>

    <div class="my-4">
      <div v-if="loading" class="text-center py-3">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Đang tải...</span>
        </div>
      </div>

      <div
        v-else-if="filteredBills.length === 0"
        class="alert alert-info text-center"
      >
        <i class="bi bi-info-circle me-2"></i>
        Không tìm thấy hoá đơn nào phù hợp với bộ lọc
      </div>

      <table v-else class="admin-table">
        <thead>
          <tr>
            <th @click="sort('id')">
              ID
              <i class="bi" :class="getSortIconClass('id')"></i>
            </th>
            <th @click="sort('billId')">
              Mã Hoá đơn
              <i class="bi" :class="getSortIconClass('billId')"></i>
            </th>
            <th @click="sort('customerName')">
              Khách hàng
              <i class="bi" :class="getSortIconClass('customerName')"></i>
            </th>
            <th @click="sort('total')">
              Tổng tiền
              <i class="bi" :class="getSortIconClass('total')"></i>
            </th>
            <th @click="sort('paymentMethod')">
              Phương thức
              <i class="bi" :class="getSortIconClass('paymentMethod')"></i>
            </th>
            <th @click="sort('createdAt')">
              Ngày tạo
              <i class="bi" :class="getSortIconClass('createdAt')"></i>
            </th>
            <th @click="sort('status')">
              Trạng thái
              <i class="bi" :class="getSortIconClass('status')"></i>
            </th>
            <th>Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="bill in filteredBills" :key="bill.id">
            <td>{{ bill.id }}</td>
            <td>{{ bill.billId }}</td>
            <td>{{ bill.customerName }}</td>
            <td>{{ formatCurrency(bill.total) }}</td>
            <td>{{ getPaymentMethodText(bill.paymentMethod) }}</td>
            <td>{{ formatDate(bill.createdAt) }}</td>
            <td>
              <span class="status-badge" :class="getStatusClass(bill.status)">
                {{ getStatusText(bill.status) }}
              </span>
            </td>
            <td>
              <div class="action-buttons">
                <button
                  class="view-button"
                  title="Xem chi tiết"
                  @click="viewBill(bill)"
                >
                  <i class="bi bi-eye"></i>
                </button>
                <button class="edit-button" title="Sửa" @click="editBill(bill)">
                  <i class="bi bi-pencil"></i>
                </button>
                <button
                  class="delete-button"
                  title="Xóa"
                  @click="confirmDelete(bill)"
                >
                  <i class="bi bi-trash"></i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Pagination -->
      <div class="pagination-container">
        <button
          class="pagination-button"
          :disabled="currentPage === 1"
          @click="currentPage--"
        >
          <i class="bi bi-chevron-left"></i>
        </button>

        <span class="pagination-info">
          Trang {{ currentPage }} / {{ totalPages }}
        </span>

        <button
          class="pagination-button"
          :disabled="currentPage === totalPages"
          @click="currentPage++"
        >
          <i class="bi bi-chevron-right"></i>
        </button>
      </div>
    </div>

    <!-- Bill Detail Modal -->
    <div
      class="modal fade"
      id="billDetailModal"
      tabindex="-1"
      aria-hidden="true"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              Chi tiết hoá đơn #{{ selectedBill?.billId || "" }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body" v-if="selectedBill">
            <div class="bill-detail-header">
              <div class="row">
                <div class="col-md-6">
                  <div class="bill-info-group">
                    <h6>Thông tin khách hàng</h6>
                    <p>
                      <strong>Tên khách hàng:</strong>
                      {{ selectedBill.customerName }}
                    </p>
                    <p>
                      <strong>Số điện thoại:</strong>
                      {{ selectedBill.customerPhone }}
                    </p>
                    <p>
                      <strong>Email:</strong> {{ selectedBill.customerEmail }}
                    </p>
                    <p>
                      <strong>Địa chỉ:</strong>
                      {{ selectedBill.customerAddress }}
                    </p>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="bill-info-group">
                    <h6>Thông tin hoá đơn</h6>
                    <p>
                      <strong>Trạng thái:</strong>
                      <span
                        class="status-badge ms-2"
                        :class="getStatusClass(selectedBill.status)"
                      >
                        {{ getStatusText(selectedBill.status) }}
                      </span>
                    </p>
                    <p>
                      <strong>Phương thức thanh toán:</strong>
                      {{ getPaymentMethodText(selectedBill.paymentMethod) }}
                    </p>
                    <p>
                      <strong>Ngày tạo:</strong>
                      {{ formatDateTime(selectedBill.createdAt) }}
                    </p>
                    <p v-if="selectedBill.paymentDate">
                      <strong>Ngày thanh toán:</strong>
                      {{ formatDateTime(selectedBill.paymentDate) }}
                    </p>
                  </div>
                </div>
              </div>
            </div>

            <div class="bill-items mt-4">
              <h6>Chi tiết sản phẩm</h6>
              <table class="table table-bordered">
                <thead>
                  <tr>
                    <th>Sản phẩm</th>
                    <th>Đơn giá</th>
                    <th>Số lượng</th>
                    <th>Thành tiền</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, index) in selectedBill.items" :key="index">
                    <td>
                      <div class="product-item">
                        <img
                          :src="item.image"
                          alt="Product image"
                          class="product-thumbnail"
                        />
                        <div>
                          <div>{{ item.name }}</div>
                          <small class="text-muted">
                            {{ getItemVariantText(item) }}
                          </small>
                        </div>
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
                    <td>{{ formatCurrency(selectedBill.subtotal) }}</td>
                  </tr>
                  <tr v-if="selectedBill.discount > 0">
                    <td colspan="3" class="text-end">
                      <strong>Giảm giá:</strong>
                    </td>
                    <td>{{ formatCurrency(selectedBill.discount) }}</td>
                  </tr>
                  <tr v-if="selectedBill.tax > 0">
                    <td colspan="3" class="text-end">
                      <strong>Thuế (VAT 8%):</strong>
                    </td>
                    <td>{{ formatCurrency(selectedBill.tax) }}</td>
                  </tr>
                  <tr>
                    <td colspan="3" class="text-end">
                      <strong>Tổng thanh toán:</strong>
                    </td>
                    <td>
                      <strong>{{ formatCurrency(selectedBill.total) }}</strong>
                    </td>
                  </tr>
                </tfoot>
              </table>
            </div>

            <div class="bill-notes mt-3" v-if="selectedBill.note">
              <h6>Ghi chú đơn hàng</h6>
              <p class="bill-note">{{ selectedBill.note }}</p>
            </div>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              Đóng
            </button>
            <button type="button" class="btn btn-primary" @click="printBill">
              <i class="bi bi-printer me-1"></i> In hoá đơn
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Bill Modal -->
    <div class="modal fade" id="editBillModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              Cập nhật hoá đơn #{{ selectedBill?.billId || "" }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body" v-if="editingBill">
            <form @submit.prevent="saveBill">
              <div class="mb-3">
                <label class="form-label">Trạng thái</label>
                <select class="form-select" v-model="editingBill.status">
                  <option value="pending">Chờ thanh toán</option>
                  <option value="paid">Đã thanh toán</option>
                  <option value="cancelled">Đã hủy</option>
                </select>
              </div>

              <div class="mb-3">
                <label class="form-label">Phương thức thanh toán</label>
                <select class="form-select" v-model="editingBill.paymentMethod">
                  <option value="cash">Tiền mặt</option>
                  <option value="card">Thẻ ATM/Visa</option>
                  <option value="transfer">Chuyển khoản</option>
                  <option value="momo">Ví MoMo</option>
                  <option value="vnpay">VNPay</option>
                </select>
              </div>

              <div class="mb-3">
                <label class="form-label">Ghi chú</label>
                <textarea
                  class="form-control"
                  rows="3"
                  v-model="editingBill.note"
                ></textarea>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              Hủy
            </button>
            <button type="button" class="btn btn-primary" @click="saveBill">
              Lưu thay đổi
            </button>
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
              Bạn có chắc chắn muốn xóa hoá đơn #{{
                selectedBill?.billId || ""
              }}?
            </p>
            <p class="text-danger">Lưu ý: Hành động này không thể hoàn tác.</p>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              Hủy
            </button>
            <button type="button" class="btn btn-danger" @click="deleteBill">
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
import bootstrap from "@/utils/bootstrapHelper";

export default {
  name: "AdminBillView",
  setup() {
    // State
    const bills = ref([]);
    const loading = ref(true);
    const searchTerm = ref("");
    const sortColumn = ref("createdAt");
    const sortDirection = ref("desc");
    const currentPage = ref(1);
    const itemsPerPage = 10;
    const selectedBill = ref(null);
    const editingBill = ref(null);

    const filters = ref({
      fromDate: "",
      toDate: "",
      status: "all",
    });

    // Sample data for development
    const sampleBills = [
      {
        id: 1,
        billId: "HD00001",
        customerName: "Nguyễn Văn An",
        customerPhone: "0901234567",
        customerEmail: "nguyenvanan@gmail.com",
        customerAddress: "123 Nguyễn Huệ, Q1, TPHCM",
        subtotal: 750000,
        discount: 0,
        tax: 60000,
        total: 810000,
        paymentMethod: "cash",
        status: "paid",
        createdAt: "2023-10-15T09:30:00Z",
        paymentDate: "2023-10-15T09:35:00Z",
        note: "",
        items: [
          {
            id: 1,
            name: "Áo thun nam basic",
            price: 250000,
            quantity: 2,
            image: "https://dosi-in.com/images/detailed/41/lnc_tr%C6%A1n_3.png",
            color: "Đen",
            size: "L",
          },
          {
            id: 3,
            name: "Áo sơ mi Minimal",
            price: 250000,
            quantity: 1,
            image: "https://product.hstatic.net/1000360022/product/ao-so-mi-linen-nam-tay-ngan-minimal-collection-form-regular__7__96ae3e35f57049438841a8a8459c336a.jpg",
            color: "Trắng",
            size: "M",
          },
        ],
      },
      {
        id: 2,
        billId: "HD00002",
        customerName: "Trần Thị Bình",
        customerPhone: "0909876543",
        customerEmail: "tranthibinh@gmail.com",
        customerAddress: "456 Lê Lợi, Q3, TPHCM",
        subtotal: 450000,
        discount: 45000,
        tax: 36000,
        total: 441000,
        paymentMethod: "card",
        status: "paid",
        createdAt: "2023-10-16T14:20:00Z",
        paymentDate: "2023-10-16T14:25:00Z",
        note: "Khách hàng VIP",
        items: [
          {
            id: 2,
            name: "Quần jeans nam slim fit",
            price: 450000,
            quantity: 1,
            image: "https://vn-test-11.slatic.net/p/9bb2a97169e7673623ade19ccafeaff3.jpg",
            color: "Xanh đậm",
            size: "32",
          },
        ],
      },
      {
        id: 3,
        billId: "HD00003",
        customerName: "Lê Văn Cường",
        customerPhone: "0908765432",
        customerEmail: "levancuong@gmail.com",
        customerAddress: "789 Cách Mạng Tháng 8, Q10, TPHCM",
        subtotal: 550000,
        discount: 0,
        tax: 44000,
        total: 594000,
        paymentMethod: "momo",
        status: "pending",
        createdAt: "2023-10-17T16:45:00Z",
        paymentDate: null,
        note: "",
        items: [
          {
            id: 4,
            name: "Váy liền thân",
            price: 550000,
            quantity: 1,
            image: "https://file.hstatic.net/200000503583/file/mau-vay-lien-than-dep-cardina__20_.jpg_b05b34e6d8e540d693df036aca23fbbd.jpg",
            coimage: "https://placehold.co/80x80?text=Dress",lor: "Xanh lá",
            size: "S",
          },
        ],
      },
      {
        id: 4,
        billId: "HD00004",
        customerName: "Phạm Thị Dung",
        customerPhone: "0907654321",
        customerEmail: "phamthidung@gmail.com",
        customerAddress: "101 Võ Văn Tần, Q3, TPHCM",
        subtotal: 830000,
        discount: 0,
        tax: 66400,
        total: 896400,
        paymentMethod: "transfer",
        status: "cancelled",
        createdAt: "2023-10-18T11:15:00Z",
        paymentDate: null,
        note: "Khách hàng đổi hàng",
        items: [
          {
            id: 5,
            name: "Áo polo nam",
            price: 320000,
            quantity: 1,
            image: "https://4men.com.vn/thumbs/2022/08/ao-polo-slimfit-stripe-color-po083-mau-den-21295-p.jpg",
            color: "Trắng",
            size: "XL",
          },
          {
            id: 8,
            name: "Áo khoác denim",
            price: 510000,
            quantity: 1,
            image: "https://product.hstatic.net/1000360022/product/z3907027303949_e9fb89295d4938b8c37bec9aecb06c46_c26702be229b4874a4b17bfc711825b9.jpg",
            color: "Xanh nhạt",
            size: "L",
          },
        ],
      },
      {
        id: 5,
        billId: "HD00005",
        customerName: "Trịnh Văn Hùng",
        customerPhone: "0906543210",
        customerEmail: "trinhvanhung@gmail.com",
        customerAddress: "254 Nguyễn Trãi, Q1, TPHCM",
        subtotal: 900000,
        discount: 90000,
        tax: 72000,
        total: 882000,
        paymentMethod: "vnpay",
        status: "paid",
        createdAt: "2023-10-19T13:40:00Z",
        paymentDate: "2023-10-19T13:45:00Z",
        note: "",
        items: [
          {
            id: 2,
            name: "Quần jeans nam slim fit",
            price: 450000,
            quantity: 2,
            image: "https://vn-test-11.slatic.net/p/9bb2a97169e7673623ade19ccafeaff3.jpg",
            color: "Xanh đậm",
            size: "33",
          },
        ],
      },
    ];

    // Fetch bills
    const fetchBills = async () => {
      try {
        loading.value = true;

        // For development with sample data
        setTimeout(() => {
          bills.value = sampleBills;
          loading.value = false;
        }, 500);

        // For production with API
        // const response = await axios.get('/api/admin/bills')
        // bills.value = response.data
        // loading.value = false
      } catch (error) {
        console.error("Error fetching bills:", error);
        loading.value = false;
      }
    };

    // Filter bills
    const filterBills = () => {
      currentPage.value = 1; // Reset to first page on filter
    };

    const filteredBills = computed(() => {
      let result = [...bills.value];

      // Apply search filter
      if (searchTerm.value) {
        const term = searchTerm.value.toLowerCase();
        result = result.filter(
          (bill) =>
            bill.billId.toLowerCase().includes(term) ||
            bill.customerName.toLowerCase().includes(term) ||
            bill.customerPhone.includes(term)
        );
      }

      // Apply date filters
      if (filters.value.fromDate) {
        const fromDate = new Date(filters.value.fromDate);
        fromDate.setHours(0, 0, 0, 0);
        result = result.filter((bill) => new Date(bill.createdAt) >= fromDate);
      }

      if (filters.value.toDate) {
        const toDate = new Date(filters.value.toDate);
        toDate.setHours(23, 59, 59, 999);
        result = result.filter((bill) => new Date(bill.createdAt) <= toDate);
      }

      // Apply status filter
      if (filters.value.status !== "all") {
        result = result.filter((bill) => bill.status === filters.value.status);
      }

      // Apply sorting
      result.sort((a, b) => {
        let comparison = 0;

        if (
          sortColumn.value === "createdAt" ||
          sortColumn.value === "paymentDate"
        ) {
          comparison =
            new Date(a[sortColumn.value]) - new Date(b[sortColumn.value]);
        } else if (sortColumn.value === "total") {
          comparison = a[sortColumn.value] - b[sortColumn.value];
        } else {
          comparison = a[sortColumn.value]?.localeCompare(b[sortColumn.value]);
        }

        return sortDirection.value === "asc" ? comparison : -comparison;
      });

      return result;
    });

    // Pagination
    const paginatedBills = computed(() => {
      const start = (currentPage.value - 1) * itemsPerPage;
      const end = start + itemsPerPage;
      return filteredBills.value.slice(start, end);
    });

    const totalPages = computed(() => {
      return Math.ceil(filteredBills.value.length / itemsPerPage);
    });

    // Sorting
    const sort = (column) => {
      if (sortColumn.value === column) {
        sortDirection.value = sortDirection.value === "asc" ? "desc" : "asc";
      } else {
        sortColumn.value = column;
        sortDirection.value = "asc";
      }
    };

    const getSortIconClass = (column) => {
      if (sortColumn.value !== column) {
        return "bi-chevron-expand";
      }
      return sortDirection.value === "asc"
        ? "bi-chevron-up"
        : "bi-chevron-down";
    };

    // Reset filters
    const resetFilters = () => {
      searchTerm.value = "";
      filters.value = {
        fromDate: "",
        toDate: "",
        status: "all",
      };
      sortColumn.value = "createdAt";
      sortDirection.value = "desc";
      currentPage.value = 1;
    };

    // Format helpers
    const formatCurrency = (value) => {
      return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
        maximumFractionDigits: 0,
      }).format(value);
    };

    const formatDate = (dateString) => {
      const date = new Date(dateString);
      return new Intl.DateTimeFormat("vi-VN", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
      }).format(date);
    };

    const formatDateTime = (dateString) => {
      if (!dateString) return "";

      const date = new Date(dateString);
      return new Intl.DateTimeFormat("vi-VN", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
      }).format(date);
    };

    const getStatusText = (status) => {
      switch (status) {
        case "paid":
          return "Đã thanh toán";
        case "pending":
          return "Chờ thanh toán";
        case "cancelled":
          return "Đã hủy";
        default:
          return "Không xác định";
      }
    };

    const getStatusClass = (status) => {
      switch (status) {
        case "paid":
          return "status-success";
        case "pending":
          return "status-warning";
        case "cancelled":
          return "status-danger";
        default:
          return "status-secondary";
      }
    };

    const getPaymentMethodText = (method) => {
      switch (method) {
        case "cash":
          return "Tiền mặt";
        case "card":
          return "Thẻ ATM/Visa";
        case "transfer":
          return "Chuyển khoản";
        case "momo":
          return "Ví MoMo";
        case "vnpay":
          return "VNPay";
        default:
          return method;
      }
    };

    const getItemVariantText = (item) => {
      let variant = [];
      if (item.size) variant.push(`Size: ${item.size}`);
      if (item.color) variant.push(`Màu: ${item.color}`);
      return variant.join(" | ");
    };

    // CRUD operations
    const viewBill = (bill) => {
      selectedBill.value = bill;
      const modal = new bootstrap.Modal(
        document.getElementById("billDetailModal")
      );
      modal.show();
    };

    const editBill = (bill) => {
      selectedBill.value = bill;
      editingBill.value = { ...bill };
      const modal = new bootstrap.Modal(
        document.getElementById("editBillModal")
      );
      modal.show();
    };

    const confirmDelete = (bill) => {
      selectedBill.value = bill;
      const modal = new bootstrap.Modal(document.getElementById("deleteModal"));
      modal.show();
    };

    const saveBill = async () => {
      try {
        loading.value = true;

        // For development
        const index = bills.value.findIndex(
          (b) => b.id === editingBill.value.id
        );
        if (index !== -1) {
          bills.value[index] = { ...editingBill.value };

          // If status changed to paid, set payment date
          if (
            editingBill.value.status === "paid" &&
            !editingBill.value.paymentDate
          ) {
            bills.value[index].paymentDate = new Date().toISOString();
          }
        }

        // For production with API
        // await axios.put(`/api/admin/bills/${editingBill.value.id}`, editingBill.value)

        const modal = bootstrap.Modal.getInstance(
          document.getElementById("editBillModal")
        );
        modal.hide();

        loading.value = false;
      } catch (error) {
        console.error("Error saving bill:", error);
        loading.value = false;
      }
    };

    const deleteBill = async () => {
      try {
        loading.value = true;

        // For development
        const index = bills.value.findIndex(
          (b) => b.id === selectedBill.value.id
        );
        if (index !== -1) {
          bills.value.splice(index, 1);
        }

        // For production with API
        // await axios.delete(`/api/admin/bills/${selectedBill.value.id}`)

        const modal = bootstrap.Modal.getInstance(
          document.getElementById("deleteModal")
        );
        modal.hide();

        loading.value = false;
      } catch (error) {
        console.error("Error deleting bill:", error);
        loading.value = false;
      }
    };

    const printBill = () => {
      // In a real application, this would open a print dialog or generate a PDF
      console.log("Printing bill", selectedBill.value);
      alert("Chức năng in hoá đơn sẽ được triển khai trong tương lai.");
    };

    onMounted(() => {
      fetchBills();
    });

    return {
      bills,
      loading,
      searchTerm,
      filters,
      sortColumn,
      sortDirection,
      currentPage,
      filteredBills: paginatedBills,
      totalPages,
      selectedBill,
      editingBill,
      filterBills,
      sort,
      getSortIconClass,
      resetFilters,
      formatCurrency,
      formatDate,
      formatDateTime,
      getStatusText,
      getStatusClass,
      getPaymentMethodText,
      getItemVariantText,
      viewBill,
      editBill,
      confirmDelete,
      saveBill,
      deleteBill,
      printBill,
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
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  gap: 15px;
}

.page-title {
  font-size: 24px;
  font-weight: bold;
  margin: 0;
}

.header-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
  align-items: center;
}

.search-container {
  position: relative;
  width: 300px;
}

.search-input {
  width: 100%;
  padding: 10px 15px;
  border: 1px solid #ddd;
  border-radius: 4px;
  background-color: #f8f9fa;
}

.search-icon {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  color: #6c757d;
}

.date-filter {
  display: flex;
  align-items: center;
  gap: 8px;
}

.date-input {
  width: 150px;
}

.filter-button {
  background-color: #6c757d;
  color: white;
  border: none;
  border-radius: 4px;
  padding: 8px 16px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s;
}

.filter-button:hover {
  background-color: #5a6268;
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
}

.admin-table th,
.admin-table td {
  border: 1px solid #dee2e6;
  padding: 12px;
  text-align: center;
}

.admin-table th {
  background-color: #f8f9fa;
  cursor: pointer;
  user-select: none;
}

.admin-table th:hover {
  background-color: #e9ecef;
}

.action-buttons {
  display: flex;
  gap: 5px;
  justify-content: center;
}

.view-button,
.edit-button,
.delete-button {
  border: none;
  border-radius: 4px;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: background-color 0.2s, transform 0.2s;
}

.view-button {
  background-color: #17a2b8;
  color: white;
}

.view-button:hover {
  background-color: #138496;
  transform: scale(1.05);
}

.edit-button {
  background-color: #ffc107;
  color: #212529;
}

.edit-button:hover {
  background-color: #e0a800;
  transform: scale(1.05);
}

.delete-button {
  background-color: #dc3545;
  color: white;
}

.delete-button:hover {
  background-color: #c82333;
  transform: scale(1.05);
}

.status-badge {
  display: inline-block;
  padding: 5px 10px;
  border-radius: 20px;
  font-size: 13px;
  font-weight: 500;
}

.status-success {
  background-color: #d4edda;
  color: #155724;
}

.status-warning {
  background-color: #fff3cd;
  color: #856404;
}

.status-danger {
  background-color: #f8d7da;
  color: #721c24;
}

.status-secondary {
  background-color: #e2e3e5;
  color: #41464b;
}

.pagination-container {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: 20px;
  gap: 10px;
}

.pagination-button {
  border: 1px solid #dee2e6;
  background-color: #fff;
  border-radius: 4px;
  padding: 5px 10px;
  cursor: pointer;
}

.pagination-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-info {
  padding: 5px 10px;
  background-color: #f8f9fa;
  border-radius: 4px;
  border: 1px solid #dee2e6;
}

.bill-detail-header {
  background-color: #f8f9fa;
  border-radius: 4px;
  padding: 15px;
  margin-bottom: 20px;
}

.bill-info-group {
  padding: 10px;
}

.bill-info-group h6 {
  margin-bottom: 12px;
  border-bottom: 1px solid #dee2e6;
  padding-bottom: 8px;
}

.product-item {
  display: flex;
  align-items: center;
  gap: 10px;
}

.product-thumbnail {
  width: 60px;
  height: 60px;
  object-fit: cover;
  border-radius: 4px;
}

.bill-note {
  background-color: #f8f9fa;
  padding: 10px;
  border-radius: 4px;
  border-left: 3px solid #6c757d;
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .header-actions {
    width: 100%;
    flex-direction: column;
  }

  .search-container {
    width: 100%;
  }

  .admin-table {
    display: block;
    overflow-x: auto;
  }
}
</style>
