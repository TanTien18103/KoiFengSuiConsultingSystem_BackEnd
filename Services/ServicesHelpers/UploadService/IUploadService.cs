using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services.ServicesHelpers.UploadService
{
    public interface IUploadService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<string> UploadVideoAsync(IFormFile file);
        Task<string> UploadPdfAsync(IFormFile file);
        Task<string> UploadDocumentAsync(IFormFile file);
        string GetImageUrl(string publicId);
        string GetVideoUrl(string publicId);
        string GetDocumentUrl(string publicId);
        string GetPdfUrl(string publicId);
        Task<List<Quiz>> UploadExcelAsync(IFormFile file, string courseId);
    }
}
