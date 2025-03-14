using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.FengShuiDocumentRepository
{
    public class FengShuiDocumentRepo : IFengShuiDocumentRepo
    {
        public Task<FengShuiDocument> GetFengShuiDocumentById(string fengShuiDocumentId)
        {
            return FengShuiDocumentDAO.Instance.GetFengShuiDocumentByIdDao(fengShuiDocumentId);
        }
        public Task<List<FengShuiDocument>> GetFengShuiDocuments()
        {
            return FengShuiDocumentDAO.Instance.GetFengShuiDocumentsDao();
        }
        public Task<FengShuiDocument> CreateFengShuiDocument(FengShuiDocument fengShuiDocument)
        {
            return FengShuiDocumentDAO.Instance.CreateFengShuiDocumentDao(fengShuiDocument);
        }
        public Task<FengShuiDocument> UpdateFengShuiDocument(FengShuiDocument fengShuiDocument)
        {
            return FengShuiDocumentDAO.Instance.UpdateFengShuiDocumentDao(fengShuiDocument);
        }
        public Task DeleteFengShuiDocument(string fengShuiDocumentId)
        {
            return FengShuiDocumentDAO.Instance.DeleteFengShuiDocumentDao(fengShuiDocumentId);
        }
    }
}
