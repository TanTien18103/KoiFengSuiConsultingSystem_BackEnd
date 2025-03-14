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
        public static ConsultationPackageDAO instance = null;
        private readonly KoiFishPondContext _context;

        public ConsultationPackageDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static ConsultationPackageDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConsultationPackageDAO();
                }
                return instance;
            }
        }

        public async Task<ConsultationPackage> GetConsultationPackageByIdDao(string consultationPackageId)
        {
            return await _context.ConsultationPackages.FindAsync(consultationPackageId);
        }

        public async Task<List<ConsultationPackage>> GetConsultationPackagesDao()
        {
            return _context.ConsultationPackages.ToList();
        }

        public async Task<ConsultationPackage> CreateConsultationPackageDao(ConsultationPackage consultationPackage)
        {
            _context.ConsultationPackages.Add(consultationPackage);
            await _context.SaveChangesAsync();
            return consultationPackage;
        }

        public async Task<ConsultationPackage> UpdateConsultationPackageDao(ConsultationPackage consultationPackage)
        {
            _context.ConsultationPackages.Update(consultationPackage);
            await _context.SaveChangesAsync();
            return consultationPackage;
        }

        public async Task DeleteConsultationPackageDao(string consultationPackageId)
        {
            var consultationPackage = await GetConsultationPackageByIdDao(consultationPackageId);
            _context.ConsultationPackages.Remove(consultationPackage);
            await _context.SaveChangesAsync();
        }
    }
}
