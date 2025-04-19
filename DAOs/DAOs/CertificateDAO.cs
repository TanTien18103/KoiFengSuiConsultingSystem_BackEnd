using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class CertificateDAO
    {
        private static volatile CertificateDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private CertificateDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static CertificateDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CertificateDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Certificate> GetCertificateByIdDao(string certificateId)
        {
            return await _context.Certificates
                .Include(c => c.Courses)
                .FirstOrDefaultAsync(c => c.CertificateId == certificateId);
        }
        public async Task<List<Certificate>> GetAllCertificatesDao()
        {
            return await _context.Certificates
                .Include(c => c.Courses)
                .ToListAsync();
        }

        public async Task<List<Certificate>> GetCertificatesByCourseIdDao(string courseId)
        {
            return await _context.Certificates
                .Where(c => c.Courses.Any(course => course.CourseId == courseId))
                .ToListAsync();
        }

        public async Task<Certificate> CreateCertificateDao(Certificate certificate)
        {
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();
            return certificate;
        }

        public async Task<Certificate> UpdateCertificateDao(Certificate certificate)
        {
            _context.Certificates.Update(certificate);
            await _context.SaveChangesAsync();
            return certificate;
        }

        public async Task DeleteCertificateDao(string certificateId)
        {
            var certificate = await GetCertificateByIdDao(certificateId);
            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Certificate>> GetCertificatesByIdsDao(List<string> certificateIds)
        {
            return await _context.Certificates
                .Where(c => certificateIds.Contains(c.CertificateId))
                .ToListAsync();
        }
    }
}
