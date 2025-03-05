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
        public Task<KoiPond> GetKoiPondById(string koiPondId)
        {
            return KoiPondDAO.Instance.GetKoiPondByIdDao(koiPondId);
        }

        public Task<KoiPond> CreateKoiPond(KoiPond koiPond)
        {
            return KoiPondDAO.Instance.CreateKoiPondDao(koiPond);
        }

        public Task<KoiPond> UpdateKoiPond(KoiPond koiPond)
        {
            return KoiPondDAO.Instance.UpdateKoiPondDao(koiPond);
        }

        public Task DeleteKoiPond(string koiPondId)
        {
            return KoiPondDAO.Instance.DeleteKoiPondDao(koiPondId);
        }

        public Task<List<KoiPond>> GetKoiPonds()
        {
            return KoiPondDAO.Instance.GetKoiPondsDao();
        }
    }
}
