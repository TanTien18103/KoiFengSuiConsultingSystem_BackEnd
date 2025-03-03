using BusinessObjects.Models;
using DAOs.DTOs;
using KoiFengSuiConsultingSystem.Request;
using KoiFengSuiConsultingSystem.Respone;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Repositories.Repository;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepo _customerRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly Dictionary<string, Dictionary<string, double>> ElementColorPoints = new()
{
    { "Metal", new() { { "White", 10 }, { "Yellow", 5 }, { "Blue", -5 }, { "Red", -10 }, { "Black", -8 } } },
    { "Wood", new() { { "Blue", 10 }, { "Black", 5 }, { "White", -5 }, { "Yellow", -8 }, { "Red", -10 } } },
    { "Water", new() { { "Black", 10 }, { "White", 5 }, { "Blue", -5 }, { "Yellow", -10 }, { "Red", -8 } } },
    { "Fire", new() { { "Red", 10 }, { "Blue", 5 }, { "White", -5 }, { "Yellow", -10 }, { "Black", -8 } } },
    { "Earth", new() { { "Yellow", 10 }, { "Red", 5 }, { "Blue", -5 }, { "White", -10 }, { "Black", -8 } } }
};

    private static readonly Dictionary<string, Dictionary<string, double>> ShapePoints = new()
{
    { "Metal", new() { { "Circle", 8 }, { "Square", -5 }, { "Rectangular", -3 } } },
    { "Wood", new() { { "Rectangular", 8 }, { "Circle", -5 }, { "Square", -3 } } },
    { "Water", new() { { "Uncertain", 8 }, { "Square", -5 }, { "Circle", -3 } } },
    { "Fire", new() { { "Tam giác", 8 }, { "Rectangular", -5 }, { "Circle", -3 } } },
    { "Earth", new() { { "Square", 8 }, { "Circle", -5 }, { "Tam giác", -3 } } }
};

    private static readonly Dictionary<string, Dictionary<string, double>> DirectionPoints = new()
{
    { "Metal", new() { { "West", 10 }, { "East", -5 }, { "North", -3 } } },
    { "Wood", new() { { "East", 10 }, { "West", -5 }, { "South", -3 } } },
    { "Water", new() { { "North", 10 }, { "South", -5 }, { "West", -3 } } },
    { "Fire", new() { { "South", 10 }, { "North", -5 }, { "East", -3 } } },
    { "Earth", new() { { "Trung tâm", 10 }, { "West", -5 }, { "East", -3 } } }
};

    Dictionary<int, string> fishCountToElement = new()
    {
        { 1, "Water" }, { 6, "Water" },
        { 2, "Fire" }, { 7, "Fire" },
        { 3, "Wood" }, { 8, "Wood" },
        { 4, "Metal" }, { 9, "Metal" },
        { 5, "Earth" }, { 10, "Earth" }
    };

    Dictionary<string, string> elementGenerates = new()
    {
        { "Metal", "Water" },
        { "Water", "Wood" },
        { "Wood", "Fire" },
        { "Fire", "Earth" },
        { "Earth", "Metal" }
    };

    Dictionary<string, string> elementDestroys = new()
    {
        { "Metal", "Wood" },
        { "Water", "Fire" },
        { "Wood", "Earth" },
        { "Fire", "Metal" },
        { "Earth", "Water" }
    };
    public CustomerService(ICustomerRepo customerRepo, IHttpContextAccessor httpContextAccessor)
    {
        _customerRepo = customerRepo;
        _httpContextAccessor = httpContextAccessor;
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

    public async Task<ElementLifePalaceDto> GetElementLifePalaceById()
    {
        var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
        if (identity == null || !identity.IsAuthenticated)
        {
            return null;
        }

        var claims = identity.Claims;
        var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(accountId))
        {
            return null;
        }

        return await _customerRepo.GetElementLifePalaceById(accountId);
    }



    public async Task<Customer> UpdateCustomer(Customer customer)
    {
        return await _customerRepo.UpdateCustomer(customer);
    }

    public async Task<FengShuiResult> CalculateCompatibility(CompatibilityRequest request)
    {
        double compatibilityScore = 0;

        var result = await GetElementLifePalaceById();
        if (result == null)
        {
            return new FengShuiResult
            {
                CompatibilityScore = 0,
                Message = "Không tìm thấy cung mệnh. Vui lòng kiểm tra lại!"
            };
        }

        double totalRatio = request.ColorRatios.Values.Sum();
        if (Math.Abs(totalRatio - 100.0) > 0.01)
        {
            return new FengShuiResult
            {
                CompatibilityScore = 0,
                Message = "Tổng tỷ lệ màu cá phải bằng 100%. Vui lòng kiểm tra lại!"
            };
        }

        if (ElementColorPoints.ContainsKey(result.Element))
        {
            foreach (var color in request.ColorRatios)
            {
                if (ElementColorPoints[result.Element].ContainsKey(color.Key))
                {
                    double colorPoint = ElementColorPoints[result.Element][color.Key] * (color.Value / 100.0);
                    compatibilityScore += colorPoint;
                }
            }
        }

        if (ShapePoints.ContainsKey(result.Element) && ShapePoints[result.Element].ContainsKey(request.PondShape))
        {
            compatibilityScore += ShapePoints[result.Element][request.PondShape];
        }

        if (DirectionPoints.ContainsKey(result.Element) && DirectionPoints[result.Element].ContainsKey(request.PondDirection))
        {
            compatibilityScore += DirectionPoints[result.Element][request.PondDirection];
        }

        compatibilityScore += CalculateFishCountBonus(request.FishCount, result.Element);

        double minScore = -50;
        double maxScore = 50;
        double normalizedScore = ((compatibilityScore - minScore) / (maxScore - minScore)) * 100;
        double finalScore = Math.Round(normalizedScore, 2);

        return new FengShuiResult
        {
            CompatibilityScore = finalScore,
            Message = GetCompatibilityMessage(finalScore) 
        };
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
            return "Rất không hợp, cần xem xét lại phong thủy.";
        else if (score < 40)
            return "Hợp mức thấp, có thể cải thiện thêm.";
        else if (score < 60)
            return "Hợp trung bình, có thể chấp nhận.";
        else if (score < 80)
            return "Hợp tốt, có thể sử dụng.";
        else
            return "Rất hợp phong thủy, lý tưởng!";
    }


}
