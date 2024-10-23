using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
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

            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Product product = new Product();
                    {
                        product.ProductName = pro.ProductName;
                        //product.PlayerId = pro.PlayerId;
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
                        product.Status = pro.Status;
                        product.CreatedAt = DateTime.Now;
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
            List<ProductResponse> product = new List<ProductResponse>();
            product = (from pro in _context.Products
                       join cate in _context.ProductCategories on pro.CategoryId equals cate.CategoryId into category
                       from cate in category.DefaultIfEmpty()
                       join season in _context.Seasons on pro.SeasonId equals season.SeasonId into seasons
                       from season in seasons.DefaultIfEmpty()
                       where (pro.Status != 0 && cate.Status != 0 && season.Status != 0)
                       select new ProductResponse
                       {
                           ProductName = pro.ProductName,
                           CategoryName = cate.CategoryName,
                           SeasonName = season.SeasonName,
                           Image = pro.Avatar,
                           Price = pro.Price,
                           ProductId = pro.ProductId,
                           Status = pro.Status,
                       }).ToList();


            return product;
        }
        public async Task<JsonResult> GetProductByIdAsync(int id)
        {
            var product = (from pro in _context.Products
                           join cate in _context.ProductCategories on pro.CategoryId equals cate.CategoryId into category
                           from cate in category.DefaultIfEmpty()
                           join season in _context.Seasons on pro.SeasonId equals season.SeasonId into seasons
                           from season in seasons.DefaultIfEmpty()
                           where (pro.Status != 0 && cate.Status != 0 && season.Status != 0)
                           select new ProductResponse
                           {
                               ProductName = pro.ProductName,
                               CategoryId = cate.CategoryId,
                               SeasonId = season.SeasonId,
                               Image = pro.Avatar,
                               Price = pro.Price,
                               ProductId = pro.ProductId,
                               Status = pro.Status,
                               Size = pro.Size,
                               Material = pro.Material,
                               Description = pro.Description

                           }).FirstOrDefault(x => x.ProductId == id && x.Status != 0);


            var proFile = _context.ProductFiles.Where(x => x.Status == 1 && x.ProductId == id).ToList();

            var data = new
            {
                Product = product,
                PFile = proFile
            };

            return new JsonResult(data);

        }
        public async Task UpdateProductAsync(int id, ProductDto pro)
        {
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
                    //  product.PlayerId = pro.PlayerId ?? product.PlayerId;
                    product.SeasonId = pro.SeasonId ?? product.SeasonId;
                    product.CategoryId = pro.CategoryId ?? product.CategoryId;
                    product.Description = pro.Description ?? product.Description;
                    if (pro.Avatar != null && pro.Avatar.Length > 0)
                    {
                        ImageUploadResult res = _cloudinary.CloudinaryUpload(pro.Avatar);
                        product.Avatar = res.SecureUrl.ToString();
                    }
                    product.Price = pro.Price ?? product.Price;
                    product.Size = pro.Size ?? product.Size;
                    product.Color = pro.Color ?? product.Color;
                    product.Material = pro.Material ?? product.Material;
                    product.Status = pro.Status ?? product.Status;

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
            List<ProductCategory> cate = _context.ProductCategories
                                                       .Where(x => x.Status == 1)
                                                       .ToList();
            //lấy player
            List<Player> players = _context.Players
                                           .Where(x => x.Status == 1)
                                           .ToList();
            //lấy mùa giải
            List<Season> season = _context.Seasons
                                           .Where(x => x.Status == 1)
                                           .ToList();
            var data = new
            {
                Cate = cate,
                Player = players,
                Season = season
            };

            return new JsonResult(data);
        }
    }
}
