using E_Commerce_API.Models;
using E_Commerce_API.Service.Interface;
using E_Commerce_API.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace E_Commerce_API.Service.Implementation
{
    public class ProductService : IProductService
    {
        private new List<string> _allowedExtention = new List<string> { ".jpg", ".png" };
 


        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache; // 👈 1. هنا
        public ProductService(IUnitOfWork unitOfWork, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }


        public async Task<Product> AddProductAsync(Product product)
        {
            await _unitOfWork.BeginTransactionAsync();
            string SavedFilePath = null;
            try
            {
                if (string.IsNullOrEmpty(product.Name))
                    throw new Exception("Product name is required");
                if (product.Price < 0)
                    throw new Exception("Price must be grater than zero");
                if (product.Stock < 0)
                    throw new Exception("Stock must be grater than zero");

                var category = await _unitOfWork.Repositoey<Category>().GetByIdAsync(product.CategoryId);
                if (category == null)
                    throw new Exception("Category Id Invalid ");

                if (product.Image != null)
                {
                    if (!product.Image.ContentType.StartsWith("image/"))
                        throw new Exception("Invalid file type");

                    if (product.Image.Length > 2_000_000) // 2MB
                        throw new Exception("File too large");

                    if (!_allowedExtention.Contains(Path.GetExtension(product.Image.FileName).ToLower()))
                        throw new Exception("Only .png and .jpg images are allwoed");

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);

                    // 1. تحديد مسار المجلد وإنشاؤه إن لم يكن موجوداً
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // 2. دمج اسم الملف مع مسار المجلد
                    var filePath = Path.Combine(directoryPath, fileName);

                    // احفظ الصورة فعليًا
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.Image.CopyToAsync(stream);
                    }

                    SavedFilePath = filePath;
                    product.ImageUrl = "/images/" + fileName;
                }

                var result = await _unitOfWork.ProductRepo.AddAsync(product);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommmetTransactionAsync();

                // مسح الكاش لإبطال القائمة القديمة
                await _cache.RemoveAsync("all_products");

                return result;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(SavedFilePath) && File.Exists(SavedFilePath))
                {
                    File.Delete(SavedFilePath);
                }
                await _unitOfWork.RollebackAsync();
                throw new Exception("Add Product -> " + ex.Message, ex);
            }
        }
        //public async Task<Product> AddProductAsync(Product product)
        //{
        //      await _unitOfWork.BeginTransactionAsync();
        //    string SavedFilePath = null;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(product.Name))
        //            throw new Exception("Product name is required");
        //        if (product.Price < 0)
        //            throw new Exception("Price must be grater than zero");
        //        if (product.Stock < 0)
        //            throw new Exception("Stock must be grater than zero");

        //        var category = await _unitOfWork.Repositoey<Category>().GetByIdAsync(product.CategoryId);
        //        if (category == null)
        //            throw new Exception("Category Id Invalid ");


        //        if (product.Image != null)
        //        {
        //            if (!product.Image.ContentType.StartsWith("image/"))
        //                throw new Exception("Invalid file type");

        //            if (product.Image.Length > 2_000_000) // 2MB
        //                throw new Exception("File too large");


        //            if (!_allowedExtention.Contains(Path.GetExtension(product.Image.FileName).ToLower()))    // here get path from image and must contian path .png or .jpg
        //                throw new Exception("Only .png and .jpg images are allwoed");

        //            // اختر اسم للملف (مثلاً استخدام الوقت أو GUID لتجنب التعارض)
        //            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);

        //            // مسار التخزين على السيرفر (wwwroot/images)
        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

        //            // احفظ الصورة فعليًا
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await product.Image.CopyToAsync(stream);
        //            }

        //            SavedFilePath = filePath;
        //             product.ImageUrl = "/images/" + fileName;



        //        }


        //        var result = await _unitOfWork.ProductRepo.AddAsync(product);    //Repositoey<Product>().AddAsync(product);
        //        await _unitOfWork.CompleteAsync();
        //        await _unitOfWork.CommmetTransactionAsync();
        //        await _cache.RemoveAsync("all_products");
        //        return result;
        //    }
        //    catch(Exception ex)
        //    {
        //        if (!string.IsNullOrEmpty(SavedFilePath)
        //           && File.Exists(SavedFilePath))
        //        {
        //            File.Delete(SavedFilePath);
        //        }
        //        await _unitOfWork.RollebackAsync();
        //        throw new Exception("Add Product -> " + ex.Message, ex);
        //    }
        //}

          
        public async Task<Product> DeleteProductAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var result = await _unitOfWork.Repositoey<Product>().GetByIdAsync(id);
                
                _unitOfWork.Repositoey<Product>().Delete(result);
                await _unitOfWork.CompleteAsync();

                if (!string.IsNullOrEmpty(result.ImageUrl))
                {
                    // احصل على المسار الكامل للصورة
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", result.ImageUrl.TrimStart('/'));

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                await _unitOfWork.CommmetTransactionAsync();    
                return result;
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollebackAsync();
                throw new Exception("Delete Product -> " + ex.Message, ex);
            }
        }

       

        public async Task<Product> GetProductByIdAsync(int id)
        {
             var product = await _unitOfWork.Repositoey<Product>()
                                  .GetAll()                     // IQueryable
                                  .Include(p => p.Category)     // Include Relations
                                  .FirstOrDefaultAsync(p => p.Id == id);
 
            return product;


        }

        public async Task<List<Product>> GetAllProducts()
        {
            string cacheKey = "all_products";

            // 1. شوف في Redis الأول
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // رجع من الريديس فوراً بدون داتابيز
                return JsonSerializer.Deserialize<List<Product>>(cachedData)!;
            }

            // 2. لو مش موجودة في الريديس -> هاتها من الداتابيز
            var products = await _unitOfWork.Repositoey<Product>()
                                           .GetAll()
                                           .Include(a => a.Category)
                                           .ToListAsync();

            // 3. احفظها في Redis لمدة 10 دقائق
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            string serializedData = JsonSerializer.Serialize(products, jsonOptions);
            await _cache.SetStringAsync(cacheKey, serializedData, options);

            return products;
        }


        //public IQueryable<Product> GetAllProducts()
        //{
        //    return _unitOfWork.Repositoey<Product>().GetAll().Include(a => a.Category);
           
        //}


        public async Task<Product> UpdateProductAsync(int id, Product product)
             
        {
            var existid = await _unitOfWork.Repositoey<Product>().GetByIdAsync(id);
             
            // خزّن مسار الصورة القديمة قبل التعديل
            string oldImagePath = existid.ImageUrl != null ?
                                  Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existid.ImageUrl.TrimStart('/'))
                                  : null;

            existid.Name = product.Name;
            existid.Description = product.Description;
            existid.CategoryId = product.CategoryId;
            existid.Price = product.Price;
            existid.Stock = product.Stock;

            string newImagePath = null;

            if (product.Image != null)
            {
                if (!product.Image.ContentType.StartsWith("image/"))
                    throw new Exception("Invalid file type");

                if (product.Image.Length > 2_000_000) // 2MB
                    throw new Exception("File too large");


                if (!_allowedExtention.Contains(Path.GetExtension(product.Image.FileName).ToLower()))    // here get path from image and must contian path .png or .jpg
                    throw new Exception("Only .png and .jpg images are allwoed");

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);
                newImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(newImagePath, FileMode.Create))
                {
                    await product.Image.CopyToAsync(stream);
                }

                // خزّن المسار الجديد
                existid.ImageUrl = "/images/" + fileName;
            }

            try
            {
                _unitOfWork.Repositoey<Product>().Update(existid);
                await _unitOfWork.CompleteAsync();

                // احذف الصورة القديمة بعد نجاح DB
                if (oldImagePath != null && File.Exists(oldImagePath))
                    File.Delete(oldImagePath);

                return existid;
            }
            catch (Exception ex) 
            {
                // امسح الصورة الجديدة لو حصل خطأ
                if (newImagePath != null && File.Exists(newImagePath))
                    File.Delete(newImagePath);

                throw new Exception("Update Product -> " + ex.Message, ex);
            }
        }


        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _unitOfWork.Repositoey<Category>()
                                     .GetAll()
                                     .AnyAsync(c => c.Id == categoryId);
        }

        public async Task<List<Product>> Search(string name)
        {
            
                return await _unitOfWork.Repositoey<Product>().GetAll().Where(p => p.Name.Contains(name)).ToListAsync();
            
        }

        public async Task<Product> UpdateStock(int id ,int stock)
        {
            var product = await _unitOfWork.Repositoey<Product>().GetByIdAsync(id);
            if (product == null)
                throw new ArgumentException("Product Not Found");

            if (stock < 0)
                throw new ArgumentException("Stock cannot be nagative");

            product.Stock = stock;
            await _unitOfWork.CompleteAsync();
            return product;

        }
         
    }
}
