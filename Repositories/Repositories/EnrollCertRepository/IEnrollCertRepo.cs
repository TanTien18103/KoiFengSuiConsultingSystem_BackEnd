using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.EnrollCertRepository
{
    public interface IEnrollCertRepo
    {
        Task<EnrollCert> GetEnrollCertById(string enrollCertId);
        Task<List<EnrollCert>> GetEnrollCerts();
        Task<EnrollCert> CreateEnrollCert(EnrollCert enrollCert);
        Task<EnrollCert> UpdateEnrollCert(EnrollCert enrollCert);
        Task DeleteEnrollCert(string enrollCertId);
        Task<EnrollCert> GetByCustomerIdAndCertificateId(string customerid, string certificateId);
        Task<List<EnrollCert>> GetEnrollCertByCustomerId(string customerid);
    }
}
