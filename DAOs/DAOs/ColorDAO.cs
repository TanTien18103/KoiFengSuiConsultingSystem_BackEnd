using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class ColorDAO
    {
        public static ColorDAO instance = null;
        private readonly KoiFishPondContext _context;

        public ColorDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static ColorDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ColorDAO();
                }
                return instance;
            }
        }

        public async Task<Color> GetColorByIdDao(string colorId)
        {
            return await _context.Colors.FindAsync(colorId);
        }

        public async Task<List<Color>> GetColorsDao()
        {
            return _context.Colors.ToList();
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
