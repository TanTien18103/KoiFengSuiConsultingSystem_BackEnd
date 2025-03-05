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
    public class EnrollCertRepo : IEnrollCertRepo
    {
        public Task<EnrollCert> GetEnrollCertById(string enrollCertId)
        {
            return EnrollCertDAO.Instance.GetEnrollCertByIdDao(enrollCertId);
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

        public Task<List<EnrollCert>> GetEnrollCerts()
        {
            return EnrollCertDAO.Instance.GetEnrollCertsDao();
        }
    }
}
