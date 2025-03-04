using BusinessObjects.Models;
using DAOs.DAOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class KoiPondRepo : IKoiPondRepo
    {
        private readonly KoiPondDAO _koiPondDAO;
        public KoiPondRepo()
        {
            _koiPondDAO = new KoiPondDAO();
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
