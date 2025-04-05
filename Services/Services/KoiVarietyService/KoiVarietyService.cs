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
using Services.ServicesHelpers.FengShuiHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;
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


        public async Task<ResultModel> GetKoiVarietiesAsync()
        {
            var res = new ResultModel();
            try
            {
                var kois = await _koiVarietyRepo.GetKoiVarieties();
                if (kois == null || !kois.Any())
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
                res.Data = _mapper.Map<List<KoiVarietyResponse>>(kois);
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

        public async Task<ResultModel> GetKoiVarietyByIdAsync(string koiVarietyId)
        {
            var res = new ResultModel();
            try
            {
                var koi = await _koiVarietyRepo.GetKoiVarietyById(koiVarietyId);
                if (koi == null)
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
                res.Data = _mapper.Map<KoiVarietyDto>(koi);
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
                    Description = koiVariety.Description,
                    ImageUrl = koiVariety.ImageUrl,
                    Introduction = koiVariety.Introduction,
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
            decimal score = 0;
            decimal totalPercentage = 0;

            if (!FengShuiHelper.ElementColorPoints.ContainsKey(personElementString))
                return 0; 

            var elementColorScores = FengShuiHelper.ElementColorPoints[personElementString];

            var uniqueVarietyColors = variety.VarietyColors
                .GroupBy(vc => vc.Color.Element)
                .Select(g => g.First())
                .ToList();

            foreach (var colorVariety in uniqueVarietyColors)
            {
                var percentage = colorVariety.Percentage ?? 0;
                if (percentage <= 0) continue;

                totalPercentage += percentage;

                var color = colorVariety.Color.Element;
                var colorScore = elementColorScores.ContainsKey(color) ? elementColorScores[color] : 0;
                    
                score += (percentage * (decimal)colorScore / 10); 
            }

            // Chuẩn hóa điểm số
            return totalPercentage > 0 ? (score / totalPercentage) * 100 : 0;
        }


        public async Task<ResultModel> GetKoiVarietiesByElementAsync(NguHanh element)
        {
            var res = new ResultModel();
            try
            {
                var compatibleElements = GetElements(element, true);
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
                allKoi = allKoi.DistinctBy(k => k.KoiVarietyId).ToList();
                if (!allKoi.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsKoiVariety.NO_MATCHES_KOIVARIETY;
                    return res;
                }
                var recommendedKoi = allKoi
                    .Select(k => new
                    {
                        Koi = k,
                        CompatibilityScore = CalculateCompatibilityScore(k, EnumToString(element)),
                        TotalPercentage = k.VarietyColors.Sum(vc => vc.Percentage ?? 0)
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
                var lowCompatibilityKoi = recommendedKoi
                    .Where(k => k.CompatibilityScore < 0.5m)
                    .Select(k => new
                    {
                        k.Koi.VarietyName,
                        k.CompatibilityScore
                    }).ToList();

                // Lấy danh sách các đối tượng KoiVariety để chuyển đổi
                var koiList = recommendedKoi.Select(k => k.Koi).ToList();

                if (lowCompatibilityKoi.Any() && recommendedKoi.All(k => k.CompatibilityScore < 0.5m))
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<List<KoiVarietyDto>>(koiList);
                    res.Message = ResponseMessageConstrantsKoiVariety.LOW_MATCHES_KOIVARIETY;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<KoiVarietyDto>>(koiList);
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
                List<KoiVariety> kois;

                if (string.IsNullOrWhiteSpace(name))
                {
                    kois = await _koiVarietyRepo.GetKoiVarieties();
                }
                else
                {
                    kois = await _koiVarietyRepo.GetKoiVarietiesByName(name);
                }

                if (kois == null || !kois.Any())
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

        public async Task<ResultModel> GetKoiVarietiesByColorsAsync(List<ColorEnums> colors)
        {
            var res = new ResultModel();
            try
            {
                var koiVarieties = await _koiVarietyRepo.GetKoiVarietiesByColors(colors);

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

        public (bool IsCompatible, List<NguHanh> Elements, string Message) GetCompatibleElementsForColors(List<ColorEnums> colors)
        {
            if (colors == null || colors.Count == 0)
            {
                return (false, new List<NguHanh>(), ResponseMessageConstrantsKoiVariety.COLOR_INPUT_REQUIRED);
            }

            // Xử lý trường hợp chỉ có một màu
            if (colors.Count == 1)
            {
                List<NguHanh> compatibleElements = new List<NguHanh>();
                foreach (var elementRelationship in _colorRelationships)
                {
                    var nguHanh = elementRelationship.Key;
                    var relationships = elementRelationship.Value;
                    if (relationships.Compatible.Contains(colors[0]) || relationships.Supportive.Contains(colors[0]))
                    {
                        compatibleElements.Add(nguHanh);
                    }
                }

                if (compatibleElements.Count == 0)
                {
                    return (false, new List<NguHanh>(), $"Không tìm thấy mệnh phù hợp với màu {colors[0]}");
                }

                return (true, compatibleElements, $"Màu {colors[0]} phù hợp với mệnh {compatibleElements.First()}");
            }

            // Xử lý trường hợp nhiều màu
            // 1. Tìm các mệnh tương thích cho từng màu
            Dictionary<ColorEnums, List<NguHanh>> colorToElementsMap = new Dictionary<ColorEnums, List<NguHanh>>();

            foreach (var color in colors)
            {
                List<NguHanh> elementsForColor = new List<NguHanh>();
                foreach (var elementRelationship in _colorRelationships)
                {
                    var nguHanh = elementRelationship.Key;
                    var relationships = elementRelationship.Value;
                    if (relationships.Compatible.Contains(color) || relationships.Supportive.Contains(color))
                    {
                        elementsForColor.Add(nguHanh);
                    }
                }
                colorToElementsMap[color] = elementsForColor;
            }

            // 2. Lọc ra các màu có mệnh tương thích
            var colorsWithElements = colorToElementsMap
                .Where(pair => pair.Value.Count > 0)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            if (colorsWithElements.Count == 0)
            {
                return (false, new List<NguHanh>(), ResponseMessageConstrantsKoiVariety.ELEMENT_COMPATIBLE_NOT_FOUND);
            }

            // 3. Tìm mệnh chung cho tất cả các màu (nếu có)
            var commonElements = new List<NguHanh>(colorsWithElements.First().Value);
            foreach (var pair in colorsWithElements.Skip(1))
            {
                commonElements = commonElements.Intersect(pair.Value).ToList();
            }

            if (commonElements.Count > 0)
            {
                // Có ít nhất một mệnh chung cho tất cả các màu
                return (true, commonElements, $"Tất cả các màu đã chọn đều phù hợp với mệnh {commonElements.First()}");
            }

            // 4. Kiểm tra xung khắc giữa các màu
            var conflictingPairs = new List<(ColorEnums, ColorEnums)>();

            for (int i = 0; i < colors.Count - 1; i++)
            {
                for (int j = i + 1; j < colors.Count; j++)
                {
                    var color1 = colors[i];
                    var color2 = colors[j];

                    if (!colorToElementsMap.ContainsKey(color1) || !colorToElementsMap.ContainsKey(color2) ||
                        colorToElementsMap[color1].Count == 0 || colorToElementsMap[color2].Count == 0)
                    {
                        continue;
                    }

                    // Kiểm tra xem hai màu có mệnh chung không
                    bool hasCommonElement = false;
                    foreach (var element1 in colorToElementsMap[color1])
                    {
                        if (colorToElementsMap[color2].Contains(element1))
                        {
                            hasCommonElement = true;
                            break;
                        }
                    }

                    if (!hasCommonElement)
                    {
                        conflictingPairs.Add((color1, color2));
                    }
                }
            }

            // 5. Tìm mệnh phổ biến nhất từ tất cả các màu
            Dictionary<NguHanh, int> elementFrequency = new Dictionary<NguHanh, int>();
            foreach (var elements in colorsWithElements.Values)
            {
                foreach (var element in elements)
                {
                    if (!elementFrequency.ContainsKey(element))
                    {
                        elementFrequency[element] = 0;
                    }
                    elementFrequency[element]++;
                }
            }

            var bestElements = elementFrequency
                .OrderByDescending(pair => pair.Value)
                .Select(pair => pair.Key)
                .ToList();

            // 6. Tạo thông báo phù hợp
            if (conflictingPairs.Count > 0)
            {
                var conflictMessage = "Các màu sau không hợp với nhau: ";
                conflictMessage += string.Join(", ", conflictingPairs.Select(pair => $"{pair.Item1} và {pair.Item2}"));
                conflictMessage += $". Ngũ hành phù hợp nhất là {bestElements.First()}.";
                return (false, bestElements, conflictMessage);
            }

            // 7. Kiểm tra màu không tìm thấy mệnh
            var unknownColors = colorToElementsMap
                .Where(pair => pair.Value.Count == 0)
                .Select(pair => pair.Key)
                .ToList();

            if (unknownColors.Count > 0)
            {
                var unknownMessage = $"Không tìm thấy mệnh cho các màu: {string.Join(", ", unknownColors)}. ";
                unknownMessage += $"Các màu còn lại phù hợp nhất với mệnh {bestElements.First()}.";
                return (false, bestElements, unknownMessage);
            }

            // 8. Trường hợp mặc định
            return (true, bestElements, $"Các màu đã chọn phù hợp nhất với mệnh {bestElements.First()}");
        }

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

        public async Task<ResultModel> FilterByColorAndElement(NguHanh? nguHanh = null, List<ColorEnums>? colors = null)
        {
            var res = new ResultModel();
            try
            {
                // Nếu không có bất kỳ điều kiện lọc nào hoặc colors là null/rỗng/chứa chuỗi rỗng
                if (!nguHanh.HasValue && (colors == null || !colors.Any() || colors.Contains(default(ColorEnums))))
                {
                    var allKois = await _koiVarietyRepo.GetKoiVarieties();
                    if (allKois == null)
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
                    res.Data = _mapper.Map<List<KoiVarietyDto>>(allKois);
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_FOUND;
                    return res;
                }

                List<KoiVariety> koiList = new List<KoiVariety>();

                // Lọc theo Bản mệnh
                if (nguHanh.HasValue)
                {
                    var koisByElement = await GetKoiVarietiesByElementAsync(nguHanh.Value);
                    if (koisByElement != null && koisByElement.IsSuccess && koisByElement.Data != null)
                    {
                        koiList = _mapper.Map<List<KoiVariety>>(koisByElement.Data);
                    }
                }

                // Chỉ lọc theo màu sắc nếu colors có giá trị và không null
                if (colors != null && colors.Any())
                {
                    if (koiList.Any())
                    {
                        // Lọc từ danh sách đã có
                        koiList = koiList.Where(k => 
                            k.VarietyColors.Any(vc => 
                                colors.Contains(Enum.Parse<ColorEnums>(vc.ColorId))
                            )
                        ).ToList();
                    }
                    else
                    {
                        // Lấy danh sách mới theo màu
                        var koisByColor = await GetKoiVarietiesByColorsAsync(colors);
                        if (koisByColor != null && koisByColor.IsSuccess && koisByColor.Data != null)
                        {
                            koiList = _mapper.Map<List<KoiVariety>>(koisByColor.Data);
                        }
                    }
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<KoiVarietyDto>>(koiList);
                
                if (!koiList.Any())
                {
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_NOT_FOUND;
                }
                else
                {
                    res.Message = ResponseMessageConstrantsKoiVariety.KOIVARIETY_FOUND;
                }
                
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
                var kois = await _koiVarietyRepo.GetKoiVarieties();
                // Loại bỏ các loại Koi trùng lặp
                allKoi = allKoi.DistinctBy(k => k.KoiVarietyId).ToList();

                if (!allKoi.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Data = _mapper.Map<List<KoiVarietyDto>>(kois);
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

                // Nếu tất cả các Koi đều có độ tương hợp < 0.5, nhưng vẫn trả về danh sách koi
                var lowCompatibilityKoi = recommendedKoi
                    .Where(k => k.CompatibilityScore < 60)
                    .Select(k => new
                    {
                        k.Koi.VarietyName,
                        k.CompatibilityScore
                    }).ToList();

                // Lấy danh sách các đối tượng KoiVariety để mapping
                var koiList = recommendedKoi.Select(k => k.Koi).ToList();

                if (lowCompatibilityKoi.Any() && recommendedKoi.All(k => k.CompatibilityScore < 60))
                {
                    res.IsSuccess = true;
                    res.ResponseCode = ResponseCodeConstants.SUCCESS;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = _mapper.Map<List<KoiVarietyDto>>(koiList);
                    res.Message = ResponseMessageConstrantsKoiVariety.LOW_MATCHES_KOIVARIETY;
                    return res;
                }

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<List<KoiVarietyDto>>(koiList);
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
