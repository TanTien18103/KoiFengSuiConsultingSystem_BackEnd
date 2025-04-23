using BunnyCDN.Net.Storage;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.ApiModels;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.ServicesHelpers.BunnyCdnService
{
    public class BunnyCdnService : IBunnyCdnService
    {
        private readonly string _apiKey;
        private readonly string _libraryId;
        private readonly HttpClient _httpClient;

        public BunnyCdnService(IConfiguration configuration, HttpClient httpClient)
        {
            _apiKey = configuration["BunnyCdn:ApiKey"];
            _libraryId = configuration["BunnyCdn:LibraryId"];
            _httpClient = httpClient;

            // Set default header for BunnyCDN API
            _httpClient.DefaultRequestHeaders.Add("AccessKey", _apiKey);
        }

        public async Task<string> UploadVideoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null");

            try
            {
                // Step 1: Create a video in BunnyCDN with default title
                var createUrl = $"https://video.bunnycdn.com/library/{_libraryId}/videos";
                var defaultTitle = "KoiFengShuiVideo_" + Guid.NewGuid().ToString("N");
                var videoMeta = new { title = defaultTitle };

                var content = new StringContent(JsonSerializer.Serialize(videoMeta), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(createUrl, content);

                // Log raw response details for debugging
                var statusCode = (int)response.StatusCode;
                var result = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                using (JsonDocument doc = JsonDocument.Parse(result))
                {
                    if (!doc.RootElement.TryGetProperty("guid", out var guidElement) || guidElement.ValueKind != JsonValueKind.String)
                    {
                        throw new Exception("Failed to extract guid from API response");
                    }

                    string guid = guidElement.GetString();

                    // Important fix: Use correct upload endpoint with the guid
                    var uploadUrl = $"https://video.bunnycdn.com/library/{_libraryId}/videos/{guid}";

                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                    var uploadResponse = await _httpClient.PutAsync(uploadUrl, fileContent);
                    uploadResponse.EnsureSuccessStatusCode();

                    // For the CDN URL, use the guid instead of a numeric videoId
                    return $"https://vz-{_libraryId}.b-cdn.net/{guid}/play.mp4";
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"BunnyCDN API error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload video: {ex.Message}", ex);
            }
        }
    }
}
