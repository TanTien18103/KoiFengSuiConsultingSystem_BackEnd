using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.KoiPondRepository
{
    public interface IKoiPondRepo
    {
        Task<KoiPond> GetKoiPondById(string koiPondId);
        Task<List<KoiPond>> GetKoiPonds();
        Task<KoiPond> CreateKoiPond(KoiPond koiPond);
        Task<KoiPond> UpdateKoiPond(KoiPond koiPond);
        Task DeleteKoiPond(string koiPondId);
    }
}
