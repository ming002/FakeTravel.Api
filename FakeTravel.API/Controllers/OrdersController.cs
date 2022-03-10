using AutoMapper;
using FakeTravel.API.Dtos;
using FakeTravel.API.ResourceParameters;
using FakeTravel.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FakeTravel.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteReposity _touristRouteReposity;
        private readonly IMapper _mapper;
        public OrdersController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteReposity touristRouteReposity,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteReposity = touristRouteReposity;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrders(
            [FromQuery] PaginationResourceParameters parameters)
        {
            //1.获得当前用户
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //使用UserId来获取订单历史记录
            var orders=await _touristRouteReposity.GetOrdersByUserId(
                userId,parameters.PageSize,parameters.PageNum);

            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }
        [HttpGet("{orderId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid OrderId)       
        {
            //1.获得当前用户
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var order = await _touristRouteReposity.GetOrderById(OrderId);
            return Ok(_mapper.Map<OrderDto>(order));
        }
    }
}
