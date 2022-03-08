using AutoMapper;
using FakeTravel.API.Dtos;
using FakeTravel.API.Models;
using FakeTravel.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FakeTravel.API.Controllers
{
    [ApiController]
    [Route("api/shoppingCart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteReposity _touristRouteReposity;
        private readonly IMapper _mapper;
        public ShoppingCartController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteReposity touristRouteReposity,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteReposity = touristRouteReposity;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetShoppingCart()
        {
            //1.获得当前用户
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //使用UserId
            var shoppingCart = await _touristRouteReposity.GetShoppingCartByUserId(userId);
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }
        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddShoppingCartItem(
            [FromBody] AddShoppingCartDto addShoppingCartDto)
        {
            //1.获得当前用户
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //使用UserId
            var shoppingCart = await _touristRouteReposity
                .GetShoppingCartByUserId(userId);

            var touristRoute =await _touristRouteReposity
                .GetTouristRouteAsync(addShoppingCartDto.TouristRouteId);
            if (touristRoute == null)
            {
                return NotFound("旅游路线不存在");
            }
            var lineItem = new LineItem()
            {
                TouristRouteId = touristRoute.Id,
                ShoppingCartId = shoppingCart.Id,
                OriginPrice = touristRoute.OriginPrice,
                DiscountPresent = touristRoute.DiscountPresent,
            };
            await _touristRouteReposity.CreateLineItem(lineItem);
            await _touristRouteReposity.SaveAsync();
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }
        [HttpPost("items/{itemId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItem(
            [FromRoute] int itemId)
        {
            var lineItem = await _touristRouteReposity
                .GetShoppingCartItemByItem(itemId);
            if (lineItem == null)
            {
                return NotFound("购物车商品找不到");
            }
            _touristRouteReposity.DeleteShoppingCartItem(lineItem);
            await _touristRouteReposity.SaveAsync();
            return NoContent();
        }
    }
}
