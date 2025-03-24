using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class CategoryDAO
    {
        private static volatile CategoryDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private CategoryDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static CategoryDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CategoryDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Category> GetCategoryByIdDao(string categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<List<Category>> GetCategoriesDao()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> CreateCategoryDao(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryDao(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategoryDao(string categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
