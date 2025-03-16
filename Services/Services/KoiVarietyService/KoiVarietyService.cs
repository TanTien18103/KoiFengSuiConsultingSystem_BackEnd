using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Exceptions;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.AccountRepository;
using Repositories.Repositories.ColorRepository;
using Repositories.Repositories.CustomerRepository;
using Repositories.Repositories.KoiVarietyRepository;
using Repositories.Repositories.VarietyColorRepository;
using Services.ApiModels;
using Services.ApiModels.Color;
using Services.ApiModels.KoiVariety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.Services.KoiVarietyService
{
    public class KoiVarietyService : IKoiVarietyService
    {
        private readonly IKoiVarietyRepo _koiVarietyRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepo _accountRepo;
        private readonly IVarietyColorRepo _varietyColorRepo;
        private readonly IColorRepo _colorRepo;

        public KoiVarietyService(IKoiVarietyRepo koiVarietyRepo, ICustomerRepo customerRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAccountRepo accountRepo, IVarietyColorRepo varietyColorRepo, IColorRepo colorRepo)
        {
            _koiVarietyRepo = koiVarietyRepo;
            _customerRepo = customerRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _accountRepo = accountRepo;
            _varietyColorRepo = varietyColorRepo;
            _colorRepo = colorRepo;
        }


        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }


        public async Task<List<KoiVariety>> GetKoiVarietiesAsync()
        {
            return await _koiVarietyRepo.GetKoiVarieties();
        }

        public async Task<KoiVariety> GetKoiVarietyByIdAsync(string koiVarietyId)
        {
            return await _koiVarietyRepo.GetKoiVarietyById(koiVarietyId);
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
                    Description = k.Description,
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
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_FOUND;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<KoiVarietyDto>>(result);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
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
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_NOT_FOUND;
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
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_COLOR_INFO_NOT_FOUND + $"của Koi Variety {id}";
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_INFO_FOUND + $"của Koi Variety {id}";
                res.Data = result;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
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
            throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsCompatibility.DESTINY_INVALID + ": " + element, StatusCodes.Status400BadRequest);
        }

        private string EnumToString(NguHanh element)
        {
            return _elementMapping.FirstOrDefault(x => x.Value == element).Key ??
                   throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsCompatibility.DESTINY_INVALID + ": " + element, StatusCodes.Status400BadRequest);
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
                        score += percentage * -0.5M;
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
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.NO_MATCHES_KOIVARIETY;
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
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.NO_MATCHES_KOIVARIETY;
                    return res;
                }

                // Nếu tất cả các Koi đều có độ tương hợp < 0.5, chỉ hiển thị tên và tổng phần trăm
                var lowCompatibilityKoi = recommendedKoi
                    .Where(k => k.CompatibilityScore < 0.5m)
                    .Select(k => new
                    {
                        k.Koi.VarietyName,
                        k.CompatibilityScore
                    }).ToList();

                if (lowCompatibilityKoi.Any() && recommendedKoi.All(k => k.CompatibilityScore < 0.5m))
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = lowCompatibilityKoi;
                    res.Message = ResponseMessageConstrantsKoiVariety.LOW_MATCHES_KOIVARIETY;
                    return res;
                }

                // Trả về danh sách đầy đủ cho Koi có độ tương hợp >= 0.5
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = recommendedKoi.Select(k => new KoiVarietyResponse
                {
                    VarietyName = k.Koi.VarietyName,
                    Description = k.Koi.Description,
                    VarietyColors = _mapper.Map<List<VarietyColorResponse>>(k.Koi.VarietyColors),
                    TotalPercentage = k.TotalPercentage
                }).ToList();

                res.Message = ResponseMessageConstrantsKoiVariety.GET_MATCHES_KOIVARIETY;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
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
                var accountId = await _accountRepo.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.UNAUTHORIZED;
                    res.Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED;
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    return res;
                }

                // Lấy thông tin mệnh của người dùng từ tài khoản
                var customer = await _customerRepo.GetCustomerByAccountId(accountId);
                if (customer == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstantsUser.USER_NOT_FOUND;
                    return res;
                }

                if (string.IsNullOrEmpty(customer.Element))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstantsUser.NOT_UPDATED_ELEMENT;
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
                    res.ResponseCode = ResponseCodeConstants.FAILED;
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
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.NO_MATCHES_KOIVARIETY;
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
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.NO_MATCHES_KOIVARIETY;
                    return res;
                }

                // Nếu tất cả các Koi đều có độ tương hợp < 0.5, chỉ hiển thị tên và tổng phần trăm
                var lowCompatibilityKoi = recommendedKoi
                    .Where(k => k.CompatibilityScore < 0.5m)
                    .Select(k => new
                    {
                        k.Koi.VarietyName,
                        k.CompatibilityScore
                    }).ToList();

                if (lowCompatibilityKoi.Any() && recommendedKoi.All(k => k.CompatibilityScore < 0.5m))
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = lowCompatibilityKoi;
                    res.Message = ResponseMessageConstrantsKoiVariety.LOW_MATCHES_KOIVARIETY;
                    return res;
                }

                // Trả về danh sách đầy đủ cho Koi có độ tương hợp >= 0.5
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = recommendedKoi.Select(k => new KoiVarietyResponse
                {
                    VarietyName = k.Koi.VarietyName,
                    Description = k.Koi.Description,
                    VarietyColors = _mapper.Map<List<VarietyColorResponse>>(k.Koi.VarietyColors),
                    TotalPercentage = k.TotalPercentage
                }).ToList();
                res.Message = ResponseMessageConstrantsKoiVariety.GET_MATCHES_KOIVARIETY;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi lấy danh sách Koi Variety phù hợp với mệnh: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> CreateKoiVarietyAsync(KoiVarietyRequest koiVariety)
        {
            var res = new ResultModel();
            try
            {
                var newKoiVariety = new KoiVariety
                {
                    KoiVarietyId = GenerateShortGuid(),
                    VarietyName = koiVariety.VarietyName,
                    Description = koiVariety.Description
                };

                foreach (var varietyColor in koiVariety.VarietyColors)
                {
                    newKoiVariety.VarietyColors.Add(new VarietyColor
                    {
                        KoiVarietyId = newKoiVariety.KoiVarietyId,
                        ColorId = varietyColor.ColorId,
                        Percentage = varietyColor.Percentage
                    });
                }

                var createdKoiVariety = await _koiVarietyRepo.CreateKoiVariety(newKoiVariety);

                if (createdKoiVariety == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantsKoiVariety.CREATE_KOIVARIETY_FAILED;
                    return res;
                }

                var KoiVarietyresponse = await _koiVarietyRepo.GetKoiVarietyById(createdKoiVariety.KoiVarietyId);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Message = ResponseMessageConstrantsKoiVariety.CREATE_KOIVARIETY_SUCCESS;
                res.Data = _mapper.Map<KoiVarietyResponse>(KoiVarietyresponse);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi tạo Koi Variety: {ex.Message}";
                return res;
            }
        }


        public async Task<ResultModel> UpdateKoiVarietyAsync(string id, KoiVarietyRequest koiVariety)
        {
            var res = new ResultModel();
            try
            {
                var existingKoiVariety = await _koiVarietyRepo.GetKoiVarietyById(id);
                if (existingKoiVariety == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_NOT_FOUND;
                    return res;
                }

                existingKoiVariety.VarietyName = koiVariety.VarietyName;
                existingKoiVariety.Description = koiVariety.Description;

                existingKoiVariety.VarietyColors.Clear();

                foreach (var varietyColor in koiVariety.VarietyColors)
                {
                    existingKoiVariety.VarietyColors.Add(new VarietyColor
                    {
                        KoiVarietyId = existingKoiVariety.KoiVarietyId,
                        ColorId = varietyColor.ColorId,
                        Percentage = varietyColor.Percentage
                    });
                }

                var updatedKoiVariety = await _koiVarietyRepo.UpdateKoiVariety(existingKoiVariety);
                var KoiVarietyresponse = await _koiVarietyRepo.GetKoiVarietyById(updatedKoiVariety.KoiVarietyId);

                if (updatedKoiVariety == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantsKoiVariety.UPDATE_KOIVARIETY_FAILED;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiVariety.UPDATE_KOIVARIETY_SUCCESS;
                res.Data = _mapper.Map<KoiVarietyResponse>(KoiVarietyresponse);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi cập nhật Koi Variety: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> DeleteKoiVarietyAsync(string id)
        {
            var res = new ResultModel();
            try
            {
                var existingKoiVariety = await _koiVarietyRepo.GetKoiVarietyById(id);
                if (existingKoiVariety == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_NOT_FOUND;
                    return res;
                }

                await _koiVarietyRepo.DeleteKoiVariety(id);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiVariety.DELETE_KOIVARIETY_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi xóa Koi Variety: {ex.Message}";
                return res;
            }
        }
    }
}
