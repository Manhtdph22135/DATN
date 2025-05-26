<template>
  <div class="admin-page">
    <div class="user-dropdown">
      <div class="dropdown">
        <button class="user-button">
          <i class="bi bi-person-circle"></i>
          <i class="bi bi-chevron-down"></i>
        </button>
      </div>
    </div>

    <h2 class="page-title">Thống Kê</h2>

    <div class="statistics-cards">
      <div class="statistic-card">
        <div class="statistic-value">200.000.000 VND</div>
        <div class="statistic-label">Tổng Doanh Thu</div>
      </div>
      <div class="statistic-card">
        <div class="statistic-value">4.567</div>
        <div class="statistic-label">Tổng Sản Phẩm đã bán</div>
      </div>
      <div class="statistic-card">
        <div class="statistic-value">567</div>
        <div class="statistic-label">Tổng Hoá Đơn đã bán</div>
      </div>
    </div>

    <div class="filters-row">
      <div class="search-container">
        <input type="text" class="search-input" placeholder="Tìm kiếm..." />
        <div class="search-icon">
          <i class="bi bi-search"></i>
        </div>
      </div>

      <div class="date-filters">
        <div class="date-filter">
          <span>Từ Ngày:</span>
          <div class="date-selector">
            <button class="date-nav-btn">
              <i class="bi bi-chevron-left"></i>
            </button>
            <span class="month">Mo</span>
            <span class="month-name">January</span>
            <span class="year">2023</span>
            <button class="date-nav-btn">
              <i class="bi bi-chevron-right"></i>
            </button>
          </div>
        </div>

        <div class="date-filter">
          <span>Đến Ngày:</span>
          <div class="date-selector">
            <button class="date-nav-btn">
              <i class="bi bi-chevron-left"></i>
            </button>
            <span class="month">Mo</span>
            <span class="month-name">January</span>
            <span class="year">2023</span>
            <button class="date-nav-btn">
              <i class="bi bi-chevron-right"></i>
            </button>
          </div>
        </div>
      </div>

      <button class="filter-button">Lọc</button>
    </div>

    <table class="admin-table">
      <thead>
        <tr>
          <th>ID</th>
          <th>Loại sản Phẩm</th>
          <th>Tên Sản Phẩm</th>
          <th>Giá</th>
          <th>Số Lượng</th>
          <th>Trạng thái</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, index) in products" :key="index">
          <td>{{ item.id }}</td>
          <td>{{ item.category }}</td>
          <td>{{ item.name }}</td>
          <td>{{ item.price }}</td>
          <td>{{ item.quantity }}</td>
          <td>{{ item.status }}</td>
        </tr>
      </tbody>
    </table>

    <!-- Chart section - In a real app we would use a charting library like Chart.js -->
    <div class="chart-section">
      <h3 class="section-title">Doanh thu theo thời gian</h3>
      <div class="chart-placeholder">
        <!-- This would be replaced by an actual chart -->
        <div class="chart-info">
          <p>Chart would be displayed here</p>
          <p>Consider using Chart.js or another charting library</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "AdminStatisticsView",
  data() {
    return {
      products: [
        {
          id: 1,
          category: "Value 1",
          name: "Value 2",
          price: "value 3",
          quantity: "Value 4",
          status: "Value 5",
        },
        {
          id: 2,
          category: "Value 2",
          name: "Value 3",
          price: "Value 4",
          quantity: "Value5",
          status: "Value 6",
        },
      ],
    };
  },
};
</script>

<style scoped>
.admin-page {
  padding: 20px;
  position: relative;
}

.user-dropdown {
  position: absolute;
  top: 20px;
  right: 20px;
}

.user-button {
  background: none;
  border: none;
  font-size: 18px;
  display: flex;
  align-items: center;
  gap: 5px;
  cursor: pointer;
}

.page-title {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 30px;
  text-align: center;
}

.statistics-cards {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
  margin-bottom: 30px;
}

.statistic-card {
  background-color: white;
  border-radius: 8px;
  padding: 20px;
  text-align: center;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.statistic-value {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 10px;
}

.statistic-label {
  color: #666;
}

.filters-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.search-container {
  position: relative;
  width: 400px;
}

.search-input {
  width: 100%;
  padding: 10px 15px;
  border: none;
  border-radius: 20px;
  background-color: #f1f1f1;
}

.search-icon {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
}

.date-filters {
  display: flex;
  gap: 20px;
}

.date-filter {
  display: flex;
  align-items: center;
  gap: 10px;
}

.date-selector {
  display: flex;
  align-items: center;
  gap: 5px;
}

.date-nav-btn {
  background: none;
  border: none;
  cursor: pointer;
}

.filter-button {
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 20px;
  padding: 8px 20px;
  font-weight: bold;
  cursor: pointer;
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 30px;
}

.admin-table th,
.admin-table td {
  border: 1px solid #dee2e6;
  padding: 12px;
  text-align: center;
}

.admin-table th {
  background-color: #f8f9fa;
}

.chart-section {
  margin-top: 40px;
}

.section-title {
  font-size: 18px;
  font-weight: bold;
  margin-bottom: 20px;
}

.chart-placeholder {
  width: 100%;
  height: 300px;
  background-color: #f8f9fa;
  border: 1px dashed #ccc;
  display: flex;
  justify-content: center;
  align-items: center;
  text-align: center;
}

.chart-info p {
  margin: 5px 0;
  color: #666;
}
</style>
