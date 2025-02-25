using DAOs.Request;
using KoiFengSuiConsultingSystem.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Đăng nhập và lấy Access Token + Refresh Token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var (accessToken, refreshToken) = await _accountService.Login(request.Email, request.Password);
                return Ok(new { accessToken, refreshToken });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                var accessToken = await _accountService.Register(registerRequest);
                return Ok(new { accessToken });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Làm mới Access Token bằng Refresh Token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var newAccessToken = await _accountService.RefreshAccessToken();
                return Ok(new { accessToken = newAccessToken });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                _accountService.Logout();
                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
