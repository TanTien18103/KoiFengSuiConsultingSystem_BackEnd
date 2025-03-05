using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class FengShuiDocumentDAO
    {
        public static FengShuiDocumentDAO instance = null;
        private readonly KoiFishPondContext _context;

        public FengShuiDocumentDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static FengShuiDocumentDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FengShuiDocumentDAO();
                }
                return instance;
            }
        }

        public async Task<FengShuiDocument> GetFengShuiDocumentByIdDao(string fengShuiDocumentId)
        {
            return await _context.FengShuiDocuments.FindAsync(fengShuiDocumentId);
        }

        public async Task<List<FengShuiDocument>> GetFengShuiDocumentsDao()
        {
            return _context.FengShuiDocuments.ToList();
        }

        public async Task<FengShuiDocument> CreateFengShuiDocumentDao(FengShuiDocument fengShuiDocument)
        {
            _context.FengShuiDocuments.Add(fengShuiDocument);
            await _context.SaveChangesAsync();
            return fengShuiDocument;
        }

        public async Task<FengShuiDocument> UpdateFengShuiDocumentDao(FengShuiDocument fengShuiDocument)
        {
            _context.FengShuiDocuments.Update(fengShuiDocument);
            await _context.SaveChangesAsync();
            return fengShuiDocument;
        }

        public async Task DeleteFengShuiDocumentDao(string fengShuiDocumentId)
        {
            var fengShuiDocument = await GetFengShuiDocumentByIdDao(fengShuiDocumentId);
            _context.FengShuiDocuments.Remove(fengShuiDocument);
            await _context.SaveChangesAsync();
        }

    }
}
