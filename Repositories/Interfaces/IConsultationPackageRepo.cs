using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IConsultationPackageRepo
    {
        Task<ConsultationPackage> GetConsultationPackageById(string consultationPackageId);
        Task<List<ConsultationPackage>> GetConsultationPackages();
        Task<ConsultationPackage> CreateConsultationPackage(ConsultationPackage consultationPackage);
        Task<ConsultationPackage> UpdateConsultationPackage(ConsultationPackage consultationPackage);
        Task DeleteConsultationPackage(string consultationPackageId);

    }
}
