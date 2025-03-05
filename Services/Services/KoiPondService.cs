using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.ApiModels;
using Services.ApiModels.KoiPond;

namespace Services.Services
{
    public class KoiPondService : IKoiPondService
    {
        private readonly IKoiPondRepo _koiPondRepo;
        private readonly IShapeRepo _shapeRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IAccountRepo _accountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public KoiPondService(IKoiPondRepo koiPondRepo, IShapeRepo shapeRepo, ICustomerRepo customerRepo, IAccountRepo accountRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _koiPondRepo = koiPondRepo;
            _shapeRepo = shapeRepo;
            _customerRepo = customerRepo;
            _accountRepository = accountRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private async Task<ResultModel> GetCustomerElement()
        {
            var res = new ResultModel();
            try
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    res.IsSuccess = false;
                    res.Message = "Token xác thực không được cung cấp";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var token = authHeader.Substring("Bearer ".Length);
                if (string.IsNullOrEmpty(token))
                {
                    res.IsSuccess = false;
                    res.Message = "Token không hợp lệ";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var accountId = await _accountRepository.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.Message = "Token không hợp lệ hoặc đã hết hạn";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var customer = (await _customerRepo.GetCustomers())
                    .FirstOrDefault(c => c.AccountId == accountId);

                if (customer == null)
                {
                    res.IsSuccess = false;
                    res.Message = "Không tìm thấy thông tin khách hàng";
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = customer.Element;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Lỗi khi lấy thông tin mệnh của khách hàng: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetPondRecommendations()
        {
            var res = new ResultModel();
            try
            {
                var elementResult = await GetCustomerElement();
                if (!elementResult.IsSuccess)
                {
                    res.IsSuccess = false;
                    res.Message = elementResult.Message;
                    res.StatusCode = elementResult.StatusCode;
                    return res;
                }

                var customerElement = elementResult.Data.ToString().ToLower();

                var suitableShapes = (await _shapeRepo.GetShapes())
                    .Where(s => s.Element.Equals(customerElement, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!suitableShapes.Any())
                {
                    res.IsSuccess = false;
                    res.Message = $"Không tìm thấy hình dạng hồ phù hợp cho mệnh {customerElement}";
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
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
                if (!fengShuiInfo.IsSuccess)
                {
                    res.IsSuccess = false;
                    res.Message = fengShuiInfo.Message;
                    res.StatusCode = fengShuiInfo.StatusCode;
                    return res;
                }

                var result = new
                {
                    Ponds = suitablePonds,
                    FengShuiInfo = fengShuiInfo.Data
                };

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = "Successfully";
                res.Data = result;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Lỗi khi lấy gợi ý hồ cá: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        private ResultModel GetFengShuiInfo(string element)
        {
            var res = new ResultModel();
            try
            {
                if (string.IsNullOrEmpty(element))
                {
                    res.IsSuccess = false;
                    res.Message = "Mệnh không được để trống";
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                element = element.ToLower();
                var fengShuiInfo = element switch
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
                    _ => null
                };

                if (fengShuiInfo == null)
                {
                    res.IsSuccess = false;
                    res.Message = $"Mệnh không hợp lệ: {element}";
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = fengShuiInfo;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Lỗi khi lấy thông tin phong thủy: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel<List<KoiPondResponse>>> GetAllKoiPonds()
        {
            var res = new ResultModel<List<KoiPondResponse>>();
            try
            {
                var koiPonds = await _koiPondRepo.GetKoiPonds();
                var response = _mapper.Map<List<KoiPondResponse>>(koiPonds);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = response;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Lỗi khi lấy danh sách hồ cá: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel<KoiPondResponse>> GetKoiPondById(string id)
        {
            var res = new ResultModel<KoiPondResponse>();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    res.IsSuccess = false;
                    res.Message = "ID hồ cá không được để trống";
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var koiPond = await _koiPondRepo.GetKoiPondById(id);
                if (koiPond == null)
                {
                    res.IsSuccess = false;
                    res.Message = $"Không tìm thấy hồ cá với ID: {id}";
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var response = _mapper.Map<KoiPondResponse>(koiPond);
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = response;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Lỗi khi lấy thông tin hồ cá: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }
    }
} 