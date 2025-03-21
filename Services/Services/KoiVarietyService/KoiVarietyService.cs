using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Exceptions;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.UriParser;
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
            { NguHanh.Kim, (new List<NguHanh> { NguHanh.Kim, NguHanh.Thủy, NguHanh.Thổ }, new List<NguHanh> { NguHanh.Mộc, NguHanh.Hỏa }) },
            { NguHanh.Mộc, (new List<NguHanh> { NguHanh.Mộc, NguHanh.Hỏa, NguHanh.Thủy }, new List<NguHanh> { NguHanh.Kim, NguHanh.Thổ }) },
            { NguHanh.Thủy, (new List<NguHanh> { NguHanh.Thủy, NguHanh.Mộc, NguHanh.Kim }, new List<NguHanh> { NguHanh.Thổ, NguHanh.Hỏa }) },
            { NguHanh.Hỏa, (new List<NguHanh> { NguHanh.Hỏa, NguHanh.Mộc, NguHanh.Thổ }, new List<NguHanh> { NguHanh.Thủy, NguHanh.Kim }) },
            { NguHanh.Thổ, (new List<NguHanh> { NguHanh.Thổ, NguHanh.Kim, NguHanh.Hỏa }, new List<NguHanh> { NguHanh.Mộc, NguHanh.Thủy }) }
        };

        private readonly Dictionary<NguHanh, (List<ColorEnums> Compatible, List<ColorEnums> Supportive, List<ColorEnums> Controlling, List<ColorEnums> Restricted)> _colorRelationships = new()
        {
            {
                NguHanh.Kim,
                (
                    new List<ColorEnums> { ColorEnums.Trắng, ColorEnums.Xám, ColorEnums.Ghi }, // Màu hợp
                    new List<ColorEnums> { ColorEnums.Vàng, ColorEnums.Nâu }, // Màu tương sinh (Thổ sinh Kim)
                    new List<ColorEnums> { ColorEnums.XanhLá }, // Màu khắc chế (Kim khắc Mộc)
                    new List<ColorEnums> { ColorEnums.Đỏ, ColorEnums.Hồng, ColorEnums.Cam, ColorEnums.Tím } // Màu bị khắc (Hỏa khắc Kim)
                )
            },
            {
                NguHanh.Mộc,
                (
                    new List<ColorEnums> { ColorEnums.XanhLá }, // Màu hợp
                    new List<ColorEnums> { ColorEnums.Đen, ColorEnums.XanhDương }, // Màu tương sinh (Thủy sinh Mộc)
                    new List<ColorEnums> { ColorEnums.Vàng, ColorEnums.Nâu }, // Màu khắc chế (Mộc khắc Thổ)
                    new List<ColorEnums> { ColorEnums.Trắng, ColorEnums.Xám, ColorEnums.Ghi } // Màu bị khắc (Kim khắc Mộc)
                )
            },
            {
                NguHanh.Thủy,
                (
                    new List<ColorEnums> { ColorEnums.Đen, ColorEnums.XanhDương }, // Màu hợp
                    new List<ColorEnums> { ColorEnums.Trắng, ColorEnums.Xám, ColorEnums.Ghi }, // Màu tương sinh (Kim sinh Thủy)
                    new List<ColorEnums> { ColorEnums.Đỏ, ColorEnums.Hồng, ColorEnums.Cam, ColorEnums.Tím }, // Màu khắc chế (Thủy khắc Hỏa)
                    new List<ColorEnums> { ColorEnums.Vàng, ColorEnums.Nâu } // Màu bị khắc (Thổ khắc Thủy)
                )
            },
            {
                NguHanh.Hỏa,
                (
                    new List<ColorEnums> { ColorEnums.Đỏ, ColorEnums.Hồng, ColorEnums.Cam, ColorEnums.Tím }, // Màu hợp
                    new List<ColorEnums> { ColorEnums.XanhLá }, // Màu tương sinh (Mộc sinh Hỏa)
                    new List<ColorEnums> { ColorEnums.Trắng, ColorEnums.Xám, ColorEnums.Ghi }, // Màu khắc chế (Hỏa khắc Kim)
                    new List<ColorEnums> { ColorEnums.Đen, ColorEnums.XanhDương } // Màu bị khắc (Thủy khắc Hỏa)
                )
            },
            {
                NguHanh.Thổ,
                (
                    new List<ColorEnums> { ColorEnums.Vàng, ColorEnums.Nâu }, // Màu hợp
                    new List<ColorEnums> { ColorEnums.Đỏ, ColorEnums.Hồng, ColorEnums.Cam, ColorEnums.Tím }, // Màu tương sinh (Hỏa sinh Thổ)
                    new List<ColorEnums> { ColorEnums.Đen, ColorEnums.XanhDương }, // Màu khắc chế (Thổ khắc Thủy)
                    new List<ColorEnums> { ColorEnums.XanhLá } // Màu bị khắc (Mộc khắc Thổ)
                )
            }
        };


        private readonly Dictionary<string, NguHanh> _elementMapping = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Kim", NguHanh.Kim },
            { "Mộc", NguHanh.Mộc },
            { "Thủy", NguHanh.Thủy },
            { "Hỏa", NguHanh.Hỏa },
            { "Thổ", NguHanh.Thổ }
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

        public async Task<ResultModel> GetKoiVarietiesByElementAsync(NguHanh element)
        {
            var res = new ResultModel();
            try
            {

                var compatibleElements = GetElements(element, true);

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
                        CompatibilityScore = CalculateCompatibilityScore(k, EnumToString(element)),
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
                    res.Data = recommendedKoi.Select(k => new KoiVarietyResponse
                    {
                        VarietyName = k.Koi.VarietyName,
                        Description = k.Koi.Description,
                        VarietyColors = _mapper.Map<List<VarietyColorResponse>>(k.Koi.VarietyColors),
                        TotalPercentage = k.TotalPercentage,
                        CompatibilityScore = k.CompatibilityScore
                    }).ToList();

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

        public async Task<ResultModel> GetKoiVarietiesByName(string name)
        {
            var res = new ResultModel();
            try
            {
                var kois = await _koiVarietyRepo.GetKoiVarietiesByName(name);
                if(kois == null || !kois.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_FOUND;
                res.Data = _mapper.Map<List<KoiVarietyDto>>(kois);
                return res;
            }
            catch(Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetKoiVarietiesByColorsAsync(List<string> colorIds)
        {
            var res = new ResultModel();
            try
            {
                var koiVarieties = await _koiVarietyRepo.GetKoiVarietiesByColors(colorIds);

                if (koiVarieties == null || !koiVarieties.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<KoiVarietyDto>>(koiVarieties);
                res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_FOUND;

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        public List<NguHanh> GetCompatibleElementsForColor(ColorEnums color)
        {
            List<NguHanh> compatibleElements = new List<NguHanh>();

            foreach (var elementRelationship in _colorRelationships)
            {
                var nguHanh = elementRelationship.Key;
                var relationships = elementRelationship.Value;

                if (relationships.Compatible.Contains(color) || relationships.Supportive.Contains(color))
                {
                    compatibleElements.Add(nguHanh);
                }
            }

            return compatibleElements;
        }

        //public (bool IsCompatible, NguHanh? Element, string Message) CheckColorsCompatibility(List<ColorEnums> colors)
        //{
        //    if (colors == null || colors.Count == 0)
        //    {
        //        return (false, null, ResponseMessageConstrantsKoiVariety.COLOR_INPUT_REQUIRED);
        //    }

        //    if (colors.Count == 1)
        //    {
        //        var element = GetCompatibleElementForColor(colors[0]);
        //        return (element != null, element, element != null
        //            ? $"Màu {colors[0]} phù hợp với mệnh {element}"
        //            : $"Không tìm thấy mệnh phù hợp với màu {colors[0]}");
        //    }

        //    var colorElements = new Dictionary<ColorEnums, NguHanh?>();
        //    foreach (var color in colors)
        //    {
        //        colorElements[color] = GetCompatibleElementForColor(color);
        //    }

        //    // Lọc ra các màu có mệnh
        //    var colorsWithElements = colorElements.Where(pair => pair.Value != null)
        //                                        .ToDictionary(pair => pair.Key, pair => pair.Value);

        //    if (colorsWithElements.Count == 0)
        //    {
        //        return (false, null, ResponseMessageConstrantsKoiVariety.ELEMENT_COMPATIBLE_NOT_FOUND);
        //    }

        //    // Kiểm tra xem tất cả các màu có cùng một mệnh không
        //    var firstElement = colorsWithElements.First().Value;
        //    var allSameElement = colorsWithElements.All(pair => pair.Value.Equals(firstElement));

        //    if (allSameElement)
        //    {
        //        return (true, firstElement, $"Tất cả các màu đã chọn đều phù hợp với mệnh {firstElement}");
        //    }

        //    // Nếu không phải tất cả cùng một mệnh, tìm các màu xung khắc
        //    var conflictingPairs = new List<(ColorEnums, ColorEnums)>();

        //    for (int i = 0; i < colors.Count - 1; i++)
        //    {
        //        for (int j = i + 1; j < colors.Count; j++)
        //        {
        //            var color1 = colors[i];
        //            var color2 = colors[j];

        //            var element1 = colorElements[color1];
        //            var element2 = colorElements[color2];

        //            if (element1 != null && element2 != null && !element1.Equals(element2))
        //            {
        //                conflictingPairs.Add((color1, color2));
        //            }
        //        }
        //    }

        //    if (conflictingPairs.Count > 0)
        //    {
        //        var conflictMessage = "Các màu sau không hợp với nhau: ";
        //        conflictMessage += string.Join(", ", conflictingPairs.Select(pair => $"{pair.Item1} và {pair.Item2}"));
        //        conflictMessage += ". Hãy chọn màu khác.";

        //        return (false, null, conflictMessage);
        //    }

        //    // Nếu có một số màu không tìm thấy mệnh
        //    var unknownColors = colorElements.Where(pair => pair.Value == null)
        //                                     .Select(pair => pair.Key)
        //                                     .ToList();

        //    if (unknownColors.Count > 0)
        //    {
        //        var commonElement = colorsWithElements.First().Value;
        //        var unknownMessage = $"Không tìm thấy mệnh cho các màu: {string.Join(", ", unknownColors)}. ";
        //        unknownMessage += $"Các màu còn lại phù hợp với mệnh {commonElement}.";

        //        return (false, commonElement, unknownMessage);
        //    }

        //    // Trường hợp mặc định không nên xảy ra
        //    return (false, null, ResponseMessageConstrantsKoiVariety.INVALID_ELEMENT_FOR_COLORS);
        //}

        public List<ColorEnums> GetPositiveColorsByElement(NguHanh nguHanh)
        {
            if (!_colorRelationships.ContainsKey(nguHanh))
            {
                return new List<ColorEnums>();
            }

            var relationships = _colorRelationships[nguHanh];

            // Chỉ lấy các màu tích cực: Compatible và Supportive
            var positiveColors = new List<ColorEnums>();
            positiveColors.AddRange(relationships.Compatible);
            positiveColors.AddRange(relationships.Supportive);

            return positiveColors;
        }

        public async Task<ResultModel> FilterByColorAndElement(NguHanh? nguHanh = null, List<string>? colorIds = null)
        {
            var res = new ResultModel();
            try 
            {
                if (nguHanh.HasValue)
                {
                    var koisByElement = await GetKoiVarietiesByElementAsync(nguHanh.Value);
                    if (koisByElement == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status500InternalServerError;
                        res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_NOT_FOUND;
                        return res;
                    }
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = koisByElement.Data;
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_FOUND;
                    return res;
                }
                if (colorIds != null && colorIds.Count > 0)
                {
                    var koisByColor = await GetKoiVarietiesByColorsAsync(colorIds);
                    if (koisByColor == null)
                    {
                        res.IsSuccess = false;
                        res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                        res.StatusCode = StatusCodes.Status500InternalServerError;
                        res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_NOT_FOUND;
                        return res;
                    }
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = koisByColor.Data;
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_FOUND;
                    return res;
                }
                var allKois = await _koiVarietyRepo.GetKoiVarieties();
                if (allKois == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<KoiVarietyDto>>(allKois);
                res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
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
                    res.Data = recommendedKoi.Select(k => new KoiVarietyResponse
                    {
                        VarietyName = k.Koi.VarietyName,
                        Description = k.Koi.Description,
                        VarietyColors = _mapper.Map<List<VarietyColorResponse>>(k.Koi.VarietyColors),
                        TotalPercentage = k.TotalPercentage,
                        CompatibilityScore = k.CompatibilityScore
                    }).ToList();

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
                    TotalPercentage = k.TotalPercentage,
                    CompatibilityScore = k.CompatibilityScore
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

        public async Task<ResultModel> GetColorById(string id)
        {
            var res = new ResultModel();
            try
            {
                var color = await _colorRepo.GetColorById(id);
                if (color == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.COLOR_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiVariety.COLOR_FOUND;
                res.Data = _mapper.Map<ColorResponse>(color);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi lấy thông tin màu: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> GetColors()
        {
            var res = new ResultModel();
            try
            {
                var colors = await _colorRepo.GetColors();
                if (colors == null || !colors.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.COLOR_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiVariety.COLOR_FOUND;
                res.Data = _mapper.Map<List<ColorResponse>>(colors);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi lấy danh sách màu: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> CreateColors(ColorRequest color)
        {
            var res = new ResultModel();
            try
            {
                var newColor = new Color
                {
                    ColorId = GenerateShortGuid(),
                    ColorName = color.ColorName,
                    ColorCode = color.ColorCode,
                    Element = color.Element
                };

                var createdColor = await _colorRepo.CreateColor(newColor);

                if (createdColor == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantsKoiVariety.CREATE_COLOR_FAILED;
                    return res;
                }

                var colorResponse = await _colorRepo.GetColorById(createdColor.ColorId);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status201Created;
                res.Message = ResponseMessageConstrantsKoiVariety.CREATE_COLOR_SUCCESS;
                res.Data = _mapper.Map<ColorResponse>(colorResponse);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi tạo màu: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> UpdateColors(string id, ColorRequest color)
        {
            var res = new ResultModel();
            try
            {
                var existingColor = await _colorRepo.GetColorById(id);
                if (existingColor == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.COLOR_NOT_FOUND;
                    return res;
                }

                existingColor.ColorName = color.ColorName;
                existingColor.ColorCode = color.ColorCode;
                existingColor.Element = color.Element;

                var updatedColor = await _colorRepo.UpdateColor(existingColor);
                var colorResponse = await _colorRepo.GetColorById(updatedColor.ColorId);

                if (updatedColor == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.Message = ResponseMessageConstrantsKoiVariety.UPDATE_COLOR_FAILED;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiVariety.UPDATE_COLOR_SUCCESS;
                res.Data = _mapper.Map<ColorResponse>(colorResponse);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi cập nhật màu: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> DeleteColors(string id)
        {
            var res = new ResultModel();
            try
            {
                var existingColor = await _colorRepo.GetColorById(id);
                if (existingColor == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.COLOR_NOT_FOUND;
                    return res;
                }

                await _colorRepo.DeleteColor(id);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Message = ResponseMessageConstrantsKoiVariety.DELETE_COLOR_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = $"Đã xảy ra lỗi khi xóa màu: {ex.Message}";
                return res;
            }
        }
    }
}
