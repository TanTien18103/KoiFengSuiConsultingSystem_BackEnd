using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.RegisterAttendRepository
{
    public interface IRegisterAttendRepo
    {
        Task<RegisterAttend> GetRegisterAttendById(string registerAttendId);
        Task<List<RegisterAttend>> GetRegisterAttends();
        Task<List<RegisterAttend>> GetRegisterAttendByCustomerId(string customerId);
        Task<string> GetCustomerIdByAccountId(string accountId);
        Task<List<RegisterAttend>> GetRegisterAttendsByWorkShopId(string workShopId);
        Task<RegisterAttend> CreateRegisterAttend(RegisterAttend registerAttend);
        Task<RegisterAttend> UpdateRegisterAttend(RegisterAttend registerAttend);
        Task DeleteRegisterAttend(string registerAttendId);
    }
}
