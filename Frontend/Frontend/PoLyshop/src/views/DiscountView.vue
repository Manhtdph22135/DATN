<script setup>
import { ref, computed, onMounted, nextTick } from "vue";
import axios from "axios";
import bootstrap from "@/utils/bootstrapHelper"; // Import từ file helper riêng

// States
const discounts = ref([]);
const products = ref([]); // Danh sách sản phẩm để chọn
const loading = ref(true);
const error = ref(null);
const searchTerm = ref("");
const isEditing = ref(false);
const isAdding = ref(false);
const selectedDiscount = ref(true);

// Form data
const currentDiscount = ref({
  id: null, // PromotionId
  name: "",
  code: "",
  percentage: 10, // DiscountValue
  description: "",
  startDate: new Date().toISOString().split("T")[0],
  endDate: new Date(new Date().setMonth(new Date().getMonth() + 1)).toISOString().split("T")[0],
  productId: null, // ID sản phẩm được chọn
  status: "Đang hoạt động"
});

// 1. Tải dữ liệu (Promotion + Product)
const fetchData = async () => {
  loading.value = true;
  try {
    // Gọi 2 API cùng lúc
    const [promoRes, prodRes] = await Promise.all([
      axios.get("https://localhost:7055/api/Promotion"),
      axios.get("https://localhost:7055/api/Product")
    ]);

    // 1. Dữ liệu Khuyến mãi
    // Map lại tên trường cho khớp với Frontend (nếu API trả về PascalCase)
    // Trong hàm fetchData
const rawPromos = promoRes.data.$values || promoRes.data || [];

discounts.value = rawPromos.map(d => ({
  // SỬA CÁC DÒNG NÀY ĐỂ BẮT CẢ CHỮ HOA VÀ THƯỜNG
  id: d.promotionId || d.PromotionId || d.id,
  name: d.name || d.Name,
  code: d.code || d.Code, // <--- QUAN TRỌNG: Sửa lỗi mất mã code
  percentage: d.discountValue || d.DiscountValue || d.percentage,
  description: d.description || d.Description,
  startDate: d.startDate || d.StartDate,
  endDate: d.endDate || d.EndDate,
  productId: d.productId || d.ProductID || d.productID, // Bắt dính cả ProductID
  status: d.status || d.Status
}));

    // 2. Dữ liệu Sản phẩm (Để chọn trong dropdown)
    products.value = prodRes.data.$values || prodRes.data || [];

  } catch (err) {
    error.value = "Không thể tải dữ liệu: " + err.message;
  } finally {
    loading.value = false;
  }
};

onMounted(fetchData);

// Filter
const filteredDiscounts = computed(() => {
  if (!searchTerm.value) return discounts.value;
  const term = searchTerm.value.toLowerCase();
  return discounts.value.filter(
    (d) =>
      d.name.toLowerCase().includes(term) ||
      (d.code && d.code.toLowerCase().includes(term))
  );
});

// Format tiền tệ (để hiển thị giá sản phẩm trong dropdown)
const formatCurrency = (price) => {
  return new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(price);
};

// Format ngày
const formatDate = (dateString) => {
  if(!dateString) return "";
  // Cắt bỏ phần giờ nếu có (để tránh lỗi hiển thị)
  const datePart = dateString.split('T')[0]; 
  return new Date(datePart).toLocaleDateString("vi-VN");
};

// Trạng thái
const getStatusClass = (discount) => {
  const now = new Date().toISOString().split('T')[0]; // So sánh theo ngày (String) cho an toàn
  if (now < discount.startDate) return "badge bg-warning";
  if (now > discount.endDate) return "badge bg-danger";
  return "badge bg-success";
};

const getStatusText = (discount) => {
  const now = new Date().toISOString().split('T')[0];
  if (now < discount.startDate) return "Sắp diễn ra";
  if (now > discount.endDate) return "Đã kết thúc";
  return "Đang diễn ra";
};

// Actions
const openAddModal = async () => {
  isEditing.value = false;
  isAdding.value = true;
  selectedDiscount.value = false;
  currentDiscount.value = {
    id: null,
    name: "",
    code: "",
    percentage: 10,
    description: "",
    startDate: new Date().toISOString().split("T")[0],
    endDate: new Date(new Date().setMonth(new Date().getMonth() + 1)).toISOString().split("T")[0],
    productId: null,
    status: "Đang hoạt động"
  };

  // 1. Chờ Vue vẽ xong
  await nextTick();

  // 2. Mở modal bằng window.bootstrap
  const modalElement = document.getElementById("discountModal");
  if (modalElement) {
    // Trong openAddModal, editDiscount, viewDiscount
const modal = new bootstrap.Modal(document.getElementById("discountModal"));
modal.show();
  }
};

const editDiscount = async (discount) => {
  isEditing.value = true;
  
  // Copy dữ liệu và xử lý ngày tháng
  currentDiscount.value = { 
    ...discount,
    // Cắt chuỗi ngày để lấy yyyy-mm-dd cho input date
    startDate: discount.startDate ? discount.startDate.split('T')[0] : '',
    endDate: discount.endDate ? discount.endDate.split('T')[0] : ''
  };

  // 1. Chờ Vue vẽ xong (để input nhận được giá trị mới)
  await nextTick(); 

  // 2. Mở modal bằng window.bootstrap
  const modalElement = document.getElementById("discountModal");
  if (modalElement) {
    // Trong openAddModal, editDiscount, viewDiscount
const modal = new bootstrap.Modal(document.getElementById("discountModal"));
modal.show();
  }
};

const viewDiscount = (discount) => {

  currentDiscount.value = { 
    ...discount,
    startDate: discount.startDate ? discount.startDate.split('T')[0] : '',
    endDate: discount.endDate ? discount.endDate.split('T')[0] : ''
  };
  // MỞ CÙNG 1 MODAL VỚI FORM SỬA
  const modal = new bootstrap.Modal(document.getElementById("discountModal"));
  modal.show();
  isAdding.value = false;  // Không phải thêm
  isEditing.value = false; // Không phải sửa -> CHẾ ĐỘ XEM (READ-ONLY)
  selectedDiscount.value = true; // Chế độ xem
};

const confirmDeleteDiscount = (discount) => {
  currentDiscount.value = { ...discount };
  const modal = new bootstrap.Modal(document.getElementById("deleteModal"));
  modal.show();
};

// API Actions (Lưu / Xóa)
const saveDiscount = async () => {
  try {
    // Chuẩn bị dữ liệu gửi lên API (Khớp với Model C#)
    const payload = {
      PromotionId: currentDiscount.value.id || 0, // Có thể null nếu thêm mới
      Name: currentDiscount.value.name,
      Code: currentDiscount.value.code,
      Description: currentDiscount.value.description,
      StartDate: currentDiscount.value.startDate,
      EndDate: currentDiscount.value.endDate,
      DiscountType: "1", // Giả sử 1 là %
      DiscountValue: currentDiscount.value.percentage,
      Status: "Đang hoạt động",
      ProductID: currentDiscount.value.productId // Quan trọng: ID sản phẩm hoặc null
    };

    if (isEditing.value) {
   // Đã có API, gọi thôi!
   await axios.put(`https://localhost:7055/api/Promotion/${currentDiscount.value.id}`, payload);
} else {
       // GỌI API POST (THÊM MỚI)
       await axios.post("https://localhost:7055/api/Promotion", payload);
    }

    // Tải lại danh sách
    await fetchData();
alert(isEditing.value ? "Cập nhật thành công!" : "Thêm mới thành công!");
    // Đóng modal
    // Trong openAddModal, editDiscount, viewDiscount
const modalElement = document.getElementById("discountModal");
        
        // Cách 1: Dùng Bootstrap Instance (Chuẩn nhất)
        const modalInstance = window.bootstrap.Modal.getInstance(modalElement);
        if (modalInstance) {
            modalInstance.hide();
        } else {
            // Cách 2: Fallback (Nếu instance bị mất) -> Tự xóa class và backdrop
            modalElement.classList.remove('show');
            modalElement.style.display = 'none';
            modalElement.setAttribute('aria-hidden', 'true');
            const backdrops = document.querySelectorAll('.modal-backdrop');
            backdrops.forEach(backdrop => backdrop.remove()); // Xóa lớp đen mờ
            document.body.classList.remove('modal-open'); // Cho phép cuộn lại
            document.body.style = ''; // Reset style body
        }
      } catch (err) {
        console.error(err);
        // ... (xử lý lỗi giữ nguyên) ...
      }
    };

const deleteDiscount = async () => {
  try {
    await axios.delete(`https://localhost:7055/api/Promotion/${currentDiscount.value.id}`);
    
    // Xóa khỏi danh sách hiển thị
    discounts.value = discounts.value.filter(d => d.id !== currentDiscount.value.id);

    const modal = bootstrap.Modal.getInstance(document.getElementById("deleteModal"));
    modal.hide();
  } catch (err) {
    alert("Lỗi khi xóa: " + err.message);
  }
};

// Helper cho nút Edit từ View Modal
const editFromView = () => {
  const viewModal = bootstrap.Modal.getInstance(document.getElementById("viewModal"));
  viewModal.hide();
  setTimeout(() => {
    isEditing.value = true;
    const editModal = new bootstrap.Modal(document.getElementById("discountModal"));
    editModal.show();
  }, 500);
};
</script>

<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý ưu đãi</h2>
      <div class="header-actions">
        <div class="search-container">
          <input type="text" class="search-input" placeholder="Tìm kiếm ưu đãi..." v-model="searchTerm" />
          <i class="bi bi-search search-icon"></i>
        </div>
        <button class="btn btn-success" @click="openAddModal">
          <i class="bi bi-plus"></i> Thêm ưu đãi
        </button>
      </div>
    </div>

    <div class="content-container">
      <div v-if="loading" class="text-center p-5">
        <div class="spinner-border text-primary" role="status"></div>
      </div>

      <div v-else-if="error" class="alert alert-danger">{{ error }}</div>

      <div v-else-if="filteredDiscounts.length === 0" class="text-center p-5">
        <p>Không tìm thấy ưu đãi nào.</p>
      </div>

      <div v-else class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>Mã</th>
              <th>Tên ưu đãi</th>
              <th>Mã Code</th>
              <th>Sản phẩm áp dụng</th> <th>Giảm giá</th>
              <th>Thời gian</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="discount in filteredDiscounts" :key="discount.id">
              <td>{{ discount.id }}</td>
              <td>{{ discount.name }}</td>
              <td>
                <span class="badge bg-secondary" v-if="discount.code">{{ discount.code }}</span>
                <span class="text-muted small" v-else>--</span>
              </td>
              <td>
                <span v-if="discount.productId" class="badge bg-primary">
                   SP ID: {{ discount.productId }}
                </span>
                <span v-else class="badge bg-success">Toàn đơn hàng</span>
              </td>
              <td>{{ discount.percentage }}%</td>
              <td>
                 <small>{{ formatDate(discount.startDate) }} <br> {{ formatDate(discount.endDate) }}</small>
              </td>
              <td>
                <span :class="getStatusClass(discount)">{{ getStatusText(discount) }}</span>
              </td>
              <td class="action-buttons">
                <button class="btn btn-sm btn-info me-1" @click="viewDiscount(discount)"><i class="bi bi-eye"></i></button>
                <button class="btn btn-sm btn-warning me-1" @click="editDiscount(discount)"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger" @click="confirmDeleteDiscount(discount)"><i class="bi bi-trash"></i></button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="modal fade" id="discountModal" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ isAdding ? "Thêm ưu đãi mới" : (isEditing ? "Chỉnh sửa ưu đãi" : "Chi tiết ưu đãi") }}
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveDiscount">
              
              <div class="mb-3">
                <label class="form-label">Tên ưu đãi</label>
                <input type="text" class="form-control" v-model="currentDiscount.name" required 
                  :disabled="!isEditing && !isAdding" /> </div>

              <div class="row">
                 <div class="col-md-6 mb-3">
                    <label class="form-label">Mã giảm giá (Code)</label>
                    <input type="text" class="form-control" v-model="currentDiscount.code"
                      :disabled="!isEditing && !isAdding" /> </div>
                 <div class="col-md-6 mb-3">
                    <label class="form-label">% Giảm giá</label>
                    <input type="number" class="form-control" v-model="currentDiscount.percentage" min="0" max="100" required 
                      :disabled="!isEditing && !isAdding" /> </div>
              </div>

              <div class="mb-3 p-3 bg-light rounded border">
                <label class="form-label fw-bold">Áp dụng cho sản phẩm</label>
                <select class="form-select" v-model="currentDiscount.productId" 
                  :disabled="!isEditing && !isAdding"> <option :value="null">-- Không chọn (Mã Voucher) --</option>
                  <option v-for="prod in products" :key="prod.productId" :value="prod.productId">
                    {{ prod.productName }} - {{ formatCurrency(prod.price) }}
                  </option>
                </select>
              </div>

              <div class="mb-3">
                <label class="form-label">Mô tả</label>
                <textarea class="form-control" v-model="currentDiscount.description" rows="2"
                  :disabled="!isEditing && !isAdding"></textarea> </div>

              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Ngày bắt đầu</label>
                  <input type="date" class="form-control" v-model="currentDiscount.startDate" required 
                    :disabled="!isEditing && !isAdding" /> </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Ngày kết thúc</label>
                  <input type="date" class="form-control" v-model="currentDiscount.endDate" required 
                    :disabled="!isEditing && !isAdding" /> </div>
              </div>

              <div class="text-end">
                <button type="button" class="btn btn-secondary me-2" data-bs-dismiss="modal">
                  {{ isEditing || isAdding ? 'Hủy' : 'Đóng' }}
                </button>
                
                <button v-if="isEditing || isAdding" type="submit" class="btn btn-primary" data-bs-dismiss="modal">Lưu</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <div class="modal fade" id="deleteModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Xác nhận xóa</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <p>Bạn có chắc chắn muốn xóa ưu đãi <strong>{{ currentDiscount.name }}</strong>?</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
            <button type="button" class="btn btn-danger" @click="deleteDiscount">Xóa</button>
          </div>
        </div>
      </div>
    </div>
    <div class="modal fade" id="viewModal" tabindex="-1" aria-hidden="true">
        </div>

  </div>
</template>



<style scoped>
.admin-page { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; flex-wrap: wrap; }
.page-title { font-size: 24px; font-weight: bold; margin-bottom: 10px; }
.header-actions { display: flex; gap: 15px; flex-wrap: wrap; }
.search-container { position: relative; min-width: 250px; }
.search-input { width: 100%; padding: 10px 15px; padding-right: 40px; border-radius: 5px; border: 1px solid #ddd; }
.search-icon { position: absolute; right: 15px; top: 50%; transform: translateY(-50%); color: #777; }
.content-container { background-color: white; border-radius: 8px; padding: 20px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); }
.admin-table { width: 100%; border-collapse: collapse; }
.admin-table th, .admin-table td { padding: 12px 15px; text-align: left; border-bottom: 1px solid #ddd; }
.admin-table th { background-color: #f5f5f5; font-weight: bold; }
.admin-table tr:hover { background-color: #f9f9f9; }
.action-buttons { white-space: nowrap; }
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
