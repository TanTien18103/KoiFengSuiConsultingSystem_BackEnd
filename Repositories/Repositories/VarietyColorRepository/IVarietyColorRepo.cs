using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.VarietyColorRepository
{
    public interface IVarietyColorRepo
    {
        Task<VarietyColor> GetVarietyColorById(string varietyColorId);
        Task<List<VarietyColor>> GetVarietyColors();
        Task<VarietyColor> CreateVarietyColor(VarietyColor varietyColor);
        Task<VarietyColor> UpdateVarietyColor(VarietyColor varietyColor);
        Task DeleteVarietyColor(string varietyColorId);
    }
}
