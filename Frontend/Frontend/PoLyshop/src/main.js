import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min.js";
import "bootstrap-icons/font/bootstrap-icons.css";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import axios from "axios";



// Development mode flag - set to true to use mock data instead of API calls
const isDevelopmentMode = true;

// Configure Axios defaults
axios.defaults.baseURL = isDevelopmentMode ? "" : "http://localhost:5006";
axios.defaults.headers.common["Content-Type"] = "application/json";
axios.defaults.timeout = 30000; // Increase timeout to 30 seconds

// Add request interceptor for better error handling
axios.interceptors.request.use(
  (config) => {
    // Add stack trace for debugging
    const stack = new Error().stack;

    // Handle specific URLs
    if (
      (isDevelopmentMode && config.url.startsWith("/api/"))
    ) {
      console.warn(`API request attempted in development mode:`, {
        url: config.url,
        stack: stack,
      });
      // Return a cancelled request to prevent actual API calls
      return {
        ...config,
        cancelToken: new axios.CancelToken((cancel) =>
          cancel("Development mode: API calls are disabled")
        ),
      };
    }

    // For non-API requests or in production mode, proceed normally
    console.log(`Request to ${config.url}:`, {
      method: config.method,
      headers: config.headers,
      data: config.data,
    });
    return config;
  },
  (error) => {
    // Do something with request error
    console.error("Request error:", error);
    return Promise.reject(error);
  }
);

// Add response interceptor for better error handling
axios.interceptors.response.use(
  (response) => {
    // Any status code within the range of 2xx causes this function to trigger
    console.log(`Response from ${response.config.url}:`, {
      status: response.status,
      data: response.data,
    });
    return response;
  },
  (error) => {
    // Ignore cancelled requests in development mode
    if (axios.isCancel(error) && isDevelopmentMode) {
      console.info(error.message);
      return Promise.resolve({ data: [], status: 200 });
    }

    // Any status codes outside the range of 2xx cause this function to trigger
    if (error.response) {
      // The request was made and the server responded with a status code
      // that falls out of the range of 2xx
      console.error("Response error:", {
        status: error.response.status,
        data: error.response.data,
        headers: error.response.headers,
        url: error.config?.url,
        method: error.config?.method,
      });
    } else if (error.request) {
      // The request was made but no response was received
      console.error("No response received:", {
        request: error.request,
        url: error.config?.url,
        method: error.config?.method,
        timeout: error.config?.timeout,
      });
    } else {
      // Something happened in setting up the request that triggered an Error
      console.error("Request setup error:", error.message);
    }
    return Promise.reject(error);
  }
);



// Global axios configuration to prevent any direct API calls to localhost:7055
// Global axios configuration to prevent any direct API calls to localhost:7055
// (Removed blocking interceptor for localhost:7055)
const app = createApp(App);

app.use(router);

app.mount("#app");
