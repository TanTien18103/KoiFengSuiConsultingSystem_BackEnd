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
        public static VarietyColorDAO instance = null;
        private readonly KoiFishPondContext _context;

        public VarietyColorDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static VarietyColorDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VarietyColorDAO();
                }
                return instance;
            }
        }

        public async Task<VarietyColor> GetVarietyColorByIdDao(string varietyColorId)
        {
            return await _context.VarietyColors.FindAsync(varietyColorId);
        }

        public async Task<List<VarietyColor>> GetVarietyColorsDao()
        {
            return _context.VarietyColors.ToList();
        }

        public async Task<VarietyColor> CreateVarietyColorDao(VarietyColor varietyColor)
        {
            _context.VarietyColors.Add(varietyColor);
            await _context.SaveChangesAsync();
            return varietyColor;
        }

        public async Task<VarietyColor> UpdateVarietyColorDao(VarietyColor varietyColor)
        {
            _context.VarietyColors.Update(varietyColor);
            await _context.SaveChangesAsync();
            return varietyColor;
        }

        public async Task DeleteVarietyColorDao(string varietyColorId)
        {
            var varietyColor = await GetVarietyColorByIdDao(varietyColorId);
            _context.VarietyColors.Remove(varietyColor);
            await _context.SaveChangesAsync();
        }
    }
}
