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
        private static volatile VarietyColorDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private VarietyColorDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static VarietyColorDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new VarietyColorDAO();
                        }
                    }
                }
                return _instance;
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
