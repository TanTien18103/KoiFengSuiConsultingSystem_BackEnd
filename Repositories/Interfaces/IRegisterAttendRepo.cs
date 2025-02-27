using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IRegisterAttendRepo
    {
        Task<RegisterAttend> GetRegisterAttendById(string registerAttendId);
        Task<List<RegisterAttend>> GetRegisterAttends();
        Task<RegisterAttend> CreateRegisterAttend(RegisterAttend registerAttend);
        Task<RegisterAttend> UpdateRegisterAttend(RegisterAttend registerAttend);
        Task DeleteRegisterAttend(string registerAttendId);
    }
}
