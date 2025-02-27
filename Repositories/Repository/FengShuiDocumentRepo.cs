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
    public class FengShuiDocumentRepo : IFengShuiDocumentRepo
    {
        private readonly FengShuiDocumentDAO _fengShuiDocumentDAO;

        public FengShuiDocumentRepo(FengShuiDocumentDAO fengShuiDocumentDAO)
        {
            _fengShuiDocumentDAO = fengShuiDocumentDAO;
        }

        public async Task<FengShuiDocument> GetFengShuiDocumentById(string fengShuiDocumentId)
        {
            return await _fengShuiDocumentDAO.GetFengShuiDocumentById(fengShuiDocumentId);
        }

        public async Task<FengShuiDocument> CreateFengShuiDocument(FengShuiDocument fengShuiDocument)
        {
            return await _fengShuiDocumentDAO.CreateFengShuiDocument(fengShuiDocument);
        }

        public async Task<FengShuiDocument> UpdateFengShuiDocument(FengShuiDocument fengShuiDocument)
        {
            return await _fengShuiDocumentDAO.UpdateFengShuiDocument(fengShuiDocument);
        }

        public async Task DeleteFengShuiDocument(string fengShuiDocumentId)
        {
            await _fengShuiDocumentDAO.DeleteFengShuiDocument(fengShuiDocumentId);
        }

        public async Task<List<FengShuiDocument>> GetFengShuiDocuments()
        {
            return await _fengShuiDocumentDAO.GetFengShuiDocuments();
        }
    }
}
