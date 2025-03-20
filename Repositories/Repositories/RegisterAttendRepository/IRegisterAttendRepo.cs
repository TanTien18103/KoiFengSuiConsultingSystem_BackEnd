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
        Task<List<RegisterAttend>> GetRegisterAttendsByWorkShopId(string workShopId);
        Task<List<RegisterAttend>> GetRegisterAttendsByGroupId(string groupId);
        Task<RegisterAttend> CreateRegisterAttend(RegisterAttend registerAttend);
        Task<RegisterAttend> UpdateRegisterAttend(RegisterAttend registerAttend);
        Task DeleteRegisterAttend(string registerAttendId);
        Task<List<RegisterAttend>> GetPendingTickets(string workshopId, string customerId);
    }
}
