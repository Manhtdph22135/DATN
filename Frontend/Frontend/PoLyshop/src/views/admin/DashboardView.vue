<template>
  <div class="admin-page">
    <div class="dashboard-header">
      <h2 class="page-title">Admin Dashboard</h2>
      <div class="date-display">{{ currentDate }}</div>
    </div>

    <!-- Quick Stats Cards -->
    <div class="dashboard-cards">
      <div class="dashboard-card">
        <div class="card-icon" style="background-color: #e3f2fd; color: #1976d2">
          <i class="bi bi-cart-check"></i>
        </div>
        <div class="card-content">
          <div class="card-value">{{ totalOrdersFormatted }}</div>
          <div class="card-label">Đơn hàng</div>
          <div class="card-trend positive">
            <i class="bi bi-arrow-up"></i> 15%
          </div>
        </div>
      </div>

      <div class="dashboard-card">
        <div class="card-icon" style="background-color: #e8f5e9; color: #2e7d32">
          <i class="bi bi-currency-dollar"></i>
        </div>
        <div class="card-content">
          <div class="card-value">{{ totalRevenueFormatted }}</div>
          <div class="card-label">Doanh thu</div>
          <div class="card-trend positive">
            <i class="bi bi-arrow-up"></i> 23%
          </div>
        </div>
      </div>

      <div class="dashboard-card">
        <div class="card-icon" style="background-color: #fff3e0; color: #e65100">
          <i class="bi bi-people"></i>
        </div>
        <div class="card-content">
          <div class="card-value">{{ totalCustomersFormatted }}</div>
          <div class="card-label">Khách hàng</div>
          <div class="card-trend positive">
            <i class="bi bi-arrow-up"></i> 8%
          </div>
        </div>
      </div>

      <div class="dashboard-card">
        <div class="card-icon" style="background-color: #ffebee; color: #c62828">
          <i class="bi bi-bag-x"></i>
        </div>
        <div class="card-content">
          <div class="card-value">{{ totalCanceledFormatted }}</div>
          <div class="card-label">Đơn hàng bị hủy</div>
          <div class="card-trend negative">
            <i class="bi bi-arrow-down"></i> 7%
          </div>
        </div>
      </div>
    </div>

    <!-- Middle Section: Charts -->
    <div class="dashboard-charts">
      <!-- Doanh thu theo tháng -->
      <div class="chart-container">
        <div class="chart-header">
          <h3 class="chart-title">Doanh thu theo tháng</h3>
          <div class="chart-actions">
            <select
              class="chart-select"
              v-model="selectedYear"
              @change="fetchMonthlyRevenue"
            >
              <option v-for="year in availableYears" :key="year" :value="year">
                Năm {{ year }}
              </option>
            </select>
          </div>
        </div>
        <div class="chart-content">
          <div class="chart-mock">
            <div
              v-for="bar in monthlyBars"
              :key="bar.month"
              class="chart-bar active"
              :style="{ height: bar.height + '%' }"
            >
              <span class="chart-label">T{{ bar.month }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Sản phẩm bán chạy -->
      <div
        class="chart-container"
        style="display: flex; flex-direction: column; height: 100%;"
      >
        <div class="chart-header">
          <h3 class="chart-title">Sản phẩm bán chạy</h3>
          <div class="chart-actions">
            <select
              class="chart-select"
              v-model.number="selectedTop"
              @change="fetchTopProducts"
            >
              <option :value="5">Top 5</option>
              <option :value="10">Top 10</option>
              <option :value="20">Top 20</option>
            </select>
          </div>
        </div>
        <div class="chart-content" style="height: auto; display: block;">
          <div class="top-products">
            <div
              class="product-item"
              v-for="(product, index) in topProducts"
              :key="product.productId"
            >
              <div class="product-rank">{{ index + 1 }}</div>
              <div class="product-info">
                <div class="product-name">{{ product.productName }}</div>
                <div class="product-sold">
                  Đã bán: {{ product.soLuongBan }}
                </div>
              </div>
              <div class="product-bar-container">
                <div
                  class="product-bar"
                  :style="{ width: product.percent + '%' }"
                ></div>
              </div>
            </div>

            <div class="product-item" v-if="topProducts.length === 0">
              Đang tải dữ liệu sản phẩm bán chạy...
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Bottom Section: Quick Actions and Recent Orders -->
    <div class="dashboard-bottom">
      <!-- Quick Actions -->
      <div class="quick-actions">
        <h3 class="section-title">Truy cập nhanh</h3>
        <div class="action-buttons">
          <router-link to="/admin/sanpham" class="action-button">
            <div class="action-icon">
              <i class="bi bi-plus-circle"></i>
            </div>
            <div class="action-label">Quản Lý sản phẩm</div>
          </router-link>
          <router-link to="/admin/donhang" class="action-button">
            <div class="action-icon">
              <i class="bi bi-cart-plus"></i>
            </div>
            <div class="action-label">Quản Lý đơn hàng</div>
          </router-link>
          <router-link to="/admin/uudai" class="action-button">
            <div class="action-icon">
              <i class="bi bi-tag"></i>
            </div>
            <div class="action-label">Quản Lý ưu đãi</div>
          </router-link>
          <router-link to="/admin/nhanvien" class="action-button">
            <div class="action-icon">
              <i class="bi bi-person-plus"></i>
            </div>
            <div class="action-label">Quản Lý nhân viên</div>
          </router-link>
        </div>
      </div>

      <!-- Recent Orders -->
      <div class="recent-orders">
        <div class="orders-header">
          <h3 class="section-title">Đơn hàng gần đây</h3>
          <button class="view-all-btn">
            Xem tất cả <i class="bi bi-arrow-right"></i>
          </button>
        </div>
        <table class="admin-table">
          <thead>
            <tr>
              <th>Mã đơn</th>
              <th>Khách hàng</th>
              <th>Ngày đặt</th>
              <th>Trạng thái</th>
              <th>Tổng tiền</th>
              <th>Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="order in recentOrders" :key="order.id">
              <td class="order-id">{{ order.orderCode }}</td>
              <td>{{ order.customer }}</td>
              <td>{{ order.date }}</td>
              <td>
                <span :class="order.statusClass">
                  {{ order.status }}
                </span>
              </td>
              <td class="order-total">{{ order.total }}</td>
              <td>
                <div class="action-buttons">
                  <button class="view-button">
                    <i class="bi bi-eye"></i>
                  </button>
                  <button class="print-button">
                    <i class="bi bi-printer"></i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "AdminDashboardView",
  data() {
    const currentYear = new Date().getFullYear();

    return {
      // info chung
      currentDate: new Date().toLocaleDateString("vi-VN", {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric",
      }),
      recentOrders: [],
      totalRevenue: 0,
      totalOrders: 0,
      totalCustomers: 0,
      totalCanceled: 0,

      // chart doanh thu theo tháng
      selectedYear: currentYear,
      availableYears: [currentYear, currentYear - 1, currentYear - 2],
      monthlyRevenueRaw: [],   // [{ month, value }]

      // top sản phẩm
      selectedTop: 5,
      topProducts: [],
    };
  },
  computed: {
    totalRevenueFormatted() {
      return this.totalRevenue
        ? Number(this.totalRevenue).toLocaleString("vi-VN", {
            style: "currency",
            currency: "VND",
            maximumFractionDigits: 0,
          })
        : "0 VNĐ";
    },
    totalOrdersFormatted() {
      return this.totalOrders
        ? Number(this.totalOrders).toLocaleString("vi-VN")
        : "0";
    },
    totalCustomersFormatted() {
      return this.totalCustomers
        ? Number(this.totalCustomers).toLocaleString("vi-VN")
        : "0";
    },
    totalCanceledFormatted() {
      return this.totalCanceled
        ? Number(this.totalCanceled).toLocaleString("vi-VN")
        : "0";
    },

    // dữ liệu cho 12 cột biểu đồ
    monthlyBars() {
      const map = new Map();
      this.monthlyRevenueRaw.forEach((m) =>
        map.set(m.month, Number(m.value || 0))
      );

      const months = Array.from({ length: 12 }, (_, i) => i + 1);
      const values = months.map((m) => map.get(m) || 0);
      const max = Math.max(...values, 1); // tránh chia 0

      return months.map((month) => {
        const value = map.get(month) || 0;
        const height = value > 0 ? 15 + (value / max) * 80 : 5;
        return { month, value, height };
      });
    },
  },
  methods: {
    mapStatusToClass(statusRaw) {
      const s = (statusRaw || "").toLowerCase();
      if (s.includes("paid") || s.includes("hoàn thành")) {
        return "status-badge status-delivered";
      }
      if (s.includes("processing") || s.includes("đang xử lý")) {
        return "status-badge status-processing";
      }
      if (s.includes("shipping") || s.includes("đang giao")) {
        return "status-badge status-shipping";
      }
      if (s.includes("pending") || s.includes("chờ")) {
        return "status-badge status-pending";
      }
      if (s.includes("cancel") || s.includes("hủy")) {
        return "status-badge status-canceled";
      }
      return "status-badge status-pending";
    },

    translateStatus(statusRaw) {
      const s = (statusRaw || "").toLowerCase();
      if (s === "paid") return "Đã thanh toán";
      if (s === "processing") return "Đang xử lý";
      if (s === "shipping") return "Đang giao";
      if (s === "pending") return "Chờ xử lý";
      if (s === "canceled" || s === "cancelled") return "Đã hủy";
      return statusRaw || "Không rõ";
    },

    // ================== FETCH TỔNG QUAN ==================
    async fetchSummaryCards() {
      const baseUrl = "https://localhost:7055";

      const revenueResponse = await fetch(`${baseUrl}/get-tong-doanh-thu`);
      const revenueData = await revenueResponse.json();
      this.totalRevenue = Number(revenueData.tongDoanhThu || 0);

      const ordersResponse = await fetch(`${baseUrl}/get-tong-don-hang`);
      const ordersData = await ordersResponse.json();
      this.totalOrders = Number(ordersData.tongDonHang || 0);

      const customersResponse = await fetch(
        `${baseUrl}/get-tong-khach-hang`
      );
      const customersData = await customersResponse.json();
      this.totalCustomers = Number(customersData.tongKhachHang || 0);
    },

    // ================== DOANH THU THEO THÁNG ==================
    async fetchMonthlyRevenue() {
      const baseUrl = "https://localhost:7055";

      try {
        const url = `${baseUrl}/get-doanh-thu-theo-thang?year=${this.selectedYear}`;
        const res = await fetch(url);

        if (!res.ok) {
          console.error("Lỗi gọi API doanh thu theo tháng:", res.status);
          this.monthlyRevenueRaw = [];
          return;
        }

        const data = await res.json();
        console.log("Monthly revenue raw:", data);

        const arr = Array.isArray(data) ? data : data.$values || [];
        this.monthlyRevenueRaw = arr.map((item) => ({
          month: item.thang,
          value: item.tongDoanhThu,
        }));
      } catch (err) {
        console.error("Exception khi load doanh thu theo tháng:", err);
        this.monthlyRevenueRaw = [];
      }
    },

    // ================== TOP SẢN PHẨM BÁN CHẠY ==================
    async fetchTopProducts() {
      const baseUrl = "https://localhost:7055";

      try {
        const url = `${baseUrl}/get-san-pham-ban-chay?top=${this.selectedTop}`;
        const res = await fetch(url);

        if (!res.ok) {
          console.error("Lỗi gọi API top sản phẩm:", res.status);
          this.topProducts = [];
          return;
        }

        const raw = await res.json();
        console.log("Top products raw:", raw);

        const arr = Array.isArray(raw) ? raw : raw.$values || [];

        const maxSold = arr.reduce(
          (max, p) => Math.max(max, Number(p.soLuongBan || 0)),
          1
        );

        this.topProducts = arr.map((p) => ({
          productId: p.productId,
          productName: p.productName,
          soLuongBan: p.soLuongBan,
          percent: 20 + (Number(p.soLuongBan || 0) / maxSold) * 80,
        }));
      } catch (err) {
        console.error("Exception khi load top sản phẩm:", err);
        this.topProducts = [];
      }
    },

    // ================== ĐƠN HÀNG GẦN ĐÂY ==================
    async fetchRecentOrders() {
      const baseUrl = "https://localhost:7055";

      const recentOrdersResponse = await fetch(
        `${baseUrl}/get-don-hang-gan-day?days=30`
      );
      const recentOrdersData = await recentOrdersResponse.json();
      const ordersArray = Array.isArray(recentOrdersData)
        ? recentOrdersData
        : recentOrdersData.$values || [];

      this.recentOrders = ordersArray.map((o) => {
        const created = o.createdAt ? new Date(o.createdAt) : null;
        return {
          id: o.id,
          orderCode: o.orderCode,
          customer: o.customerName,
          date: created
            ? created.toLocaleString("vi-VN", {
                day: "2-digit",
                month: "2-digit",
                year: "numeric",
                hour: "2-digit",
                minute: "2-digit",
              })
            : "",
          status: this.translateStatus(o.status),
          statusClass: this.mapStatusToClass(o.status),
          total: Number(o.total || 0).toLocaleString("vi-VN", {
            style: "currency",
            currency: "VND",
            maximumFractionDigits: 0,
          }),
        };
      });
    },

    // ================== LOAD TẤT CẢ ==================
    async fetchData() {
      try {
        await Promise.all([
          this.fetchSummaryCards(),
          this.fetchMonthlyRevenue(),
          this.fetchTopProducts(),
          this.fetchRecentOrders(),
        ]);
      } catch (err) {
        console.error("Error fetching dashboard data:", err);
      }
    },
  },
  mounted() {
    this.fetchData();
  },
};
</script>


<style scoped>
.admin-page {
  padding: 24px;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.page-title {
  font-size: 28px;
  font-weight: bold;
  margin: 0;
  color: #333;
}

.date-display {
  font-size: 16px;
  color: #666;
}

/* Cards Section */
.dashboard-cards {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 24px;
  margin-bottom: 30px;
}

.dashboard-card {
  background-color: white;
  border-radius: 12px;
  padding: 24px;
  display: flex;
  align-items: center;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.dashboard-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.12);
}

.card-icon {
  font-size: 32px;
  width: 70px;
  height: 70px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 20px;
}

.card-content {
  flex: 1;
}

.card-value {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 8px;
  color: #333;
}

.card-label {
  color: #666;
  font-size: 14px;
  margin-bottom: 8px;
}

.card-trend {
  font-size: 14px;
  font-weight: 600;
  display: flex;
  align-items: center;
}

.card-trend.positive {
  color: #2e7d32;
}

.card-trend.negative {
  color: #c62828;
}

/* Charts Section */
.dashboard-charts {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 24px;
  margin-bottom: 30px;
}

.chart-container {
  background-color: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.chart-title {
  font-size: 18px;
  font-weight: 600;
  margin: 0;
  color: #333;
}

.chart-select {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 6px;
  background-color: #f9f9f9;
  font-size: 14px;
}

.chart-content {
  height: 300px;
  display: flex;
  align-items: flex-end;
}

.chart-mock {
  width: 100%;
  height: 270px;
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  padding-top: 20px;
  position: relative;
}

.chart-mock::before {
  content: "";
  position: absolute;
  left: 0;
  top: 0;
  width: 100%;
  height: 1px;
  background-color: #eee;
}

.chart-bar {
  width: 30px;
  background-color: #e0e0e0;
  border-radius: 4px 4px 0 0;
  position: relative;
  transition: background-color 0.3s ease;
}

.chart-bar:hover,
.chart-bar.active {
  background-color: #1976d2;
}

.chart-label {
  position: absolute;
  bottom: -25px;
  left: 50%;
  transform: translateX(-50%);
  font-size: 12px;
  color: #666;
}

/* Top Products */
.top-products {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.product-item {
  display: flex;
  align-items: center;
  padding: 10px 0;
}

.product-rank {
  width: 28px;
  height: 28px;
  background-color: #f5f5f5;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  margin-right: 15px;
}

.product-info {
  width: 200px;
  margin-right: 15px;
}

.product-name {
  font-weight: 600;
  margin-bottom: 3px;
}

.product-sold {
  font-size: 12px;
  color: #666;
}

.product-bar-container {
  flex: 1;
  height: 8px;
  background-color: #f5f5f5;
  border-radius: 4px;
  overflow: hidden;
}

.product-bar {
  height: 100%;
  background-color: #1976d2;
  border-radius: 4px;
}

/* Bottom Section */
.dashboard-bottom {
  display: flex;
  flex-direction: column;
  gap: 30px;
}

.section-title {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 20px;
  color: #333;
}

/* Quick Actions */
.action-buttons {
  display: flex;
  gap: 20px;
}

.action-button {
  background-color: white;
  border-radius: 12px;
  padding: 20px;
  text-align: center;
  text-decoration: none;
  color: black;
  flex: 1;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
  cursor: pointer;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.action-button:hover {
  transform: translateY(-5px);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.12);
}

.action-icon {
  font-size: 32px;
  color: #000000;
  margin-bottom: 10px;
}

.action-label {
  font-weight: 600;
}

/* Recent Orders */
.orders-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

.view-all-btn {
  background: none;
  border: none;
  color: #1976d2;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 5px;
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
}

.admin-table th,
.admin-table td {
  padding: 15px;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.admin-table th {
  font-weight: 600;
  color: #666;
  background-color: #f9f9f9;
}

.admin-table tr:hover {
  background-color: #f5f5f5;
}

.order-id {
  font-weight: 600;
  color: #1976d2;
}

.order-total {
  font-weight: 600;
}

.status-badge {
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  display: inline-block;
}

.status-delivered {
  background-color: #e8f5e9;
  color: #2e7d32;
}

.status-processing {
  background-color: #e3f2fd;
  color: #1976d2;
}

.status-shipping {
  background-color: #fff3e0;
  color: #e65100;
}

.status-pending {
  background-color: #f3e5f5;
  color: #7b1fa2;
}

.status-canceled {
  background-color: #ffebee;
  color: #c62828;
}

.action-buttons {
  display: flex;
  gap: 10px;
}

.view-button,
.print-button {
  width: 34px;
  height: 34px;
  border-radius: 8px;
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.view-button {
  background-color: #e3f2fd;
  color: #1976d2;
}

.print-button {
  background-color: #e8f5e9;
  color: #2e7d32;
}

.view-button:hover {
  background-color: #bbdefb;
}

.print-button:hover {
  background-color: #c8e6c9;
}

/* Responsive Adjustments */
@media (max-width: 1200px) {
  .dashboard-cards,
  .dashboard-charts {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .dashboard-cards,
  .dashboard-charts {
    grid-template-columns: 1fr;
  }

  .action-buttons {
    flex-wrap: wrap;
  }

  .action-button {
    flex-basis: calc(50% - 10px);
  }
}
</style>
