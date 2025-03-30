using Services.ApiModels.FengShuiDocument;
using Services.ApiModels;
﻿using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.FengShuiDocumentService
{
    public interface IFengShuiDocumentService
    {
        Task<ResultModel> CreateFengShuiDocument(CreateFengShuiDocumentRequest request);
        Task<ResultModel> GetFengShuiDocumentByBookingOfflineId(string bookingOfflineId);
        Task<ResultModel> CancelDocumentByManager(string documentId);
        Task<ResultModel> CancelDocumentByCustomer(string documentId);
        Task<ResultModel> ConfirmDocumentByCustomer(string documentId);
        Task<ResultModel> ConfirmDocumentByManager(string documentId);
        Task<ResultModel> GetFengShuiDocumentById(string id);
        Task<ResultModel> GetAllFengShuiDocuments();
    }
}
