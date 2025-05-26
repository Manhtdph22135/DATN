<template>
    <div class="forgot-password-container">
        <h2>Quên mật khẩu</h2>
        <form @submit.prevent="submitForm">
            <div class="form-group">
                <label for="email">Email</label>
                <input
                    v-model="email"
                    type="email"
                    id="email"
                    required
                    placeholder="Nhập email của bạn"
                />
            </div>
            <button type="submit" :disabled="loading">
                {{ loading ? 'Đang gửi...' : 'Gửi liên kết đặt lại mật khẩu' }}
            </button>
            <p v-if="message" :class="{ success: success, error: !success }">{{ message }}</p>
        </form>
        <router-link to="/login">Quay lại đăng nhập</router-link>
    </div>
</template>

<script setup>
import { ref } from 'vue'

const email = ref('')
const loading = ref(false)
const message = ref('')
const success = ref(false)

const submitForm = async () => {
    loading.value = true
    message.value = ''
    success.value = false
    try {
        // Giả lập gọi API
        await new Promise(resolve => setTimeout(resolve, 1500))
        // Nếu thành công
        message.value = 'Liên kết đặt lại mật khẩu đã được gửi đến email của bạn.'
        success.value = true
    } catch (e) {
        message.value = 'Có lỗi xảy ra. Vui lòng thử lại.'
        success.value = false
    } finally {
        loading.value = false
    }
}
</script>

<style scoped>
.forgot-password-container {
    max-width: 400px;
    margin: 60px auto;
    padding: 32px;
    background: #fff;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.08);
}
h2 {
    text-align: center;
    margin-bottom: 24px;
}
.form-group {
    margin-bottom: 18px;
}
label {
    display: block;
    margin-bottom: 6px;
    font-weight: 500;
}
input[type="email"] {
    width: 100%;
    padding: 10px;
    border: 1px solid #ddd;
    border-radius: 4px;
}
button {
    width: 100%;
    padding: 10px;
    background: #e9e9e9;
    color: #493838;
    border: none;
    border-radius: 4px;
    font-weight: 600;
    cursor: pointer;
    margin-bottom: 12px;
}
button:disabled {
    background: #90caf9;
    cursor: not-allowed;
}
.success {
    color: #388e3c;
}
.error {
    color: #d32f2f;
}
a {
    display: block;
    text-align: center;
    margin-top: 16px;
    color: #493838;
    text-decoration: none;
}
</style>