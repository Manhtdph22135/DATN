// src/utils/cartStore.js
import { reactive, watch } from 'vue';

const CART_KEY = 'polyshop_cart';

// 1. State
const savedCart = localStorage.getItem(CART_KEY);
export const cartState = reactive({
  items: savedCart ? JSON.parse(savedCart) : []
});

// 2. Watcher
watch(
  () => cartState.items,
  (newItems) => {
    localStorage.setItem(CART_KEY, JSON.stringify(newItems));
  },
  { deep: true }
);

// 3. Actions
export const addToCart = (product) => {
  // Tạo ID duy nhất: productId_sizeId_colorId
  const uniqueId = `${product.productId}_${product.sizeId || '0'}_${product.colorId || '0'}`;
  
  const existingItem = cartState.items.find(item => item.uniqueId === uniqueId);

  if (existingItem) {
    existingItem.quantity++;
  } else {
    cartState.items.push({
      uniqueId: uniqueId,
      productId: product.productId,
      name: product.productName,
      price: product.price,
      image: product.image,
      category: product.categoryName,
      sizeName: product.sizeName || 'Mặc định', 
      colorName: product.colorName || 'Mặc định',
      quantity: 1,
      selected: true
    });
  }
  alert("Đã thêm vào giỏ hàng!");
};

export const removeFromCart = (index) => {
  cartState.items.splice(index, 1);
};

export const updateQuantity = (index, change) => {
  const item = cartState.items[index];
  const newQty = item.quantity + change;
  if (newQty >= 1) {
    item.quantity = newQty;
  }
};

export const calculateTotal = () => {
  return cartState.items
    .filter(item => item.selected)
    .reduce((total, item) => total + (item.price * item.quantity), 0);
};