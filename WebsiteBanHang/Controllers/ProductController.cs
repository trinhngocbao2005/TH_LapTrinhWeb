using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories;

namespace WebsiteBanHang.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB

        public ProductController(
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // Display list of cameras
        public IActionResult Index(int? categoryId, string? searchString, string? priceRange)
        {
            var products = _productRepository.GetAll();

            // 1. Filter by category
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
                ViewBag.SelectedCategoryId = categoryId.Value;
            }

            // 2. Filter by search string (name or description)
            if (!string.IsNullOrEmpty(searchString))
            {
                string query = searchString.Trim().ToLower();
                products = products.Where(p => p.Name.ToLower().Contains(query) || 
                                               p.Description.ToLower().Contains(query));
                ViewBag.SearchString = searchString;
            }

            // 3. Filter by price range
            if (!string.IsNullOrEmpty(priceRange))
            {
                switch (priceRange.ToLower())
                {
                    case "under50m":
                        products = products.Where(p => p.Price < 50000000);
                        break;
                    case "50mto80m":
                        products = products.Where(p => p.Price >= 50000000 && p.Price <= 80000000);
                        break;
                    case "over80m":
                        products = products.Where(p => p.Price > 80000000);
                        break;
                }
                ViewBag.SelectedPriceRange = priceRange;
            }

            ViewBag.Categories = _categoryRepository.GetAll();
            return View(products);
        }

        // Display details
        public IActionResult Details(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryName = _categoryRepository.GetById(product.CategoryId)?.Name ?? "Không xác định";
            return View(product);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_categoryRepository.GetAll(), "Id", "Name");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            // Custom Validation for AvatarFile
            if (product.AvatarFile != null)
            {
                var extension = Path.GetExtension(product.AvatarFile.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AvatarFile", "Chỉ cho phép định dạng ảnh .jpg, .jpeg, .png");
                }
                if (product.AvatarFile.Length > _maxFileSize)
                {
                    ModelState.AddModelError("AvatarFile", "Kích thước ảnh đại diện tối đa là 5MB");
                }
            }
            else
            {
                ModelState.AddModelError("AvatarFile", "Ảnh đại diện sản phẩm là bắt buộc");
            }

            // Custom Validation for GalleryFiles
            if (product.GalleryFiles != null && product.GalleryFiles.Any())
            {
                foreach (var file in product.GalleryFiles)
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    if (!_allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("GalleryFiles", $"Tệp {file.FileName} không đúng định dạng (.jpg, .jpeg, .png)");
                    }
                    if (file.Length > _maxFileSize)
                    {
                        ModelState.AddModelError("GalleryFiles", $"Tệp {file.FileName} vượt quá kích thước tối đa 5MB");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                // Process Avatar
                if (product.AvatarFile != null)
                {
                    product.AvatarUrl = SaveImage(product.AvatarFile);
                }

                // Process Gallery
                product.GalleryUrls = new List<string>();
                if (product.GalleryFiles != null && product.GalleryFiles.Any())
                {
                    foreach (var file in product.GalleryFiles)
                    {
                        var savedPath = SaveImage(file);
                        if (!string.IsNullOrEmpty(savedPath))
                        {
                            product.GalleryUrls.Add(savedPath);
                        }
                    }
                }

                _productRepository.Add(product);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_categoryRepository.GetAll(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Edit
        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_categoryRepository.GetAll(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            var existingProduct = _productRepository.GetById(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Custom Validation for AvatarFile
            if (product.AvatarFile != null)
            {
                var extension = Path.GetExtension(product.AvatarFile.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AvatarFile", "Chỉ cho phép định dạng ảnh .jpg, .jpeg, .png");
                }
                if (product.AvatarFile.Length > _maxFileSize)
                {
                    ModelState.AddModelError("AvatarFile", "Kích thước ảnh đại diện tối đa là 5MB");
                }
            }

            // Custom Validation for GalleryFiles
            if (product.GalleryFiles != null && product.GalleryFiles.Any())
            {
                foreach (var file in product.GalleryFiles)
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    if (!_allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("GalleryFiles", $"Tệp {file.FileName} không đúng định dạng (.jpg, .jpeg, .png)");
                    }
                    if (file.Length > _maxFileSize)
                    {
                        ModelState.AddModelError("GalleryFiles", $"Tệp {file.FileName} vượt quá kích thước tối đa 5MB");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                // Update properties
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;

                // Process new Avatar if uploaded
                if (product.AvatarFile != null)
                {
                    existingProduct.AvatarUrl = SaveImage(product.AvatarFile);
                }

                // Process new Gallery if uploaded
                if (product.GalleryFiles != null && product.GalleryFiles.Any())
                {
                    // Clear or append? Standard is append or replace. Let's replace/append.
                    // For safety, let's append new images to the existing gallery.
                    foreach (var file in product.GalleryFiles)
                    {
                        var savedPath = SaveImage(file);
                        if (!string.IsNullOrEmpty(savedPath))
                        {
                            existingProduct.GalleryUrls.Add(savedPath);
                        }
                    }
                }

                _productRepository.Update(existingProduct);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_categoryRepository.GetAll(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryName = _categoryRepository.GetById(product.CategoryId)?.Name ?? "Không xác định";
            return View(product);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _productRepository.GetById(id);
            if (product != null)
            {
                _productRepository.Delete(id);
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method to save uploaded image physically
        private string SaveImage(IFormFile file)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return "/images/products/" + uniqueFileName;
        }
    }
}
