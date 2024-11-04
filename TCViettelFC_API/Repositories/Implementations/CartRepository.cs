using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using TCViettelFC_API.Dtos.Category;
using TCViettelFC_API.Dtos.Matches;
using TCViettelFC_API.Dtos.Product;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;




namespace TCViettelFC_API.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly Sep490G53Context _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("keyCartSep490G53keyCartSep490G53"); 
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("keyCartSep490G53"); 

        public CartRepository(Sep490G53Context context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _contextAccessor = httpContextAccessor;

        }

        public static string EncryptString(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string DecryptString(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        //public void AddTicketToCard(int customerId, int quantityTicket = 1)
        //{
        //    try
        //    {
        //        var httpContext = _contextAccessor.HttpContext;
        //        string cartTicketCookie = httpContext.Request.Cookies["CartTicket"];
        //        List<TicketCart> cartItem;
        //    }
        //}
        public void AddProductToCart(int productId, int quantity = 1)
        {

            try
            {
                var httpContext = _contextAccessor.HttpContext;

                string cartCookie = httpContext.Request.Cookies["CartProduct"];

                List<ProductCart> cartItems;


                if (string.IsNullOrEmpty(cartCookie))
                {
                    cartItems = new List<ProductCart>();
                }
                else
                {
                    string decryptedCart = DecryptString(cartCookie);
                    cartItems = JsonSerializer.Deserialize<List<ProductCart>>(decryptedCart);
                }

                ProductCart item = cartItems.FirstOrDefault(x => x.ProductId == productId);
                Product Product = _context.Products.FirstOrDefault(x => x.ProductId == productId);
                if (item != null)
                {
                    item.Quantity += quantity;
                }
                else
                {
                    cartItems.Add(new ProductCart
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        ProductName = Product.ProductName,
                        Price = Product.Price,
                        Avatar = Product.Avatar,
                        Size = Product.Size,
                        Material = Product.Material,
                        Description = Product.Description,
                        CategoryId = Product.CategoryId,

                    });
                }

                // Lưu lại cookie
                string updatedCartJson = JsonSerializer.Serialize(cartItems);
                string encryptedCart = EncryptString(updatedCartJson);
                CookieOptions op = new CookieOptions 
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = true
                };

                httpContext.Response.Cookies.Append("CartProduct", encryptedCart, op);
            }
            catch
            {

            }
        }

        public void UpdateProductToCart(int productId, int quantity)
        {
            var httpContext = _contextAccessor.HttpContext;

            string cartCookie = httpContext.Request.Cookies["CartProduct"];
            List<ProductCart> cartItems;
            if (cartCookie == null || string.IsNullOrEmpty(cartCookie))
            {
                return;
            }
            string decryptedCart = DecryptString(cartCookie);
            cartItems = JsonSerializer.Deserialize<List<ProductCart>>(decryptedCart);

            ProductCart item = cartItems.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }

                // Lưu lại cookie
                string updatedCartJson = JsonSerializer.Serialize(cartItems);
                string encryptedCart = EncryptString(updatedCartJson);
                CookieOptions op = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = true
                };

                httpContext.Response.Cookies.Append("CartProduct", encryptedCart, op);

             
            }
        }
    }
}
