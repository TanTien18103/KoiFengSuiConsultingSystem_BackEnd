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

        public Task<VarietyColor> GetVarietyColorById(string varietyColorId)
        {
            return VarietyColorDAO.Instance.GetVarietyColorByIdDao(varietyColorId);
        }

        public Task<VarietyColor> CreateVarietyColor(VarietyColor varietyColor)
        {
            return VarietyColorDAO.Instance.CreateVarietyColorDao(varietyColor);
        }

        public Task<VarietyColor> UpdateVarietyColor(VarietyColor varietyColor)
        {
            return VarietyColorDAO.Instance.UpdateVarietyColorDao(varietyColor);
        }

        public Task DeleteVarietyColor(string varietyColorId)
        {
            return VarietyColorDAO.Instance.DeleteVarietyColorDao(varietyColorId);
        }

        public Task<List<VarietyColor>> GetVarietyColors()
        {
            return VarietyColorDAO.Instance.GetVarietyColorsDao();
        }
    }
}
