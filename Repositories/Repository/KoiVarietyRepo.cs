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
    public class KoiVarietyRepo : IKoiVarietyRepo
    {
        public Task<KoiVariety> GetKoiVarietyById(string koiVarietyId)
        {
            return KoiVarietyDAO.Instance.GetKoiVarietyByIdDao(koiVarietyId);
        }

        public Task<KoiVariety> CreateKoiVariety(KoiVariety koiVariety)
        {
            return KoiVarietyDAO.Instance.CreateKoiVarietyDao(koiVariety);
        }

        public Task<KoiVariety> UpdateKoiVariety(KoiVariety koiVariety)
        {
            return KoiVarietyDAO.Instance.UpdateKoiVarietyDao(koiVariety);
        }

        public Task DeleteKoiVariety(string koiVarietyId)
        {
            return KoiVarietyDAO.Instance.DeleteKoiVarietyDao(koiVarietyId);
        }

        public Task<List<KoiVariety>> GetKoiVarieties()
        {
            return KoiVarietyDAO.Instance.GetKoiVarietiesDao();
        }

        public Task<List<KoiVariety>> GetKoiVarietyWithColors()
        {
            return KoiVarietyDAO.Instance.GetAllKoiVarietiesWithColorsDao();
        }
        public Task<KoiVariety> GetKoiVarietyWithColorsById(string id)
        {
            return KoiVarietyDAO.Instance.GetAllKoiVarietiesWithColorsByIdDao(id);
        }
        public Task<List<KoiVariety>> GetKoiVarietiesByElement(string element)
        {
            return KoiVarietyDAO.Instance.GetKoiVarietiesByCustomerElementDao(element);
        }
    }
}
