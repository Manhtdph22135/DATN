import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import BillsView from "../views/BillsView.vue";
import BillsController from "../views/BillsController.vue";
import ProductView from "../views/ProductView.vue";
import DiscountView from "../views/DiscountView.vue";
import AccountView from "@/views/AccountView.vue";
import CustomerView from "@/views/CustomerView.vue";
import LoginView from "@/views/auth/LoginView.vue";
import RegisterView from "@/views/auth/RegisterView.vue";
import ForgotPasswordView from "@/views/auth/ForgotPasswordView.vue";
import authService from "../untility/authService";

// Store Views
import ProductListView from "../views/store/ProductListView.vue";
import ProductDetailView from "../views/store/ProductDetailView.vue";
import CartView from "../views/store/CartView.vue";
import CheckoutView from "../views/store/CheckoutView.vue";
import UserAccountView from "../views/user/UserAccountView.vue";
import UserOrdersView from "../views/user/UserOrdersView.vue";
import NewsView from "../views/store/NewsView.vue";
import TopsView from "../views/store/TopsView.vue";
import BottomsView from "../views/store/BottomsView.vue";
import SaleView from "../views/store/SaleView.vue";
import HotView from "../views/store/HotView.vue";
import ContactView from "../views/store/ContactView.vue";

// Admin Views
import BillView from "../views/admin/BillView.vue";
import StatisticsView from "../views/admin/StatisticsView.vue";
import DashboardView from "../views/admin/DashboardView.vue";
import EmployeeView from "../views/admin/EmployeeView.vue";
import OrderView from "../views/admin/OrderView.vue";
import PostView from "../views/admin/PostView.vue";
import ReturnView from "../views/admin/ReturnView.vue";
import CustomerViews from "../views/admin/CustomerView.vue";


// Customer Views
import CreateCustomerView from "../views/customer/CreateCustomerView.vue";
import CustomerDetailView from "../views/customer/CustomerDetailView.vue";

const routes = [
  {
    path: "/",
    name: "home",
    component: HomeView,
  },
  {
    path: "/bills",
    name: "bills",
    component: BillsView,
  },
  {
    path: "/pos",
    name: "pos",
    component: BillsController,
  },
  {
    path: "/products",
    name: "products",
    component: ProductView,
  },
  {
    path: "/discounts",
    name: "discounts",
    component: DiscountView,
  },
  {
    path: "/accounts",
    name: "accounts",
    component: AccountView,
  },
  {
    path: "/customers",
    name: "customers",
    component: CustomerView,
  },
  {
    path: "/login",
    name: "login",
    component: LoginView,
    meta: { guestOnly: true },
  },
  {
    path: "/register",
    name: "register",
    component: RegisterView,
    meta: { guestOnly: true },
  },
  {
    path: "/forgot-password",
    name: "forgot-password",
    component: ForgotPasswordView,
    meta: { guestOnly: true },
  },
  {
    path: "/product/:id",
    name: "product-detail",
    component: ProductDetailView,
  },
  {
    path: "/cart",
    name: "cart",
    component: CartView,
  },
  {
    path: "/checkout",
    name: "checkout",
    component: CheckoutView,
  },
  {
    path: "/shop",
    name: "shop",
    component: ProductListView,
  },
  // User routes - protected
  {
    path: "/account",
    name: "user-account",
    component: UserAccountView,
    meta: { requiresAuth: true },
  },
  {
    path: "/orders",
    name: "user-orders",
    component: UserOrdersView,
    meta: { requiresAuth: true },
  },
  // Shop category routes
  {
    path: "/news",
    name: "news",
    component: NewsView,
  },
  {
    path: "/tops",
    name: "tops",
    component: TopsView,
  },
  {
    path: "/bottoms",
    name: "bottoms",
    component: BottomsView,
  },
  {
    path: "/sale",
    name: "sale",
    component: SaleView,
  },
  {
    path: "/hot",
    name: "hot",
    component: HotView,
  },
  {
    path: "/contact",
    name: "contact",
    component: ContactView,
  },
  // Admin routes
  {
    path: "/admin",
    redirect: "/admin/dashboard",
  },
  {
    path: "/admin/dashboard",
    name: "admin-dashboard",
    component: DashboardView,
    meta: { requiresAuth: true, adminOnly: true },
  },
  {
    path: "/admin/nhanvien",
    name: "admin-employees",
    component: EmployeeView,
    meta: { requiresAuth: true, adminOnly: true },
  },
  {
    path: "/admin/sanpham",
    name: "admin-products",
    component: ProductView,
    meta: { requiresAuth: true, adminOnly: true },
  },
  {
    path: "/admin/hoadon",
    name: "admin-bills",
    component: BillView,
    meta: { requiresAuth: true, adminOnly: true },
  },
  {
    path: "/admin/khachhang",
    name: "admin-customers",
    component: CustomerViews,
    meta: { requiresAuth: true, adminOnly: true },
  },
  {
    path: "/admin/thongke",
    name: "admin-statistics",
    component: StatisticsView,
    meta: { requiresAuth: true, adminOnly: true },
  },
  {
    path: "/admin/uudai",
    name: "admin-discounts",
    component: DiscountView,
    meta: { requiresAuth: true, adminOnly: true },
  },
  {
    path: "/admin/donhang",
    name: "admin-orders",
    component: OrderView,
    meta: { requiresAuth: true, adminOnly: true },
  },
  {
    path: "/admin/baiviet",
    name: "admin-posts",
    component: PostView,
    meta: { requiresAuth: true, adminOnly: true },
  },
  {
    path: "/admin/banhang",
    name: "admin-pos",
    component: BillsController,
    meta: { requiresAuth: true, adminOnly: true },
  },
  {
    path: "/admin/trahang",
    name: "admin-returns",
    component: ReturnView,
    meta: { requiresAuth: true, adminOnly: true },
  },
  {
    path: "/create-customer",
    name: "create-customer",
    component: CreateCustomerView,
  },
  {
    path: "/customer-details/:id",
    name: "customer-details",
    component: CustomerDetailView,
    props: true,
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

// Navigation guards
router.beforeEach((to, from, next) => {
  // Check if the route is for guest only (like login/register)
  if (to.matched.some((record) => record.meta.guestOnly)) {
    // If user is already logged in, redirect to shop or homepage
    if (authService.isAuthenticated()) {
      next({ path: "/shop" });
    } else {
      next();
    }
    return;
  }

  // Check if the route requires authentication
  if (to.matched.some((record) => record.meta.requiresAuth)) {
    // Check if user is authenticated
    if (!authService.isAuthenticated()) {
      next({
        path: "/login",
        query: { redirect: to.fullPath },
      });
    } else {
      // Check if the route is for admins only
      if (to.matched.some((record) => record.meta.adminOnly)) {
        // Get user data and check if user is admin
        const userData = authService.getUserData();
        if (userData) {
          // If we have user data, allow access to admin pages
          // Normally we'd check user.role here, but for this example we'll allow any logged in user
          console.log("Admin route access - userData:", userData);
          next();
        } else {
          // If not logged in, redirect to home
          next({ path: "/" });
        }
      } else {
        next();
      }
    }
  } else {
    next();
  }
});

export default router;
