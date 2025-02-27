using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ICertificateRepo
    {

        Task<Certificate> GetCertificateById(string certificateId);
        Task<List<Certificate>> GetCertificatesByCourseId(string courseId);
        Task<Certificate> CreateCertificate(Certificate certificate);
        Task<Certificate> UpdateCertificate(Certificate certificate);
        Task DeleteCertificate(string certificateId);

    }
}
