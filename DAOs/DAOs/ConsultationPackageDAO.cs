using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class ConsultationPackageDAO
    {
        private readonly KoiFishPondContext _context;

        public ConsultationPackageDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<ConsultationPackage> GetConsultationPackageById(string consultationPackageId)
        {
            return await _context.ConsultationPackages.FindAsync(consultationPackageId);
        }

        public async Task<List<ConsultationPackage>> GetConsultationPackages()
        {
            return _context.ConsultationPackages.ToList();
        }

        public async Task<ConsultationPackage> CreateConsultationPackage(ConsultationPackage consultationPackage)
        {
            _context.ConsultationPackages.Add(consultationPackage);
            await _context.SaveChangesAsync();
            return consultationPackage;
        }

        public async Task<ConsultationPackage> UpdateConsultationPackage(ConsultationPackage consultationPackage)
        {
            _context.ConsultationPackages.Update(consultationPackage);
            await _context.SaveChangesAsync();
            return consultationPackage;
        }

        public async Task DeleteConsultationPackage(string consultationPackageId)
        {
            var consultationPackage = await GetConsultationPackageById(consultationPackageId);
            _context.ConsultationPackages.Remove(consultationPackage);
            await _context.SaveChangesAsync();
        }

    }
}
