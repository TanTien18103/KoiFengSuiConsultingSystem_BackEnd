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

        public Task<FengShuiDocument> GetFengShuiDocumentByBookingOfflineId(string bookingOfflineId)
        {
            return FengShuiDocumentDAO.Instance.GetFengShuiDocumentByBookingOfflineIdDao(bookingOfflineId);
        }

        public Task<FengShuiDocument> UpdateFengShuiDocumentStatus(string documentId, string status)
        {
            return FengShuiDocumentDAO.Instance.UpdateFengShuiDocumentStatusDao(documentId, status);
        }

        public Task<BookingOffline> AssignDocumentToBooking(string bookingOfflineId, string documentId)
        {
            return FengShuiDocumentDAO.Instance.AssignDocumentToBookingDao(bookingOfflineId, documentId);
        }

        public Task<List<FengShuiDocument>> GetFengShuiDocumentsByMaster(string masterId)
        {
            return FengShuiDocumentDAO.Instance.GetFengShuiDocumentsByMasterDao(masterId);
        }

        public Task<FengShuiDocument> UpdateFengShuiDocumentWithBooking(string documentId, string bookingOfflineId)
        {
            return FengShuiDocumentDAO.Instance.UpdateFengShuiDocumentWithBookingDao(documentId, bookingOfflineId);
        }
    }
}
