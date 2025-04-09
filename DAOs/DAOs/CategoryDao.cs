using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class CategoryDao
    {
        private static volatile CategoryDao _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private CategoryDao()
        {
            _context = new KoiFishPondContext();
        }

        public static CategoryDao Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CategoryDao();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Category> GetCategoryByIdDao(string id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
        }

        public async Task<List<Category>> GetAllCatogoriesDao()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> CreateCategoryDao(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategorytDao(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategory(string id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
