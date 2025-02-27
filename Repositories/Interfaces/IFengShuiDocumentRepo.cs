using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IFengShuiDocumentRepo
    {
        Task<FengShuiDocument> GetFengShuiDocumentById(string fengShuiDocumentId);
        Task<List<FengShuiDocument>> GetFengShuiDocuments();
        Task<FengShuiDocument> CreateFengShuiDocument(FengShuiDocument fengShuiDocument);
        Task<FengShuiDocument> UpdateFengShuiDocument(FengShuiDocument fengShuiDocument);
        Task DeleteFengShuiDocument(string fengShuiDocumentId);
    }
}
