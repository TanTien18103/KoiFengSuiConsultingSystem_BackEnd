﻿using BusinessObjects.Enums;
using BusinessObjects.Models;
using Services.ApiModels;
using Services.ApiModels.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<ResultModel> GetCategoryById(string id);
        Task<ResultModel> GetAllCategories();
        Task<ResultModel> GetAllActiveCategories();
        Task<ResultModel> CreateCategory(CategoryRequest request);
        Task<ResultModel> UpdateCategory(string id, CategoryUpdateRequest request);
        Task<ResultModel> DeleteCategory(string id);
        Task<ResultModel> UpdateCategoryStatus(string id, CategoryStatusEnums status);

    }
}
