<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý sản phẩm</h2>

      <div class="header-actions">
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
      </div>
    </div>

    <div class="action-row">
      <button class="add-button">
        <i class="bi bi-plus"></i>
      </button>

      <button class="filter-button">Lọc</button>
    </div>

    <table class="admin-table">
      <thead>
        <tr>
          <th>ID</th>
          <th>ID sản phẩm</th>
          <th>Tên sản phẩm</th>
          <th>Ngày tạo</th>
          <th>Loại sản phẩm</th>
          <th>Thao tác</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, index) in products" :key="index">
          <td>{{ item.id }}</td>
          <td>{{ item.productId }}</td>
          <td>{{ item.name }}</td>
          <td>{{ item.createdAt }}</td>
          <td>{{ item.category }}</td>
          <td>
            <div class="action-buttons">
              <button class="edit-button">
                <i class="bi bi-pencil"></i>
              </button>
              <button class="delete-button">
                <i class="bi bi-trash"></i>
              </button>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
export default {
  name: "AdminProductView",
  data() {
    return {
      products: [
        {
          id: 1,
          productId: "Value 1",
          name: "Value 2",
          createdAt: "value 3",
          category: "Value 4",
        },
        {
          id: 2,
          productId: "Value 2",
          name: "Value 3",
          createdAt: "Value 4",
          category: "Value5",
        },
        {
          id: 1,
          productId: "Value 1",
          name: "Value 2",
          createdAt: "value 3",
          category: "Value 4",
        },
      ],
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
  margin-bottom: 30px;
}

.page-title {
  font-size: 24px;
  font-weight: bold;
}

.header-actions {
  display: flex;
  gap: 20px;
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

.action-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 20px;
}

.add-button {
  width: 40px;
  height: 40px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 50%;
  font-size: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.filter-button {
  background-color: #ffc107;
  color: #333;
  border: none;
  border-radius: 20px;
  padding: 8px 20px;
  font-weight: bold;
  cursor: pointer;
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
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

.action-buttons {
  display: flex;
  gap: 10px;
  justify-content: center;
}

.edit-button {
  background-color: #ffeb3b;
  color: #333;
  border: none;
  border-radius: 50%;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.delete-button {
  background-color: #f44336;
  color: white;
  border: none;
  border-radius: 50%;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}
</style>
