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
        private readonly EnrollCertDAO _enrollCertDAO;

        public EnrollCertRepo(EnrollCertDAO enrollCertDAO)
        {
            _enrollCertDAO = enrollCertDAO;
        }

        public async Task<EnrollCert> GetEnrollCertById(string enrollCertId)
        {
            return await _enrollCertDAO.GetEnrollCertById(enrollCertId);
        }

        public async Task<EnrollCert> CreateEnrollCert(EnrollCert enrollCert)
        {
            return await _enrollCertDAO.CreateEnrollCert(enrollCert);
        }

        public async Task<EnrollCert> UpdateEnrollCert(EnrollCert enrollCert)
        {
            return await _enrollCertDAO.UpdateEnrollCert(enrollCert);
        }

        public async Task DeleteEnrollCert(string enrollCertId)
        {
            await _enrollCertDAO.DeleteEnrollCert(enrollCertId);
        }

        public async Task<List<EnrollCert>> GetEnrollCerts()
        {
            return await _enrollCertDAO.GetEnrollCerts();
        }
    }
}
