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
          <div class="card-value">235</div>
          <div class="card-label">Đơn hàng mới</div>
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
          <div class="card-value">78.500.000 VNĐ</div>
          <div class="card-label">Doanh thu tháng</div>
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
          <div class="card-value">548</div>
          <div class="card-label">Khách hàng mới</div>
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
          <div class="card-value">12</div>
          <div class="card-label">Đơn hàng bị hủy</div>
          <div class="card-trend negative">
            <i class="bi bi-arrow-down"></i> 7%
          </div>
        </div>
      </div>
    </div>

    <!-- Middle Section: Charts -->
    <div class="dashboard-charts">
      <div class="chart-container">
        <div class="chart-header">
          <h3 class="chart-title">Doanh thu theo tháng</h3>
          <div class="chart-actions">
            <select class="chart-select">
              <option>Năm 2025</option>
              <option>Năm 2024</option>
            </select>
          </div>
        </div>
        <div class="chart-content">
          <div class="chart-mock">
            <div class="chart-bar active" style="height: 30%">
              <span class="chart-label">T1</span>
            </div>
            <div class="chart-bar active" style="height: 45%">
              <span class="chart-label">T2</span>
            </div>
            <div class="chart-bar active" style="height: 65%">
              <span class="chart-label">T3</span>
            </div>
            <div class="chart-bar active" style="height: 40%">
              <span class="chart-label">T4</span>
            </div>
            <div class="chart-bar" style="height: 55%">
              <span class="chart-label">T5</span>
            </div>
            <div class="chart-bar" style="height: 75%">
              <span class="chart-label">T6</span>
            </div>
            <div class="chart-bar" style="height: 85%">
              <span class="chart-label">T7</span>
            </div>
            <div class="chart-bar" style="height: 60%">
              <span class="chart-label">T8</span>
            </div>
            <div class="chart-bar" style="height: 50%">
              <span class="chart-label">T9</span>
            </div>
            <div class="chart-bar" style="height: 70%">
              <span class="chart-label">T10</span>
            </div>
            <div class="chart-bar" style="height: 65%">
              <span class="chart-label">T11</span>
            </div>
            <div class="chart-bar" style="height: 90%">
              <span class="chart-label">T12</span>
            </div>
          </div>
        </div>
      </div>

      <div class="chart-container" style="display: flex; flex-direction: column; height: 100%;">
        <div class="chart-header">
          <h3 class="chart-title">Sản phẩm bán chạy</h3>
          <div class="chart-actions">
            <select class="chart-select">
              <option>Tháng này</option>
              <option>3 tháng gần đây</option>
            </select>
          </div>
        </div>
        <div class="chart-content" style="height: auto; display: block;">
          <div class="top-products">
            <div class="product-item">
              <div class="product-rank">1</div>
              <div class="product-info">
                <div class="product-name">Áo Thun Poly Basic</div>
                <div class="product-sold">Đã bán: 156</div>
              </div>
              <div class="product-bar-container">
                <div class="product-bar" style="width: 95%"></div>
              </div>
            </div>
            <div class="product-item">
              <div class="product-rank">2</div>
              <div class="product-info">
                <div class="product-name">Quần Jeans Slim Fit</div>
                <div class="product-sold">Đã bán: 123</div>
              </div>
              <div class="product-bar-container">
                <div class="product-bar" style="width: 75%"></div>
              </div>
            </div>
            <div class="product-item">
              <div class="product-rank">3</div>
              <div class="product-info">
                <div class="product-name">Áo Khoác Bomber</div>
                <div class="product-sold">Đã bán: 98</div>
              </div>
              <div class="product-bar-container">
                <div class="product-bar" style="width: 60%"></div>
              </div>
            </div>
            <div class="product-item">
              <div class="product-rank">4</div>
              <div class="product-info">
                <div class="product-name">Áo Sơ Mi Caro</div>
                <div class="product-sold">Đã bán: 87</div>
              </div>
              <div class="product-bar-container">
                <div class="product-bar" style="width: 55%"></div>
              </div>
            </div>
            <div class="product-item">
              <div class="product-rank">5</div>
              <div class="product-info">
                <div class="product-name">Quần Short Kaki</div>
                <div class="product-sold">Đã bán: 76</div>
              </div>
              <div class="product-bar-container">
                <div class="product-bar" style="width: 45%"></div>
              </div>
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
              <td class="order-id">{{ order.id }}</td>
              <td>{{ order.customer }}</td>
              <td>{{ order.date }}</td>
              <td>
                <span :class="'status-badge ' + order.statusClass">
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
    return {
      currentDate: new Date().toLocaleDateString("vi-VN", {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric",
      }),
      recentOrders: [
        {
          id: "#ORD-5426",
          customer: "Phạm Minh Quân",
          date: "25/08/2023",
          status: "Đã giao",
          statusClass: "status-delivered",
          total: "1.850.000 VNĐ",
        },
        {
          id: "#ORD-5425",
          customer: "Ngô Thị Hạnh",
          date: "24/08/2023",
          status: "Đang xử lý",
          statusClass: "status-processing",
          total: "2.150.000 VNĐ",
        },
        {
          id: "#ORD-5424",
          customer: "Vũ Đức Long",
          date: "24/08/2023",
          status: "Đang giao",
          statusClass: "status-shipping",
          total: "3.500.000 VNĐ",
        },
        {
          id: "#ORD-5423",
          customer: "Đặng Thị Mai",
          date: "23/08/2023",
          status: "Chờ xác nhận",
          statusClass: "status-pending",
          total: "950.000 VNĐ",
        },
        {
          id: "#ORD-5422",
          customer: "Lý Quốc Bảo",
          date: "22/08/2023",
          status: "Đã hủy",
          statusClass: "status-canceled",
          total: "1.250.000 VNĐ",
        },
      ],
    };
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
