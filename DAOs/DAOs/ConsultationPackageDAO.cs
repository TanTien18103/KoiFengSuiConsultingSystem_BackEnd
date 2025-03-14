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
        private static volatile ConsultationPackageDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private ConsultationPackageDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static ConsultationPackageDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConsultationPackageDAO();
                        }
                    }
                }
                return _instance;
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
