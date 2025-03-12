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
    public class RegisterAttendRepo : IRegisterAttendRepo
    {
        public Task<RegisterAttend> GetRegisterAttendById(string registerAttendId)
        {
            return RegisterAttendDAO.Instance.GetRegisterAttendByIdDao(registerAttendId);
        }

        public Task<RegisterAttend> CreateRegisterAttend(RegisterAttend registerAttend)
        {
            return RegisterAttendDAO.Instance.CreateRegisterAttendDao(registerAttend);
        }

        public Task<RegisterAttend> UpdateRegisterAttend(RegisterAttend registerAttend)
        {
            return RegisterAttendDAO.Instance.UpdateRegisterAttendDao(registerAttend);
        }

        public Task DeleteRegisterAttend(string registerAttendId)
        {
            return RegisterAttendDAO.Instance.DeleteRegisterAttendDao(registerAttendId);
        }

        public Task<List<RegisterAttend>> GetRegisterAttends()
        {
            return RegisterAttendDAO.Instance.GetRegisterAttendsDao();
        }

        public Task<List<RegisterAttend>> GetRegisterAttendByCustomerId(string customerId)
        {
            return RegisterAttendDAO.Instance.GetRegisterAttendsByCustomerIdDao(customerId);
        }

        public Task<string> GetCustomerIdByAccountId(string accountId)
        {
            return RegisterAttendDAO.Instance.GetCustomerIdByAccountIdDao(accountId);
        }

        public Task<List<RegisterAttend>> GetRegisterAttendsByWorkShopId(string workShopId)
        {
            return RegisterAttendDAO.Instance.GetRegisterAttendsByWorkshopIdDao(workShopId);
        }
    }
}
