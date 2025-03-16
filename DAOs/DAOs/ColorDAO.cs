using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class ColorDAO
    {
        private static volatile ColorDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private ColorDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static ColorDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ColorDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Color> GetColorByIdDao(string colorId)
        {
            return await _context.Colors.FindAsync(colorId);
        }

        public async Task<List<Color>> GetColorsDao()
        {
            return await  _context.Colors.ToListAsync();
        }

        public async Task<Color> CreateColorDao(Color color)
        {
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return color;
        }

        public async Task<Color> UpdateColorDao(Color color)
        {
            _context.Colors.Update(color);
            await _context.SaveChangesAsync();
            return color;
        }

        public async Task DeleteColorDao(string colorId)
        {
            var color = await GetColorByIdDao(colorId);
            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
        }
    }
}
