using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels;
using Services.Services.AccountService;

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
        /// Lấy danh sách tài khoản theo role
        /// </summary>
        /// <param name="role">Role để lọc (null hoặc --: lấy tất cả, 0: Admin, 1: Customer, 2: Master, 3: Staff)</param>
        /// <returns>Danh sách tài khoản đã lọc</returns>
        [HttpGet("accounts")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAccounts([FromQuery] string? role = "--")
        {
            string? filterRole = role switch
            {
                null or "--" => null, // Không filter
                "0" => "Admin",
                "1" => "Customer", 
                "2" => "Master",
                "3" => "Staff",
                _ => null
            };

            var result = await _accountService.GetAllAccounts(filterRole);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("toggle-account-status/{accountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleAccountStatus(string accountId, [FromQuery] bool isActive)
        {
            var result = await _accountService.ToggleAccountStatus(accountId, isActive);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete-account/{accountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAccount(string accountId)
        {
            var result = await _accountService.DeleteAccount(accountId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update-account-role/{accountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAccountRole(string accountId, [FromQuery] string newRole)
        {
            var result = await _accountService.UpdateAccountRole(accountId, newRole);
            return StatusCode(result.StatusCode, result);
        }
    }
} 