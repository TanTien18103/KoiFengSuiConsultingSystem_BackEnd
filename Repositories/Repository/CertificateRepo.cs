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
    public class CertificateRepo : ICertificateRepo
    {
        private readonly CertificateDAO _certificateDAO;

        public CertificateRepo(CertificateDAO certificateDAO)
        {
            _certificateDAO = certificateDAO;
        }

        public async Task<Certificate> GetCertificateById(string certificateId)
        {
            return await _certificateDAO.GetCertificateById(certificateId);
        }
        public async Task<Certificate> CreateCertificate(Certificate certificate)
        {
            return await _certificateDAO.CreateCertificate(certificate);
        }

        public async Task<Certificate> UpdateCertificate(Certificate certificate)
        {
            return await _certificateDAO.UpdateCertificate(certificate);
        }

        public async Task DeleteCertificate(string certificateId)
        {
            await _certificateDAO.DeleteCertificate(certificateId);
        }

        public async Task<List<Certificate>> GetCertificatesByCourseId(string courseId)
        {
            return await _certificateDAO.GetCertificatesByCourseId(courseId);
        }
    }
}
