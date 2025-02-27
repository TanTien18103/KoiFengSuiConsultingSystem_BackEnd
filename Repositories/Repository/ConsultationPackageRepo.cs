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
    public class ConsultationPackageRepo : IConsultationPackageRepo
    {
        private readonly ConsultationPackageDAO _consultationPackageDAO;

        public ConsultationPackageRepo(ConsultationPackageDAO consultationPackageDAO)
        {
            _consultationPackageDAO = consultationPackageDAO;
        }

        public async Task<ConsultationPackage> GetConsultationPackageById(string consultationPackageId)
        {
            return await _consultationPackageDAO.GetConsultationPackageById(consultationPackageId);
        }

        public async Task<ConsultationPackage> CreateConsultationPackage(ConsultationPackage consultationPackage)
        {
            return await _consultationPackageDAO.CreateConsultationPackage(consultationPackage);
        }

        public async Task<ConsultationPackage> UpdateConsultationPackage(ConsultationPackage consultationPackage)
        {
            return await _consultationPackageDAO.UpdateConsultationPackage(consultationPackage);
        }

        public async Task DeleteConsultationPackage(string consultationPackageId)
        {
            await _consultationPackageDAO.DeleteConsultationPackage(consultationPackageId);
        }

        public async Task<List<ConsultationPackage>> GetConsultationPackages()
        {
            return await _consultationPackageDAO.GetConsultationPackages();
        }
    }
}
