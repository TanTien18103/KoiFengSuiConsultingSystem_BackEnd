using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.CategoryRepository
{
    public interface ICategoryRepo
    {
        Task<Category> GetCategoryById(string categoryId);
        Task<List<Category>> GetCategories();
        Task<Category> CreateCategory(Category category);
        Task<Category> UpdateCategory(Category category);
        Task DeleteCategory(string categoryId);

    }
}
