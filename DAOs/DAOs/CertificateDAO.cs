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
        private readonly KoiFishPondContext _context;

        public CertificateDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Certificate> GetCertificateById(string certificateId)
        {
            return await _context.Certificates.FindAsync(certificateId);
        }

        public async Task<List<Certificate>> GetCertificatesByCourseId(string courseId)
        {
            return await _context.Certificates
                .Where(c => c.Courses.Any(course => course.CourseId == courseId))
                .ToListAsync();
        }

        public async Task<Certificate> CreateCertificate(Certificate certificate)
        {
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();
            return certificate;
        }

        public async Task<Certificate> UpdateCertificate(Certificate certificate)
        {
            _context.Certificates.Update(certificate);
            await _context.SaveChangesAsync();
            return certificate;
        }

        public async Task DeleteCertificate(string certificateId)
        {
            var certificate = await GetCertificateById(certificateId);
            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();
        }
    }
}
