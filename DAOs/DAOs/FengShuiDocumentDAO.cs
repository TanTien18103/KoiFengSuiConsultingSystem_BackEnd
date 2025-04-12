using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class FengShuiDocumentDAO
    {
        private static volatile FengShuiDocumentDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private FengShuiDocumentDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static FengShuiDocumentDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new FengShuiDocumentDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<FengShuiDocument> GetFengShuiDocumentByIdDao(string fengShuiDocumentId)
        {
            return await _context.FengShuiDocuments
                .Include(d => d.BookingOfflines)
                    .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.Account)
                .Include(d => d.BookingOfflines)
                    .ThenInclude(b => b.Master)
                        .ThenInclude(m => m.Account)
                .Include(d => d.BookingOfflines)
                    .ThenInclude(b => b.ConsultationPackage)
                .Include(d => d.BookingOfflines)
                    .ThenInclude(b => b.Contract)
                .FirstOrDefaultAsync(d => d.FengShuiDocumentId == fengShuiDocumentId);
        }

        public async Task<List<FengShuiDocument>> GetFengShuiDocumentsDao()
        {
            return await _context.FengShuiDocuments
                .Include(d => d.BookingOfflines)
                .ToListAsync();
        }

        public async Task<List<FengShuiDocument>> GetFengShuiDocumentsByMasterDao(string masterId)
        {
            return await _context.FengShuiDocuments
                .Include(c => c.BookingOfflines)
                .Where(c => c.CreateBy == masterId)
                .ToListAsync();
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

        public async Task<FengShuiDocument> UpdateFengShuiDocumentWithBookingDao(string documentId, string bookingOfflineId)
        {
            // Lấy document hiện tại từ database
            var existingDocument = await _context.FengShuiDocuments
                .Include(d => d.BookingOfflines)
                .FirstOrDefaultAsync(d => d.FengShuiDocumentId == documentId);

            if (existingDocument == null)
                return null;

            // Lấy booking từ database
            var existingBooking = await _context.BookingOfflines
                .FirstOrDefaultAsync(b => b.BookingOfflineId == bookingOfflineId);

            if (existingBooking == null)
                return null;

            // Xóa tất cả các booking cũ
            existingDocument.BookingOfflines.Clear();

            // Thêm booking mới
            existingDocument.BookingOfflines.Add(existingBooking);

            await _context.SaveChangesAsync();
            return existingDocument;
        }

        public async Task DeleteFengShuiDocumentDao(string fengShuiDocumentId)
        {
            var fengShuiDocument = await GetFengShuiDocumentByIdDao(fengShuiDocumentId);
            _context.FengShuiDocuments.Remove(fengShuiDocument);
            await _context.SaveChangesAsync();
        }

        public async Task<FengShuiDocument> GetFengShuiDocumentByBookingOfflineIdDao(string bookingOfflineId)
        {
            var booking = await _context.BookingOfflines
                .Include(b => b.Document)
                .Include(b => b.Customer).ThenInclude(c => c.Account)
                .Include(b => b.Master).ThenInclude(m => m.Account)
                .FirstOrDefaultAsync(b => b.BookingOfflineId == bookingOfflineId);

            return booking?.Document;
        }

        public async Task<FengShuiDocument> UpdateFengShuiDocumentStatusDao(string documentId, string status)
        {
            var document = await _context.FengShuiDocuments
                .Include(d => d.BookingOfflines)
                    .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.Account)
                .Include(d => d.BookingOfflines)
                    .ThenInclude(b => b.Master)
                        .ThenInclude(m => m.Account)
                .FirstOrDefaultAsync(d => d.FengShuiDocumentId == documentId);

            if (document == null)
                return null;

            document.Status = status;
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<BookingOffline> AssignDocumentToBookingDao(string bookingOfflineId, string documentId)
        {
            var booking = await _context.BookingOfflines
                .Include(b => b.Customer).ThenInclude(c => c.Account)
                .Include(b => b.Master).ThenInclude(m => m.Account)
                .FirstOrDefaultAsync(b => b.BookingOfflineId == bookingOfflineId);

            if (booking == null)
                return null;

            booking.DocumentId = documentId;
            await _context.SaveChangesAsync();
            return booking;
        }
    }
}
