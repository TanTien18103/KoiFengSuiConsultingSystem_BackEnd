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

        public Task Delete<T>(string id) where T : class
        {
            return MasterDAO.Instance.DeleteDao<T>(id);
        }

        public Task<List<T>> GetAll<T>() where T : class
        {
            return MasterDAO.Instance.GetAllDao<T>();
        }

        public Task<T> GetById<T>(string id) where T : class
        {
            return MasterDAO.Instance.GetByIdDao<T>(id);
        }

        public Task<T> Update<T>(T entity) where T : class
        {
            return MasterDAO.Instance.UpdateDao(entity);
        }
    }
}
