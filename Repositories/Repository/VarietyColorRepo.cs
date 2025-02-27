using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class VarietyColorRepo : IVarietyColorRepo
    {
        private readonly VarietyColorDAO _varietyColorDAO;

        public VarietyColorRepo(VarietyColorDAO varietyColorDAO)
        {
            _varietyColorDAO = varietyColorDAO;
        }

        public async Task<VarietyColor> GetVarietyColorById(string varietyColorId)
        {
            return await _varietyColorDAO.GetVarietyColorById(varietyColorId);
        }

        public async Task<VarietyColor> CreateVarietyColor(VarietyColor varietyColor)
        {
            return await _varietyColorDAO.CreateVarietyColor(varietyColor);
        }

        public async Task<VarietyColor> UpdateVarietyColor(VarietyColor varietyColor)
        {
            return await _varietyColorDAO.UpdateVarietyColor(varietyColor);
        }

        public async Task DeleteVarietyColor(string varietyColorId)
        {
            await _varietyColorDAO.DeleteVarietyColor(varietyColorId);
        }

        public async Task<List<VarietyColor>> GetVarietyColors()
        {
            return await _varietyColorDAO.GetVarietyColors();
        }
    }
}
