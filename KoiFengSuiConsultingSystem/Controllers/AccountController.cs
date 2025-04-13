using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Services.ApiModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Services.Services.AccountService;
using Services.ApiModels;
using BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.AccountRepository;
using Services.ApiModels.Master;
using Repositories.Repositories.CustomerRepository;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAccountRepo _accountRepo;
        private readonly ICustomerRepo _customerRepo;

        public AccountController(IAccountService accountService, IAccountRepo accountRepo, ICustomerRepo customerRepo)
        {
            _accountService = accountService;
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
        }


        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                return BadRequest("Google authentication failed.");
            }

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            if (claims == null)
            {
                return BadRequest("No claims found.");
            }

            string email = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            string name = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
            {
                return BadRequest("Email or name not found in claims.");
            }

            var accessToken = await _accountService.RegisterGoogleUser(name, email);

            return Ok(new { accessToken });
        }

        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse), "Account", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Services.ApiModels.Account.LoginRequest request)
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

      
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] Services.ApiModels.Account.RegisterRequest registerRequest)
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

        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile([FromForm] EditProfileRequest request)
        {
            var res = await _accountService.EditProfile(request);
            return StatusCode(res.StatusCode, res);
        }


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

        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var claims = identity.Claims;
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var user = await _accountRepo.GetAccountById(accountId);
                var customer = await _customerRepo.GetCustomerByAccountId(accountId);
                if (user != null)
                {
                    return Ok(new
                    {
                        AccountId = accountId,
                        Email = email,
                        Role = role,
                        PhoneNumber = user.PhoneNumber,
                        FullName = user.FullName,
                        Gender = user.Gender,
                        Dob = user.Dob,
                        ImageUrl = customer?.ImageUrl,
                        BankId = user.BankId,
                        AccountNo = user.AccountNo,
                        AccountName = user.AccountName,
                    });
                }

                return Ok(new
                {
                    AccountId = accountId,
                    Email = email,
                    Role = role,
                    PhoneNumber = (string)null,
                    FullName = (string)null,
                    Gender = user.Gender,
                    Dob = user.Dob,
                    ImageUrl = customer?.ImageUrl,
                    BankId = user.BankId,
                    AccountNo = user.AccountNo,
                    AccountName = user.AccountName,
                });
            }

            return Unauthorized(new { message = "User is not logged in." });
        }


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

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var res = await _accountService.ChangePassword(request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] Services.ApiModels.Account.ForgotPasswordRequest forgotPasswordRequest)
        {
                var res = await _accountService.ForgotPassword(forgotPasswordRequest.Email);
                return Ok(res);
        }
        [HttpGet("accounts")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAccounts([FromQuery] RoleEnums? role = null)
        {
            var result = await _accountService.GetAllAccounts(role?.ToString());
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-all-staff")]
        public async Task<IActionResult> GetAllStaffs()
        {
            var result = await _accountService.GetAllStaff();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-accounts-by-role/{role}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAccountsByRole(string role)
        {
            var result = await _accountService.GetAccountsByRole(role);
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

        [HttpPut("update-master-profile")]
        [Authorize(Roles = "Master")]
        public async Task<IActionResult> UpdateMasterProfile([FromForm] MasterRequest request)
        {
            var result = await _accountService.UpdateMasterProfile(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
