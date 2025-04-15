using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.MasterRepository
{
    public interface IMasterRepo
    {
        Task<List<Master>> GetAllMasters();
        Task<Master> GetMasterByAccountId(string accountId);
        Task<Master> GetByMasterId(string masterId);
        Task<string> GetMasterIdByAccountId(string accountId);
        Task<T> Create<T>(T entity) where T : class;
        Task<T> Update<T>(T entity) where T : class;
        Task<Master> GetMasterByWorkshopId(string workshopId);
    }
}
