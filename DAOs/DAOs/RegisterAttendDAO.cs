using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class RegisterAttendDAO
    {
        private readonly KoiFishPondContext _context;

        public RegisterAttendDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<RegisterAttend> GetRegisterAttendById(string registerAttendId)
        {
            return await _context.RegisterAttends.FindAsync(registerAttendId);
        }

        public async Task<List<RegisterAttend>> GetRegisterAttends()
        {
            return _context.RegisterAttends.ToList();
        }

        public async Task<RegisterAttend> CreateRegisterAttend(RegisterAttend registerAttend)
        {
            _context.RegisterAttends.Add(registerAttend);
            await _context.SaveChangesAsync();
            return registerAttend;
        }

        public async Task<RegisterAttend> UpdateRegisterAttend(RegisterAttend registerAttend)
        {
            _context.RegisterAttends.Update(registerAttend);
            await _context.SaveChangesAsync();
            return registerAttend;
        }

        public async Task DeleteRegisterAttend(string registerAttendId)
        {
            var registerAttend = await GetRegisterAttendById(registerAttendId);
            _context.RegisterAttends.Remove(registerAttend);
            await _context.SaveChangesAsync();
        }
    }
}
