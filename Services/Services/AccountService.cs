using BusinessObjects.Models;
using DAOs.Request;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.Interface;
using Services.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepo _customerRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(IAccountRepository accountRepository, ICustomerRepo customerRepo)
        {
            _accountRepository = accountRepository;
            _customerRepo = customerRepo;
        }

        public async Task<string> GetAccount(string email, string password)
        {
            return await _accountRepository.GetAccount(email, password);
        }

        public async Task<object> Login(string email, string password)
        {
            var (accessToken, refreshToken, role, accountId) = await _accountRepository.Login(email, password);
            
            switch (role.ToLower())
            {
                case "member":
                case "customer":
                    var customer = (await _customerRepo.GetCustomers())
                        .FirstOrDefault(c => c.AccountId == accountId);
                    
                    if (customer == null)
                        throw new Exception("Customer information not found");

                    return new
                    {
                        accessToken,
                        refreshToken,
                        role,
                        customerInfo = new
                        {
                            customerId = customer.CustomerId,
                            element = customer.Element,
                            lifePalace = customer.LifePalace
                        }
                    };

                case "admin":
                    return new
                    {
                        accessToken,
                        refreshToken,
                        role
                    };

                case "master":
                    return new
                    {
                        accessToken,
                        refreshToken,
                        role
                    };

                default:
                    throw new Exception($"Unsupported role: {role}");
            }
        }

        public async Task<string> Register(RegisterRequest registerRequest)
        {
            return await _accountRepository.Register(registerRequest);
        }

        public async Task<string> RefreshAccessToken()
        {
            return await _accountRepository.RefreshAccessToken();
        }

        public void Logout()
        {
            _accountRepository.Logout();
        }

        public async Task<string> RegisterGoogleUser(string name, string email)
        {
            return await _accountRepository.RegisterGoogleUser(name, email);
        }

        //public async Task<ResultModel> EditProfile(EditProfileRequest request)
        //{
        //    var res = new ResultModel();
        //    try
        //    {
        //        var currentUserIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid);
        //        if (currentUserIdClaim == null || !short.TryParse(currentUserIdClaim.Value, out short currentUserId))
        //        {
        //            res.IsSuccess = false;
        //            res.Code = (int)HttpStatusCode.BadRequest;
        //            res.Data = null;
        //            res.Message = "User is not authenticated or ID is invalid";
        //        }
        //        var existingAccount = await _accountRepository.GetAccount(currentUserId).FirstOrDefaultAsync();
        //        if (existingAccount == null)
        //        {
        //            return new ResultModel<UpdateProfileRequest>
        //            {
        //                IsSuccess = false,
        //                Code = (int)HttpStatusCode.NotFound,
        //                Message = "System account not found"
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.Code = (int)HttpStatusCode.InternalServerError;
        //        res.Data = null;
        //        res.Message = ex.Message;
        //    }
        //}
    }
}
