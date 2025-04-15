using BunnyCDN.Net.Storage;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers.BunnyCdnService
{
    public class BunnyCdnService : IBunnyCdnService
    {
        private readonly BunnyCDNStorage _storage;
        private readonly string _cdnUrl;

        public BunnyCdnService(BunnyCdnSettings settings)
        {
            _storage = new BunnyCDNStorage(settings.StorageZoneName, settings.ApiKey);
            _cdnUrl = settings.CdnUrl ?? $"https://{settings.StorageZoneName}.b-cdn.net";
        }

        public async Task<string> UploadVideoAsync(IFormFile file, string folder = "videos")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.UtcNow.Ticks}{Path.GetExtension(file.FileName)}";
            var path = $"{folder}/{fileName}";

            using var stream = file.OpenReadStream();
            await _storage.UploadAsync(stream, path);

            return $"{_cdnUrl}/{path}";
        }
    }
}
