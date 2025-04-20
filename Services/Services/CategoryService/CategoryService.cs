using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using DAOs.DAOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repositories.CategoryRepository;
using Services.ApiModels;
using Services.ApiModels.Answer;
using Services.ApiModels.Category;
using Services.ApiModels.Course;
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

        public async Task<ResultModel> GetAllActiveCategories()
        {
            var res = new ResultModel();
            try
            {
                var categories = await _categoryRepo.GetAllCatogories();
                var activeCategories = categories.Where(x => x.Status == CategoryStatusEnums.Active.ToString());
                if (activeCategories == null)
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
                res.Data = _mapper.Map<List<CategoryResponse>>(activeCategories);
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
                category.Status = CategoryStatusEnums.Inactive.ToString();
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

        public async Task<ResultModel> UpdateCategory(string id, CategoryUpdateRequest request)
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

                // Kiểm tra trùng tên nếu có truyền CategoryName
                if (!string.IsNullOrWhiteSpace(request.CategoryName))
                {
                    var categories = await _categoryRepo.GetAllCatogories();
                    var existingCategoryName = categories.FirstOrDefault(x =>
                        x.CategoryName == request.CategoryName && x.CategoryId != id);

                    if (existingCategoryName != null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status400BadRequest;
                        res.ResponseCode = ResponseCodeConstants.FAILED;
                        res.Message = ResponseMessageConstrantsCategory.CATEGORY_ALREADY_EXIST;
                        return res;
                    }

                    category.CategoryName = request.CategoryName;
                }

                // Nếu có ảnh mới thì upload
                if (request.ImageUrl != null)
                {
                    category.ImageUrl = await _uploadService.UploadImageAsync(request.ImageUrl);
                }

                await _categoryRepo.UpdateCategory(category);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsCategory.CATEGORY_UPDATED_SUCCESS;
                res.Data = _mapper.Map<CategoryResponse>(category);
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = $"Đã xảy ra lỗi khi cập nhật danh mục: {ex.Message}";
                return res;
            }
        }

        public async Task<ResultModel> DeleteCategory(string id)
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

                await _categoryRepo.DeleteCategory(id);

                res.IsSuccess = true;
                res.StatusCode = StatusCodes.Status200OK;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageConstrantsCategory.CATEGORY_DELETED_SUCCESS;
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

        public async Task<ResultModel> UpdateCategoryStatus(string id, CategoryStatusEnums status)
        {
            var res = new ResultModel();

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsCategory.CATEGORY_ID_INVALID;
                    return res;
                }

                // Làm sạch id
                id = id.Trim();

                // Validate status enum (phòng trường hợp nhận từ body dạng int bị sai)
                if (!Enum.IsDefined(typeof(CategoryStatusEnums), status))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsCategory.STATUS_INVALID;
                    return res;
                }

                var category = await _categoryRepo.GetCategoryById(id);
                if (category == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    res.Message = ResponseMessageConstrantsCategory.CATEGORY_NOT_FOUND;
                    return res;
                }

                var newStatus = status.ToString();

                if (category.Status == newStatus)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = ResponseMessageConstrantsCategory.CATEGORY_ALREADY_HAS_THIS_STATUS;
                    return res;
                }

                category.Status = newStatus;
                await _categoryRepo.UpdateCategory(category);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = _mapper.Map<CategoryResponse>(category);
                res.Message = ResponseMessageConstrantsCategory.CATEGORY_STATUS_UPDATED_SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = "Đã xảy ra lỗi nội bộ: " + ex.Message;
                return res;
            }
        }
    }
}
