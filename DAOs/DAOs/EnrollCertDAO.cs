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
        private static volatile EnrollCertDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private EnrollCertDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static EnrollCertDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new EnrollCertDAO();
                        }
                    }
                }
                return _instance;
            }
        }
        public async Task<EnrollCert> GetEnrollCertByIdDao(string enrollCertId)
        {
            return await _context.EnrollCerts.FindAsync(enrollCertId);
        }

        public async Task<List<EnrollCert>> GetEnrollCertsDao()
        {
            return _context.EnrollCerts.ToList();
        }

        public async Task<EnrollCert> CreateEnrollCertDao(EnrollCert enrollCert)
        {
            _context.EnrollCerts.Add(enrollCert);
            await _context.SaveChangesAsync();
            return enrollCert;
        }

        public async Task<EnrollCert> UpdateEnrollCertDao(EnrollCert enrollCert)
        {
            _context.EnrollCerts.Update(enrollCert);
            await _context.SaveChangesAsync();
            return enrollCert;
        }

        public async Task DeleteEnrollCertDao(string enrollCertId)
        {
            var enrollCert = await GetEnrollCertByIdDao(enrollCertId);
            _context.EnrollCerts.Remove(enrollCert);
            await _context.SaveChangesAsync();
        }
    }
}
