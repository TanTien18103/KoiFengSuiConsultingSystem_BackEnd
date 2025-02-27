using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IMasterRepo
    {
        Task<T> GetById<T>(string id);
        Task<List<T>> GetAll<T>();
        Task<T> Create<T>(T entity);
        Task<T> Update<T>(T entity);
        Task Delete<T>(string id);
    }
}
