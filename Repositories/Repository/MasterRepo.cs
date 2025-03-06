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
    public class MasterRepo : IMasterRepo
    {
        public Task<T> Create<T>(T entity) where T : class
        {
            return MasterDAO.Instance.CreateDao(entity);
        }

        public Task<List<Master>> GetAllMasters()
        {
            return MasterDAO.Instance.GetAllMastersDAO();
        }

        public Task<Master> GetByMasterId(string masterId)
        {
            return MasterDAO.instance.GetByMasterIdDao(masterId);
        }

        public Task<T> Update<T>(T entity) where T : class
        {
            return MasterDAO.Instance.UpdateDao(entity);
        }
    }
}
