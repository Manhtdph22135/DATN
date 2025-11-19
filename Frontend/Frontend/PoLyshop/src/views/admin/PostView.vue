<template>
  <div class="admin-page">
    <div class="page-header">
      <h2 class="page-title">Quản lý bài viết</h2>
      <div class="header-actions">
        <div class="search-container">
          <input
            type="text"
            class="search-input"
            placeholder="Tìm kiếm bài viết..."
            v-model="searchTerm"
          />
          <i class="bi bi-search search-icon"></i>
        </div>
        <button class="btn btn-success" @click="openAddModal">
          <i class="bi bi-plus"></i> Thêm bài viết
        </button>
      </div>
    </div>

    <div class="content-container">
      <div class="post-filters mb-4">
        <div class="filter-group">
          <div class="category-filter">
            <label>Danh mục:</label>
            <select v-model="categoryFilter" class="form-select">
              <option value="">Tất cả danh mục</option>
              <option value="tin-tuc">Tin tức</option>
              <option value="khuyen-mai">Khuyến mãi</option>
              <option value="huong-dan">Hướng dẫn mua hàng</option>
              <option value="thoi-trang">Thời trang</option>
            </select>
          </div>
          <div class="status-filter">
            <label>Trạng thái:</label>
            <select v-model="statusFilter" class="form-select">
              <option value="">Tất cả trạng thái</option>
              <option value="published">Đã xuất bản</option>
              <option value="draft">Bản nháp</option>
              <option value="pending">Chờ duyệt</option>
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

      <div v-else-if="filteredPosts.length === 0" class="text-center p-5">
        <p>Không tìm thấy bài viết nào.</p>
      </div>

      <div v-else class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Tiêu đề</th>
              <th>Danh mục</th>
              <th>Tác giả</th>
              <th>Ngày tạo</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="post in filteredPosts" :key="post.id">
              <td>{{ post.id }}</td>
              <td>
                <div class="post-title-cell">
                  <img
                    v-if="post.thumbnail"
                    :src="post.thumbnail"
                    :alt="post.title"
                    class="post-thumbnail"
                  />
                  <span class="post-title">{{ post.title }}</span>
                </div>
              </td>
              <td>{{ getCategoryName(post.category) }}</td>
              <td>{{ post.author }}</td>
              <td>{{ formatDate(post.createdAt) }}</td>
              <td>
                <span class="badge" :class="getStatusClass(post.status)">
                  {{ getStatusText(post.status) }}
                </span>
              </td>
              <td class="action-buttons">
                <button
                  class="btn btn-sm btn-info me-1"
                  @click="viewPost(post)"
                  title="Xem chi tiết"
                >
                  <i class="bi bi-eye"></i>
                </button>
                <button
                  class="btn btn-sm btn-primary me-1"
                  @click="editPost(post)"
                  title="Chỉnh sửa"
                >
                  <i class="bi bi-pencil"></i>
                </button>
                <button
                  class="btn btn-sm btn-danger"
                  @click="confirmDeletePost(post)"
                  title="Xóa"
                >
                  <i class="bi bi-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add/Edit Post Modal -->
    <div class="modal fade" id="postModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ isEditing ? "Chỉnh sửa bài viết" : "Thêm bài viết mới" }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="savePost">
              <div class="mb-3">
                <label class="form-label">Tiêu đề</label>
                <input
                  type="text"
                  class="form-control"
                  v-model="currentPost.title"
                  required
                />
              </div>
              <div class="row mb-3">
                <div class="col-md-6">
                  <label class="form-label">Danh mục</label>
                  <select
                    class="form-select"
                    v-model="currentPost.category"
                    required
                  >
                    <option value="tin-tuc">Tin tức</option>
                    <option value="khuyen-mai">Khuyến mãi</option>
                    <option value="huong-dan">Hướng dẫn mua hàng</option>
                    <option value="thoi-trang">Thời trang</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Trạng thái</label>
                  <select class="form-select" v-model="currentPost.status">
                    <option value="published">Đã xuất bản</option>
                    <option value="draft">Bản nháp</option>
                    <option value="pending">Chờ duyệt</option>
                  </select>
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label">Ảnh đại diện</label>
                <input
                  type="text"
                  class="form-control"
                  placeholder="URL ảnh đại diện"
                  v-model="currentPost.thumbnail"
                />
                <small class="text-muted">Nhập URL ảnh hoặc để trống</small>
              </div>
              <div class="mb-3">
                <label class="form-label">Ảnh chi tiết</label>
                <input
                  type="text"
                  class="form-control"
                  placeholder="URL ảnh chi tiết"
                  v-model="currentPost.image"
                />
                <small class="text-muted">Nhập URL ảnh chi tiết hoặc để trống</small>
              </div>
              <div class="mb-3">
                <label class="form-label">Tóm tắt</label>
                <textarea
                  class="form-control"
                  v-model="currentPost.summary"
                  rows="2"
                ></textarea>
              </div>
              <div class="mb-3">
                <label class="form-label">Nội dung</label>
                <textarea
                  class="form-control"
                  v-model="currentPost.content"
                  rows="6"
                  required
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
                <button type="submit" class="btn btn-primary">
                  {{ isEditing ? "Cập nhật" : "Thêm" }}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- View Post Modal -->
    <div class="modal fade" id="postViewModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ currentPost.title }}</h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <div class="post-preview">
              <div class="post-meta">
                <div>
                  <strong>Danh mục:</strong>
                  {{ getCategoryName(currentPost.category) }}
                </div>
                <div><strong>Tác giả:</strong> {{ currentPost.author }}</div>
                <div>
                  <strong>Ngày tạo:</strong>
                  {{ formatDate(currentPost.createdAt) }}
                </div>
                <div>
                  <strong>Trạng thái:</strong>
                  <span
                    class="badge"
                    :class="getStatusClass(currentPost.status)"
                  >
                    {{ getStatusText(currentPost.status) }}
                  </span>
                </div>
              </div>

              <div class="post-summary my-3">
                <strong>Tóm tắt:</strong>
                <p>{{ currentPost.summary }}</p>
              </div>

              <div v-if="currentPost.thumbnail" class="text-center mb-3">
                <img
                  :src="currentPost.thumbnail"
                  :alt="currentPost.title"
                  class="img-fluid post-image"
                />
              </div>

              <div v-if="currentPost.image && currentPost.image !== currentPost.thumbnail" class="text-center mb-3">
                <img
                  :src="currentPost.image"
                  :alt="currentPost.title"
                  class="img-fluid post-image"
                />
              </div>

              <div class="post-content">
                <strong>Nội dung:</strong>
                <div class="content-preview border p-3 mt-2">
                  {{ currentPost.content }}
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-primary me-2" @click="editPost(currentPost)">
              <i class="bi bi-pencil"></i> Sửa
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
            <p>Bạn có chắc chắn muốn xóa bài viết này?</p>
            <p class="text-danger fw-bold">{{ currentPost.title }}</p>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              Hủy
            </button>
            <button type="button" class="btn btn-danger" @click="deletePost">
              Xóa
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import axios from "axios";
import bootstrap from "@/utils/bootstrapHelper";

const posts = ref([]);
const loading = ref(true);
const error = ref(null);
const searchTerm = ref("");
const categoryFilter = ref("");
const statusFilter = ref("");
const isEditing = ref(false);
const currentPost = ref({
  id: null,
  title: "",
  summary: "",
  content: "",
  author: "",
  category: "tin-tuc",
  status: "draft",
  thumbnail: "",
  image: "",
  date: "",
  createdAt: "",
});

const fetchPosts = async () => {
  try {
    loading.value = true;
    error.value = null;
    
    console.log('🔄 Đang gọi API News...');
    
    const response = await axios.get('https://localhost:7055/api/News');
    console.log('✅ Data nhận được:', response.data);
    
    // FIX: Xử lý cấu trúc Entity Framework ($values)
    let postsData = response.data;
    
    // Nếu có $values (Entity Framework serialization)
    if (postsData && postsData.$values && Array.isArray(postsData.$values)) {
      console.log('🔧 Entity Framework structure detected, using $values');
      postsData = postsData.$values;
    }
    // Nếu không phải array, chuyển thành array hoặc dùng array rỗng
    else if (!Array.isArray(postsData)) {
      console.warn('⚠️ API trả về không phải array, chuyển đổi...');
      
      // Nếu là object có dữ liệu, chuyển thành array
      if (postsData && typeof postsData === 'object') {
        // Thử lấy mảng từ property nào đó
        if (postsData.data && Array.isArray(postsData.data)) {
          postsData = postsData.data;
        } else {
          // Nếu là single object, wrap thành array
          postsData = [postsData];
        }
      } else {
        postsData = [];
      }
    }
    
    console.log('📊 Data sau khi xử lý:', postsData);
    
    // Filter out null records và map data structure chính xác
    posts.value = postsData
      .filter(post => post && post.id && post.title) // Loại bỏ record NULL
      .map(post => ({
        id: post.id,
        title: post.title || 'Không có tiêu đề',
        summary: post.summary || '',
        content: post.content || '',
        author: post.author || 'Unknown',
        category: post.category || 'tin-tuc',
        status: post.status || 'draft',
        thumbnail: post.thumbnail || '',
        image: post.image || '',
        date: post.date || '',
        createdAt: post.createdAt || new Date().toISOString()
      }));
    
    console.log('📝 Posts sau khi xử lý:', posts.value);
    console.log('🔢 Số lượng bài viết:', posts.value.length);
    
  } catch (err) {
    console.error('❌ Lỗi API:', err);
    error.value = "Không thể tải dữ liệu bài viết: " + err.message;
    
    if (err.response) {
      error.value += ` (HTTP ${err.response.status})`;
      console.error('📊 Response data:', err.response.data);
    } else if (err.request) {
      error.value += " (Không thể kết nối đến server)";
    }
    
    posts.value = [];
  } finally {
    loading.value = false;
  }
};

const filteredPosts = computed(() => {
  let result = posts.value;

  if (searchTerm.value) {
    const term = searchTerm.value.toLowerCase();
    result = result.filter(
      (post) =>
        post.title.toLowerCase().includes(term) ||
        (post.summary && post.summary.toLowerCase().includes(term)) ||
        (post.author && post.author.toLowerCase().includes(term))
    );
  }

  if (categoryFilter.value) {
    result = result.filter((post) => post.category === categoryFilter.value);
  }

  if (statusFilter.value) {
    result = result.filter((post) => post.status === statusFilter.value);
  }

  return result;
});

const openAddModal = () => {
  isEditing.value = false;
  currentPost.value = {
    id: null,
    title: "",
    summary: "",
    content: "",
    author: "Admin",
    category: "tin-tuc",
    status: "draft",
    thumbnail: "",
    image: "",
    date: new Date().toISOString(),
    createdAt: new Date().toISOString(),
  };
  const modal = new bootstrap.Modal(document.getElementById("postModal"));
  modal.show();
};

const editPost = (post) => {
  isEditing.value = true;
  currentPost.value = { 
    ...post,
    thumbnail: post.thumbnail || '',
    image: post.image || '',
    date: post.date || post.createdAt || new Date().toISOString()
  };

  const viewModal = bootstrap.Modal.getInstance(
    document.getElementById("postViewModal")
  );
  if (viewModal) {
    viewModal.hide();
  }

  setTimeout(() => {
    const modal = new bootstrap.Modal(document.getElementById("postModal"));
    modal.show();
  }, 500);
};

const viewPost = (post) => {
  currentPost.value = { 
    ...post,
    thumbnail: post.thumbnail || '',
    image: post.image || ''
  };
  const modal = new bootstrap.Modal(document.getElementById("postViewModal"));
  modal.show();
};

const confirmDeletePost = (post) => {
  currentPost.value = { ...post };
  const modal = new bootstrap.Modal(document.getElementById("deleteModal"));
  modal.show();
};

const deletePost = async () => {
  try {
    await axios.delete(`https://localhost:7055/api/News/${currentPost.value.id}`);
    await fetchPosts();
    const modal = bootstrap.Modal.getInstance(document.getElementById("deleteModal"));
    modal.hide();
    alert("Xóa bài viết thành công!");
  } catch (err) {
    error.value = "Không thể xóa bài viết: " + err.message;
    console.error("Error deleting post:", err);
    alert("Lỗi khi xóa bài viết: " + err.message);
  }
};

const savePost = async () => {
  try {
    // Chuẩn bị data để gửi lên API
    const postData = {
      title: currentPost.value.title,
      summary: currentPost.value.summary,
      content: currentPost.value.content,
      author: currentPost.value.author,
      category: currentPost.value.category,
      status: currentPost.value.status,
      thumbnail: currentPost.value.thumbnail,
      image: currentPost.value.image,
      date: currentPost.value.date || new Date().toISOString(),
      createdAt: currentPost.value.createdAt || new Date().toISOString()
    };

    console.log('📤 Data gửi lên API:', postData);

    if (isEditing.value) {
      // Thêm ID khi update
      postData.id = currentPost.value.id;
      await axios.put(`https://localhost:7055/api/News/${currentPost.value.id}`, postData);
    } else {
      await axios.post('https://localhost:7055/api/News', postData);
    }

    await fetchPosts();
    const modal = bootstrap.Modal.getInstance(document.getElementById("postModal"));
    modal.hide();
    
    alert(isEditing.value ? "Cập nhật bài viết thành công!" : "Thêm bài viết thành công!");
    
  } catch (err) {
    const errorMsg = `Không thể ${isEditing.value ? "cập nhật" : "thêm"} bài viết: ` + err.message;
    error.value = errorMsg;
    console.error("Error saving post:", err);
    alert(errorMsg);
    
    if (err.response) {
      console.error("Response error:", err.response.data);
    }
  }
};

const formatDate = (dateString) => {
  if (!dateString) return "N/A";
  try {
    const options = {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
    };
    return new Date(dateString).toLocaleDateString("vi-VN", options);
  } catch (e) {
    return "Invalid Date";
  }
};

const getCategoryName = (categoryKey) => {
  const categories = {
    "tin-tuc": "Tin tức",
    "khuyen-mai": "Khuyến mãi",
    "huong-dan": "Hướng dẫn mua hàng",
    "thoi-trang": "Thời trang",
  };
  return categories[categoryKey] || categoryKey;
};

const getStatusClass = (status) => {
  switch (status) {
    case "published":
      return "bg-success";
    case "draft":
      return "bg-secondary";
    case "pending":
      return "bg-warning";
    default:
      return "bg-info";
  }
};

const getStatusText = (status) => {
  switch (status) {
    case "published":
      return "Đã xuất bản";
    case "draft":
      return "Bản nháp";
    case "pending":
      return "Chờ duyệt";
    default:
      return "Không xác định";
  }
};

onMounted(() => {
  fetchPosts();
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

.post-filters {
  display: flex;
  margin-bottom: 20px;
}

.filter-group {
  display: flex;
  gap: 15px;
  align-items: flex-end;
  flex-wrap: wrap;
}

.category-filter,
.status-filter {
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

.post-title-cell {
  display: flex;
  align-items: center;
}

.post-thumbnail {
  width: 40px;
  height: 40px;
  object-fit: cover;
  border-radius: 4px;
  margin-right: 10px;
}

.post-title {
  font-weight: 500;
}

.post-image {
  max-height: 300px;
  object-fit: contain;
}

.post-meta {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
  margin-bottom: 15px;
}

.content-preview {
  max-height: 300px;
  overflow-y: auto;
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

  .filter-group {
    flex-direction: column;
    align-items: stretch;
  }

  .post-meta {
    grid-template-columns: 1fr;
  }
}
</style>