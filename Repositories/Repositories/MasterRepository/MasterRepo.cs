using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.MasterRepository
{
    public class MasterRepo : IMasterRepo
    {
        public Task<List<Master>> GetAllMasters()
        {
            return MasterDAO.Instance.GetAllMastersDAO();
        }
        public Task<Master> GetMasterByAccountId(string accountId)
        {
            return MasterDAO.Instance.GetMasterByAccountIdDao(accountId);
        }
        public Task<Master> GetByMasterId(string masterId)
        {
            return MasterDAO.Instance.GetByMasterIdDao(masterId);
        }
        public Task<string> GetMasterIdByAccountId(string accountId)
        {
            return MasterDAO.Instance.GetMasterIdByAccountIdDao(accountId);
        }
        public Task<T> Create<T>(T entity) where T : class
        {
            return MasterDAO.Instance.CreateDao(entity);
        }
        public Task<T> Update<T>(T entity) where T : class
        {
            return MasterDAO.Instance.UpdateDao(entity);
        }
    }
}
