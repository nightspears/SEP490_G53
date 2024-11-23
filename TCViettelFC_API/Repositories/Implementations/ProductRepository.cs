using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Dtos.Product;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly Sep490G53Context _context;
        private readonly ICloudinarySetting _cloudinary;

        public ProductRepository(Sep490G53Context context, ICloudinarySetting cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;

        }
        public async Task AddProductAsync(ProductDto pro)
        {

            if (string.IsNullOrEmpty(pro.ProductName))
            {
                throw new ArgumentException("The system returns an error, no new Product are added.");
            }
            if (pro.ProductName.Length > 255)
            {
                throw new ArgumentException("The system returns an error, no new Product are added.");
            }
            if (string.IsNullOrEmpty(pro.SeasonId.ToString()))
            {
                throw new ArgumentException("The system returns an error, no new Product are added.");
            }
            if (string.IsNullOrEmpty(pro.CategoryId.ToString()))
            {
                throw new ArgumentException("The system returns an error, no new Product are added.");
            }
            if (string.IsNullOrEmpty(pro.Price.ToString()))
            {
                throw new ArgumentException("The system returns an error, no new Product are added.");
            }

            if (string.IsNullOrEmpty(pro.Size))
            {
                throw new ArgumentException("The system returns an error, no new Product are added.");
            }



            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                   
                    Product product = new Product();
                    {
                        product.ProductName = pro.ProductName;
                        product.SeasonId = pro.SeasonId;
                        product.CategoryId = pro.CategoryId;
                        product.Description = pro.Description;
                        if (pro.Avatar != null && pro.Avatar.Length > 0)
                        {
                            ImageUploadResult res = _cloudinary.CloudinaryUpload(pro.Avatar);
                            product.Avatar = res.SecureUrl.ToString();
                        }
                        else
                        {
                            product.Avatar = "/image/imagelogo/ImageFail.jpg";
                        }
                        product.Price = pro.Price;
                        product.Size = pro.Size;
                        product.Color = pro.Color;
                        product.Material = pro.Material;
                        product.Status = pro.Status == null ? 2 : pro.Status;
                        product.CreatedAt =  DateTime.Now  ;
                        product.DiscountId = pro.DiscountId;
                    };
                    await _context.Products.AddAsync(product);
                    await _context.SaveChangesAsync();
                    if (pro.DataFile != null && pro.DataFile.Count > 0)
                    {
                        foreach (var f in pro.DataFile)
                        {
                            ProductFile ProductFile = new ProductFile();
                            {
                                ProductFile.FileName = f.File.FileName;
                                //  sửa lại db trường filepath cho đọ dài lên 255
                                if (f.File != null && f.File.Length > 0)
                                {
                                    ImageUploadResult res = _cloudinary.CloudinaryUpload(f.File);
                                    ProductFile.FilePath = res.SecureUrl.ToString();
                                }
                                else
                                {
                                    ProductFile.FilePath = "/image/imagelogo/ImageFail.jpg";
                                }
                                ProductFile.CreatedAt = DateTime.Now;
                                ProductFile.Status = 1;
                                ProductFile.ProductId = product.ProductId;
                                await _context.ProductFiles.AddAsync(ProductFile);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }


                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                }
            }
        }
        public async Task DeleteProductAsync(int id)
        {
            var pro = await _context.Products.FindAsync(id);
            if (pro == null || pro.Status == 0) throw new KeyNotFoundException("Product not found");

            try
            {
                pro.Status = 0;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Delete failed", ex);
            }
        }

        public async Task<List<ProductResponse>> GetProductAsync()
        {

            _context.Database.ExecuteSqlRaw("EXEC UpdateDiscountStatus");

            List<ProductResponse> product = new List<ProductResponse>();
            product = (from pro in _context.Products
                       join cate in _context.ProductCategories on pro.CategoryId equals cate.CategoryId into category
                       from cate in category.DefaultIfEmpty()
                       join season in _context.Seasons on pro.SeasonId equals season.SeasonId into seasons
                       from season in seasons.DefaultIfEmpty()
                       join dis in _context.Discounts on pro.DiscountId equals dis.DiscountId into discout
                       from dis in discout.DefaultIfEmpty()
                       where (pro.Status != 0 && cate.Status != 0 && season.Status != 0)
                       select new ProductResponse
                       {
                           ProductName = pro.ProductName,
                           CategoryName = cate.CategoryName,
                           SeasonName = season.SeasonName,
                           Image = pro.Avatar,
                           Price = pro.Price,
                           discoutPercent = dis != null && dis.Status == 1 ? dis.DiscountPercent : null,
                           ProductId = pro.ProductId,
                           Status = pro.Status,
                       }).ToList();

            return product;
        }

        public async Task<List<ProductResponse>> GetSanPhamAsync(int cid)
        {
            _context.Database.ExecuteSqlRaw("EXEC UpdateDiscountStatus");
            List<ProductResponse> product = new List<ProductResponse>();


            if (cid == 0)
            {
                product = (from pro in _context.Products
                           join dis in _context.Discounts on pro.DiscountId equals dis.DiscountId into discout
                           from dis in discout.DefaultIfEmpty()
                           where pro.Status == 1
                           select new ProductResponse
                           {
                               ProductName = pro.ProductName,
                               Image = pro.Avatar,
                               Price = pro.Price,
                               discoutPercent = dis != null && dis.Status == 1 ? dis.DiscountPercent : null,
                               ProductId = pro.ProductId,
                               Status = pro.Status,
                           }).ToList();

                return product;
            }
            else
            {

                product = (from pro in _context.Products
                           join dis in _context.Discounts on pro.DiscountId equals dis.DiscountId into discout
                           from dis in discout.DefaultIfEmpty()
                           where pro.Status == 1 && pro.CategoryId == cid
                           select new ProductResponse
                           {
                               ProductName = pro.ProductName,
                               Image = pro.Avatar,
                               Price = pro.Price,
                               discoutPercent = dis != null && dis.Status == 1 ? dis.DiscountPercent : null,
                               ProductId = pro.ProductId,
                               Status = pro.Status,
                           }).ToList();
                return product;
            }
        }
        public async Task<JsonResult> GetProductByIdAsync(int id)
        {

            if(id <= 0 )
            {
                throw new NullReferenceException("ProductId not valid");
            }
            else
            {
                var product = (from pro in _context.Products
                               join cate in _context.ProductCategories on pro.CategoryId equals cate.CategoryId into category
                               from cate in category.DefaultIfEmpty()
                               join season in _context.Seasons on pro.SeasonId equals season.SeasonId into seasons
                               from season in seasons.DefaultIfEmpty()
                               join dis in _context.Discounts on pro.DiscountId equals dis.DiscountId into discout
                               from dis in discout.DefaultIfEmpty()
                               where (pro.Status != 0 && cate.Status != 0 && season.Status != 0)
                               select new ProductResponse
                               {
                                   ProductName = pro.ProductName,
                                   CategoryId = cate.CategoryId,
                                   DiscountId = dis.DiscountId,
                                   SeasonId = season.SeasonId,
                                   Image = pro.Avatar,
                                   Price = pro.Price,
                                   ProductId = pro.ProductId,
                                   Status = pro.Status,
                                   discoutPercent = dis != null && dis.Status == 1 ? dis.DiscountPercent : null,
                                   Size = pro.Size,
                                   Material = pro.Material,
                                   Description = pro.Description

                               }).FirstOrDefault(x => x.ProductId == id && x.Status != 0);


                var proFile = _context.ProductFiles.Where(x => x.Status == 1 && x.ProductId == id).ToList();

                var data = new
                {
                    Product = product,
                    PFile = proFile,

                };

                return new JsonResult(data);
            }
            

        }

        public async Task<JsonResult> GetSanPhamByIdAsync(int id)
        {
            var product = (from pro in _context.Products
                           join cate in _context.ProductCategories on pro.CategoryId equals cate.CategoryId into category
                           from cate in category.DefaultIfEmpty()
                           join season in _context.Seasons on pro.SeasonId equals season.SeasonId into seasons
                           from season in seasons.DefaultIfEmpty()
                           join dis in _context.Discounts on pro.DiscountId equals dis.DiscountId into discout
                           from dis in discout.DefaultIfEmpty()
                           where (pro.Status == 1 && cate.Status == 1 && season.Status == 1)
                           select new ProductResponse
                           {
                               ProductName = pro.ProductName,
                               CategoryId = cate.CategoryId,
                               SeasonId = season.SeasonId,
                               Image = pro.Avatar,
                               Price = pro.Price,
                               ProductId = pro.ProductId,
                               Status = pro.Status,
                               discoutPercent = dis != null && dis.Status == 1 ? dis.DiscountPercent : null,
                               Size = pro.Size,
                               Material = pro.Material,
                               Description = pro.Description

                           }).FirstOrDefault(x => x.ProductId == id && x.Status == 1);


            var proFile = _context.ProductFiles.Where(x => x.Status == 1 && x.ProductId == id).ToList();
            var player = _context.Players.Where(p => p.Status == 1 && p.SeasonId == product.SeasonId).ToList();

            var lstLienQuan = _context.Products.Include(p => p.Discount)
                .Where(x => x.CategoryId == product.CategoryId && x.ProductId != id && x.Status == 1)
                .Select(v => new ProductResponse
                {
                    ProductName = v.ProductName,
                    Image = v.Avatar,
                    Price = v.Price,
                    ProductId = v.ProductId,
                    Status = v.Status,
                    discoutPercent = v.Discount != null && v.Discount.Status == 1 ? v.Discount.DiscountPercent : null,
                    Size = v.Size,
                    Material = v.Material,
                    Description = v.Description
                }).Take(12).ToList();

            var data = new
            {
                Product = product,
                PFile = proFile,
                players = player,
                LienQuan = lstLienQuan,
            };

            return new JsonResult(data);

        }
        public async Task UpdateProductAsync(int id, ProductDto pro)

        {
            if (id <= 0)
            {
                throw new ArgumentException("ProductId not valid");
            }
            if (string.IsNullOrEmpty(pro.ProductName))
            {
                throw new ArgumentException("The system returns an error, no Product are updated.");
            }
            if (pro.ProductName.Length > 255)
            {
                throw new ArgumentException("The system returns an error, no Product are updated.");
            }
            if (string.IsNullOrEmpty(pro.SeasonId.ToString()))
            {
                throw new ArgumentException("The system returns an error, no Product are updated.");
            }
            if (string.IsNullOrEmpty(pro.CategoryId.ToString()))
            {
                throw new ArgumentException("The system returns an error, no Product are updated.");
            }
            if (string.IsNullOrEmpty(pro.Price.ToString()))
            {
                throw new ArgumentException("The system returns an error, no Product are updated.");
            }

            if (string.IsNullOrEmpty(pro.Size))
            {
                throw new ArgumentException("The system returns an error, no Product are updated.");
            }


            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var product = await _context.Products.FindAsync(id);
                    if (product == null || product.Status == 0)
                    {
                        throw new Exception("Product not found");
                    }

                    // Update product properties
                    product.ProductName = pro.ProductName ?? product.ProductName;
                    product.SeasonId = pro.SeasonId ?? product.SeasonId;
                    product.CategoryId = pro.CategoryId ?? product.CategoryId;
                    product.Description = pro.Description ;
                    if (pro.Avatar != null && pro.Avatar.Length > 0)
                    {
                        ImageUploadResult res = _cloudinary.CloudinaryUpload(pro.Avatar);
                        product.Avatar = res.SecureUrl.ToString();
                    }
                    product.Price = pro.Price ?? product.Price;
                    product.Size = pro.Size ?? product.Size;
                    //product.Color = pro.Color ?? product.Color;
                    product.Material = pro.Material ;
                    product.DiscountId = pro.DiscountId ;
                    product.Status = pro.Status ;

                    if (pro.ListExist != null && pro.ListExist.Count > 0)
                    {
                        var SPFile = _context.ProductFiles.Where(f => f.ProductId == id && f.Status == 1).ToList();
                        foreach (var f in SPFile)
                        {
                            if (!pro.ListExist.Contains(f.FileId))
                            {
                                f.Status = 0;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }

                    if (pro.DataFile != null && pro.DataFile.Count > 0)
                    {
                        foreach (var f in pro.DataFile)
                        {
                            ProductFile ProductFile = new ProductFile();
                            {
                                ProductFile.FileName = f.File.FileName;
                                if (f.File != null && f.File.Length > 0)
                                {
                                    ImageUploadResult res = _cloudinary.CloudinaryUpload(f.File);
                                    ProductFile.FilePath = res.SecureUrl.ToString();
                                }
                                else
                                {
                                    ProductFile.FilePath = "/image/imagelogo/ImageFail.jpg";
                                }
                                ProductFile.CreatedAt = DateTime.Now;
                                ProductFile.Status = 1;
                                ProductFile.ProductId = product.ProductId;
                                await _context.ProductFiles.AddAsync(ProductFile);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                }
            }
        }

        public async Task<JsonResult> GetDataJsonAsync()
        {
            //lấy cate 
            List<ProductCategory> cate = _context.ProductCategories.Where(x => x.Status == 1).ToList();

            //lấy mùa giải
            List<Season> season = _context.Seasons.Where(x => x.Status == 1).ToList();
            List<Discount> Discount = _context.Discounts.Where(x => x.Status == 1).ToList();

            var data = new
            {
                Cate = cate,
                Season = season,
                Dis = Discount
            };

            return new JsonResult(data);
        }

        public void UpdateStatus(int status , int id)
        {
           var product = _context.Products.Find(id);
           product.Status = status;
           _context.SaveChanges();
        }

        public async Task<JsonResult> GetLienQuanProductAsync(List<int> lstID)
        {
            List<ProductResponse> products = new List<ProductResponse>();

            if (lstID.Count > 0)
            {
                products = _context.Products.Include(z => z.Discount).Where(x => !lstID.Contains(x.ProductId) && x.Status == 1).Select(x => new ProductResponse
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    discoutPercent = x.Discount != null && x.Discount.Status == 1 ? x.Discount.DiscountPercent : null,
                    Size = x.Size,
                    Image = x.Avatar,
                    Status = x.Status,

                }).ToList();
            }
            else
            {
                products = _context.Products.Include(z => z.Discount).Where(x => x.Status == 1).Select(x => new ProductResponse
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    discoutPercent = x.Discount != null && x.Discount.Status == 1 ? x.Discount.DiscountPercent : null,
                    Size = x.Size,
                    Image = x.Avatar,
                    Status = x.Status,

                }).ToList();
            }

            var data = new
            {
                Products = products,
            };

            return new JsonResult(data);
        }
    }
}
