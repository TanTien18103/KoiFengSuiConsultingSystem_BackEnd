using AutoMapper;
using BusinessObjects.Models;
using DAOs.DTOs;
using Microsoft.AspNetCore.Http;
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

namespace Services.Services
{
    public class KoiVarietyService : IKoiVarietyService
    {
        private readonly IKoiVarietyRepo _koiVarietyRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly IMapper _mapper;
        
        public KoiVarietyService(IKoiVarietyRepo koiVarietyRepo, ICustomerRepo customerRepo, IMapper mapper)
        {
            _koiVarietyRepo = koiVarietyRepo;
            _customerRepo = customerRepo;
            _mapper = mapper;
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

        public async Task<List<KoiVarietyElementDTO>> GetKoiVarietiesByElementAsync(string element)
        {
            return await _koiVarietyRepo.GetKoiVarietiesByElement(element);
        }

        public async Task<List<KoiVarietyElementDTO>> GetKoiVarietiesByCustomerElementAsync(Customer customer)
        {
            var customerElement = await _customerRepo.GetElementLifePalaceById(customer.CustomerId);
            if (customerElement == null || string.IsNullOrEmpty(customerElement.Element))
            {
                return new List<KoiVarietyElementDTO>();
            }

            return await _koiVarietyRepo.GetKoiVarietiesByElement(customerElement.Element);
        }
    }
}
