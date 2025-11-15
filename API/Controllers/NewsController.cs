using Microsoft.AspNetCore.Mvc;
using API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private static List<News> mockNews = new()
        {
            new News { Id = 1, Title = "POLY ra mắt bộ sưu tập Hè 2025", Summary = "Đón chào mùa hè với BST mới đầy năng động và trẻ trung từ POLY", Content = "Hè 2025 đánh dấu sự trở lại mạnh mẽ của POLY...", Image = "https://cdn.brvn.vn/editor/2023/07/A42_333084-fashion-show-6_1688631273.jpg", Date = DateTime.Now, Author = "Admin POLY" },
            new News { Id = 2, Title = "Phong cách Y2K đang trở lại", Summary = "Xu hướng Y2K đang làm mưa làm gió trong làng thời trang", Content = "Phong cách Y2K đình đám những năm 2000 đang quay trở lại mạnh mẽ...", Image = "https://thieuhoa.com.vn/wp-content/uploads/2023/03/BfFzk1NZAuPK0bZMRFLFgS0vIfxHWls0Wne7C4PS.webp", Date = DateTime.Now, Author = "StylePOLY" },
            new News { Id = 3, Title = "POLY hợp tác với nghệ sĩ local", Summary = "Dự án hợp tác độc quyền với các nghệ sĩ đường phố Việt Nam", Content = "POLY hợp tác với 5 nghệ sĩ graffiti nổi tiếng...", Image = "https://img.vietcetera.com/uploads/images/09-oct-2020/untitled-1.jpg", Date = DateTime.Now, Author = "Creative Team" },
            new News { Id = 4, Title = "POLY cam kết thời trang bền vững", Summary = "Chuyển đổi quy trình sản xuất sang hướng thân thiện với môi trường", Content = "POLY cam kết chuyển đổi 50% quy trình sản xuất...", Image = "https://btnmt.1cdn.vn/2020/06/28/tui-vai-bao-ve-moi-truong.jpg", Date = DateTime.Now, Author = "Green Team" },
            new News { Id = 5, Title = "Mở rộng cửa hàng POLY tại Cầu Giấy", Summary = "Cửa hàng thứ 3 của POLY sẽ khai trương vào tháng 7/2024", Content = "Tiếp nối thành công của hai cửa hàng tại Đống Đa và Hà Đông...", Image = "https://images.squarespace-cdn.com/content/v1/591fd77d29687fd09cca478b/1555546030336-YXVPG30KTCM92JW89UTL/AdobeStock_139559217.jpeg", Date = DateTime.Now, Author = "Marketing POLY" }
        };

        [HttpGet]
        public IActionResult GetAll() => Ok(mockNews);

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = mockNews.FirstOrDefault(x => x.Id == id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }
    }
}