using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Answer;
using Services.ApiModels.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile() 
        {
            CreateMap<Category, CategoryResponse>();

            CreateMap<CategoryRequest, Category>();
        }
    }
}
