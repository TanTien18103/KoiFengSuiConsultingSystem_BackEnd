using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Models;
using DAOs.DAOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repositories.CategoryRepository;
using Services.ApiModels;
using Services.ApiModels.Answer;
using Services.ApiModels.Category;
using Services.ServicesHelpers.UploadService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepo _categoryRepo;
        private readonly IMapper _mapper;
        private readonly IUploadService _uploadService;
        public CategoryService(ICategoryRepo categoryRepo, IMapper mapper, IUploadService uploadService) 
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
            _uploadService = uploadService;
        }

        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }

        public async Task<ResultModel> GetCategoryById(string id)
        {
            var res = new ResultModel();
            try
            {
                var category = await _categoryRepo.GetCategoryById(id);
                if (category == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsCategory.CATEGORY_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<CategoryResponse>(category);
                res.Message = ResponseMessageConstrantsCategory.CATEGORY_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> GetAllCategories()
        {
            var res = new ResultModel();
            try
            {
                var categories = await _categoryRepo.GetAllCatogories();
                if(categories == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsCategory.CATEGORY_NOT_FOUND;
                    return res;
                }

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<List<CategoryResponse>>(categories);
                res.Message = ResponseMessageConstrantsCategory.CATEGORY_FOUND;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> CreateCategory(CategoryRequest request)
        {
            var res = new ResultModel();
            try
            {
                var categories = await _categoryRepo.GetAllCatogories();
                var existingCategoryName = categories.FirstOrDefault(x => x.CategoryName == request.CategoryName);

                if (existingCategoryName != null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = ResponseMessageConstrantsCategory.CATEGORY_ALREADY_EXIST;
                    return res;
                }

                var category = _mapper.Map<Category>(request);
                category.CategoryId = GenerateShortGuid();
                category.CategoryName = request.CategoryName;
                category.ImageUrl = await _uploadService.UploadImageAsync(request.ImageUrl);
                await _categoryRepo.CreateCategory(category);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status201Created;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<CategoryResponse>(category);
                res.Message = ResponseMessageConstrantsCategory.CATEGORY_CREATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ResultModel> UpdateCategory(string id, CategoryRequest request)
        {
            var res = new ResultModel();
            try
            {
                var category = await _categoryRepo.GetCategoryById(id);
                if (category == null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageConstrantsCategory.CATEGORY_NOT_FOUND;
                    return res;
                }
                var categories = await _categoryRepo.GetAllCatogories();
                var existingCategoryName = categories.FirstOrDefault(x => x.CategoryName == request.CategoryName && x.CategoryId != id);

                if (existingCategoryName != null)
                {
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.ResponseCode = ResponseCodeConstants.FAILED;
                    res.Message = ResponseMessageConstrantsCategory.CATEGORY_ALREADY_EXIST;
                    return res;
                }

                _mapper.Map(request, category);
                await _categoryRepo.UpdateCategory(category);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Data = _mapper.Map<CategoryResponse>(category);
                res.Message = ResponseMessageConstrantsCategory.CATEGORY_UPDATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                return res;
            }
        }
    }
}
