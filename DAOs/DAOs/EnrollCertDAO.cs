using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class EnrollCertDAO
    {
        private readonly KoiFishPondContext _context;

        public EnrollCertDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<EnrollCert> GetEnrollCertById(string enrollCertId)
        {
            return await _context.EnrollCerts.FindAsync(enrollCertId);
        }

        public async Task<List<EnrollCert>> GetEnrollCerts()
        {
            return _context.EnrollCerts.ToList();
        }

        public async Task<EnrollCert> CreateEnrollCert(EnrollCert enrollCert)
        {
            _context.EnrollCerts.Add(enrollCert);
            await _context.SaveChangesAsync();
            return enrollCert;
        }

        public async Task<EnrollCert> UpdateEnrollCert(EnrollCert enrollCert)
        {
            _context.EnrollCerts.Update(enrollCert);
            await _context.SaveChangesAsync();
            return enrollCert;
        }

        public async Task DeleteEnrollCert(string enrollCertId)
        {
            var enrollCert = await GetEnrollCertById(enrollCertId);
            _context.EnrollCerts.Remove(enrollCert);
            await _context.SaveChangesAsync();
        }
    }
}
