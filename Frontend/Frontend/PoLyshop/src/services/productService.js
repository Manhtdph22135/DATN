import axios from 'axios';

const API_URL = 'https://localhost:7055/api';

export const productService = {
  async getProducts() {
    try {
      const response = await axios.get(`${API_URL}/Product`);
      return response.data;
    } catch (error) {
      console.error('Error fetching products:', error);
      throw error;
    }
  },

  async getProductsByCategory(categoryId) {
    try {
      const response = await axios.get(`${API_URL}/Product/by-category/${categoryId}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching products by category:', error);
      throw error;
    }
  },

  async getCategories() {
    try {
      const response = await axios.get(`${API_URL}/ProductCategory`);
      return response.data;
    } catch (error) {
      console.error('Error fetching categories:', error);
      throw error;
    }
  }
};