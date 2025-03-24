using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers.UploadService
{
    public class UploadService : IUploadService
    {
        private readonly Cloudinary _cloudinary;
        public UploadService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public string GetDocumentUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return null;

            var url = _cloudinary.Api.Url.BuildUrl(publicId);
            return url;
        }

        public string GetImageUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return null;

            var url = _cloudinary.Api.UrlImgUp.Transform(new Transformation()
                .Quality("auto")
                .FetchFormat("auto"))
                .BuildUrl(publicId);

            return url; 
        }

        public string GetVideoUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return null;

            var url = _cloudinary.Api.Url.BuildUrl(publicId);
            return url;
        }

        public async Task<string> UploadDocumentAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "documents",
                    PublicId = Path.GetFileNameWithoutExtension(file.FileName)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"Lỗi khi upload tài liệu: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi upload tài liệu: {ex.Message}");
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;
            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "images",
                    Transformation = new Transformation().Quality("auto").Crop("fill").FetchFormat("auto")
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                {
                    throw new Exception($"Lỗi khi upload ảnh: {uploadResult.Error.Message}");
                }
                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi upload ảnh: {ex.Message}");
            }
        }

        public async Task<string> UploadPdfAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "pdfs",
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"Lỗi khi upload PDF: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi upload PDF: {ex.Message}");
            }
        }

        public async Task<string> UploadVideoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;
            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "videos",
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"Lỗi khi upload video: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi upload video: {ex.Message}");
            }
        }
    }
}
