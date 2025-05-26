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
      <div class="blog-filters mb-4">
        <div class="filter-group">
          <div class="category-filter">
            <label>Danh mục:</label>
            <select v-model="categoryFilter" class="form-select">
              <option value="">Tất cả danh mục</option>
              <option v-for="cat in categories" :key="cat" :value="cat">
                {{ cat }}
              </option>
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

      <div v-else>
        <div class="blog-grid">
          <div v-for="post in filteredPosts" :key="post.id" class="blog-card">
            <div class="blog-image">
              <img :src="post.thumbnail" :alt="post.title" />
              <span class="badge" :class="getStatusClass(post.status)">
                {{ getStatusText(post.status) }}
              </span>
            </div>
            <div class="blog-content">
              <h5 class="blog-title">{{ post.title }}</h5>
              <div class="blog-meta">
                <span><i class="bi bi-person"></i> {{ post.author }}</span>
                <span
                  ><i class="bi bi-calendar3"></i>
                  {{ formatDate(post.createdDate) }}</span
                >
                <span><i class="bi bi-tag"></i> {{ post.category }}</span>
              </div>
              <p class="blog-excerpt">{{ post.excerpt }}</p>
              <div class="blog-actions">
                <button
                  class="btn btn-sm btn-info me-1"
                  @click="viewPost(post)"
                >
                  <i class="bi bi-eye"></i> Xem
                </button>
                <button
                  class="btn btn-sm btn-primary me-1"
                  @click="editPost(post)"
                >
                  <i class="bi bi-pencil"></i> Sửa
                </button>
                <button
                  class="btn btn-sm btn-danger"
                  @click="confirmDeletePost(post)"
                >
                  <i class="bi bi-trash"></i> Xóa
                </button>
              </div>
            </div>
          </div>
        </div>
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
                <label class="form-label">Tiêu đề bài viết</label>
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
                    <option v-for="cat in categories" :key="cat" :value="cat">
                      {{ cat }}
                    </option>
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
                <label class="form-label">Tóm tắt</label>
                <textarea
                  class="form-control"
                  v-model="currentPost.excerpt"
                  rows="2"
                  required
                ></textarea>
              </div>

              <div class="mb-3">
                <label class="form-label">Nội dung</label>
                <textarea
                  class="form-control"
                  v-model="currentPost.content"
                  rows="10"
                  required
                ></textarea>
                <small class="text-muted">
                  Tính năng trình soạn thảo nâng cao sẽ được triển khai sau.
                </small>
              </div>

              <div class="mb-3">
                <label class="form-label">Ảnh đại diện</label>
                <input type="file" class="form-control" />
                <small class="text-muted">
                  Tính năng upload ảnh sẽ được triển khai sau.
                </small>
              </div>

              <div class="mb-3" v-if="isEditing && currentPost.thumbnail">
                <label class="form-label">Ảnh hiện tại</label>
                <div class="current-image">
                  <img
                    :src="currentPost.thumbnail"
                    :alt="currentPost.title"
                    class="img-fluid"
                  />
                </div>
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

    <!-- View Post Modal -->
    <div class="modal fade" id="viewPostModal" tabindex="-1" aria-hidden="true">
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
            <div class="view-post-header">
              <div class="post-meta">
                <div>
                  <strong>Tác giả:</strong> {{ currentPost.author }} |
                  <strong>Ngày tạo:</strong>
                  {{ formatDate(currentPost.createdDate) }} |
                  <strong>Danh mục:</strong> {{ currentPost.category }}
                </div>
                <div>
                  <span
                    class="badge"
                    :class="getStatusClass(currentPost.status)"
                  >
                    {{ getStatusText(currentPost.status) }}
                  </span>
                </div>
              </div>
              <div class="post-thumbnail">
                <img
                  :src="currentPost.thumbnail"
                  :alt="currentPost.title"
                  class="img-fluid"
                />
              </div>
              <div class="post-excerpt">
                <em>{{ currentPost.excerpt }}</em>
              </div>
            </div>
            <hr />
            <div class="post-content">
              <p v-for="(paragraph, index) in contentParagraphs" :key="index">
                {{ paragraph }}
              </p>
            </div>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-primary me-2"
              @click="editPost(currentPost)"
            >
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
            <p>
              Bạn có chắc chắn muốn xóa bài viết
              <strong>"{{ currentPost.title }}"</strong>?
            </p>
            <p class="text-danger">Hành động này không thể hoàn tác.</p>
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

<script>
import { ref, computed, onMounted } from "vue";
import axios from "axios";
import * as bootstrap from "bootstrap";

export default {
  name: "BlogPostView",
  setup() {
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
      content: "",
      excerpt: "",
      author: "",
      category: "Tin tức",
      thumbnail: "",
      createdDate: "",
      status: "draft",
    });

    const categories = ref([
      "Tin tức",
      "Khuyến mãi",
      "Thời trang",
      "Xu hướng",
      "Hướng dẫn",
      "Sự kiện",
    ]);

    // Sample data for development
    const samplePosts = [
      {
        id: 1,
        title: "Xu hướng thời trang mùa hè 2023",
        content:
          "Mùa hè năm nay, chúng ta sẽ thấy sự trở lại của những tone màu rực rỡ, họa tiết nhiệt đới và chất liệu thoáng mát.\n\nVải linen, cotton organic và những chất liệu thân thiện với môi trường đang ngày càng được ưa chuộng.\n\nPhong cách Y2K tiếp tục làm mưa làm gió với những item như quần cạp trễ, áo croptop và váy mini.\n\nMàu sắc chủ đạo của mùa hè này sẽ là xanh dương, hồng pastel và các màu neon.",
        excerpt:
          "Khám phá những xu hướng thời trang nổi bật nhất cho mùa hè 2023.",
        author: "Nguyễn Thị Thảo",
        category: "Xu hướng",
        thumbnail: "https://placehold.co/800x400",
        createdDate: "2023-04-15",
        status: "published",
      },
      {
        id: 2,
        title: "Khuyến mãi lớn nhân dịp khai trương cửa hàng mới",
        content:
          "Nhân dịp khai trương cửa hàng mới tại Quận 7, PoLyShop tưng bừng khuyến mãi với nhiều ưu đãi hấp dẫn.\n\nGiảm giá đến 50% cho tất cả sản phẩm trong tuần khai trương.\n\nTặng voucher 200.000đ cho hóa đơn từ 1.000.000đ.\n\nMua 2 tặng 1 đối với tất cả sản phẩm phụ kiện.\n\nChương trình bốc thăm trúng thưởng với nhiều phần quà giá trị.",
        excerpt:
          "PoLyShop tưng bừng khuyến mãi với nhiều ưu đãi hấp dẫn nhân dịp khai trương cửa hàng mới tại Quận 7.",
        author: "Trần Văn Minh",
        category: "Khuyến mãi",
        thumbnail: "https://placehold.co/800x400",
        createdDate: "2023-05-20",
        status: "published",
      },
      {
        id: 3,
        title: "Cách phối đồ cho nam giới công sở",
        content:
          "Đối với nam giới công sở, việc lựa chọn trang phục vừa lịch sự vừa thoải mái là điều quan trọng.\n\nNhững gam màu trung tính như xám, navy, be sẽ giúp bạn dễ dàng phối đồ và luôn trông chỉn chu.\n\nÁo sơ mi trắng là item không thể thiếu trong tủ đồ của mọi quý ông, có thể kết hợp với quần âu hoặc jeans tùy vào môi trường làm việc.\n\nGiày Oxford hoặc Loafer sẽ là lựa chọn hoàn hảo để hoàn thiện vẻ ngoài chuyên nghiệp.\n\nPhụ kiện như đồng hồ, thắt lưng da sẽ nâng tầm bộ trang phục của bạn.",
        excerpt:
          "Những bí quyết giúp nam giới công sở luôn trông chỉn chu và chuyên nghiệp trong mọi hoàn cảnh.",
        author: "Lê Quang Đạt",
        category: "Hướng dẫn",
        thumbnail: "https://placehold.co/800x400",
        createdDate: "2023-06-10",
        status: "pending",
      },
      {
        id: 4,
        title: "Bộ sưu tập Thu-Đông 2023",
        content:
          "PoLyShop hân hạnh giới thiệu bộ sưu tập Thu-Đông 2023 với nhiều thiết kế độc đáo và thời thượng.\n\nLấy cảm hứng từ phong cách vintage châu Âu, bộ sưu tập mang đến những item ấm áp nhưng không kém phần thời trang.\n\nChất liệu chủ đạo là len, nỉ và dạ với gam màu trầm ấm đặc trưng của mùa thu như nâu, đỏ rượu, xanh rêu.\n\nNổi bật trong bộ sưu tập là những chiếc áo khoác dạ dáng dài, áo len oversized và quần âu cạp cao.\n\nTất cả sản phẩm trong bộ sưu tập đều được thiết kế và sản xuất tại Việt Nam với chất lượng cao.",
        excerpt:
          "Khám phá bộ sưu tập Thu-Đông 2023 với nhiều thiết kế độc đáo lấy cảm hứng từ phong cách vintage châu Âu.",
        author: "Phạm Minh Tú",
        category: "Tin tức",
        thumbnail: "https://placehold.co/800x400",
        createdDate: "2023-08-05",
        status: "draft",
      },
    ];

    const fetchPosts = async () => {
      try {
        loading.value = true;
        // Uncomment when API is ready
        // const response = await axios.get('/api/blog-posts');
        // posts.value = response.data;

        // For development
        setTimeout(() => {
          posts.value = samplePosts;
          loading.value = false;
        }, 500);
      } catch (err) {
        error.value = "Không thể tải dữ liệu bài viết: " + err.message;
        loading.value = false;
      }
    };

    const filteredPosts = computed(() => {
      let result = posts.value;

      // Filter by search term
      if (searchTerm.value) {
        const term = searchTerm.value.toLowerCase();
        result = result.filter(
          (post) =>
            post.title.toLowerCase().includes(term) ||
            post.content.toLowerCase().includes(term) ||
            post.excerpt.toLowerCase().includes(term) ||
            post.author.toLowerCase().includes(term)
        );
      }

      // Filter by category
      if (categoryFilter.value) {
        result = result.filter(
          (post) => post.category === categoryFilter.value
        );
      }

      // Filter by status
      if (statusFilter.value) {
        result = result.filter((post) => post.status === statusFilter.value);
      }

      return result;
    });

    const contentParagraphs = computed(() => {
      if (!currentPost.value.content) return [];
      return currentPost.value.content.split("\n\n");
    });

    const openAddModal = () => {
      isEditing.value = false;
      currentPost.value = {
        id: null,
        title: "",
        content: "",
        excerpt: "",
        author: "Quản trị viên", // Default author
        category: categories.value[0],
        thumbnail: "https://placehold.co/800x400", // Default thumbnail
        createdDate: new Date().toISOString().split("T")[0],
        status: "draft",
      };
      const modal = new bootstrap.Modal(document.getElementById("postModal"));
      modal.show();
    };

    const editPost = (post) => {
      isEditing.value = true;
      currentPost.value = { ...post };

      // Close view modal if it's open
      const viewModal = bootstrap.Modal.getInstance(
        document.getElementById("viewPostModal")
      );
      if (viewModal) {
        viewModal.hide();
      }

      // Show edit modal
      setTimeout(() => {
        const modal = new bootstrap.Modal(document.getElementById("postModal"));
        modal.show();
      }, 500);
    };

    const viewPost = (post) => {
      currentPost.value = { ...post };
      const modal = new bootstrap.Modal(
        document.getElementById("viewPostModal")
      );
      modal.show();
    };

    const confirmDeletePost = (post) => {
      currentPost.value = { ...post };
      const modal = new bootstrap.Modal(document.getElementById("deleteModal"));
      modal.show();
    };

    const savePost = async () => {
      try {
        if (isEditing.value) {
          // Uncomment when API is ready
          // await axios.put(`/api/blog-posts/${currentPost.value.id}`, currentPost.value);

          // For development
          const index = posts.value.findIndex(
            (p) => p.id === currentPost.value.id
          );
          if (index !== -1) {
            posts.value[index] = { ...currentPost.value };
          }
        } else {
          // Uncomment when API is ready
          // const response = await axios.post('/api/blog-posts', currentPost.value);
          // posts.value.push(response.data);

          // For development
          const newId = Math.max(0, ...posts.value.map((p) => p.id)) + 1;
          posts.value.push({
            ...currentPost.value,
            id: newId,
          });
        }

        // Close the modal
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("postModal")
        );
        modal.hide();
      } catch (err) {
        error.value =
          `Không thể ${isEditing.value ? "cập nhật" : "thêm"} bài viết: ` +
          err.message;
      }
    };

    const deletePost = async () => {
      try {
        // Uncomment when API is ready
        // await axios.delete(`/api/blog-posts/${currentPost.value.id}`);

        // For development
        posts.value = posts.value.filter((p) => p.id !== currentPost.value.id);

        // Close the modal
        const modal = bootstrap.Modal.getInstance(
          document.getElementById("deleteModal")
        );
        modal.hide();
      } catch (err) {
        error.value = "Không thể xóa bài viết: " + err.message;
      }
    };

    const formatDate = (dateString) => {
      const options = { year: "numeric", month: "2-digit", day: "2-digit" };
      return new Date(dateString).toLocaleDateString("vi-VN", options);
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
          return "bg-secondary";
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

      // Check if Bootstrap JS is loaded for modals
      if (typeof bootstrap === "undefined") {
        console.error("Bootstrap JS is not loaded. Modals will not work.");
      }
    });

    return {
      posts,
      loading,
      error,
      searchTerm,
      categoryFilter,
      statusFilter,
      categories,
      isEditing,
      currentPost,
      contentParagraphs,
      filteredPosts,
      openAddModal,
      editPost,
      viewPost,
      confirmDeletePost,
      savePost,
      deletePost,
      formatDate,
      getStatusClass,
      getStatusText,
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

.blog-filters {
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

.blog-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
}

.blog-card {
  background: #fff;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.blog-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.15);
}

.blog-image {
  position: relative;
  height: 180px;
}

.blog-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.blog-image .badge {
  position: absolute;
  top: 10px;
  right: 10px;
}

.blog-content {
  padding: 15px;
}

.blog-title {
  margin-bottom: 10px;
  font-weight: bold;
  font-size: 1.1rem;
  height: 2.5rem;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
}

.blog-meta {
  display: flex;
  flex-wrap: wrap;
  font-size: 0.8rem;
  color: #6c757d;
  margin-bottom: 10px;
  gap: 10px;
}
.blog-excerpt {
  height: 4.5rem;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  line-clamp: 3;
  -webkit-box-orient: vertical;
  margin-bottom: 10px;
  font-size: 0.9rem;
  color: #555;
}

.blog-actions {
  display: flex;
  justify-content: flex-end;
}

.view-post-header {
  margin-bottom: 20px;
}

.post-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
  font-size: 0.9rem;
}

.post-thumbnail {
  margin-bottom: 15px;
}

.post-thumbnail img {
  width: 100%;
  max-height: 400px;
  object-fit: cover;
  border-radius: 5px;
}

.post-excerpt {
  font-size: 1.1rem;
  margin-bottom: 20px;
  color: #555;
}

.post-content {
  line-height: 1.6;
}

.current-image {
  max-width: 200px;
  margin-top: 10px;
  border: 1px solid #ddd;
  padding: 5px;
  border-radius: 4px;
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
