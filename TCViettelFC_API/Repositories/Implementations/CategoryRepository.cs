using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TCViettelFC_API.Dtos.Category;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly Sep490G53Context _context;

        public CategoryRepository(Sep490G53Context context)
        {
            _context = context;

        }
        public async Task AddCateAsync(CategoryDto category)
        {

            ProductCategory cate = new ProductCategory();
            {
                cate.CategoryName = category.CategoryName;
                cate.CreatedAt = DateTime.Now;
                cate.Status = category.Status;
            };

          

            try
            {
                await _context.ProductCategories.AddAsync(cate);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Matches not found");
            }
        }
        public async Task DeleteCateAsync(int id)
        {
            var cate = await _context.ProductCategories.FindAsync(id);
            if (cate == null|| cate.Status == 0) throw new KeyNotFoundException("Category not found");

            try
            {
                cate.Status = 0;
                await _context.SaveChangesAsync();
               
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception("Delete failed", ex);
            }
        }
      

        public async Task<List<ProductCategory>> GetCateAsync()
        {
            List<ProductCategory> cate = new List<ProductCategory>();
            cate = await _context.ProductCategories.Where(x => x.Status != 0).ToListAsync();

            return cate;
        }
        public async Task<ProductCategory> GetCateByIdAsync(int id)
        {
            ProductCategory category = new ProductCategory();
            category =  _context.ProductCategories.FirstOrDefault(x => x.CategoryId == id );   
            if (category == null)
            {
                throw new Exception("Matches not found");
            }
            else
            {
                return category;
            }
        }
        public async Task UpdateCateAsync(int id, CategoryDto category)
        {
            try
            {
                var cate = await _context.ProductCategories.FindAsync(id);
                if (cate == null || cate.Status == 0)
                {
                    throw new Exception("Matches not found");
                }

                // Update category properties
                cate.CategoryName = category.CategoryName ?? cate.CategoryName;
                cate.Status = category.Status ?? cate.Status;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating category: " + ex.Message);
            }
        }
    }
}
