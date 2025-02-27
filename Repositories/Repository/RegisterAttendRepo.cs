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
        private readonly RegisterAttendDAO _registerAttendDAO;

        public RegisterAttendRepo(RegisterAttendDAO registerAttendDAO)
        {
            _registerAttendDAO = registerAttendDAO;
        }

        public async Task<RegisterAttend> GetRegisterAttendById(string registerAttendId)
        {
            return await _registerAttendDAO.GetRegisterAttendById(registerAttendId);
        }

        public async Task<RegisterAttend> CreateRegisterAttend(RegisterAttend registerAttend)
        {
            return await _registerAttendDAO.CreateRegisterAttend(registerAttend);
        }

        public async Task<RegisterAttend> UpdateRegisterAttend(RegisterAttend registerAttend)
        {
            return await _registerAttendDAO.UpdateRegisterAttend(registerAttend);
        }

        public async Task DeleteRegisterAttend(string registerAttendId)
        {
            await _registerAttendDAO.DeleteRegisterAttend(registerAttendId);
        }

        public async Task<List<RegisterAttend>> GetRegisterAttends()
        {
            return await _registerAttendDAO.GetRegisterAttends();
        }
    }
}
