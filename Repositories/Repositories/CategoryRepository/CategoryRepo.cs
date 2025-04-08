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
        public Task<Category> GetCategoryById(string id)
        {
            return CategoryDao.Instance.GetCategoryByIdDao(id);
        }
        public Task<List<Category>> GetAllCatogories()
        {
            return CategoryDao.Instance.GetAllCatogoriesDao();
        }
        public Task<Category> CreateCategory(Category category)
        {
            return CategoryDao.Instance.CreateCategoryDao(category);
        }
        public Task<Category> UpdateCategory(Category category)
        {
            return CategoryDao.Instance.UpdateCategorytDao(category);
        }

        public Task<List<Category>> GetCategories()
        {
            return CategoryDao.Instance.GetAllCatogoriesDao();
        }

        public Task DeleteCategory(string id)
        {
            return CategoryDao.Instance.DeleteCategory(id);
        }
    }
}
