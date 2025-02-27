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
    public class KoiPondRepp : IKoiPondRepo
    {
        private readonly KoiPondDAO _koiPondDAO;

        public KoiPondRepp(KoiPondDAO koiPondDAO)
        {
            _koiPondDAO = koiPondDAO;
        }

        public async Task<KoiPond> GetKoiPondById(string koiPondId)
        {
            return await _koiPondDAO.GetKoiPondById(koiPondId);
        }

        public async Task<KoiPond> CreateKoiPond(KoiPond koiPond)
        {
            return await _koiPondDAO.CreateKoiPond(koiPond);
        }

        public async Task<KoiPond> UpdateKoiPond(KoiPond koiPond)
        {
            return await _koiPondDAO.UpdateKoiPond(koiPond);
        }

        public async Task DeleteKoiPond(string koiPondId)
        {
            await _koiPondDAO.DeleteKoiPond(koiPondId);
        }

        public async Task<List<KoiPond>> GetKoiPonds()
        {
            return await _koiPondDAO.GetKoiPonds();
        }
    }
}
