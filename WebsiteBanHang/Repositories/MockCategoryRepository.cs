using System;
using System.Collections.Generic;
using System.Linq;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categories;

        public MockCategoryRepository()
        {
            _categories = new List<Category>
            {
                new Category { Id = 1, Name = "Mirrorless", Description = "Máy ảnh không gương lật hiện đại, nhỏ gọn và mạnh mẽ" },
                new Category { Id = 2, Name = "DSLR", Description = "Máy ảnh phản xạ ống kính đơn kỹ thuật số truyền thống và bền bỉ" },
                new Category { Id = 3, Name = "Compact", Description = "Máy ảnh du lịch nhỏ gọn, tiện lợi mang theo hàng ngày" },
                new Category { Id = 4, Name = "Phụ kiện máy ảnh", Description = "Ống kính, chân máy, túi đựng và các phụ kiện nhiếp ảnh khác" }
            };
        }

        public IEnumerable<Category> GetAll()
        {
            return _categories;
        }

        public Category? GetById(int id)
        {
            return _categories.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Category category)
        {
            category.Id = _categories.Any() ? _categories.Max(c => c.Id) + 1 : 1;
            _categories.Add(category);
        }

        public void Update(Category category)
        {
            var existing = GetById(category.Id);
            if (existing != null)
            {
                existing.Name = category.Name;
                existing.Description = category.Description;
            }
        }

        public void Delete(int id)
        {
            var existing = GetById(id);
            if (existing != null)
            {
                _categories.Remove(existing);
            }
        }
    }
}
