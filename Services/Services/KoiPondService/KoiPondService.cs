using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.AccountRepository;
using Repositories.Repositories.CustomerRepository;
using Repositories.Repositories.KoiPondRepository;
using Repositories.Repositories.ShapeRepository;
using Services.ApiModels;
using Services.ApiModels.KoiPond;
using Services.ApiModels.Shape;

namespace Services.Services.KoiPondService
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
        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
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
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_NOT_SEND;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var token = authHeader.Substring("Bearer ".Length);
                if (string.IsNullOrEmpty(token))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var accountId = await _accountRepository.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                var customer = (await _customerRepo.GetCustomers())
                    .FirstOrDefault(c => c.AccountId == accountId);

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
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = customer.Element;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
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
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
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
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_DESTINY_FOUND;
                res.Data = result;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
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
                element = element.ToLower();
                var fengShuiInfo = element switch
                {
                    var e when e == NguHanh.Kim.ToString().ToLower() => new
                    {
                        SuitableDirections = new[] { "Tây", "Tây Bắc", "Bắc" },
                        RecommendedFishCount = new[] { 4, 9 },
                        SuitableFishColors = new[] { "Trắng", "Bạc", "Xám", "Vàng", "Nâu" },
                        Description = "Hồ cá phù hợp với người mệnh Kim nên đặt ở hướng Tây hoặc Tây Bắc (hành Kim) hoặc Bắc (hành Thủy - tương sinh với Kim). Hình dạng hồ nên là hình tròn hoặc oval."
                    },
                    var e when e == NguHanh.Mộc.ToString().ToLower() => new
                    {
                        SuitableDirections = new[] { "Đông", "Đông Nam" },
                        RecommendedFishCount = new[] { 3, 8 },
                        SuitableFishColors = new[] { "Xanh lá", "Đen", "Xanh dương" },
                        Description = "Hồ cá phù hợp với người mệnh Mộc nên đặt ở hướng Đông hoặc Đông Nam. Hình dạng hồ nên uốn lượn tự nhiên hoặc hình chữ nhật."
                    },
                    var e when e == NguHanh.Thủy.ToString().ToLower() => new
                    {
                        SuitableDirections = new[] { "Bắc", "Tây", "Tây Bắc" },
                        RecommendedFishCount = new[] { 1, 6 },
                        SuitableFishColors = new[] { "Đen", "Xanh dương", "Trắng", "Bạc" },
                        Description = "Hồ cá phù hợp với người mệnh Thủy nên đặt ở hướng Bắc hoặc Tây, Tây Bắc. Hình dạng hồ nên là hình tròn, oval hoặc dạng lượn sóng."
                    },
                    var e when e == NguHanh.Hỏa.ToString().ToLower() => new
                    {
                        SuitableDirections = new[] { "Nam" },
                        RecommendedFishCount = new[] { 2, 7 },
                        SuitableFishColors = new[] { "Đỏ", "Cam", "Xanh lá" },
                        Description = "Hồ cá phù hợp với người mệnh Hỏa nên đặt ở hướng Nam, nhưng cần có thác nước để cân bằng. Hình dạng hồ nên là hình tam giác hoặc có thác nước."
                    },
                    var e when e == NguHanh.Thổ.ToString().ToLower() => new
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
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsCompatibility.DESTINY_INVALID + element;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = fengShuiInfo;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy thông tin phong thủy: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetAllKoiPonds()
        {
            var res = new ResultModel();
            try
            {
                var koiPonds = await _koiPondRepo.GetKoiPonds();
                if (koiPonds == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_FOUND;
                res.Data = _mapper.Map<List<KoiPondResponse>>(koiPonds); ;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy danh sách hồ cá: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetKoiPondById(string id)
        {
            var res = new ResultModel();
            try
            {
                var koiPond = await _koiPondRepo.GetKoiPondById(id);
                if (koiPond == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_FOUND;
                res.Data = _mapper.Map<KoiPondResponse>(koiPond); ;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi lấy thông tin hồ cá: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetKoiPondByShapeId(string shapeId)
        {
            var res = new ResultModel();
            try
            {
                var koiPonds = await _koiPondRepo.GetKoiPondByShapeId(shapeId);
                if (koiPonds == null || !koiPonds.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_FOUND;
                res.Data = _mapper.Map<List<KoiPondResponse>>(koiPonds); ;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> CreateKoiPond(KoiPondRequest koiPond)
        {
            var res = new ResultModel();
            try
            {
                if (koiPond == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_INVALID;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var koiPondModel = _mapper.Map<KoiPond>(koiPond);
                koiPondModel.KoiPondId = GenerateShortGuid();
                var createdKoiPond = await _koiPondRepo.CreateKoiPond(koiPondModel);
                if (createdKoiPond == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_CREATE_FAILED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }
                var shape = await _shapeRepo.GetShapeById(createdKoiPond.ShapeId);
                if(shape == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsKoiPond.SHAPE_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_CREATED;
                res.Data = _mapper.Map<KoiPondResponse>(createdKoiPond);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi tạo hồ cá: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> UpdateKoiPond(string id, KoiPondRequest koiPond)
        {
            var res = new ResultModel();
            try
            {
                if (koiPond == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_INVALID;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var existingKoiPond = await _koiPondRepo.GetKoiPondById(id);
                if (existingKoiPond == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                // Cập nhật thuộc tính của existingKoiPond thay vì tạo một instance mới
                _mapper.Map(koiPond, existingKoiPond);

                var updatedKoiPond = await _koiPondRepo.UpdateKoiPond(existingKoiPond);
                if (updatedKoiPond == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_UPDATE_FAILED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_UPDATED;
                res.Data = _mapper.Map<KoiPondResponse>(updatedKoiPond);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi cập nhật hồ cá: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> DeleteKoiPond(string id)
        {
            var res = new ResultModel();
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = "ID hồ cá không hợp lệ.";
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var existingKoiPond = await _koiPondRepo.GetKoiPondById(id);
                if (existingKoiPond == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = "Hồ cá không tồn tại.";
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                await _koiPondRepo.DeleteKoiPond(id);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiPond.KOIPOND_DELETED;
                return res;
            }
            catch (Exception ex)
            {

                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Lỗi khi xóa hồ cá: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }


        public async Task<ResultModel> GetAllShapes()
        {
            var res = new ResultModel();
            try
            {
                var shapes = await _shapeRepo.GetShapes();
                if (shapes == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsKoiPond.SHAPE_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsKoiPond.SHAPE_FOUND;
                res.Data = _mapper.Map<List<ShapResponse>>(shapes); ;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }
    }
}
