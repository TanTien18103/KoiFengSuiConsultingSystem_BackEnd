using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.CertificateRepository
{
    public class CertificateRepo : ICertificateRepo
    {
        public Task<Certificate> GetCertificateById(string certificateId)
        {
            return CertificateDAO.Instance.GetCertificateByIdDao(certificateId);
        }
        public Task<List<Certificate>> GetCertificatesByCourseId(string courseId)
        {
            return CertificateDAO.Instance.GetCertificatesByCourseIdDao(courseId);
        }
        public Task<Certificate> CreateCertificate(Certificate certificate)
        {
            return CertificateDAO.Instance.CreateCertificateDao(certificate);
        }
        public Task<Certificate> UpdateCertificate(Certificate certificate)
        {
            return CertificateDAO.Instance.UpdateCertificateDao(certificate);
        }
        public Task DeleteCertificate(string certificateId)
        {
            return CertificateDAO.Instance.DeleteCertificateDao(certificateId);
        }

        public Task<List<Certificate>> GetAllCertificates()
        {
            return CertificateDAO.Instance.GetAllCertificatesDao();
        }

        public Task<List<Certificate>> GetCertificatesByIds(List<string> certificateIds)
        {
            return CertificateDAO.Instance.GetCertificatesByIdsDao(certificateIds);
        }
    }
}
