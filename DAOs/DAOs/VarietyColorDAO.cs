using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class VarietyColorDAO
    {
        private readonly KoiFishPondContext _context;

        public VarietyColorDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<VarietyColor> GetVarietyColorById(string varietyColorId)
        {
            return await _context.VarietyColors.FindAsync(varietyColorId);
        }

        public async Task<List<VarietyColor>> GetVarietyColors()
        {
            return _context.VarietyColors.ToList();
        }

        public async Task<VarietyColor> CreateVarietyColor(VarietyColor varietyColor)
        {
            _context.VarietyColors.Add(varietyColor);
            await _context.SaveChangesAsync();
            return varietyColor;
        }

        public async Task<VarietyColor> UpdateVarietyColor(VarietyColor varietyColor)
        {
            _context.VarietyColors.Update(varietyColor);
            await _context.SaveChangesAsync();
            return varietyColor;
        }

        public async Task DeleteVarietyColor(string varietyColorId)
        {
            var varietyColor = await GetVarietyColorById(varietyColorId);
            _context.VarietyColors.Remove(varietyColor);
            await _context.SaveChangesAsync();
        }
    }
}
