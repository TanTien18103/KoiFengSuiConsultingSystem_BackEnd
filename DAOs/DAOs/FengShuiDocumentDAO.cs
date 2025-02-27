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
        private readonly KoiFishPondContext _context;

        public FengShuiDocumentDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<FengShuiDocument> GetFengShuiDocumentById(string fengShuiDocumentId)
        {
            return await _context.FengShuiDocuments.FindAsync(fengShuiDocumentId);
        }

        public async Task<List<FengShuiDocument>> GetFengShuiDocuments()
        {
            return _context.FengShuiDocuments.ToList();
        }

        public async Task<FengShuiDocument> CreateFengShuiDocument(FengShuiDocument fengShuiDocument)
        {
            _context.FengShuiDocuments.Add(fengShuiDocument);
            await _context.SaveChangesAsync();
            return fengShuiDocument;
        }

        public async Task<FengShuiDocument> UpdateFengShuiDocument(FengShuiDocument fengShuiDocument)
        {
            _context.FengShuiDocuments.Update(fengShuiDocument);
            await _context.SaveChangesAsync();
            return fengShuiDocument;
        }

        public async Task DeleteFengShuiDocument(string fengShuiDocumentId)
        {
            var fengShuiDocument = await GetFengShuiDocumentById(fengShuiDocumentId);
            _context.FengShuiDocuments.Remove(fengShuiDocument);
            await _context.SaveChangesAsync();
        }

    }
}
