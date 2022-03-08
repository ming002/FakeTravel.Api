using FakeTravel.API.Dtos;
using FakeTravel.API.Models;
using FakeTravel.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FakeTravel.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITouristRouteReposity _touristRouteReposity;
        public AuthenticateController(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            //1.验证用户名和密码
            var loginResult = await _signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                false, false);
            if (!loginResult.Succeeded)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByNameAsync(loginDto.Email);
            //2.创建jwt
            var signingAlgorithm = SecurityAlgorithms.HmacSha256;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id),
            };
            var roleNames = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);
            }

            var secretyByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecreKey"]);
            //使用非对称算法对私钥加密
            var signingKey = new SymmetricSecurityKey(secretyByte);
            //使用sha256验证加密后的私钥
            var signingCredentials = new SigningCredentials(signingKey, signingAlgorithm);
            //生成token
            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials
                );
            //以字符串形式添加token
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            //3.初始化购物车
            var shoppingCart=new ShoppingCart()
            {
                Id=Guid.NewGuid(),
                UserId=user.Id
            };
            await _touristRouteReposity.CreateShoppingCart(shoppingCart);
            await _touristRouteReposity.SaveAsync();
            //4.return 200ok+jwt    
            return Ok(tokenStr);

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            //1.使用用户名创建用户对象
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
            };
            //2.hash密码，保存用户
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            return Ok();
            //3.return
        }
    }
}
