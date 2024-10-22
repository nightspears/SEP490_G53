using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cart;

        public CartController(ICartRepository cart)
        {
            _cart = cart;

        }


        [HttpGet("addToCart")]
        public async void  addToCart(int productId , int quantity)
        {
            try
            {
                _cart.AddProductToCart(productId, quantity);

            }
            catch (Exception ex) {
                throw new Exception("Lỗi thêm sản phẩm");
            }

        }
        [HttpGet("updateCart")]
        public async void UpdateCart(int productId, int quantity)
        {
            try
            {
                _cart.UpdateProductToCart(productId, quantity);

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi update sản phẩm");
            }

        }
    }
}
