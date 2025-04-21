using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.CustomerRepository;
using Repositories.Repositories;
using Services.ApiModels;
using Services.ApiModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Services.ServicesHelpers.FengShuiHelper;
using BusinessObjects.Enums;

namespace Services.Services.CustomerService;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepo _customerRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public CustomerService(ICustomerRepo customerRepo, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _customerRepo = customerRepo;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<Customer> CreateCustomer(Customer customer)
    {
        return await _customerRepo.CreateCustomer(customer);
    }

    public async Task DeleteCustomer(string customerId)
    {
        await _customerRepo.DeleteCustomer(customerId);
    }
    public async Task<Customer> GetCustomerById(string customerId)
    {
        return await _customerRepo.GetCustomerById(customerId);
    }

    public async Task<List<Customer>> GetCustomers()
    {
        return await _customerRepo.GetCustomers();
    }

    public async Task<ResultModel> GetCurrentCustomerElement()
    {
        var res = new ResultModel();
        try
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                res.StatusCode = StatusCodes.Status401Unauthorized;
                return res;
            }

            var claims = identity.Claims;
            var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountId))
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                res.StatusCode = StatusCodes.Status400BadRequest;
                return res;
            }

            var customer = await _customerRepo.GetElementLifePalaceById(accountId);

            if (customer == null)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.Message = ResponseMessageConstantsUser.CUSTOMER_INFO_NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                return res;
            }

            res.IsSuccess = true;
            res.ResponseCode = ResponseCodeConstants.SUCCESS;
            res.Message = ResponseMessageConstantsUser.GET_USER_INFO_SUCCESS;
            res.StatusCode = StatusCodes.Status200OK;
            res.Data = _mapper.Map<ElementLifePalaceDto>(customer);
            return res;
        }
        catch (Exception ex)
        {
            res.IsSuccess = false;
            res.ResponseCode = ResponseCodeConstants.FAILED;
            res.Message = $"Đã xảy ra lỗi: {ex.Message}";
            res.StatusCode = StatusCodes.Status500InternalServerError;
            return res;
        }
    }

    public async Task<Customer> UpdateCustomer(Customer customer)
    {
        return await _customerRepo.UpdateCustomer(customer);
    }


    public async Task<ResultModel> CalculateCompatibility(CompatibilityRequest request)
    {
        var res = new ResultModel();
        double compatibilityScore = 0;
        ElementLifePalaceDto elementLifePalace = null;

        if (request.BirthDate.HasValue)
        {
            var nguHanh = AmDuongNienHelper.GetNguHanh(request.BirthDate.Value.Year).NguHanh;
            elementLifePalace = new ElementLifePalaceDto
            {
                Element = nguHanh.ToString(),
            };
        }
        else
        {
            var result = await GetCurrentCustomerElement();
            if (!result.IsSuccess || result.Data == null)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                res.Message = ResponseMessageConstrantsCompatibility.DESTINY_NOT_FOUND;
                res.StatusCode = StatusCodes.Status404NotFound;
                return res;
            }

            elementLifePalace = result.Data as ElementLifePalaceDto;
        }

        if (elementLifePalace == null || string.IsNullOrEmpty(elementLifePalace.Element))
        {
            res.IsSuccess = false;
            res.Message = ResponseMessageConstrantsCompatibility.DESTINY_NOT_FOUND;
            res.StatusCode = StatusCodes.Status404NotFound;
            return res;
        }

        double totalRatio = request.ColorRatios.Values.Sum();
        if (Math.Abs(totalRatio - 100.0) > 0.01)
        {
            res.IsSuccess = false;
            res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
            res.Message = ResponseMessageConstrantsCompatibility.INCORRECT_TOTAL_COLOR_RATIO;
            res.StatusCode = StatusCodes.Status400BadRequest;
            return res;
        }

        string userElement = elementLifePalace.Element;
        int fishCount = request.FishCount;

        // Tính điểm theo màu sắc
        if (FengShuiHelper.ElementColorPoints.ContainsKey(userElement))
        {
            foreach (var color in request.ColorRatios)
            {
                if (FengShuiHelper.ElementColorPoints[userElement].ContainsKey(color.Key))
                {
                    double colorPoint = FengShuiHelper.ElementColorPoints[userElement][color.Key] * (color.Value / 100.0);
                    compatibilityScore += colorPoint;
                }
            }
        }

        // Tính điểm theo hình dạng hồ
        if (FengShuiHelper.ShapePoints.ContainsKey(userElement) && FengShuiHelper.ShapePoints[userElement].ContainsKey(request.PondShape))
        {
            compatibilityScore += FengShuiHelper.ShapePoints[userElement][request.PondShape];
        }

        // Tính điểm theo hướng hồ
        if (FengShuiHelper.DirectionPoints.ContainsKey(userElement) && FengShuiHelper.DirectionPoints[userElement].ContainsKey(request.PondDirection))
        {
            compatibilityScore += FengShuiHelper.DirectionPoints[userElement][request.PondDirection];
        }

        // Bonus theo số lượng cá
        compatibilityScore += FengShuiHelper.CalculateFishCountBonus(fishCount, userElement);

        // Chuẩn hóa điểm từ -50 đến 50 thành 0 - 100
        double minScore = -50;
        double maxScore = 50;
        double normalizedScore = (compatibilityScore - minScore) / (maxScore - minScore) * 100;
        double finalScore = Math.Round(normalizedScore, 2);

        // Gọi phương thức trả về message tương thích
        string message = FengShuiHelper.GetCompatibilityMessage(finalScore, userElement, fishCount);

        res.IsSuccess = true;
        res.Message = message;
        res.StatusCode = StatusCodes.Status200OK;
        res.Data = new FengShuiResult
        {
            CompatibilityScore = finalScore
        };
        return res;
    }
}

