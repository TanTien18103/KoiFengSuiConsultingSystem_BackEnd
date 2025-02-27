using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IMasterRepo
    {
        Task<T> Create<T>(T entity) where T : class;
        Task Delete<T>(string id) where T : class;
        Task<List<T>> GetAll<T>() where T : class;
        Task<T> GetById<T>(string id) where T : class;
        Task<T> Update<T>(T entity) where T : class;
    }
}
