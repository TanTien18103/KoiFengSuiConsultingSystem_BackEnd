using BusinessObjects.Models;
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
        Task<List<Master>> GetAllMasters();
        Task<Master> GetByMasterId(string masterId);
        Task<T> Update<T>(T entity) where T : class;
    }
}
