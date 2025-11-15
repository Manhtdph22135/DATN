<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";

const route = useRoute();
const router = useRouter();
const postId = route.params.id;

const article = ref(null);
const isLoading = ref(true);
const error = ref(null);

onMounted(async () => {
  try {
    console.log(`🔄 Đang tải bài viết ID: ${postId}`);
    
    const response = await fetch(`https://localhost:7055/api/News/${postId}`);
    
    console.log('📡 Detail Status:', response.status);
    
    if (!response.ok) {
      throw new Error(`Không tìm thấy bài viết (HTTP ${response.status})`);
    }
    
    const data = await response.json();
    console.log('✅ Detail data raw:', data);
    
    // Xử lý response
    if (Array.isArray(data)) {
      article.value = data[0];
    } else if (data.$values && Array.isArray(data.$values)) {
      article.value = data.$values[0];
    } else {
      article.value = data;
    }
    
    console.log('📖 Article content:', article.value.content);
    
  } catch (err) {
    console.error('❌ Lỗi:', err);
    error.value = err.message;
  } finally {
    isLoading.value = false;
  }
});

function formatDate(dateString) {
  if (!dateString) return 'Không có ngày';
  try {
    return new Date(dateString).toLocaleDateString("vi-VN");
  } catch {
    return 'Ngày không hợp lệ';
  }
}
</script>

<template>
  <div class="detail-page">
    <button @click="router.push('/news')" class="back-btn">← Quay lại tin tức</button>

    <div v-if="isLoading" class="loading">
      <div class="spinner"></div>
      <p>Đang tải bài viết...</p>
    </div>
    
    <div v-else-if="error" class="error-message">
      <p>⚠️ {{ error }}</p>
      <button @click="router.push('/news')">Quay lại trang tin tức</button>
    </div>

    <div v-else-if="article" class="article-detail">
      <h1 class="title">{{ article.title || 'Không có tiêu đề' }}</h1>
      
      <div class="meta">
        <span class="date">{{ formatDate(article.date) }}</span> 
        <span class="author">• {{ article.author || 'Không rõ tác giả' }}</span>
      </div>
      
      <div class="image">
        <img 
          :src="article.image || 'https://via.placeholder.com/800x400'" 
          :alt="article.title || 'No image'"
          class="article-image" 
        />
      </div>
      
      <div class="summary-box" v-if="article.summary">
        <h3>📌 Tóm tắt</h3>
        <p class="summary">{{ article.summary }}</p>
      </div>
      
      <!-- NỘI DUNG TÙY CHỈNH THEO ID -->
      <div class="content">
        <h3>📖 Nội dung chi tiết</h3>
        <div class="content-text">
          
          <!-- BÀI VIẾT 1: POLY RA MẮT BST HÈ 2025 -->
          <div v-if="postId == '1'">
            <p><strong>Sau nhiều tháng nghiên cứu và phát triển, POLY chính thức giới thiệu bộ sưu tập Hè 2025 với chủ đề 'Ocean Breeze'.</strong> BST mang đến làn gió mới cho mùa hè này với những thiết kế độc đáo, chất liệu cao cấp và màu sắc tươi sáng.</p>
            
            <h4>🌟 THIẾT KẾ ĐỘC ĐÁO</h4>
            <ul>
              <li><strong>Áo thun cotton:</strong> Công nghệ in kỹ thuật số sắc nét, hình ảnh sống động về đại dương</li>
              <li><strong>Quần short linen:</strong> Thiết kế ergonomic, ống rộng vừa phải, thoáng khí</li>
              <li><strong>Váy maxi:</strong> Họa tiết lá dừa, san hô, chất liệu voan mát mẻ</li>
              <li><strong>Áo sơ mi:</strong> Cổ bẻ đa dạng, tay lỡ, phù hợp nhiều dịp</li>
              <li><strong>Áo khoác:</strong> Chất liệu siêu nhẹ, gấp gọn dễ mang theo</li>
            </ul>
            
            <h4>🎨 BẢNG MÀU CHỦ ĐẠO</h4>
            <ul>
              <li><strong>Xanh Đại Dương (#1E90FF):</strong> Tông màu chủ đạo, cảm giác mát mẻ</li>
              <li><strong>Vàng Chanh (#FFD700):</strong> Điểm nhấn năng động, trẻ trung</li>
              <li><strong>Trắng Ngà (#F5F5DC):</strong> Thanh lịch, dễ phối đồ</li>
              <li><strong>Hồng San Hô (#FF69B4):</strong> Nữ tính, hoàn hảo cho tiệc</li>
              <li><strong>Xanh Lá (#32CD32):</strong> Tươi mới, gần gũi thiên nhiên</li>
            </ul>
            
            <h4>🏷️ CHẤT LIỆU CAO CẤP</h4>
            <ul>
              <li><strong>Cotton Organic 100%:</strong> Mềm mại, thấm hút tốt, an toàn cho da</li>
              <li><strong>Linen tự nhiên:</strong> Thoáng khí, nhanh khô, không nhăn</li>
              <li><strong>Tencel™ Lyocell:</strong> Mát lạnh tự nhiên, bền màu vượt trội</li>
              <li><strong>Vải tái chế:</strong> 70% vật liệu tái chế, thân thiện môi trường</li>
            </ul>
            
            <h4>📏 SIZE & FIT</h4>
            <ul>
              <li><strong>Size XS-XXL:</strong> Đa dạng kích thước cho mọi dáng người</li>
              <li><strong>Form Regular & Oversize:</strong> 2 form dáng phù hợp sở thích</li>
              <li><strong>Hướng dẫn chọn size:</strong> Có sẵn trên website và app</li>
            </ul>
            
            <h4>💰 GIÁ THÀNH & PHÂN KHÚC</h4>
            <ul>
              <li><strong>Phổ thông:</strong> 290.000 - 590.000 VNĐ (Áo thun, quần short)</li>
              <li><strong>Cao cấp:</strong> 790.000 - 1.290.000 VNĐ (Váy, áo sơ mi thiết kế)</li>
              <li><strong>Giới hạn:</strong> 1.590.000 - 2.490.000 VNĐ (BST Limited Edition)</li>
            </ul>
            
            <h4>🗓️ LỊCH RA MẮT CHI TIẾT</h4>
            
            <h5>📍 ĐỢT 1 - BASIC COLLECTION (15/06/2025)</h5>
            <ul>
              <li><strong>Áo thun graphic:</strong> 15 mẫu, in hình sinh vật biển</li>
              <li><strong>Quần short:</strong> 8 mẫu, chất linen và cotton</li>
              <li><strong>Phụ kiện:</strong> Mũ bucket, túi tote canvas, kính mát</li>
            </ul>
            
            <h5>📍 ĐỢT 2 - PREMIUM COLLECTION (01/07/2025)</h5>
            <ul>
              <li><strong>Váy & đầm:</strong> 12 thiết kế, chất voan và lụa</li>
              <li><strong>Áo sơ mi:</strong> 10 mẫu thiết kế đặc biệt</li>
              <li><strong>BST Limited:</strong> 5 mẫu, chỉ 50 sản phẩm/mẫu</li>
            </ul>
            
            <h5>📍 ĐỢT 3 - ACCESSORIES (15/07/2025)</h5>
            <ul>
              <li><strong>Túi xách:</strong> Da thực vật, thiết kế hình sò biển</li>
              <li><strong>Trang sức:</strong> Vòng cổ, bông tai vỏ ốc</li>
              <li><strong>Thắt lưng:</strong> Dây dù, khóa hình ngọc trai</li>
            </ul>
            
            <h4>🎁 CHƯƠNG TRÌNH KHUYẾN MÃI</h4>
            <ul>
              <li><strong>Giảm 25%:</strong> Cho đơn hàng đầu tiên trên website</li>
              <li><strong>Quà tặng:</strong> Túi tote canvas cho 500 khách đầu</li>
              <li><strong>Bảo hành:</strong> 1 năm cho tất cả sản phẩm</li>
              <li><strong>Đổi trả:</strong> Miễn phí trong 30 ngày</li>
              <li><strong>Free ship:</strong> Đơn hàng từ 500.000 VNĐ</li>
            </ul>
            
            <h4>🏪 ĐIỂM BÁN HÀNG</h4>
            <ul>
              <li><strong>Hà Nội:</strong> 45 Tràng Tiền, 78 Xuân Thủy</li>
              <li><strong>TP.HCM:</strong> 123 Nguyễn Trãi Q1, 456 Lê Văn Việt Q9</li>
              <li><strong>Đà Nẵng:</strong> 123 Bạch Đằng (Khai trương 15/07)</li>
              <li><strong>Online:</strong> Website, App, Fanpage, Tiki, Shopee</li>
            </ul>
            
            <h4>📞 DỊCH VỤ HỖ TRỢ</h4>
            <ul>
              <li><strong>Hotline:</strong> 1900 1234 (Miễn phí)</li>
              <li><strong>Zalo:</strong> 0912 345 678</li>
              <li><strong>Email:</strong> support@polyfashion.com</li>
              <li><strong>Facebook:</strong> POLY Fashion Vietnam</li>
              <li><strong>Instagram:</strong> @poly.fashion</li>
            </ul>
            
            <p class="closing"><strong>Hãy cùng POLY tận hưởng mùa hè rực rỡ và thể hiện phong cách cá nhân của bạn! 🌴☀️</strong></p>
          </div>
          
          <!-- BÀI VIẾT 2: PHONG CÁCH Y2K -->
          <div v-else-if="postId == '2'">
            <p><strong>Phong cách Y2K - biểu tượng thời trang những năm 2000 đang có màn trở lại ngoạn mục trong năm 2024.</strong> Tại POLY, chúng tôi đã dành 6 tháng nghiên cứu và cho ra mắt BST 'Y2K Revolution' - sự kết hợp hoàn hảo giữa nét hoài niệm và xu hướng hiện đại.</p>
            
            <h4>🔥 XU HƯỚNG Y2K 2024</h4>
            <ul>
              <li><strong>Áo crop top:</strong> Kết hợp quần baggy, tạo silhouette cân bằng</li>
              <li><strong>Váy mini:</strong> Họa tiết metallic, sequin lấp lánh</li>
              <li><strong>Áo khoác denim:</strong> Rách có điểm nhấn, chất liệu mới</li>
              <li><strong>Giày platform:</strong> Thiết kế ergonomic, êm ái hơn</li>
              <li><strong>Quần cargo:</strong> Nhiều túi, chất liệu nylon</li>
            </ul>
            
            <h4>🎨 MÀU SẮC Y2K ĐẶC TRƯNG</h4>
            <ul>
              <li><strong>Hồng neon:</strong> Màu chủ đạo, nổi bật</li>
              <li><strong>Xanh điện:</strong> Tương phản, cá tính</li>
              <li><strong>Bạc kim loại:</strong> Hi-tech, tương lai</li>
              <li><strong>Tím lavender:</strong> Dịu dàng, nữ tính</li>
              <li><strong>Cam rực:</strong> Năng động, trẻ trung</li>
            </ul>
            
            <h4>👕 BST Y2K REVOLUTION CỦA POLY</h4>
            
            <h5>ÁO THUN GRAPHIC (15 MẪU)</h5>
            <ul>
              <li><strong>In hình:</strong> Nhân vật hoạt hình năm 2000</li>
              <li><strong>Chất liệu:</strong> Cotton mềm, co giãn 4 chiều</li>
              <li><strong>Form:</strong> Unisex, phù hợp cả nam và nữ</li>
              <li><strong>Giá:</strong> 350.000 - 450.000 VNĐ</li>
            </ul>
            
            <h5>QUẦN BAGGY (8 MẪU)</h5>
            <ul>
              <li><strong>Thiết kế:</strong> Ống rộng vừa phải</li>
              <li><strong>Chất liệu:</strong> Denim và corduroy cao cấp</li>
              <li><strong>Đặc điểm:</strong> Nhiều túi, tiện lợi</li>
              <li><strong>Giá:</strong> 590.000 - 790.000 VNĐ</li>
            </ul>
            
            <h5>VÁY & ĐẦM (12 MẪU)</h5>
            <ul>
              <li><strong>Váy mini:</strong> Họa tiết hologram</li>
              <li><strong>Đầm bodycon:</strong> Phối sequin</li>
              <li><strong>Chất liệu:</strong> Stretch thoải mái</li>
              <li><strong>Giá:</strong> 690.000 - 1.290.000 VNĐ</li>
            </ul>
            
            <h5>PHỤ KIỆN Y2K</h5>
            <ul>
              <li><strong>Kính mát:</strong> Hình trái tim, màu pastel</li>
              <li><strong>Vòng cổ:</strong> Choker đính đá</li>
              <li><strong>Túi xách:</strong> Mini hình trái cây</li>
              <li><strong>Giày:</strong> Platform 5cm</li>
              <li><strong>Giá:</strong> 190.000 - 490.000 VNĐ</li>
            </ul>
            
            <h4>💫 HƯỚNG DẪN PHỐI ĐỒ Y2K</h4>
            
            <h5>1. MIX & MATCH HIỆN ĐẠI</h5>
            <ul>
              <li>Kết hợp 1 item Y2K với đồ basic</li>
              <li>Ví dụ: Quần baggy Y2K + áo thun trơn</li>
              <li>Giữ balance: Trên cầu kỳ - dưới đơn giản</li>
            </ul>
            
            <h5>2. COLOR BLOCKING TINH TẾ</h5>
            <ul>
              <li>Phối 2-3 màu tương phản</li>
              <li>Tránh quá nhiều màu cùng lúc</li>
              <li>Dùng màu trung tính làm nền</li>
            </ul>
            
            <h5>3. LAYERED ACCESSORIES</h5>
            <ul>
              <li>Đeo 2-3 layer vòng cổ</li>
              <li>Kết hợp kính mát oversize</li>
              <li>Túi xách mini hoặc đeo chéo</li>
            </ul>
            
            <h4>🌟 BÍ QUYẾT MẶC Y2K THEO DÁNG NGƯỜI</h4>
            
            <h5>DÁNG NHỎ</h5>
            <ul>
              <li>Tránh quần quá rộng</li>
              <li>Ưu tiên áo crop top</li>
              <li>Chọn giày platform</li>
            </ul>
            
            <h5>DÁNG CAO</h5>
            <ul>
              <li>Mặc thoải mái quần baggy</li>
              <li>Kết hợp layer phức tạp</li>
              <li>Tận dụng áo dáng dài</li>
            </ul>
            
            <h5>DÁNG TRÒN</h5>
            <ul>
              <li>Chọn vải có độ co giãn</li>
              <li>Tránh họa tiết quá lớn</li>
              <li>Ưu tiên tông màu tối</li>
            </ul>
            
            <h4>🏪 ĐIỂM BÁN ĐỘC QUYỀN</h4>
            <ul>
              <li><strong>Hà Nội:</strong> POLY Flagship Store - 45 Tràng Tiền</li>
              <li><strong>TP.HCM:</strong> POLY Saigon Center - 123 Nguyễn Trãi</li>
              <li><strong>Online:</strong> Website POLY - www.polyfashion.com</li>
            </ul>
            
            <h4>📅 SỰ KIỆN ĐẶC BIỆT</h4>
            <ul>
              <li><strong>Workshop:</strong> "Y2K Styling" - 20/06/2024</li>
              <li><strong>Triển lãm:</strong> Y2K Fashion Show - 25/06/2024</li>
              <li><strong>Gặp gỡ:</strong> Fashion Influencer - 30/06/2024</li>
            </ul>
            
            <p class="closing"><strong>Hãy đến POLY để khám phá BST Y2K Revolution và tìm cho mình phong cách hoàn hảo! ✨</strong></p>
          </div>
          
          <!-- BÀI VIẾT 3: HỢP TÁC NGHỆ SĨ LOCAL -->
          <div v-else-if="postId == '3'">
            <p><strong>Với mong muốn quảng bá văn hóa đường phố Việt Nam ra thế giới, POLY vừa công bố dự án hợp tác đặc biệt với 5 nghệ sĩ graffiti nổi tiếng trong nước.</strong> Dự án 'Street Art Meets Fashion' hứa hẹn mang đến làn gió mới cho thời trang đường phố Việt Nam.</p>
            
            <h4>🎨 GIỚI THIỆU DỰ ÁN</h4>
            <ul>
              <li><strong>Tên dự án:</strong> Street Art Meets Fashion</li>
              <li><strong>Thời gian:</strong> 6 tháng (06/2024 - 12/2024)</li>
              <li><strong>Số nghệ sĩ:</strong> 5 nghệ sĩ graffiti hàng đầu</li>
              <li><strong>Sản phẩm:</strong> BST Limited Edition</li>
              <li><strong>Mục tiêu:</strong> Quảng bá văn hóa đường phố Việt</li>
            </ul>
            
            <h4>🌟 NGHỆ SĨ THAM GIA</h4>
            
            <h5>1. NGHỆ SĨ ZERO - HÀ NỘI</h5>
            <ul>
              <li><strong>Phong cách:</strong> Graffiti 3D, Optical Art</li>
              <li><strong>Tác phẩm nổi bật:</strong> "The Soul of Hanoi"</li>
              <li><strong>BST riêng:</strong> "Urban Geometry"</li>
              <li><strong>Đặc điểm:</strong> Hình học, không gian ảo giác</li>
            </ul>
            
            <h5>2. NGHỆ SĨ RUBY - SÀI GÒN</h5>
            <ul>
              <li><strong>Phong cách:</strong> Màu sắc rực rỡ, văn hóa Nam Bộ</li>
              <li><strong>Tác phẩm nổi bật:</strong> "Saigon Memories"</li>
              <li><strong>BST riêng:</strong> "Colorful Chaos"</li>
              <li><strong>Đặc điểm:</strong> Hỗn độn màu sắc có trật tự</li>
            </ul>
            
            <h5>3. NGHỆ SĨ KAYEN - ĐÀ NẴNG</h5>
            <ul>
              <li><strong>Phong cách:</strong> Graffiti + Thư pháp</li>
              <li><strong>Tác phẩm nổi bật:</strong> "Ocean Calligraphy"</li>
              <li><strong>BST riêng:</strong> "Ocean Calligraphy"</li>
              <li><strong>Đặc điểm:</strong> Kết hợp chữ và hình ảnh biển</li>
            </ul>
            
            <h5>4. NGHỆ SĨ TEO - HẢI PHÒNG</h5>
            <ul>
              <li><strong>Phong cách:</strong> Vintage, hoài niệm</li>
              <li><strong>Tác phẩm nổi bật:</strong> "Portrait Stories"</li>
              <li><strong>BST riêng:</strong> "Portrait Stories"</li>
              <li><strong>Đặc điểm:</strong> Chân dung và câu chuyện</li>
            </ul>
            
            <h5>5. NGHỆ SĨ LUNA - CẦN THƠ</h5>
            <ul>
              <li><strong>Phong cách:</strong> Feminist Graffiti</li>
              <li><strong>Tác phẩm nổi bật:</strong> "Feminine Power"</li>
              <li><strong>BST riêng:</strong> "Feminine Power"</li>
              <li><strong>Đặc điểm:</strong> Sức mạnh nữ giới, màu pastel</li>
            </ul>
            
            <h4>👕 DÒNG SẢN PHẨM ĐẶC BIỆT</h4>
            
            <h5>ÁO THUN LIMITED EDITION</h5>
            <ul>
              <li><strong>Số lượng:</strong> 500 chiếc/nghệ sĩ (đánh số)</li>
              <li><strong>In ấn:</strong> Kỹ thuật số độ phân giải cao</li>
              <li><strong>Chứng nhận:</strong> Certificate of Authenticity</li>
              <li><strong>Giá:</strong> 890.000 VNĐ/chiếc</li>
            </ul>
            
            <h5>PHỤ KIỆN NGHỆ THUẬT</h5>
            <ul>
              <li><strong>Túi tote canvas:</strong> In graffiti, 290.000 VNĐ</li>
              <li><strong>Mũ bucket:</strong> Thiết kế độc quyền, 390.000 VNĐ</li>
              <li><strong>Vòng tay:</strong> Handmade, 190.000 VNĐ</li>
              <li><strong>Sticker collection:</strong> Bộ 10 sticker, 90.000 VNĐ</li>
            </ul>
            
            <h5>ITEMS CAO CẤP</h5>
            <ul>
              <li><strong>Áo khoác denim:</strong> Vẽ tay, 2.490.000 VNĐ</li>
              <li><strong>Váy:</strong> In tác phẩm nguyên bản, 1.890.000 VNĐ</li>
              <li><strong>Quần jeans:</strong> Custom, 1.590.000 VNĐ</li>
            </ul>
            
            <h4>🏛️ TRIỂN LÃM & SỰ KIỆN</h4>
            
            <h5>LỊCH TRIỂN LÃM</h5>
            <ul>
              <li><strong>Hà Nội:</strong> 15-30/07/2024 - POLY Flagship Store</li>
              <li><strong>Sài Gòn:</strong> 05-20/08/2024 - POLY Saigon Center</li>
              <li><strong>Đà Nẵng:</strong> 01-15/09/2024 - POLY Danang</li>
            </ul>
            
            <h5>HOẠT ĐỘNG ĐẶC BIỆT</h5>
            <ul>
              <li><strong>Workshop:</strong> Vẽ graffiti cơ bản (miễn phí)</li>
              <li><strong>Giao lưu:</strong> Trực tiếp với nghệ sĩ</li>
              <li><strong>Live painting:</strong> Biểu diễn vẽ trực tiếp</li>
              <li><strong>Photo booth:</strong> Không gian nghệ thuật</li>
            </ul>
            
            <h4>💝 Ý NGHĨA CỘNG ĐỒNG</h4>
            <ul>
              <li><strong>20% lợi nhuận</strong> đóng góp vào:</li>
              <li>Quỹ hỗ trợ nghệ sĩ trẻ Việt Nam</li>
              <li>Chương trình dạy nghệ thuật cho trẻ em</li>
              <li>Bảo tồn không gian nghệ thuật công cộng</li>
            </ul>
            
            <h4>🌍 TẦM NHÌN TOÀN CẦU</h4>
            <ul>
              <li>Tham gia tuần lễ thời trang Berlin 2024</li>
              <li>Triển lãm tại Paris Fashion Week</li>
              <li>Hợp tác với các gallery quốc tế</li>
              <li>Xuất khẩu sang thị trường châu Âu</li>
            </ul>
            
            <h4>📱 CÁCH SỞ HỮU</h4>
            <ul>
              <li><strong>Đặt trước online:</strong> Từ 01/07/2024</li>
              <li><strong>Mua trực tiếp:</strong> Tại triển lãm</li>
              <li><strong>Ứng dụng POLY:</strong> AR try-on feature</li>
              <li><strong>Hotline đặt hàng:</strong> 1900 1234</li>
            </ul>
            
            <p class="closing"><strong>Hãy cùng POLY và các nghệ sĩ đường phố viết nên chương mới cho thời trang nghệ thuật Việt Nam! 🎨✨</strong></p>
          </div>
          
          <!-- BÀI VIẾT 4: THỜI TRANG BỀN VỮNG -->
          <div v-else-if="postId == '4'">
            <p><strong>Hưởng ứng làn sóng thời trang bền vững toàn cầu, POLY chính thức công bố kế hoạch 'Green Revolution 2025' - cam kết chuyển đổi toàn diện sang mô hình thời trang bền vững và có trách nhiệm với môi trường.</strong></p>
            
            <h4>🌿 LỘ TRÌNH PHÁT TRIỂN BỀN VỮNG</h4>
            
            <h5>2024 - GIAI ĐOẠN KHỞI ĐỘNG</h5>
            <ul>
              <li>Chuyển đổi 50% quy trình sản xuất</li>
              <li>Sử dụng 30% vật liệu tái chế</li>
              <li>Giảm 40% lượng nước sử dụng</li>
              <li>Đạt chứng nhận OCS 100</li>
            </ul>
            
            <h5>2025 - GIAI ĐOẠN TĂNG TỐC</h5>
            <ul>
              <li>80% sản phẩm từ vật liệu bền vững</li>
              <li>100% bao bì phân hủy sinh học</li>
              <li>Đạt chứng nhận GOTS và GRS</li>
              <li>Giảm 60% carbon footprint</li>
            </ul>
            
            <h5>2026 - GIAI ĐOẠN HOÀN THIỆN</h5>
            <ul>
              <li>100% chuỗi cung ứng xanh</li>
              <li>Zero waste manufacturing</li>
              <li>Carbon neutral operations</li>
              <li>100% năng lượng tái tạo</li>
            </ul>
            
            <h4>🎯 GIẢI PHÁP CỤ THỂ</h4>
            
            <h5>♻️ VẬT LIỆU BỀN VỮNG</h5>
            <ul>
              <li><strong>Cotton hữu cơ:</strong> Không thuốc trừ sâu, tiết kiệm 90% nước</li>
              <li><strong>Linen tự nhiên:</strong> Không tưới tiêu nhân tạo, phân hủy hoàn toàn</li>
              <li><strong>Tencel™ Lyocell:</strong> Từ bột gỗ bền vững, khép kín 99%</li>
              <li><strong>Vải tái chế:</strong> PET từ chai nhựa, cotton từ vải thừa</li>
            </ul>
            
            <h5>💧 CÔNG NGHỆ TIẾT KIỆM NƯỚC</h5>
            <ul>
              <li><strong>Dyeing technology:</strong> Giảm 95% nước nhuộm</li>
              <li><strong>Laser finishing:</strong> Thay thế hóa chất</li>
              <li><strong>Ozone treatment:</strong> Làm sạch không dùng nước</li>
              <li><strong>Water recycling:</strong> Tái sử dụng 85% nước thải</li>
            </ul>
            
            <h5>🏭 NHÀ MÁY XANH</h5>
            <ul>
              <li>Hệ thống năng lượng mặt trời 500kWp</li>
              <li>Thu gom và tái sử dụng nước mưa</li>
              <li>Vườn cây xanh điều hòa không khí</li>
              <li>Xử lý nước thải đạt chuẩn A trước khi xả</li>
            </ul>
            
            <h4>📦 BAO BÌ THÂN THIỆN</h4>
            <ul>
              <li><strong>Túi vải canvas:</strong> Tái sử dụng nhiều lần</li>
              <li><strong>Hộp giấy tái chế:</strong> In mực đậu nành</li>
              <li><strong>Tag hạt giống:</strong> Trồng được cây sau khi dùng</li>
              <li><strong>Dây ruy-băng:</strong> Vải tự nhiên, không nylon</li>
            </ul>
            
            <h4>🌱 CHƯƠNG TRÌNH TÁI CHẾ</h4>
            
            <h5>1. POLY RECYCLE HUB</h5>
            <ul>
              <li>Thu nhận quần áo cũ tại cửa hàng</li>
              <li>Tái chế thành vải mới</li>
              <li>Ưu đãi 15% cho khách hàng tái chế</li>
              <li>Đã thu gom: 5 tấn quần áo/năm</li>
            </ul>
            
            <h5>2. REPAIR & CARE</h5>
            <ul>
              <li>Dịch vụ sửa chữa miễn phí</li>
              <li>Hướng dẫn bảo quản đồ</li>
              <li>Workshop upcycling hàng tháng</li>
              <li>Dịch vụ làm mới sản phẩm</li>
            </ul>
            
            <h5>3. RENTAL SERVICE</h5>
            <ul>
              <li>Cho thuê trang phục đặc biệt</li>
              <li>Giảm tiêu thụ, tăng vòng đời</li>
              <li>Đã có 2,000+ thành viên</li>
              <li>Tiết kiệm 8 tấn CO2/năm</li>
            </ul>
            
            <h4>📊 MINH BẠCH & TRÁCH NHIỆM</h4>
            <ul>
              <li>Công khai toàn bộ chuỗi cung ứng</li>
              <li>Báo cáo tác động môi trường hàng quý</li>
              <li>Hợp tác với tổ chức môi trường độc lập</li>
              <li>Đạt 4.5/5 sao xếp hạng bền vững</li>
            </ul>
            
            <h4>🤝 ĐỐI TÁC CHIẾN LƯỢC</h4>
            <ul>
              <li>WWF Việt Nam - Bảo tồn thiên nhiên</li>
              <li>Fashion Revolution - Minh bạch ngành</li>
              <li>Zero Waste Vietnam - Giảm rác thải</li>
              <li>ĐH Bách Khoa HN - Nghiên cứu vật liệu</li>
            </ul>
            
            <h4>🎁 CAM KẾT VỚI KHÁCH HÀNG</h4>
            <p>Khi mua sắm tại POLY, bạn đang:</p>
            <ul>
              <li>Ủng hộ nông nghiệp hữu cơ</li>
              <li>Giảm 3kg rác thải thời trang/năm</li>
              <li>Tiết kiệm 2,500 lít nước/áo thun</li>
              <li>Hỗ trợ công nhân được đối xử công bằng</li>
            </ul>
            
            <h4>📈 THÀNH TỰU ĐẠT ĐƯỢC</h4>
            <ul>
              <li>Giảm 45% lượng nước sử dụng</li>
              <li>Giảm 60% rác thải sản xuất</li>
              <li>45% sản phẩm từ vật liệu tái chế</li>
              <li>12,000+ khách hàng tham gia tái chế</li>
            </ul>
            
            <p class="closing"><strong>Hãy cùng POLY kiến tạo tương lai thời trang bền vững! 🌍💚</strong></p>
          </div>
          
          <!-- BÀI VIẾT 5: MỞ RỘNG CỬA HÀNG -->
          <div v-else-if="postId == '5'">
            <p><strong>Tiếp nối thành công của hai cửa hàng tại Hà Nội và TP.HCM, POLY chính thức công bố kế hoạch mở rộng hệ thống với cửa hàng thứ 3 tại Đà Nẵng - thành phố đáng sống nhất Việt Nam.</strong></p>
            
            <h4>🏪 THÔNG TIN CỬA HÀNG POLY ĐÀ NẴNG</h4>
            
            <h5>📍 VỊ TRÍ VÀNG</h5>
            <ul>
              <li><strong>Địa chỉ:</strong> 123 Bạch Đằng, Quận Hải Châu</li>
              <li><strong>View:</strong> Hướng sông Hàn, gần cầu Rồng</li>
              <li><strong>Diện tích:</strong> 500m2, 3 tầng</li>
              <li><strong>Vị trí:</strong> Trung tâm thành phố, dễ tiếp cận</li>
            </ul>
            
            <h5>🎨 THIẾT KẾ KIẾN TRÚC ĐỘC ĐÁO</h5>
            
            <h6>TẦNG 1 - POLY EXPERIENCE</h6>
            <ul>
              <li>Không gian trưng bày BST mới nhất</li>
              <li>Art installation của nghệ sĩ địa phương</li>
              <li>Cafe zone với view sông</li>
              <li>Digital fitting room công nghệ cao</li>
            </ul>
            
            <h6>TẦNG 2 - FASHION GALLERY</h6>
            <ul>
              <li>Trưng bày theo concept gallery</li>
              <li>Khu vực styling consultation</li>
              <li>VIP fitting room cao cấp</li>
              <li>Art corner triển lãm luân phiên</li>
            </ul>
            
            <h6>TẦNG 3 - COMMUNITY SPACE</h6>
            <ul>
              <li>Workshop area rộng 100m2</li>
              <li>Event space cho 100 khách</li>
              <li>Co-working corner</li>
              <li>Rooftop garden với view thành phố</li>
            </ul>
            
            <h4>🌟 TRẢI NGHIỆM ĐỘC QUYỀN</h4>
            
            <h5>1. DIGITAL FITTING ROOM</h5>
            <ul>
              <li>Công nghệ AR thử đồ ảo</li>
              <li>Gợi ý phối đồ AI</li>
              <li>Lưu lịch sử thử đồ</li>
              <li>Chia sẻ hình ảnh trực tiếp</li>
            </ul>
            
            <h5>2. PERSONAL STYLING SERVICE</h5>
            <ul>
              <li>Tư vấn 1:1 với stylist</li>
              <li>Phân tích màu da, dáng người</li>
              <li>Lên moodboard cá nhân</li>
              <li>Dịch vụ miễn phí cho khách VIP</li>
            </ul>
            
            <h5>3. CUSTOMIZATION CORNER</h5>
            <ul>
              <li>Thêu tên, thiết kế riêng</li>
              <li>Sửa đổi theo yêu cầu</li>
              <li>In graphic độc quyền</li>
              <li>Thời gian: 2-3 ngày</li>
            </ul>
            
            <h4>🎪 SỰ KIỆN KHAI TRƯƠNG</h4>
            
            <h5>📅 NGÀY 15/07/2024</h5>
            <ul>
              <li><strong>9:00 AM:</strong> Ribbon cutting ceremony</li>
              <li><strong>10:00 AM:</strong> Fashion show đặc biệt</li>
              <li><strong>2:00 PM:</strong> Workshop "Summer Styling"</li>
              <li><strong>6:00 PM:</strong> Grand opening party</li>
            </ul>
            
            <h5>🎁 QUÀ TẶNG ĐẶC BIỆT</h5>
            <ul>
              <li><strong>100 khách đầu:</strong> Voucher 1,000,000 VND</li>
              <li><strong>500 khách tiếp:</strong> Túi tote limited edition</li>
              <li><strong>Tất cả khách:</strong> Bộ quà tặng khai trương</li>
              <li><strong>Mini concert:</strong> Ban nhạc indie nổi tiếng</li>
            </ul>
            
            <h4>👥 ĐỘI NGŨ ĐÀ NẴNG</h4>
            <ul>
              <li><strong>20 nhân viên:</strong> Đào tạo chuyên sâu</li>
              <li><strong>5 stylist:</strong> Kinh nghiệm 3+ năm</li>
              <li><strong>3 chuyên gia:</strong> Customer care</li>
              <li><strong>Hỗ trợ:</strong> 4 ngôn ngữ (Việt, Anh, Hàn, Nhật)</li>
            </ul>
            
            <h4>🛍️ DỊCH VỤ ĐẶC BIỆT</h4>
            <ul>
              <li>Free alteration trong 7 ngày</li>
              <li>Personal shopper service</li>
              <li>Home delivery within 10km</li>
              <li>International shipping</li>
              <li>Gift wrapping miễn phí</li>
            </ul>
            
            <h4>📈 KẾ HOẠCH PHÁT TRIỂN</h4>
            
            <h5>2024 - ĐÀ NẴNG</h5>
            <ul>
              <li>Khai trương cửa hàng flagship</li>
              <li>Phát triển cộng đồng local</li>
              <li>Hợp tác với artists Đà Nẵng</li>
              <li>Mục tiêu: 5,000 khách/tháng</li>
            </ul>
            
            <h5>2025 - MỞ RỘNG MIỀN TRUNG</h5>
            <ul>
              <li>Nha Trang: Q2/2025</li>
              <li>Huế: Q3/2025</li>
              <li>Quy Nhơn: Q4/2025</li>
              <li>Pop-up stores tại resort</li>
            </ul>
            
            <h5>2026 - PHỦ SÓNG TOÀN QUỐC</h5>
            <ul>
              <li>15 cửa hàng trên cả nước</li>
              <li>E-commerce platform</li>
              <li>Mobile app với AI</li>
              <li>Venture ra quốc tế</li>
            </ul>
            
            <h4>🌐 KẾT NỐI CỘNG ĐỒNG</h4>
            <ul>
              <li>Hợp tác với ĐH Kiến Trúc Đà Nẵng</li>
              <li>Sponsorship sự kiện văn hóa</li>
              <li>Workshop cho sinh viên</li>
              <li>Triển lãm nghệ thuật địa phương</li>
            </ul>
            
            <h4>📞 THÔNG TIN LIÊN HỆ</h4>
            <ul>
              <li><strong>Hotline:</strong> 0236 123 4567</li>
              <li><strong>Email:</strong> danang@polyfashion.com</li>
              <li><strong>Fanpage:</strong> POLY Da Nang</li>
              <li><strong>Instagram:</strong> @polydanang</li>
              <li><strong>Giờ mở cửa:</strong> 9:00 - 22:00 hàng ngày</li>
            </ul>
            
            <p class="closing"><strong>Hẹn gặp các bạn tại POLY Đà Nẵng - nơi thời trang gặp gỡ nghệ thuật! 🏙️✨</strong></p>
          </div>
          
          <!-- MẶC ĐỊNH - DÙNG NỘI DUNG TỪ API -->
          <div v-else-if="article.content">
            {{ article.content }}
          </div>
          
          <!-- KHÔNG CÓ NỘI DUNG -->
          <div v-else>
            <p>📭 Bài viết đang được cập nhật nội dung chi tiết...</p>
            <p>Xin vui lòng quay lại sau!</p>
          </div>
          
        </div>
      </div>
    </div>

    <div v-else class="not-found">
      <p>📭 Không tìm thấy bài viết</p>
      <button @click="router.push('/news')">Quay lại trang tin tức</button>
    </div>
  </div>
</template>

<style scoped>
/* Giữ nguyên toàn bộ CSS từ code trước */
.detail-page {
  max-width: 800px;
  margin: 0 auto;
  padding: 40px 20px;
  min-height: 80vh;
}

.back-btn {
  background-color: transparent;
  border: none;
  font-size: 16px;
  margin-bottom: 30px;
  cursor: pointer;
  color: #666;
  transition: color 0.2s ease;
  padding: 8px 0;
}

.back-btn:hover {
  color: #000;
}

.loading {
  text-align: center;
  padding: 60px 0;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #000;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 15px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-message {
  text-align: center;
  padding: 60px 0;
  color: #d63031;
}

.error-message button {
  margin-top: 15px;
  padding: 10px 20px;
  background-color: #000;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.article-detail {
  background: white;
  border-radius: 12px;
  padding: 40px;
  box-shadow: 0 2px 20px rgba(0,0,0,0.1);
}

.title {
  font-size: 32px;
  font-weight: 700;
  margin-bottom: 20px;
  line-height: 1.3;
  color: #333;
}

.meta {
  font-size: 14px;
  color: #666;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 1px solid #eee;
}

.date {
  font-weight: 500;
}

.author {
  color: #888;
}

.image {
  width: 100%;
  margin: 0 auto 30px;
  border-radius: 8px;
  overflow: hidden;
}

.article-image {
  width: 100%;
  height: auto;
  object-fit: cover;
  border-radius: 8px;
}

.summary-box {
  background: #f8f9fa;
  padding: 25px;
  border-radius: 8px;
  border-left: 4px solid #007bff;
  margin-bottom: 30px;
}

.summary-box h3 {
  font-size: 18px;
  margin-bottom: 15px;
  color: #007bff;
}

.summary {
  font-size: 16px;
  line-height: 1.6;
  color: #444;
  margin: 0;
}

.content {
  margin-top: 40px;
}

.content h3 {
  font-size: 20px;
  margin-bottom: 20px;
  color: #333;
  border-bottom: 2px solid #000;
  padding-bottom: 10px;
}

.content-text {
  font-size: 16px;
  line-height: 1.8;
  color: #555;
}

.content-text h4 {
  font-size: 18px;
  margin: 25px 0 15px 0;
  color: #333;
  border-left: 4px solid #007bff;
  padding-left: 12px;
}

.content-text h5 {
  font-size: 16px;
  margin: 20px 0 12px 0;
  color: #555;
}

.content-text h6 {
  font-size: 15px;
  margin: 15px 0 10px 0;
  color: #666;
  font-weight: 600;
}

.content-text ul {
  margin: 15px 0;
  padding-left: 20px;
}

.content-text li {
  margin-bottom: 8px;
  line-height: 1.6;
}

.content-text strong {
  font-weight: 600;
  color: #333;
}

.content-text .closing {
  background: #f8f9fa;
  padding: 20px;
  border-radius: 8px;
  border-left: 4px solid #28a745;
  margin-top: 25px;
  font-size: 17px;
}

.no-content {
  text-align: center;
  padding: 40px;
  color: #666;
  background: #f8f9fa;
  border-radius: 8px;
  margin-top: 30px;
}

.not-found {
  text-align: center;
  padding: 60px 0;
  color: #666;
}

.not-found button {
  margin-top: 15px;
  padding: 10px 20px;
  background-color: #000;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

@media (max-width: 768px) {
  .detail-page {
    padding: 20px 15px;
  }
  
  .article-detail {
    padding: 25px 20px;
  }
  
  .title {
    font-size: 24px;
  }
  
  .summary-box,
  .content {
    padding: 20px;
  }
  
  .content-text {
    font-size: 15px;
  }
  
  .content-text h4 {
    font-size: 17px;
  }
  
  .content-text h5 {
    font-size: 15px;
  }
}
</style>