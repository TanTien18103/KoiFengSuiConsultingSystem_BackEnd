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
        private readonly KoiFishPondContext _context;

        public ColorDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Color> GetColorById(string colorId)
        {
            return await _context.Colors.FindAsync(colorId);
        }

        public async Task<List<Color>> GetColors()
        {
            return _context.Colors.ToList();
        }

        public async Task<Color> CreateColor(Color color)
        {
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return color;
        }

        public async Task<Color> UpdateColor(Color color)
        {
            _context.Colors.Update(color);
            await _context.SaveChangesAsync();
            return color;
        }

        public async Task DeleteColor(string colorId)
        {
            var color = await GetColorById(colorId);
            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
        }

    }
}
