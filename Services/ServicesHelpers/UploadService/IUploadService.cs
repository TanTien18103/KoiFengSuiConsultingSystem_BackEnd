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
        string GetImageUrl(string publicId);
        string GetVideoUrl(string publicId);
    }
}
