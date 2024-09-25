﻿using TCViettelFC_API.Dtos;
namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<int> RegisterAsync(RegisterDto registerDto);
        bool ExistedUser(string username, string phoneNumber);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<string> AdminLoginAsync(AdminLoginDto loginDto);
    }
}
