using System;
using System.Collections.Generic;
using System.Linq;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories
{
    public class MockProductRepository : IProductRepository
    {
        private readonly List<Product> _products;

        public MockProductRepository()
        {
            _products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Sony Alpha A7 IV",
                    Price = 57990000,
                    Description = "Sony Alpha A7 IV là máy ảnh không gương lật thế hệ mới, sở hữu cảm biến Full-Frame Exmor R CMOS 33.0 MP cùng bộ xử lý BIONZ XR cực đỉnh, mang lại hiệu năng lấy nét thời gian thực vượt trội cùng khả năng quay video 4K 60p ấn tượng.",
                    CategoryId = 1,
                    AvatarUrl = "/images/products/sony_a7iv.png",
                    GalleryUrls = new List<string> { "/images/products/hero_1.png", "/images/products/hero_2.png" }
                },
                new Product
                {
                    Id = 2,
                    Name = "Fujifilm X-T5",
                    Price = 43490000,
                    Description = "Fujifilm X-T5 mang trong mình kiểu dáng cổ điển hoài niệm đặc trưng cùng sức mạnh nhiếp ảnh vượt trội của cảm biến X-Trans CMOS 5 HR lên đến 40.2 MP, chống rung 5 trục IBIS trong thân máy cực kỳ nhỏ gọn.",
                    CategoryId = 1,
                    AvatarUrl = "/images/products/fujifilm_xt5.png",
                    GalleryUrls = new List<string> { "/images/products/hero_2.png", "/images/products/hero_3.png" }
                },
                new Product
                {
                    Id = 3,
                    Name = "Canon EOS R5",
                    Price = 89900000,
                    Description = "Canon EOS R5 định nghĩa lại tiêu chuẩn máy ảnh không gương lật chuyên nghiệp với cảm biến CMOS Full-Frame 45.0 MP, khả năng quay video RAW 8K không nén và hệ thống lấy nét Dual Pixel CMOS AF II tiên tiến.",
                    CategoryId = 1,
                    AvatarUrl = "/images/products/canon_r5.png",
                    GalleryUrls = new List<string> { "/images/products/hero_3.png", "/images/products/hero_1.png" }
                },
                new Product
                {
                    Id = 4,
                    Name = "Nikon Z6 II",
                    Price = 46500000,
                    Description = "Nikon Z6 II là phiên bản nâng cấp hoàn hảo với bộ xử lý hình ảnh Dual EXPEED 6 kép, khả năng chụp liên tục 14 khung hình/giây và khe cắm thẻ nhớ kép linh hoạt cho các nhiếp ảnh gia sự kiện.",
                    CategoryId = 1,
                    AvatarUrl = "/images/products/nikon_z6ii.png",
                    GalleryUrls = new List<string> { "/images/products/hero_1.png", "/images/products/hero_2.png" }
                }
            };
        }

        public IEnumerable<Product> GetAll()
        {
            return _products;
        }

        public Product? GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public void Add(Product product)
        {
            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            _products.Add(product);
        }

        public void Update(Product product)
        {
            var existing = GetById(product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Price = product.Price;
                existing.Description = product.Description;
                existing.CategoryId = product.CategoryId;
                
                if (!string.IsNullOrEmpty(product.AvatarUrl))
                {
                    existing.AvatarUrl = product.AvatarUrl;
                }

                if (product.GalleryUrls != null && product.GalleryUrls.Any())
                {
                    existing.GalleryUrls = product.GalleryUrls;
                }
            }
        }

        public void Delete(int id)
        {
            var existing = GetById(id);
            if (existing != null)
            {
                _products.Remove(existing);
            }
        }
    }
}
