using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Summary { get; set; }
        public string? Content { get; set; }

        // Đổi tên để khớp với cả 2 frontend
        public string? Thumbnail { get; set; }  // Admin dùng thumbnail
        public string? Image { get; set; }      // User page dùng image

        public DateTime Date { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? Author { get; set; } = "Admin";
        public string Category { get; set; } = "tin-tuc";
        public string Status { get; set; } = "draft";
    }
}