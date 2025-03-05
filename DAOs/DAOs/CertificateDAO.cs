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
        public static CertificateDAO instance = null;
        private readonly KoiFishPondContext _context;

        public CertificateDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static CertificateDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CertificateDAO();
                }
                return instance;
            }
        }

        public async Task<Certificate> GetCertificateByIdDao(string certificateId)
        {
            return await _context.Certificates.FindAsync(certificateId);
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
    }
}
