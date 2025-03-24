using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.CategoryRepository
{
    public class CategoryRepo : ICategoryRepo
    {
        public Task<Category> CreateCategory(Category category)
        {
            return CategoryDAO.Instance.CreateCategoryDao(category);
        }

        public Task DeleteCategory(string categoryId)
        {
            return CategoryDAO.Instance.DeleteCategoryDao(categoryId);
        }

        public Task<List<Category>> GetCategories()
        {
            return CategoryDAO.Instance.GetCategoriesDao();
        }

        public Task<Category> GetCategoryById(string categoryId)
        {
            return CategoryDAO.Instance.GetCategoryByIdDao(categoryId);
        }

        public Task<Category> UpdateCategory( Category category)
        {
            return CategoryDAO.Instance.UpdateCategoryDao(category);
        }
    }
}
