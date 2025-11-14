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
            new News { Id = 1, Title = "POLY ra mắt bộ sưu tập Hè 2025", Summary = "Đón chào mùa hè...", Content = "Chi tiết bài viết 1", Image = "https://...", Date = DateTime.Now, Author = "Admin POLY" },
            new News { Id = 2, Title = "Phong cách Y2K đang trở lại", Summary = "Xu hướng Y2K...", Content = "Chi tiết bài viết 2", Image = "https://...", Date = DateTime.Now, Author = "StylePOLY" },
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
