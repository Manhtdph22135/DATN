const API_URL = 'https://localhost:7055/api';

export const customerService = {
    async getCustomers() { 
        try {
            const response = await fetch(`${API_URL}/Customer`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const data = await response.json();
            return data;
        }
        catch (error) {
            console.error('Error fetching customers:', error);
            throw error;
        }
    },

    async getCustomerById(customerId) {
        try {
            const response = await fetch(`${API_URL}/Customer/${customerId}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const data = await response.json();
            return data;
        }
        catch (error) {
            console.error('Error fetching customer by ID:', error);
            throw error;
        }
    }
};