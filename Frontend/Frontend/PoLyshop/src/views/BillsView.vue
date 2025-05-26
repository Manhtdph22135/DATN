<script setup>
import { ref, onMounted, computed } from "vue";
import axios from "axios";

const bills = ref([]);
const searchTerm = ref("");
const error = ref(null);
const loading = ref(false);

const showModal = ref(false);
const selectedBill = ref(null);

// Sample data for development
const sampleBills = [
  {
    billID: 1,
    customer: "Nguyễn Văn A",
    productName: "Áo thun nam",
    cusAddress: "123 Nguyễn Huệ, Quận 1, TP. HCM",
    phone: "0901234567",
    status: 0,
    quantity: 2,
    size: "L",
    color: "Đen",
    material: "Cotton",
    image: "https://placehold.co/200x200",
  },
  {
    billID: 2,
    customer: "Trần Thị B",
    productName: "Quần jean nữ",
    cusAddress: "456 Lê Lợi, Quận 3, TP. HCM",
    phone: "0911234567",
    status: 1,
    quantity: 1,
    size: "M",
    color: "Xanh",
    material: "Denim",
    image: "https://placehold.co/200x200",
  },
  {
    billID: 3,
    customer: "Lê Văn C",
    productName: "Giày thể thao",
    cusAddress: "789 Điện Biên Phủ, Quận 10, TP. HCM",
    phone: "0921234567",
    status: 1,
    quantity: 1,
    size: "42",
    color: "Trắng",
    material: "Da",
    image: "https://placehold.co/200x200",
  },
];

const fetchBills = async () => {
  try {
    loading.value = true;

    // For development - using sample data
    setTimeout(() => {
      bills.value = sampleBills;
      loading.value = false;
    }, 500);

    // API implementation (commented out for development)
    // const res = await axios.get('/api/Bills/hoadon')
    // bills.value = res.data.$values || []
  } catch (err) {
    error.value = "Không thể lấy dữ liệu hóa đơn.";
    loading.value = false;
  }
};

const filteredBills = computed(() => {
  return bills.value.filter((bill) => {
    const searchString = searchTerm.value.toLowerCase();
    return (
      (bill.customer && bill.customer.toLowerCase().includes(searchString)) ||
      (bill.productName &&
        bill.productName.toLowerCase().includes(searchString)) ||
      (bill.cusAddress &&
        bill.cusAddress.toLowerCase().includes(searchString)) ||
      (bill.phone && bill.phone.toString().includes(searchString))
    );
  });
});

function openModal(bill) {
  selectedBill.value = bill;
  showModal.value = true;
}

function closeModal() {
  showModal.value = false;
  selectedBill.value = null;
}

onMounted(fetchBills);
</script>

<template>
  <div class="container py-5">
    <h3 class="text-center">Quản lý hóa đơn</h3>
    <div class="d-flex justify-content-center mb-3">
      <div class="d-flex w-50">
        <input
          type="text"
          class="form-control"
          placeholder="Tìm kiếm"
          v-model="searchTerm"
        />
        <button class="btn btn-outline-secondary ms-2" title="Search">
          <i class="bi bi-search"></i>
        </button>
      </div>
    </div>

    <!-- Loading indicator -->
    <div v-if="loading" class="text-center py-3">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Đang tải...</span>
      </div>
    </div>

    <div class="row" v-else>
      <div v-if="error" class="alert alert-danger text-center">
        {{ error }}
      </div>
      <div v-if="filteredBills.length === 0" class="text-center">
        <p>Không có hóa đơn nào.</p>
      </div>
      <div
        class="col-md-5 m-2 p-3 border rounded"
        v-for="bill in filteredBills"
        :key="bill.billID"
      >
        <p><strong>Tên khách hàng: </strong>{{ bill.customer ?? "" }}</p>
        <p><strong>Sản phẩm: </strong>{{ bill.productName ?? "" }}</p>
        <p><strong>Địa chỉ: </strong>{{ bill.cusAddress ?? "" }}</p>
        <p><strong>SĐT: </strong>{{ bill.phone ?? "" }}</p>
        <p>
          <strong>Trạng thái: </strong>
          <span v-if="bill.status === 0">Chờ Thanh toán</span>
          <span v-else>Đã Thanh toán</span>
        </p>
        <button class="btn btn-primary" @click="openModal(bill)">
          Chi Tiết
        </button>
      </div>
    </div>

    <!-- Modal -->
    <transition name="modal-fade">
      <div
        v-if="showModal"
        class="modal fade show"
        tabindex="-1"
        style="display: block; background: rgba(0, 0, 0, 0.5)"
      >
        <div class="modal-dialog modal-lg">
          <!-- Thêm modal-lg để modal rộng hơn -->
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Chi Tiết Hóa Đơn</h5>
              <button
                type="button"
                class="btn-close"
                @click="closeModal"
              ></button>
            </div>
            <div class="modal-body" v-if="selectedBill">
              <div class="row">
                <div
                  class="col-md-6 d-flex flex-column align-items-center justify-content-center"
                >
                  <div
                    class="card shadow-sm border-0"
                    style="width: 100%; max-width: 320px"
                  >
                    <div
                      v-if="selectedBill.image"
                      class="bg-secondary d-flex align-items-center justify-content-center"
                      style="height: 220px; border-radius: 8px 8px 0 0"
                    >
                      <img
                        :src="selectedBill.image"
                        alt="Ảnh sản phẩm"
                        class="img-fluid rounded-top"
                        style="max-height: 200px; object-fit: contain"
                      />
                    </div>
                    <div class="card-body text-center">
                      <p class="mb-2">
                        <span class="badge text-black fs-6">
                          Số Lượng: {{ selectedBill.quantity ?? "" }}
                        </span>
                      </p>
                    </div>
                  </div>
                </div>
                <div class="col-md-6">
                  <p>
                    <strong>Tên khách hàng: </strong
                    >{{ selectedBill.customer ?? "" }}
                  </p>
                  <p>
                    <strong>Sản phẩm: </strong
                    >{{ selectedBill.productName ?? "" }}
                  </p>
                  <p>
                    <strong>Địa chỉ: </strong
                    >{{ selectedBill.cusAddress ?? "" }}
                  </p>
                  <p><strong>SĐT: </strong>{{ selectedBill.phone ?? "" }}</p>
                  <p><strong>Kích cỡ: </strong>{{ selectedBill.size ?? "" }}</p>
                  <p>
                    <strong>Màu sắc: </strong>{{ selectedBill.color ?? "" }}
                  </p>
                  <p>
                    <strong>Chất liệu: </strong
                    >{{ selectedBill.material ?? "" }}
                  </p>
                  <p>
                    <strong>Trạng thái: </strong>
                    <span v-if="selectedBill.status === 0">Chờ Thanh toán</span>
                    <span v-else>Đã Thanh toán</span>
                  </p>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button class="btn btn-secondary" @click="closeModal">
                Đóng
              </button>
            </div>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>
<style scoped>
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.3s, transform 0.3s;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
  transform: scale(0.95) translateY(30px);
}

.modal-fade-enter-to,
.modal-fade-leave-from {
  opacity: 1;
  transform: scale(1) translateY(0);
}
</style>
