﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class NewRepository : INewRepository
    {
        private readonly Sep490G53Context _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        public NewRepository(Sep490G53Context context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _contextAccessor = httpContextAccessor;
        }
        public IQueryable<GetNewDto> GetAllNewsAsQueryable() 
        { 
            var news  =  _context.News.Include(x=> x.Creator).Include(x => x.NewsCategory).Select(n => new GetNewDto
            {
                Id = n.Id,
                CreatorId = n.CreatorId,
                NewsCategory = n.NewsCategory.CategoryName,
                Title = n.Title,
                Content = n.Content,
                Image = n.Image,
                CreatedAt = n.CreatedAt,
                Status = n.Status

            }).AsQueryable();

            return news;

        }
        public async Task<List<GetNewDto>> GetAllNewsAsync()
            {
                var news = await _context.News.Include(x => x.Creator).Include(x => x.NewsCategory)
                    .Select(n => new GetNewDto
                    {
                        Id = n.Id,
                        CreatorId = n.CreatorId,
                        NewsCategory = n.NewsCategory.CategoryName,
                        Title = n.Title,
                        Content = n.Content,
                        Image = n.Image,
                        CreatedAt = n.CreatedAt,
                        Status = n.Status
                    })
                    .ToListAsync();

                return news;
            
        }
        // Method to get news by ID and convert to GetNewDto
        public async Task<GetNewDto?> GetNewsByIdAsync(int id)
        {
            var news = await _context.News
                .Include(x => x.Creator)
                .Include(x => x.NewsCategory)
                .Where(n => n.Id == id)
                .Select(n => new GetNewDto
                {
                    Id = n.Id,
                    CreatorId = n.CreatorId,
                    NewsCategory = n.NewsCategory.CategoryName,
                    Title = n.Title,
                    Content = n.Content,
                    Image = n.Image,
                    CreatedAt = n.CreatedAt,
                    Status = n.Status
                })
                .FirstOrDefaultAsync();

            return news;
        }
        public async Task<bool> UpdateNewsStatusAsync(int id, int newStatus)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);
            if (news == null)
            {
                return false; 
            }
            news.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CreateNewsAsync(CreateNewDto newDto)
        {
            var news = new News
            {
                CreatorId = newDto.CreatorId,
                NewsCategoryId = newDto.NewsCategoryId,
                Title = newDto.Title,
                Content = newDto.Content,
                Image = newDto.Image,
                CreatedAt = DateTime.UtcNow,
                Status = newDto.Status
            };

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            return news.Id;
        }

        public async Task<bool> UpdateNewsAsync(int id, UpdateNewDto newDto)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);
            if (news == null)
            {
                return false;
            }
            news.CreatorId = newDto.CreatorId;
            news.NewsCategoryId = newDto.NewsCategoryId;
            news.Title = newDto.Title;
            news.Content = newDto.Content;
            news.Image = newDto.Image;
            news.CreatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteNewsAsync(int id)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);
            if (news == null)
            {
                return false;
            }
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
