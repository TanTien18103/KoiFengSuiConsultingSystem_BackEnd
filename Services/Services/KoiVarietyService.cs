using AutoMapper;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Services.ApiModels;
using Services.ApiModels.Color;
using Services.ApiModels.KoiVariety;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Services.Services
{
    public class KoiVarietyService : IKoiVarietyService
    {
        private readonly IKoiVarietyRepo _koiVarietyRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepo _accountRepo;

        public KoiVarietyService(IKoiVarietyRepo koiVarietyRepo, ICustomerRepo customerRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountRepo accountRepo)
        {
            _koiVarietyRepo = koiVarietyRepo;
            _customerRepo = customerRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountRepo = accountRepo;
        }
        
        public async Task<KoiVariety> CreateKoiVarietyAsync(KoiVariety koiVariety)
        {
            return await _koiVarietyRepo.CreateKoiVariety(koiVariety);
        }

        public async Task DeleteKoiVarietyAsync(string koiVarietyId)
        {
            await _koiVarietyRepo.DeleteKoiVariety(koiVarietyId);
        }

        public async Task<List<KoiVariety>> GetKoiVarietiesAsync()
        {
            return await _koiVarietyRepo.GetKoiVarieties();
        }

        public async Task<KoiVariety> GetKoiVarietyByIdAsync(string koiVarietyId)
        {
            return await _koiVarietyRepo.GetKoiVarietyById(koiVarietyId);
        }

        public async Task<KoiVariety> UpdateKoiVarietyAsync(KoiVariety koiVariety)
        {
            return await _koiVarietyRepo.UpdateKoiVariety(koiVariety);
        }

        public async Task<ResultModel> GetKoiVarietyWithColorsAsync()
        {
            var res = new ResultModel();

            try
            {
                var koiVarieties = await _koiVarietyRepo.GetKoiVarietyWithColors();

                var result = koiVarieties.Select(k => new FishesWithColorsDTO
                {
                    Id = k.KoiVarietyId,
                    VarietyName = k.VarietyName,
                    Colors = k.VarietyColors
                        .Where(vc => vc.Color != null)
                        .Select(vc => new ColorPercentageDto
                        {
                            ColorName = vc.Color.ColorName,
                            ColorCode = vc.Color.ColorCode,
                            Percentage = vc.Percentage ?? 0
                        }).ToList()
                }).ToList();

                if (result.Count == 0)
                {
                    res.IsSuccess = false;
                    res.Message = "Không tìm thấy dữ liệu Koi Variety";
                    res.Data = _mapper.Map<List<KoiVarietyDto>>(result);
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                res.IsSuccess = true;
                res.Message = "Lấy danh sách Koi Variety thành công";
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = result;

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Đã xảy ra lỗi khi lấy danh sách Koi Variety: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> GetKoiVarietyWithColorsByIdAsync(string id)
        {
            var res = new ResultModel();

            try
            {
                var koiVariety = await _koiVarietyRepo.GetKoiVarietyWithColorsById(id);

                if (koiVariety == null)
                {
                    res.IsSuccess = false;
                    res.Message = $"Không tìm thấy Koi Variety có ID {id}";
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var result = new FishesWithColorsDTO
                {
                    Id = koiVariety.KoiVarietyId,
                    VarietyName = koiVariety.VarietyName,
                    Colors = koiVariety.VarietyColors
                        .Where(vc => vc.Color != null)
                        .Select(vc => new ColorPercentageDto
                        {
                            ColorName = vc.Color.ColorName,
                            ColorCode = vc.Color.ColorCode,
                            Percentage = vc.Percentage ?? 0
                        }).ToList()
                };

                if (result.Colors.Count == 0)
                {
                    res.IsSuccess = false;
                    res.Message = $"Koi Variety {id} không có thông tin màu sắc";
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                res.IsSuccess = true;
                res.Message = $"Lấy thông tin Koi Variety {id} thành công";
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = result;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"Đã xảy ra lỗi khi lấy thông tin Koi Variety {id}: {ex.Message}";
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }


        private readonly Dictionary<NguHanh, (List<NguHanh> Compatible, List<NguHanh> Incompatible)> _elementRelationships = new()
    {
        { NguHanh.Kim, (new List<NguHanh> { NguHanh.Kim, NguHanh.Thuy }, new List<NguHanh> { NguHanh.Hoa }) },
        { NguHanh.Moc, (new List<NguHanh> { NguHanh.Moc, NguHanh.Thuy }, new List<NguHanh> { NguHanh.Kim }) },
        { NguHanh.Thuy, (new List<NguHanh> { NguHanh.Thuy, NguHanh.Kim }, new List<NguHanh> { NguHanh.Tho }) },
        { NguHanh.Hoa, (new List<NguHanh> { NguHanh.Hoa, NguHanh.Moc }, new List<NguHanh> { NguHanh.Thuy }) },
        { NguHanh.Tho, (new List<NguHanh> { NguHanh.Tho, NguHanh.Hoa }, new List<NguHanh> { NguHanh.Moc }) }
    };

        private readonly Dictionary<string, NguHanh> _elementMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Kim", NguHanh.Kim },
        { "Moc", NguHanh.Moc },
        { "Thuy", NguHanh.Thuy },
        { "Hoa", NguHanh.Hoa },
        { "Tho", NguHanh.Tho }
    };

        private List<NguHanh> GetElements(NguHanh element, bool getCompatible)
            {
                if (_elementRelationships.TryGetValue(element, out var relationship))
                {
                    return getCompatible ? relationship.Compatible : relationship.Incompatible;
                }
                return new List<NguHanh>();
            }

        private NguHanh StringToEnum(string element)
            {
                if (_elementMapping.TryGetValue(element, out var result))
                {
                    return result;
                }
                throw new ArgumentException($"Invalid element: {element}");
            }

        private string EnumToString(NguHanh element)
            {
                return _elementMapping.FirstOrDefault(x => x.Value == element).Key ??
                       throw new ArgumentException($"Invalid element: {element}");
            }

        private List<string> GetElementsAsStrings(string element, bool getCompatible)
            {
                var nguHanh = StringToEnum(element);
                var elementEnums = GetElements(nguHanh, getCompatible);
                return elementEnums.Select(e => EnumToString(e)).ToList();
            }

        private decimal CalculateCompatibilityScore(KoiVariety variety, string personElementString)
        {
            var personElement = StringToEnum(personElementString);
            decimal score = 0;
            decimal totalPercentage = 0;

            var compatibleElements = GetElements(personElement, true);
            var incompatibleElements = GetElements(personElement, false);

            var uniqueVarietyColors = variety.VarietyColors
                .GroupBy(vc => vc.Color.Element)
                .Select(g => g.First())
                .ToList();

            foreach (var colorVariety in uniqueVarietyColors)
            {
                var percentage = colorVariety.Percentage ?? 0;
                if (percentage <= 0) continue;

                totalPercentage += percentage;

                try
                {
                    var colorElement = StringToEnum(colorVariety.Color.Element);

                    if (compatibleElements.Contains(colorElement))
                    {
                        score += percentage;
                    }
                    else if (incompatibleElements.Contains(colorElement))
                    {
                        score += percentage * (-0.5M);
                    }
                    else
                    {
                        score += percentage * 0.2M;
                    }
                }
                catch (ArgumentException)
                {
                    score += percentage * 0.2M;
                }
            }

            // Normalize score based on total percentage
            return totalPercentage > 0 ? score / totalPercentage : 0;
        }


        public async Task<ResultModel> GetKoiVarietiesByElementAsync(string element)
            {
            var res = new ResultModel();
            try
            {
                
                var compatibleElements = GetElements(StringToEnum(element), true);

                // Lấy tất cả các loại Koi từ các mệnh tương hợp
                var elementStrings = compatibleElements.Select(EnumToString).ToList();
                var allKoi = new List<KoiVariety>();

                foreach (var elementString in elementStrings)
                {
                    var koi = await _koiVarietyRepo.GetKoiVarietiesByElement(elementString);
                    if (koi != null && koi.Any())
                    {
                        allKoi.AddRange(koi);
                    }
                }

                // Loại bỏ các loại Koi trùng lặp
                allKoi = allKoi.DistinctBy(k => k.KoiVarietyId).ToList();

                if (!allKoi.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy Koi Variety phù hợp với mệnh của bạn";
                    return res;
                }

                // Sắp xếp theo điểm tương hợp và tính tổng phần trăm hợp
                var recommendedKoi = allKoi
                    .Select(k => new
                    {
                        Koi = k,
                        CompatibilityScore = CalculateCompatibilityScore(k, element),
                        TotalPercentage = k.VarietyColors.Sum(vc => vc.Percentage ?? 0) // Tính tổng %
                    })
                    .OrderByDescending(k => k.CompatibilityScore)
                    .ToList();

                if (!recommendedKoi.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy Koi Variety phù hợp với mệnh của bạn";
                    return res;
                }

                // Nếu tất cả các Koi đều có độ tương hợp < 0.5, chỉ hiển thị tên và tổng phần trăm
                var lowCompatibilityKoi = recommendedKoi
                    .Where(k => k.CompatibilityScore < 0.5m)
                    .Select(k => new
                    {
                        VarietyName = k.Koi.VarietyName,
                        CompatibilityScore = k.CompatibilityScore
                    }).ToList();

                if (lowCompatibilityKoi.Any() && recommendedKoi.All(k => k.CompatibilityScore < 0.5m))
                {
                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = lowCompatibilityKoi;
                    res.Message = "Chỉ tìm thấy Koi Variety có độ tương hợp thấp";
                    return res;
                }

                // Trả về danh sách đầy đủ cho Koi có độ tương hợp >= 0.5
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = recommendedKoi.Select(k => new KoiVarietyResponse
                {
                    VarietyName = k.Koi.VarietyName,
                    Description = k.Koi.Description,
                    VarietyColors = _mapper.Map<List<VarietyColorResponse>>(k.Koi.VarietyColors),
                    TotalPercentage = k.TotalPercentage
                }).ToList();

                res.Message = "Lấy danh sách Koi Variety phù hợp với mệnh thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi lấy danh sách Koi Variety phù hợp với mệnh: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> GetRecommendedKoiVarietiesAsync()
        {
            var res = new ResultModel();
            try
            {
                // Xác thực người dùng và lấy accountId từ token
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
                var accountId = await _accountRepo.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.Message = "Token không hợp lệ hoặc đã hết hạn";
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                // Lấy thông tin mệnh của người dùng từ tài khoản
                var customer = await _customerRepo.GetCustomerByAccountId(accountId);
                if (customer == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = "Không tìm thấy thông tin người dùng";
                    return res;
                }

                if (string.IsNullOrEmpty(customer.Element))
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = "Người dùng chưa cập nhật thông tin mệnh";
                    return res;
                }

                string personElementString = customer.Element;
                NguHanh personElement;
                try
                {
                    personElement = StringToEnum(personElementString);
                }
                catch (ArgumentException ex)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ex.Message;
                    return res;
                }

                var compatibleElements = GetElements(personElement, true);

                // Lấy tất cả các loại Koi từ các mệnh tương hợp
                var elementStrings = compatibleElements.Select(EnumToString).ToList();
                var allKoi = new List<KoiVariety>();

                foreach (var elementString in elementStrings)
                {
                    var koi = await _koiVarietyRepo.GetKoiVarietiesByElement(elementString);
                    if (koi != null && koi.Any())
                    {
                        allKoi.AddRange(koi);
                    }
                }

                // Loại bỏ các loại Koi trùng lặp
                allKoi = allKoi.DistinctBy(k => k.KoiVarietyId).ToList();

                if (!allKoi.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy Koi Variety phù hợp với mệnh của bạn";
                    return res;
                }

                // Sắp xếp theo điểm tương hợp và tính tổng phần trăm hợp
                var recommendedKoi = allKoi
                    .Select(k => new
                    {
                        Koi = k,
                        CompatibilityScore = CalculateCompatibilityScore(k, EnumToString(personElement)),
                        TotalPercentage = k.VarietyColors.Sum(vc => vc.Percentage ?? 0) // Tính tổng %
                    })
                    .OrderByDescending(k => k.CompatibilityScore)
                    .ToList();

                if (!recommendedKoi.Any())
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = null;
                    res.Message = "Không tìm thấy Koi Variety phù hợp với mệnh của bạn";
                    return res;
                }

                // Nếu tất cả các Koi đều có độ tương hợp < 0.5, chỉ hiển thị tên và tổng phần trăm
                var lowCompatibilityKoi = recommendedKoi
                    .Where(k => k.CompatibilityScore < 0.5m)
                    .Select(k => new
                    {
                        VarietyName = k.Koi.VarietyName,
                        CompatibilityScore = k.CompatibilityScore
                    }).ToList();

                if (lowCompatibilityKoi.Any() && recommendedKoi.All(k => k.CompatibilityScore < 0.5m))
                {
                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = lowCompatibilityKoi;
                    res.Message = "Chỉ tìm thấy Koi Variety có độ tương hợp thấp";
                    return res;
                }

                // Trả về danh sách đầy đủ cho Koi có độ tương hợp >= 0.5
                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = recommendedKoi.Select(k => new KoiVarietyResponse
                {
                    VarietyName = k.Koi.VarietyName,
                    Description = k.Koi.Description,
                    VarietyColors = _mapper.Map<List<VarietyColorResponse>>(k.Koi.VarietyColors),
                    TotalPercentage = k.TotalPercentage
                }).ToList();

                res.Message = "Lấy danh sách Koi Variety phù hợp với mệnh thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi lấy danh sách Koi Variety phù hợp với mệnh: {ex.Message}";
                return res;
            }
        }
    }
}
