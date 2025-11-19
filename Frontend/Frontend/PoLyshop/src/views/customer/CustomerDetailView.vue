<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-lg-10">
        <!-- Loading indicator -->
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Đang tải...</span>
          </div>
        </div>

        <div v-else-if="error" class="alert alert-danger">
          {{ error }}
        </div>

        <div v-else-if="!customer" class="alert alert-warning">
          Không tìm thấy thông tin khách hàng
        </div>

        <div v-else>
          <div class="card shadow mb-4">
            <!-- Header -->
            <div
              class="card-header bg-primary text-white d-flex justify-content-between align-items-center py-3"
            >
              <h4 class="mb-0">Thông tin khách hàng</h4>
              <div>
                <router-link
                  :to="`/edit-customer/${customer.customerId}`"
                  class="btn btn-light btn-sm me-2"
                >
                  <i class="bi bi-pencil"></i> Chỉnh sửa
                </router-link>
                <router-link to="/admin/khachhang" class="btn btn-light btn-sm">
                  <i class="bi bi-arrow-left"></i> Quay lại
                </router-link>
              </div>
            </div>

            <!-- Customer profile -->
            <div class="card-body">
              <div class="row">
                <div class="col-md-3 text-center mb-4 mb-md-0">
                  <div class="customer-avatar">
                    <img
                      :src="
                        customer.avatar ||
                        'https://placehold.co/200x200?text=Avatar'
                      "
                      :alt="customer.fullName"
                      class="img-fluid rounded-circle shadow mb-3"
                      style="max-width: 160px"
                    />
                    <div class="membership-badge mb-3">
                      <span
                        class="badge rounded-pill px-3 py-2"
                        :class="getMembershipClass(customer.membership)"
                      >
                        {{ getMembershipText(customer.membership) }}
                      </span>
                    </div>
                    <p>
                      <span
                        class="badge"
                        :class="
                          customer.status === 'active'
                            ? 'bg-success'
                            : 'bg-secondary'
                        "
                      >
                        {{
                          customer.status === "active"
                            ? "Đang hoạt động"
                            : "Không hoạt động"
                        }}
                      </span>
                    </p>
                  </div>
                </div>

                <div class="col-md-9">
                  <h3 class="mb-4 border-bottom pb-2">
                    {{ customer.fullName }}
                  </h3>

                  <div class="row mb-4">
                    <div class="col-md-6">
                      <div class="mb-3">
                        <label class="text-muted">Email:</label>
                        <p class="lead mb-0">{{ customer.email }}</p>
                      </div>
                      <div class="mb-3">
                        <label class="text-muted">Số điện thoại:</label>
                        <p class="lead mb-0">{{ customer.phone }}</p>
                      </div>
                    </div>
                    <div class="col-md-6">
                      <div class="mb-3">
                        <label class="text-muted">Địa chỉ:</label>
                        <p class="lead mb-0">
                          {{ customer.address || "Chưa cập nhật" }}
                        </p>
                      </div>
                      <div class="mb-3">
                        <label class="text-muted">Ngày sinh:</label>
                        <p class="lead mb-0">
                          {{ formatDate(customer.birthday) }}
                        </p>
                      </div>
                    </div>
                  </div>

                  <div class="row mb-2">
                    <div class="col-md-6">
                      <div class="mb-3">
                        <label class="text-muted">Ngày đăng ký:</label>
                        <p class="lead mb-0">
                          {{ formatDate(customer.registerDate) }}
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Stats -->
          <div class="row mb-4">
            <div class="col-md-4 mb-3">
              <div class="card h-100 text-center shadow-sm hover-card">
                <div
                  class="card-body d-flex flex-column justify-content-center"
                >
                  <h3 class="display-4 text-primary mb-0">
                    {{ customer.orderCount || 0 }}
                  </h3>
                  <p class="text-muted">Tổng đơn hàng</p>
                </div>
              </div>
            </div>
            <div class="col-md-4 mb-3">
              <div class="card h-100 text-center shadow-sm hover-card">
                <div
                  class="card-body d-flex flex-column justify-content-center"
                >
                  <h3 class="display-4 text-primary mb-0">
                    {{ formatCurrency(customer.totalSpent || 0) }}
                  </h3>
                  <p class="text-muted">Tổng chi tiêu</p>
                </div>
              </div>
            </div>
            <div class="col-md-4 mb-3">
              <div class="card h-100 text-center shadow-sm hover-card">
                <div
                  class="card-body d-flex flex-column justify-content-center"
                >
                  <h3 class="display-4 text-primary mb-0">
                    {{ customer.points || 0 }}
                  </h3>
                  <p class="text-muted">Điểm tích lũy</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Recent orders -->
          <div class="card shadow">
            <div class="card-header bg-light">
              <h5 class="mb-0">Đơn hàng gần đây</h5>
            </div>
            <div class="card-body">
              <div v-if="loading" class="text-center py-3">
                <div
                  class="spinner-border spinner-border-sm text-primary"
                  role="status"
                >
                  <span class="visually-hidden">Đang tải...</span>
                </div>
              </div>
              <div
                v-else-if="recentOrders.length === 0"
                class="alert alert-info"
              >
                Khách hàng chưa có đơn hàng nào.
              </div>
              <div v-else class="table-responsive">
                <table class="table table-hover">
                  <thead>
                    <tr>
                      <th>Mã đơn</th>
                      <th>Ngày đặt</th>
                      <th>Tổng tiền</th>
                      <th>Trạng thái</th>
                      <th>Thao tác</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="order in recentOrders" :key="order.id">
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
                        <button class="btn btn-sm btn-outline-primary">
                          <i class="bi bi-eye"></i> Chi tiết
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import axios from "axios";

export default {
  name: "CustomerDetailView",
  props: {
    id: {
      type: [String, Number],
      required: true,
    },
  },
  setup(props) {
    const customer = ref(null);
    const loading = ref(true);
    const error = ref(null);
    const recentOrders = ref([]);

    // Sample data for development
    const sampleCustomers = [
      {
        customerId: 1,
        fullName: "Nguyễn Văn An",
        email: "nguyenvanan@gmail.com",
        phone: "0901234567",
        address: "123 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh",
        birthday: "1990-05-15",
        membership: "gold",
        status: "active",
        registerDate: "2022-01-10",
        orderCount: 15,
        totalSpent: 12500000,
        points: 2500,
        avatar: "https://placehold.co/150x150?text=User",
      },
      {
        customerId: 2,
        fullName: "Trần Thị Bình",
        email: "tranthibinh@gmail.com",
        phone: "0911234567",
        address: "456 Lê Lợi, Quận 3, TP. Hồ Chí Minh",
        birthday: "1995-12-20",
        membership: "silver",
        status: "active",
        registerDate: "2022-03-22",
        orderCount: 8,
        totalSpent: 5800000,
        points: 1160,
        avatar: "https://placehold.co/150x150?text=User",
      },
      {
        customerId: 3,
        fullName: "Lê Văn Cường",
        email: "levancuong@gmail.com",
        phone: "0921234567",
        address: "789 Điện Biên Phủ, Quận 10, TP. Hồ Chí Minh",
        birthday: "1988-08-30",
        membership: "bronze",
        status: "active",
        registerDate: "2022-08-15",
        orderCount: 3,
        totalSpent: 2600000,
        points: 520,
        avatar: "https://placehold.co/150x150?text=User",
      },
      {
        customerId: 4,
        fullName: "Phạm Thị Dung",
        email: "phamthidung@gmail.com",
        phone: "0931234567",
        address: "234 Cách Mạng Tháng Tám, Quận 3, TP. Hồ Chí Minh",
        birthday: "1993-04-12",
        membership: "bronze",
        status: "inactive",
        registerDate: "2023-02-20",
        orderCount: 2,
        totalSpent: 980000,
        points: 196,
        avatar: "https://placehold.co/150x150?text=User",
      },
    ];

    // Sample orders for development
    const sampleOrders = [
      {
        id: 1001,
        customerId: 1,
        orderDate: "2023-10-15",
        total: 1250000,
        status: "delivered",
      },
      {
        id: 1002,
        customerId: 1,
        orderDate: "2023-09-20",
        total: 850000,
        status: "delivered",
      },
      {
        id: 1003,
        customerId: 1,
        orderDate: "2023-08-05",
        total: 450000,
        status: "delivered",
      },
      {
        id: 1004,
        customerId: 2,
        orderDate: "2023-10-18",
        total: 2150000,
        status: "shipped",
      },
      {
        id: 1005,
        customerId: 2,
        orderDate: "2023-10-05",
        total: 1750000,
        status: "delivered",
      },
      {
        id: 1006,
        customerId: 3,
        orderDate: "2023-09-25",
        total: 950000,
        status: "delivered",
      },
      {
        id: 1007,
        customerId: 4,
        orderDate: "2023-03-10",
        total: 580000,
        status: "cancelled",
      },
      {
        id: 1008,
        customerId: 4,
        orderDate: "2023-02-28",
        total: 400000,
        status: "delivered",
      },
    ];

    const fetchCustomerData = () => {
      loading.value = true;
      error.value = null;

      // For development purposes, we'll use sample data
      setTimeout(() => {
        const foundCustomer = sampleCustomers.find(
          (c) => c.customerId === parseInt(props.id)
        );

        if (foundCustomer) {
          customer.value = foundCustomer;
          recentOrders.value = sampleOrders.filter(
            (o) => o.customerId === parseInt(props.id)
          );
        } else {
          error.value = "Không tìm thấy thông tin khách hàng.";
        }

        loading.value = false;
      }, 500);

      // API implementation (commented out for development)
      // try {
      //   const response = await axios.get(`/api/customers/${props.id}`);
      //   customer.value = response.data;
      //
      //   const orderResponse = await axios.get(`/api/customers/${props.id}/orders`);
      //   recentOrders.value = orderResponse.data;
      //   loading.value = false;
      // } catch (err) {
      //   error.value = "Không thể tải thông tin khách hàng: " + err.message;
      //   loading.value = false;
      // }
    };

    const formatDate = (dateString) => {
      if (!dateString) return "Chưa cập nhật";

      const options = {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
      };

      return new Date(dateString).toLocaleDateString("vi-VN", options);
    };

    const formatCurrency = (value) => {
      return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
        maximumFractionDigits: 0,
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
          return "Thành viên Đồng";
        case "silver":
          return "Thành viên Bạc";
        case "gold":
          return "Thành viên Vàng";
        case "platinum":
          return "Thành viên Bạch Kim";
        default:
          return "Thành viên Thường";
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
      fetchCustomerData();
    });

    return {
      customer,
      loading,
      error,
      recentOrders,
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
.customer-avatar img {
  border: 3px solid #eee;
}

.hover-card {
  transition: transform 0.3s, box-shadow 0.3s;
}

.hover-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.1) !important;
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

label.text-muted {
  font-size: 0.85rem;
  margin-bottom: 0;
}
</style>
