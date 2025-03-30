using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.FengShuiDocumentService
{
    public interface IFengShuiDocumentService
    {
        Task<ResultModel> GetFengShuiDocumentById(string id);
        Task<ResultModel> GetAllFengShuiDocuments();
    }
}
