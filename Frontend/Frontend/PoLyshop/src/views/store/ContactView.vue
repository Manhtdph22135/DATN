<script setup>
import { ref } from "vue";

// Form data
const contactForm = ref({
  name: "",
  email: "",
  phone: "",
  subject: "",
  message: "",
});

// Form validation
const errors = ref({});
const isSubmitting = ref(false);
const showSuccessMessage = ref(false);

// Validate form
function validateForm() {
  errors.value = {};
  let isValid = true;

  if (!contactForm.value.name.trim()) {
    errors.value.name = "Vui lòng nhập họ tên";
    isValid = false;
  }

  if (!contactForm.value.email.trim()) {
    errors.value.email = "Vui lòng nhập email";
    isValid = false;
  } else if (!isValidEmail(contactForm.value.email)) {
    errors.value.email = "Email không đúng định dạng";
    isValid = false;
  }

  if (!contactForm.value.phone.trim()) {
    errors.value.phone = "Vui lòng nhập số điện thoại";
    isValid = false;
  } else if (!isValidPhone(contactForm.value.phone)) {
    errors.value.phone = "Số điện thoại không hợp lệ";
    isValid = false;
  }

  if (!contactForm.value.subject.trim()) {
    errors.value.subject = "Vui lòng nhập tiêu đề";
    isValid = false;
  }

  if (!contactForm.value.message.trim()) {
    errors.value.message = "Vui lòng nhập nội dung tin nhắn";
    isValid = false;
  }

  return isValid;
}

// Email validation
function isValidEmail(email) {
  const re =
    /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  return re.test(String(email).toLowerCase());
}

// Phone validation (VN format)
function isValidPhone(phone) {
  const re = /(84|0[3|5|7|8|9])+([0-9]{8})\b/;
  return re.test(String(phone));
}

// Submit form
async function submitForm() {
  if (!validateForm()) {
    return;
  }

  isSubmitting.value = true;

  try {
    // Giả lập API call
    await new Promise((resolve) => setTimeout(resolve, 1500));

    // Reset form sau khi thành công
    contactForm.value = {
      name: "",
      email: "",
      phone: "",
      subject: "",
      message: "",
    };

    showSuccessMessage.value = true;
    setTimeout(() => {
      showSuccessMessage.value = false;
    }, 5000);
  } catch (error) {
    console.error("Error submitting form:", error);
  } finally {
    isSubmitting.value = false;
  }
}
</script>

<template>
  <div class="contact-page">
    <div class="page-header">
      <h1>LIÊN HỆ</h1>
      <p class="subtitle">Liên hệ với chúng tôi nếu bạn cần hỗ trợ</p>
    </div>

    <div class="contact-container">
      <!-- Contact information -->
      <div class="contact-info-container">
        <div class="contact-info">
          <h2>THÔNG TIN LIÊN HỆ</h2>

          <div class="info-item">
            <div class="info-icon">
              <i class="bi bi-geo-alt-fill"></i>
            </div>
            <div class="info-content">
              <h3>Địa chỉ</h3>
              <p>123 Đường Láng, Đống Đa, HN</p>
              <p>2 Nguyễn Trãi, Hà Đông, HN</p>
            </div>
          </div>

          <div class="info-item">
            <div class="info-icon">
              <i class="bi bi-telephone-fill"></i>
            </div>
            <div class="info-content">
              <h3>Điện thoại</h3>
              <p>Hotline: 1900 1234</p>
              <p>CSKH: 0901 234 567</p>
            </div>
          </div>

          <div class="info-item">
            <div class="info-icon">
              <i class="bi bi-envelope-fill"></i>
            </div>
            <div class="info-content">
              <h3>Email</h3>
              <p>info@polyshop.vn</p>
              <p>support@polyshop.vn</p>
            </div>
          </div>

          <div class="info-item">
            <div class="info-icon">
              <i class="bi bi-clock-fill"></i>
            </div>
            <div class="info-content">
              <h3>Giờ làm việc</h3>
              <p>Thứ 2 - Thứ 6: 8:30 - 20:30</p>
              <p>Thứ 7 - Chủ nhật: 9:00 - 21:00</p>
            </div>
          </div>

          <div class="social-links">
            <a href="#" class="social-link">
              <i class="bi bi-facebook"></i>
            </a>
            <a href="#" class="social-link">
              <i class="bi bi-instagram"></i>
            </a>
            <a href="#" class="social-link">
              <i class="bi bi-tiktok"></i>
            </a>
            <a href="#" class="social-link">
              <i class="bi bi-youtube"></i>
            </a>
          </div>
        </div>
      </div>

      <!-- Contact form -->
      <div class="contact-form-container">
        <div class="contact-form">
          <h2>GỬI TIN NHẮN CHO CHÚNG TÔI</h2>

          <div class="success-message" v-if="showSuccessMessage">
            <i class="bi bi-check-circle"></i>
            Tin nhắn của bạn đã được gửi thành công! Chúng tôi sẽ phản hồi trong
            thời gian sớm nhất.
          </div>

          <form @submit.prevent="submitForm">
            <div class="form-row">
              <div class="form-group">
                <label for="name">Họ tên <span class="required">*</span></label>
                <input
                  type="text"
                  id="name"
                  v-model="contactForm.name"
                  :class="{ 'has-error': errors.name }"
                />
                <span class="error-message" v-if="errors.name">{{
                  errors.name
                }}</span>
              </div>

              <div class="form-group">
                <label for="email">Email <span class="required">*</span></label>
                <input
                  type="email"
                  id="email"
                  v-model="contactForm.email"
                  :class="{ 'has-error': errors.email }"
                />
                <span class="error-message" v-if="errors.email">{{
                  errors.email
                }}</span>
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="phone"
                  >Số điện thoại <span class="required">*</span></label
                >
                <input
                  type="tel"
                  id="phone"
                  v-model="contactForm.phone"
                  :class="{ 'has-error': errors.phone }"
                />
                <span class="error-message" v-if="errors.phone">{{
                  errors.phone
                }}</span>
              </div>

              <div class="form-group">
                <label for="subject"
                  >Tiêu đề <span class="required">*</span></label
                >
                <input
                  type="text"
                  id="subject"
                  v-model="contactForm.subject"
                  :class="{ 'has-error': errors.subject }"
                />
                <span class="error-message" v-if="errors.subject">{{
                  errors.subject
                }}</span>
              </div>
            </div>

            <div class="form-group">
              <label for="message"
                >Tin nhắn <span class="required">*</span></label
              >
              <textarea
                id="message"
                rows="6"
                v-model="contactForm.message"
                :class="{ 'has-error': errors.message }"
              ></textarea>
              <span class="error-message" v-if="errors.message">{{
                errors.message
              }}</span>
            </div>

            <button
              type="submit"
              class="submit-button"
              :disabled="isSubmitting"
            >
              <span v-if="isSubmitting">
                <i class="bi bi-arrow-repeat spinning"></i> ĐANG XỬ LÝ...
              </span>
              <span v-else>GỬI TIN NHẮN</span>
            </button>
          </form>
        </div>
      </div>
    </div>

    <!-- Map -->
    <div class="map-container">
      <h2>BẢN ĐỒ</h2>
      <div class="map-frame">
        <iframe
          src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.5062770225795!2d105.80603877597778!3d21.012419280632898!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135ab61dbeba2ad:0xa75b8941f8f12568!2zMTIzIMSQLiBMw6FuZywgVHJ1bmcgSG_DoCwgxJDhu5FuZyDEkGEsIEjDoCBO4buZaSwgVmnhu4d0IE5hbQ!5e0!3m2!1svi!2s!4v1748061336598!5m2!1svi!2s"
          width="100%"
          height="450"
          style="border: 0"
          allowfullscreen=""
          loading="lazy"
          referrerpolicy="no-referrer-when-downgrade"
        ></iframe>
      </div>
    </div>

    <!-- Store locations -->
    <div class="store-locations">
      <div class="location-container">
        <h2>SHOWROOM POLY</h2>

        <div class="locations-grid">
          <div class="location-card">
            <div class="location-header">
              <h3>POLYSHOP ĐỐNG ĐA</h3>
              <span class="flagship">FLAGSHIP STORE</span>
            </div>
            <div class="location-details">
              <p>
                <i class="bi bi-geo-alt"></i> 123 Đường Láng, Đống Đa, HN
              </p>
              <p><i class="bi bi-telephone"></i> 028 1234 5678</p>
              <p><i class="bi bi-clock"></i> 08:30 - 20:30 (Hàng ngày)</p>
            </div>
          </div>

          <div class="location-card">
            <div class="location-header">
              <h3>POLYSHOP HÀ ĐÔNG</h3>
            </div>
            <div class="location-details">
              <p>
                <i class="bi bi-geo-alt"></i> 2 Nguyễn Trãi, Hà Đông, HN
              </p>
              <p><i class="bi bi-telephone"></i> 024 1234 5678</p>
              <p><i class="bi bi-clock"></i> 08:30 - 20:30 (Hàng ngày)</p>
            </div>
          </div>

          <div class="location-card coming-soon">
            <div class="location-header">
              <h3>POLYSHOP CẦU GIẤY</h3>
              <span class="coming-soon-tag">COMING SOON</span>
            </div>
            <div class="location-details">
              <p>
                <i class="bi bi-geo-alt"></i> 225 Cầu Giấy, Cầu Giấy, HN
              </p>
              <p>
                <i class="bi bi-calendar-event"></i> Dự kiến khai trương:
                T7/2024
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.contact-page {
  padding-bottom: 60px;
}

.page-header {
  text-align: center;
  margin-bottom: 40px;
  padding: 40px 0;
  background-color: #f9f9f9;
}

.page-header h1 {
  font-size: 28px;
  margin-bottom: 10px;
  font-weight: 600;
}

.subtitle {
  color: #666;
  font-size: 16px;
}

.contact-container {
  display: flex;
  max-width: 1200px;
  margin: 0 auto 60px;
  padding: 0 20px;
}

/* Contact info styles */
.contact-info-container {
  flex: 1;
  padding-right: 30px;
}

.contact-info {
  background-color: #f9f9f9;
  border-radius: 8px;
  padding: 30px;
  height: 100%;
}

.contact-info h2 {
  font-size: 20px;
  margin-bottom: 30px;
  font-weight: 600;
  position: relative;
  padding-bottom: 15px;
}

.contact-info h2::after {
  content: "";
  position: absolute;
  left: 0;
  bottom: 0;
  width: 50px;
  height: 2px;
  background-color: #000;
}

.info-item {
  display: flex;
  margin-bottom: 25px;
}

.info-icon {
  width: 50px;
  height: 50px;
  background-color: #000;
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  margin-right: 15px;
}

.info-content h3 {
  font-size: 16px;
  margin-bottom: 5px;
  font-weight: 600;
}

.info-content p {
  color: #666;
  margin-bottom: 5px;
  font-size: 14px;
}

.social-links {
  display: flex;
  gap: 10px;
  margin-top: 30px;
}

.social-link {
  width: 40px;
  height: 40px;
  background-color: #f1f1f1;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #333;
  font-size: 18px;
  text-decoration: none;
  transition: all 0.2s;
}

.social-link:hover {
  background-color: #000;
  color: white;
}

/* Contact form styles */
.contact-form-container {
  flex: 1;
}

.contact-form {
  background-color: white;
  border-radius: 8px;
  padding: 30px;
  box-shadow: 0 0 15px rgba(0, 0, 0, 0.05);
}

.contact-form h2 {
  font-size: 20px;
  margin-bottom: 30px;
  font-weight: 600;
  position: relative;
  padding-bottom: 15px;
}

.contact-form h2::after {
  content: "";
  position: absolute;
  left: 0;
  bottom: 0;
  width: 50px;
  height: 2px;
  background-color: #000;
}

.form-row {
  display: flex;
  gap: 20px;
  margin-bottom: 20px;
}

.form-group {
  flex: 1;
  margin-bottom: 20px;
}

label {
  display: block;
  margin-bottom: 8px;
  font-size: 14px;
  font-weight: 500;
}

.required {
  color: #e53637;
}

input,
textarea {
  width: 100%;
  padding: 12px 15px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  transition: border-color 0.2s;
}

input:focus,
textarea:focus {
  border-color: #000;
  outline: none;
}

.has-error {
  border-color: #e53637;
}

.error-message {
  font-size: 12px;
  color: #e53637;
  margin-top: 5px;
  display: block;
}

.submit-button {
  background-color: #000;
  color: white;
  border: none;
  padding: 15px 25px;
  border-radius: 4px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s;
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.submit-button:hover {
  background-color: #333;
}

.submit-button:disabled {
  background-color: #999;
  cursor: not-allowed;
}

.spinning {
  animation: spin 1s linear infinite;
  margin-right: 8px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

.success-message {
  background-color: #d4edda;
  color: #155724;
  padding: 15px;
  border-radius: 4px;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
}

.success-message i {
  margin-right: 8px;
  font-size: 18px;
}

/* Map styles */
.map-container {
  max-width: 1200px;
  margin: 0 auto 60px;
  padding: 0 20px;
}

.map-container h2 {
  font-size: 24px;
  margin-bottom: 20px;
  font-weight: 600;
  text-align: center;
}

.map-frame {
  border-radius: 8px;
  overflow: hidden;
}

/* Store locations styles */
.store-locations {
  background-color: #f9f9f9;
  padding: 60px 0;
}

.location-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

.location-container h2 {
  font-size: 24px;
  margin-bottom: 30px;
  font-weight: 600;
  text-align: center;
}

.locations-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 30px;
}

.location-card {
  background-color: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
}

.location-header {
  margin-bottom: 15px;
  padding-bottom: 15px;
  border-bottom: 1px solid #eee;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.location-header h3 {
  font-size: 18px;
  font-weight: 600;
}

.flagship {
  background-color: #000;
  color: white;
  padding: 3px 8px;
  font-size: 11px;
  border-radius: 3px;
}

.coming-soon {
  opacity: 0.8;
}

.coming-soon-tag {
  background-color: #6c757d;
  color: white;
  padding: 3px 8px;
  font-size: 11px;
  border-radius: 3px;
}

.location-details p {
  margin-bottom: 10px;
  display: flex;
  align-items: center;
  font-size: 14px;
}

.location-details i {
  margin-right: 8px;
  color: #666;
}

@media (max-width: 992px) {
  .locations-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .contact-container {
    flex-direction: column;
  }

  .contact-info-container {
    padding-right: 0;
    margin-bottom: 30px;
  }

  .form-row {
    flex-direction: column;
    gap: 0;
  }

  .locations-grid {
    grid-template-columns: 1fr;
  }
}
</style>
