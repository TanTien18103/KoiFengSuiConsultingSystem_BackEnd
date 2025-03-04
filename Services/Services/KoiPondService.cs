using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KoiFengSuiConsultingSystem.Response;

namespace Services.Services
{
    public class KoiPondService : IKoiPondService
    {
        private readonly IKoiPondRepo _koiPondRepo;
        private readonly IShapeRepo _shapeRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public KoiPondService(IKoiPondRepo koiPondRepo, IShapeRepo shapeRepo, ICustomerRepo customerRepo, IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _koiPondRepo = koiPondRepo;
            _shapeRepo = shapeRepo;
            _customerRepo = customerRepo;
            _accountRepository = accountRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private async Task<string> GetCustomerElement()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("No authentication token provided");
            }

            var token = authHeader.Substring("Bearer ".Length);
            var accountId = await _accountRepository.GetAccountIdFromToken(token);
            
            var customer = (await _customerRepo.GetCustomers())
                .FirstOrDefault(c => c.AccountId == accountId);

            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            return customer.Element;
        }

        public async Task<object> GetPondRecommendations()
        {
            try
            {
                var customerElement = await GetCustomerElement();

                var suitableShapes = (await _shapeRepo.GetShapes())
                    .Where(s => s.Element.Equals(customerElement, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!suitableShapes.Any())
                {
                    throw new Exception($"Không tìm thấy hình dạng hồ phù hợp cho mệnh {customerElement}");
                }

                var suitablePonds = new List<KoiPondResponse>();
                foreach (var shape in suitableShapes)
                {
                    var ponds = (await _koiPondRepo.GetKoiPonds())
                        .Where(p => p.ShapeId == shape.ShapeId)
                        .ToList();

                    var pondResponses = _mapper.Map<List<KoiPondResponse>>(ponds);
                    suitablePonds.AddRange(pondResponses);
                }

                var fengShuiInfo = GetFengShuiInfo(customerElement);

                return new
                {
                    Ponds = suitablePonds,
                    FengShuiInfo = fengShuiInfo
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting pond recommendations: {ex.Message}", ex);
            }
        }

        private object GetFengShuiInfo(string element)
        {
            return element.ToLower() switch
            {
                "metal" => new
                {
                    SuitableDirections = new[] { "Tây", "Tây Bắc", "Bắc" },
                    RecommendedFishCount = new[] { 4, 9 },
                    SuitableFishColors = new[] { "Trắng", "Bạc", "Xám", "Vàng", "Nâu" },
                    Description = "Hồ cá phù hợp với người mệnh Kim nên đặt ở hướng Tây hoặc Tây Bắc (hành Kim) hoặc Bắc (hành Thủy - tương sinh với Kim). Hình dạng hồ nên là hình tròn hoặc oval."
                },
                "wood" => new
                {
                    SuitableDirections = new[] { "Đông", "Đông Nam" },
                    RecommendedFishCount = new[] { 3, 8 },
                    SuitableFishColors = new[] { "Xanh lá", "Đen", "Xanh dương" },
                    Description = "Hồ cá phù hợp với người mệnh Mộc nên đặt ở hướng Đông hoặc Đông Nam. Hình dạng hồ nên uốn lượn tự nhiên hoặc hình chữ nhật."
                },
                "water" => new
                {
                    SuitableDirections = new[] { "Bắc", "Tây", "Tây Bắc" },
                    RecommendedFishCount = new[] { 1, 6 },
                    SuitableFishColors = new[] { "Đen", "Xanh dương", "Trắng", "Bạc" },
                    Description = "Hồ cá phù hợp với người mệnh Thủy nên đặt ở hướng Bắc hoặc Tây, Tây Bắc. Hình dạng hồ nên là hình tròn, oval hoặc dạng lượn sóng."
                },
                "fire" => new
                {
                    SuitableDirections = new[] { "Nam" },
                    RecommendedFishCount = new[] { 2, 7 },
                    SuitableFishColors = new[] { "Đỏ", "Cam", "Xanh lá" },
                    Description = "Hồ cá phù hợp với người mệnh Hỏa nên đặt ở hướng Nam, nhưng cần có thác nước để cân bằng. Hình dạng hồ nên là hình tam giác hoặc có thác nước."
                },
                "earth" => new
                {
                    SuitableDirections = new[] { "Tây Nam", "Đông Bắc" },
                    RecommendedFishCount = new[] { 5, 10 },
                    SuitableFishColors = new[] { "Vàng", "Nâu", "Đỏ", "Cam" },
                    Description = "Hồ cá phù hợp với người mệnh Thổ nên đặt ở hướng Tây Nam hoặc Đông Bắc. Hình dạng hồ nên là hình vuông."
                },
                _ => throw new Exception("Mệnh không hợp lệ")
            };
        }

        public async Task<List<KoiPondResponse>> GetAllKoiPonds()
        {
            var koiPonds = await _koiPondRepo.GetKoiPonds();
            return _mapper.Map<List<KoiPondResponse>>(koiPonds);
        }

        public async Task<KoiPondResponse> GetKoiPondById(string id)
        {
            var koiPond = await _koiPondRepo.GetKoiPondById(id);
            return _mapper.Map<KoiPondResponse>(koiPond);
        }
    }
} 