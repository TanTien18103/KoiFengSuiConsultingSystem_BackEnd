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

namespace Services.Services.CustomerService;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepo _customerRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    private static readonly Dictionary<string, Dictionary<string, double>> ElementColorPoints = new()
{
    { "Kim", new() { { "Trắng", 10 }, { "Vàng", 5 }, { "Xanh dương", -5 }, { "Đỏ", -10 }, { "Black", -8 } } },
    { "Mộc", new() { { "Xanh dương", 10 }, { "Black", 5 }, { "Trắng", -5 }, { "Vàng", -8 }, { "Đỏ", -10 } } },
    { "Thủy", new() { { "Black", 10 }, { "Trắng", 5 }, { "Xanh dương", -5 }, { "Vàng", -10 }, { "Đỏ", -8 } } },
    { "Hỏa", new() { { "Đỏ", 10 }, { "Xanh dương", 5 }, { "Trắng", -5 }, { "Vàng", -10 }, { "Black", -8 } } },
    { "Thổ", new() { { "Vàng", 10 }, { "Đỏ", 5 }, { "Xanh dương", -5 }, { "Trắng", -10 }, { "Black", -8 } } }
};

    private static readonly Dictionary<string, Dictionary<string, double>> ShapePoints = new()
{
    { "Kim", new() { { "Tròn", 8 }, { "Vuông", -5 }, { "Chữ nhật", -3 } } },
    { "Mộc", new() { { "Chữ nhật", 8 }, { "Tròn", -5 }, { "Vuông", -3 } } },
    { "Thủy", new() { { "Tự do", 8 }, { "Vuông", -5 }, { "Tròn", -3 } } },
    { "Hỏa", new() { { "Tam giác", 8 }, { "Chữ nhật", -5 }, { "Tròn", -3 } } },
    { "Thổ", new() { { "Vuông", 8 }, { "Tròn", -5 }, { "Tam giác", -3 } } }
};

    private static readonly Dictionary<string, Dictionary<string, double>> DirectionPoints = new()
{
    { "Kim", new() { { "Tây", 10 }, { "Đông", -5 }, { "Bắc", -3 } } },
    { "Mộc", new() { { "Đông", 10 }, { "Tây", -5 }, { "Nam", -3 } } },
    { "Thủy", new() { { "Bắc", 10 }, { "Nam", -5 }, { "Tây", -3 } } },
    { "Hỏa", new() { { "Nam", 10 }, { "Bắc", -5 }, { "Đông", -3 } } },
    { "Thổ", new() { { "Trung tâm", 10 }, { "Tây", -5 }, { "Đông", -3 } } }
};

    Dictionary<int, string> fishCountToElement = new()
    {
        { 1, "Thủy" }, { 6, "Thủy" },
        { 2, "Hỏa" }, { 7, "Hỏa" },
        { 3, "Mộc" }, { 8, "Mộc" },
        { 4, "Kim" }, { 9, "Kim" },
        { 5, "Thổ" }, { 10, "Thổ" }
    };

    Dictionary<string, string> elementGenerates = new()
    {
        { "Kim", "Thủy" },
        { "Thủy", "Mộc" },
        { "Mộc", "Hỏa" },
        { "Hỏa", "Thổ" },
        { "Thổ", "Kim" }
    };

    Dictionary<string, string> elementDestroys = new()
    {
        { "Kim", "Mộc" },
        { "Thủy", "Hỏa" },
        { "Mộc", "Thổ" },
        { "Hỏa", "Kim" },
        { "Thổ", "Thủy" }
    };
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

        var result = await GetCurrentCustomerElement();
        if (!result.IsSuccess || result.Data == null)
        {
            res.IsSuccess = false;
            res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
            res.Message = ResponseMessageConstrantsCompatibility.DESTINY_NOT_FOUND;
            res.StatusCode = StatusCodes.Status404NotFound;
            return res;
        }

        var elementLifePalace = result.Data as ElementLifePalaceDto;
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

        if (ElementColorPoints.ContainsKey(elementLifePalace.Element))
        {
            foreach (var color in request.ColorRatios)
            {
                if (ElementColorPoints[elementLifePalace.Element].ContainsKey(color.Key))
                {
                    double colorPoint = ElementColorPoints[elementLifePalace.Element][color.Key] * (color.Value / 100.0);
                    compatibilityScore += colorPoint;
                }
            }
        }

        if (ShapePoints.ContainsKey(elementLifePalace.Element) && ShapePoints[elementLifePalace.Element].ContainsKey(request.PondShape))
        {
            compatibilityScore += ShapePoints[elementLifePalace.Element][request.PondShape];
        }

        if (DirectionPoints.ContainsKey(elementLifePalace.Element) && DirectionPoints[elementLifePalace.Element].ContainsKey(request.PondDirection))
        {
            compatibilityScore += DirectionPoints[elementLifePalace.Element][request.PondDirection];
        }

        compatibilityScore += CalculateFishCountBonus(request.FishCount, elementLifePalace.Element);

        double minScore = -50;
        double maxScore = 50;
        double normalizedScore = (compatibilityScore - minScore) / (maxScore - minScore) * 100;
        double finalScore = Math.Round(normalizedScore, 2);

        res.IsSuccess = true;
        res.Message = GetCompatibilityMessage(finalScore);
        res.StatusCode = StatusCodes.Status200OK;
        res.Data =
            new FengShuiResult
            {
                CompatibilityScore = finalScore,
                Message = res.Message
            };
        return res;
    }

    private double CalculateFishCountBonus(int fishCount, string userElement)
    {
        int modFishCount = fishCount % 10;
        if (modFishCount == 0) modFishCount = 10;


        if (fishCountToElement.ContainsKey(modFishCount))
        {
            string fishElement = fishCountToElement[modFishCount];

            if (fishElement == userElement) return 10;
            if (elementGenerates[fishElement] == userElement) return 10;
            if (elementDestroys[fishElement] == userElement) return -10;
        }

        return 0;
    }

    private string GetCompatibilityMessage(double score)
    {
        if (score < 20)
            return ResponseMessageConstrantsCompatibility.VERY_LOW;
        else if (score < 40)
            return ResponseMessageConstrantsCompatibility.LOW;
        else if (score < 60)
            return ResponseMessageConstrantsCompatibility.MEDIUM;
        else if (score < 80)
            return ResponseMessageConstrantsCompatibility.HIGH;
        else
            return ResponseMessageConstrantsCompatibility.VERY_HIGH;
    }


}
