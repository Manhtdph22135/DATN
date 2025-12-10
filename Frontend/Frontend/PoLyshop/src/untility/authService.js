import axios from "axios";
import { ref } from "vue";

// --- 1. TRẠNG THÁI (STATE) ---
// Auth state that can be watched by components
export const authState = {
  isLoggedIn: ref(false),
  user: ref(null),
};

// --- 2. CẤU HÌNH API ---
const API_BASE_URL = "https://localhost:7055/api/Auth"; // Kiểm tra đúng Port của bạn
const AUTH_ENDPOINTS = {
  REGISTER: `${API_BASE_URL}/register`,
  LOGIN: `${API_BASE_URL}/login`,
};

// --- 3. KHỞI TẠO (INITIALIZATION) ---
// Initialize auth state from storage on load
function initializeAuthState() {
  const token =
    localStorage.getItem("userToken") || sessionStorage.getItem("userToken");
  const userData =
    localStorage.getItem("userData") || sessionStorage.getItem("userData");

  if (token && userData) {
    authState.isLoggedIn.value = true;
    try {
      authState.user.value = JSON.parse(userData);
    } catch (e) {
      console.error("Error parsing user data:", e);
      authState.user.value = null;
    }
  }
}

// Run initialization immediately
initializeAuthState();

// --- 4. SERVICE CHÍNH ---
export const authService = {
  // === LOGIN ===
  async login(username, password) {
    try {
      console.log("Login attempt with:", { Username: username });
      
      // [QUAN TRỌNG] Đã sửa: Gửi 'Password' thay vì 'PasswordHash'
      const response = await axios.post(AUTH_ENDPOINTS.LOGIN, {
        Username: username,
        Password: password, // Khớp với property trong LoginDOT của C#
      });

      console.log("Login response:", response.data);

      // Tạo object user từ response
      const userData = {
        username: username,
        token: response.data.token,
        fullName: response.data.fullName || username,
        roleId: response.data.roleId,
        accountId: response.data.accountId,
        message: response.data.message || "Login successful!",
      };

      // Cập nhật state (chưa lưu vào storage, việc lưu do component quyết định qua hàm saveUserData)
      authState.isLoggedIn.value = true;
      authState.user.value = userData;

      return userData;
    } catch (error) {
      console.error(
        "Login error details:",
        error.response || error.message || error
      );
      throw error;
    }
  },

  // === REGISTER ===
  async register(userData) {
    try {
      console.log("Register attempt with:", userData);

      // --- XỬ LÝ TỰ ĐỘNG TẠO USERNAME TỪ EMAIL ---
      // Ví dụ: quangduc@gmail.com -> Lấy "quangduc"
      // 1. Lấy Email (Chấp nhận cả viết Hoa và Thường)
    let rawEmail = userData.Email || userData.email; 

    // 2. Xử lý Username
    let autoUsername = "";
    
    // Kiểm tra trên biến rawEmail vừa lấy được
    if (rawEmail && rawEmail.includes("@")) {
        autoUsername = rawEmail.split("@")[0];
    } else {
        // Fallback: Nếu không có email thì lấy random
        // Cũng check luôn userData.Username hoặc userData.username
        let rawName = userData.Username || userData.username;
        autoUsername = rawName || "user" + Math.floor(Math.random() * 1000);
    }

      // Mapping dữ liệu
      // --- XỬ LÝ CHUYỂN ĐỔI DỮ LIỆU ---
      
      // 2. Xử lý Giới tính
      // Kiểm tra kỹ cả trường hợp viết Hoa và viết Thường
      let rawSex = userData.Sex || userData.sex; // Lấy giá trị (ưu tiên viết hoa trước)
      let genderValue = true; 
      if (rawSex == "Nữ" || rawSex == "Nu" || rawSex == "Female" || rawSex == "0" || rawSex === false) {
          genderValue = false;
      }

      // 3. Xử lý Ngày sinh
      let rawDob = userData.Dob || userData.dob; // Lấy giá trị
      let dobValue = null;
      if (rawDob) {
          dobValue = new Date(rawDob).toISOString().split('T')[0]; 
      } else {
          dobValue = new Date().toISOString().split('T')[0];
      }

      // === MAPPING DỮ LIỆU (SỬA QUAN TRỌNG Ở ĐÂY) ===
      // Phải dùng userData.Password (Viết hoa) để lấy được dữ liệu từ form của bạn
      const registerData = {
        // Account info
        Username: autoUsername, 
        Password: userData.Password || userData.password, // <--- SỬA: Thêm userData.Password
        ConfirmPassword: userData.ConfirmPassword || userData.confirmPassword, // <--- SỬA
        RoleId: userData.RoleId || userData.roleId || 3,

        // Customer info
        Ho: userData.Ho || userData.ho || "",
        Ten: userData.Ten || userData.ten || "",
        Email: userData.Email || userData.email,       // <--- SỬA
        Phone: userData.Phone || userData.phone || "", // <--- SỬA
        
        Sex: genderValue,
        Dob: dobValue,
        
        Address: userData.Address || userData.address || "",
      };

      // Log ra để kiểm tra lần cuối trước khi gửi
      console.log("Payload chuẩn bị gửi đi:", JSON.stringify(registerData));

      const response = await axios.post(AUTH_ENDPOINTS.REGISTER, registerData);
      console.log("Register successful:", response.data);
      return response.data;
    } catch (error) {
      console.error(
        "Register error details:",
        error.response || error.message || error
      );
      const errorMessage =
        error.response?.data?.message ||
        error.response?.data?.errors?.Username?.[0] ||
        "Đăng ký thất bại. Vui lòng kiểm tra lại thông tin.";
      throw new Error(errorMessage);
    }
  },

  // === LƯU TRỮ (STORAGE) ===
  saveUserData(userData, rememberMe = false) {
    const storage = rememberMe ? localStorage : sessionStorage;
    
    // Xóa sạch storage cũ trước khi lưu mới để tránh xung đột
    localStorage.removeItem("userToken");
    localStorage.removeItem("userData");
    sessionStorage.removeItem("userToken");
    sessionStorage.removeItem("userData");

    storage.setItem("userToken", userData.token);
    storage.setItem("userData", JSON.stringify(userData));

    // Cập nhật lại state lần nữa để chắc chắn
    authState.isLoggedIn.value = true;
    authState.user.value = userData;

    console.log("User data saved to:", rememberMe ? "LocalStorage" : "SessionStorage");
  },

  // === LOGOUT ===
  logout() {
    localStorage.removeItem("userToken");
    localStorage.removeItem("userData");
    sessionStorage.removeItem("userToken");
    sessionStorage.removeItem("userData");

    authState.isLoggedIn.value = false;
    authState.user.value = null;
    
    // Có thể reload trang hoặc redirect tại đây nếu cần
    // window.location.href = '/login'; 
  },

  // === HELPER METHODS ===
  isAuthenticated() {
    const token =
      localStorage.getItem("userToken") || sessionStorage.getItem("userToken");
    return !!token;
  },

  getUserData() {
    const userData =
      localStorage.getItem("userData") || sessionStorage.getItem("userData");
    return userData ? JSON.parse(userData) : null;
  },

  getToken() {
    return (
      localStorage.getItem("userToken") || sessionStorage.getItem("userToken")
    );
  },
};

// --- 5. AXIOS INTERCEPTOR ---
// Tự động gắn Token vào mỗi request gửi đi
axios.interceptors.request.use(
  (config) => {
    const token = authService.getToken();
    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default authService;