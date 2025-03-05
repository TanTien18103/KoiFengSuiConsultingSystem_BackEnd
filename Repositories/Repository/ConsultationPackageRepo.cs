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
        public Task<ConsultationPackage> GetConsultationPackageById(string consultationPackageId)
        {
            return ConsultationPackageDAO.Instance.GetConsultationPackageByIdDao(consultationPackageId);
        }

        public Task<ConsultationPackage> CreateConsultationPackage(ConsultationPackage consultationPackage)
        {
            return ConsultationPackageDAO.Instance.CreateConsultationPackageDao(consultationPackage);
        }

        public Task<ConsultationPackage> UpdateConsultationPackage(ConsultationPackage consultationPackage)
        {
            return ConsultationPackageDAO.Instance.UpdateConsultationPackageDao(consultationPackage);
        }

        public Task DeleteConsultationPackage(string consultationPackageId)
        {
            return ConsultationPackageDAO.Instance.DeleteConsultationPackageDao(consultationPackageId);
        }

        public Task<List<ConsultationPackage>> GetConsultationPackages()
        {
            return ConsultationPackageDAO.Instance.GetConsultationPackagesDao();
        }
    }
}
