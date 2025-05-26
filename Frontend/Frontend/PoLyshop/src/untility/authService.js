// authService.js - Centralized authentication service
import axios from "axios";
import { ref } from "vue";

// Auth state that can be watched by components
export const authState = {
  isLoggedIn: ref(false),
  user: ref(null),
};

// The API endpoints for authentication
const AUTH_ENDPOINTS = {
  REGISTER: "/api/Auth/register",
  LOGIN: "/api/Auth/login",
};

// Initialize auth state from storage
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

// Initialize on import
initializeAuthState();

export const authService = {
  // Login method
  async login(username, password) {
    try {
      console.log("Login attempt with:", { Username: username });
      // Backend expects Username and PasswordHash properties
      const response = await axios.post(AUTH_ENDPOINTS.LOGIN, {
        Username: username,
        PasswordHash: password, // Server will check this against the stored hash
      });

      console.log("Login response:", response.data);

      // Create a user object with the response data
      const userData = {
        username: username,
        token: response.data.token || "dummy-token",
        message: response.data.message || "Login successful!",
      };

      // Update the auth state
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

  // Register method
  async register(userData) {
    try {
      console.log("Register attempt with:", userData);
      // Transform the userData to match what the backend expects
      const registerData = {
        Username: userData.userName,
        Password: userData.password,
        ConfirmPassword: userData.confirmPassword,
        RoleId: 3, // Default to customer role
      };

      const response = await axios.post(AUTH_ENDPOINTS.REGISTER, registerData);
      console.log("Register response:", response.data);
      return response.data;
    } catch (error) {
      console.error(
        "Register error details:",
        error.response || error.message || error
      );
      throw error;
    }
  },

  // Save user data to storage
  saveUserData(userData, rememberMe = false) {
    const storage = rememberMe ? localStorage : sessionStorage;
    storage.setItem("userToken", userData.token);
    storage.setItem("userData", JSON.stringify(userData));

    // Update the auth state
    authState.isLoggedIn.value = true;
    authState.user.value = userData;

    console.log("User data saved:", userData);
  },

  // Logout method - clear storage
  logout() {
    localStorage.removeItem("userToken");
    localStorage.removeItem("userData");
    sessionStorage.removeItem("userToken");
    sessionStorage.removeItem("userData");

    // Update the auth state
    authState.isLoggedIn.value = false;
    authState.user.value = null;
  },

  // Check if user is authenticated
  isAuthenticated() {
    const token =
      localStorage.getItem("userToken") || sessionStorage.getItem("userToken");
    return !!token;
  },

  // Get authenticated user data
  getUserData() {
    const userData =
      localStorage.getItem("userData") || sessionStorage.getItem("userData");
    return userData ? JSON.parse(userData) : null;
  },

  // Get authentication token
  getToken() {
    return (
      localStorage.getItem("userToken") || sessionStorage.getItem("userToken")
    );
  },
};

// Set axios default Authorization header when service is imported
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
