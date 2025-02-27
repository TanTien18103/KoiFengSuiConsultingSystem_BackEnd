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
        private readonly KoiVarietyDAO _koiVarietyDAO;

        public KoiVarietyRepo(KoiVarietyDAO koiVarietyDAO)
        {
            _koiVarietyDAO = koiVarietyDAO;
        }

        public async Task<KoiVariety> GetKoiVarietyById(string koiVarietyId)
        {
            return await _koiVarietyDAO.GetKoiVarietyById(koiVarietyId);
        }

        public async Task<KoiVariety> CreateKoiVariety(KoiVariety koiVariety)
        {
            return await _koiVarietyDAO.CreateKoiVariety(koiVariety);
        }

        public async Task<KoiVariety> UpdateKoiVariety(KoiVariety koiVariety)
        {
            return await _koiVarietyDAO.UpdateKoiVariety(koiVariety);
        }

        public async Task DeleteKoiVariety(string koiVarietyId)
        {
            await _koiVarietyDAO.DeleteKoiVariety(koiVarietyId);
        }

        public async Task<List<KoiVariety>> GetKoiVarieties()
        {
            return await _koiVarietyDAO.GetKoiVarieties();
        }
    }
}
