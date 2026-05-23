using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên máy ảnh không được để trống")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Tên máy ảnh phải từ 5 đến 100 ký tự")]
        [Display(Name = "Tên máy ảnh")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá bán không được để trống")]
        [Range(100000, 1000000000, ErrorMessage = "Giá bán phải từ 100.000đ đến 1.000.000.000đ")]
        [Display(Name = "Giá bán (VNĐ)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Mô tả máy ảnh không được để trống")]
        [Display(Name = "Mô tả sản phẩm")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? AvatarUrl { get; set; }

        [Display(Name = "Bộ sưu tập ảnh")]
        public List<string> GalleryUrls { get; set; } = new List<string>();

        [NotMapped]
        [Display(Name = "Tải lên ảnh đại diện")]
        public IFormFile? AvatarFile { get; set; }

        [NotMapped]
        [Display(Name = "Tải lên bộ sưu tập ảnh")]
        public List<IFormFile>? GalleryFiles { get; set; }
    }
}
