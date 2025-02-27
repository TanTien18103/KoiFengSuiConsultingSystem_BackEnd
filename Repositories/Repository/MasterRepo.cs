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
        private readonly MasterDAO _masterDAO;

        public MasterRepo(MasterDAO masterDAO)
        {
            _masterDAO = masterDAO;
        }

        public async Task<T> Create<T>(T entity) where T : class
        {
            return await _masterDAO.Create(entity);
        }

        public async Task Delete<T>(string id) where T : class
        {
            await _masterDAO.Delete<T>(id);
        }

        public async Task<List<T>> GetAll<T>() where T : class
        {
            return await _masterDAO.GetAll<T>();
        }

        public async Task<T> GetById<T>(string id) where T : class
        {
            return await _masterDAO.GetById<T>(id);
        }

        public async Task<T> Update<T>(T entity) where T : class
        {
            return await _masterDAO.Update(entity);
        }
    }
}
