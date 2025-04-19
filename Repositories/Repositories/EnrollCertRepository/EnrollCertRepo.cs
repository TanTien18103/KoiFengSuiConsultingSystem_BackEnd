using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EnrollCertRepository
{
    public class EnrollCertRepo : IEnrollCertRepo
    {
        public Task<EnrollCert> GetEnrollCertById(string enrollCertId)
        {
            return EnrollCertDAO.Instance.GetEnrollCertByIdDao(enrollCertId);
        }
        public Task<List<EnrollCert>> GetEnrollCerts()
        {
            return EnrollCertDAO.Instance.GetEnrollCertsDao();
        }
        public Task<EnrollCert> CreateEnrollCert(EnrollCert enrollCert)
        {
            return EnrollCertDAO.Instance.CreateEnrollCertDao(enrollCert);
        }
        public Task<EnrollCert> UpdateEnrollCert(EnrollCert enrollCert)
        {
            return EnrollCertDAO.Instance.UpdateEnrollCertDao(enrollCert);
        }
        public Task DeleteEnrollCert(string enrollCertId)
        {
            return EnrollCertDAO.Instance.DeleteEnrollCertDao(enrollCertId);
        }

        public Task<EnrollCert> GetByCustomerIdAndCertificateId(string customerid, string certificateId)
        {
            return EnrollCertDAO.Instance.GetByCustomerIdAndCertificateIdDao(customerid, certificateId);
        }

        public Task<List<EnrollCert>> GetEnrollCertByCustomerId(string customerid)
        {
            return EnrollCertDAO.Instance.GetEnrollCertByCustomerIdDao(customerid);
        }
    }
}
